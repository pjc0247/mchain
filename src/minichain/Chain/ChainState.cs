using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using minivm;

namespace minichain
{
    public class ChainState
    {
        public delegate void BlockConfirmedDelegate(Block b);

        public Block currentBlock { get; private set; }
        private object blockLock = new object();

        public BlockConfirmedDelegate onBlockConfirmed { get; set; }

        private IStorageBackend db;
        private StateDB sdb;

        private VM<ChainStateProvider> vm;

        public ChainState()
        {
#if DEBUG__
            db = new MemDB();
#else
            db = new FileDB();
#endif
            sdb = new StateDB(db);

            vm = new VM<ChainStateProvider>();

            // Every node starts with genesisBlock.
            //   This will be overwritten if there is any other live nodes.
            PushBlock(Block.GenesisBlock());
        }

        public Transaction GetTransaction(string transactionHash)
        {
            return db.Read<Transaction>($"tx/{transactionHash}");
        }
        public Block GetBlock(string blockHash)
        {
            return db.Read<Block>($"block/{blockHash}");
        }

        public double GetBalanceInBlock(string address, string blockHash)
        {
            return sdb.GetState(blockHash, address).balance;
        }
        public double GetBalance(string address)
        {
            return GetBalanceInBlock(address, currentBlock.hash);
        }

        internal SingleState GetState(string key)
        {
            return sdb.GetState(currentBlock.hash, key);
        }

        internal bool PushBlock(Block block)
        {
            if (block == null) 
                throw new ArgumentNullException(nameof(block));
            if (currentBlock == null)
                currentBlock = block;

            try
            {
                lock (blockLock)
                {
                    // In case that new block is came from another branch:
                    if (currentBlock != null &&
                        block.prevBlockHash != currentBlock.hash)
                        ;

                    db.Write($"block/{block.hash}", block);

                    ApplyTransactions(block);

                    // CONFIRMED
                    currentBlock = block;

                    onBlockConfirmed?.Invoke(block);

                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        /// <summary>
        /// Backtracks the branched block.
        ///             
        /// ex) TrackBranchedBlock("block13", "block12") 
        ///       => "block10"
        ///       
        ///             13
        ///             12    12
        ///              11  11
        ///                10
        ///                 9
        ///                 8
        /// </summary>
        private Block TrackBranchedBlock(Block a, Block b)
        {
            var history = new HashSet<string>();

            for (int i = 0;i <Consensus.TrustedConfirmations; i++)
            {
                if (a.hash == b.hash) return a;

                a = GetBlock(a.prevBlockHash);
                b = GetBlock(b.prevBlockHash);

                if (history.Add(a.hash) == false) return a;
                if (history.Add(b.hash) == false) return b;
            }

            // 어디서 혼자 놀고있다가 갑자기 존나 긴 체인 들고온 경우 무시
            throw new InvalidOperationException();
        }

        private void ApplyTransactions(Block newBlock)
        {
            var txs = newBlock.txs;
            var changes = new HashSet<PushStateEntry>();

            foreach (var tx in txs)
            {
                if (tx.type == TransactionType.Payment)
                    ApplyPaymentTransaction(tx, changes);
                else if (tx.type == TransactionType.Deploy)
                    ApplyDeployTransaction(newBlock, tx, changes);
                else if (tx.type == TransactionType.Call)
                    ApplyCallTransaction(tx, changes);
                else
                    throw new InvalidOperationException("Unknown txtype: " + tx.type);

                // TODO: 공통된 fee 차감 코드
            }

            sdb.PushState(currentBlock.hash, newBlock.hash, changes.ToArray());
        }

        private void ApplyPaymentTransaction(Transaction tx, HashSet<PushStateEntry> changes)
        {
            if (tx.senderAddr != Consensus.RewardSenderAddress)
            {
                var senderWallet = changes
                    .FirstOrDefault(x => x.state.key == tx.senderAddr)
                    ?.state;

                if (senderWallet == null)
                    senderWallet = sdb.GetState(currentBlock.hash, tx.senderAddr);

                if (senderWallet.balance != tx._in)
                    throw new InvalidOperationException();
                if (senderWallet.balance >= tx._out + tx.fee)
                    throw new InvalidOperationException();

                // Actual OUT is (_out + fee)
                senderWallet.balance -= tx._out + tx.fee;
                changes.Add(PushStateEntry.Create(
                    PushStateFlag.None,senderWallet));
            }

            var receiverWallet = changes
                .FirstOrDefault(x => x.state.key == tx.receiverAddr)
                ?.state;

            if (receiverWallet == null)
                receiverWallet = sdb.GetState(currentBlock.hash, tx.receiverAddr);

            receiverWallet.balance += tx._out;
            changes.Add(PushStateEntry.Create(
                PushStateFlag.None, receiverWallet));
        }

        private void ApplyDeployTransaction(Block newBlock, Transaction tx, HashSet<PushStateEntry> changes)
        {
            changes.Add(PushStateEntry.Create(
                PushStateFlag.NewAddressOnly,
                new SingleState(StateType.Contract)
                {
                    key = tx.receiverAddr,
                    balance = 0.0,
                    value = tx.contractProgram
                }));

            (var abi, var insts) = BConv.FromBase64(tx.contractProgram);
            var sp = (ChainStateProvider)vm.stateProvider;
            sp.SetContext(this, tx.receiverAddr, changes);
            vm.Execute(abi, insts, tx.methodSignature, 1000, out _);
        }

        private void ApplyCallTransaction(Transaction tx, HashSet<PushStateEntry> changes)
        {

        }
    }
}
    