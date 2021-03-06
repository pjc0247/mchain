﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private KeyValueDB blockDB;
        private KeyValueDB blockHashLookupDB;
        private StateDB stateDB;

        private VM<ChainStateProvider> vm;

        private int syncLock = 0;

        public ChainState()
        {
#if UNITTEST
            db = new MemDB();
#else
            db = new FileDB();
#endif
            stateDB = new StateDB(db);
            blockDB = new KeyValueDB("block/", db);
            blockHashLookupDB = new KeyValueDB("blockhash/",db);

            vm = new VM<ChainStateProvider>();

            // Every node starts with genesisBlock.
            //   This will be overwritten if there is any other live nodes.
            PushBlock(Block.GenesisBlock());
        }

        public bool IsValidTransactionForBlock(Hash blockHash, Transaction tx)
        {
            var balance = GetBalanceInBlock(tx.senderAddr, blockHash);

            if (balance < tx._out + tx.fee)
                return false;

            if (tx.type == TransactionType.Deploy)
            {
                // Target must be an empty address
                if (GetStateInBlock(tx.receiverAddr, blockHash).type != StateType.Empty)
                    return false;
            }
            else if (tx.type == TransactionType.Call)
            {
                var contract = GetContract(tx.receiverAddr);
                if (contract == null)
                    return false;

                (var abi, var insts) = BConv.FromBase64(contract);
                if (abi.methods.Any(x => x.signature == tx.methodSignature) == false)
                    return false;
            }
            else if (tx.type == TransactionType.RegisterANS)
            {
                if (GetStateInBlock(Sig2Hash.ANS(tx.ANSname), blockHash) != null)
                    return false;
            }

            return true;
        }

        public Transaction GetTransaction(Hash transactionHash)
        {
            return db.Read<Transaction>($"tx/{transactionHash}");
        }
        public Block GetBlock(Hash blockHash)
        {
            return blockDB.Get<Block>(blockHash);
        }
        public Block GetBlock(int blockNo)
        {
            var lookUp = blockHashLookupDB.Get($"{blockNo}");
            if (string.IsNullOrEmpty(lookUp))
                throw new ArgumentException($"No lookup for #{blockNo}");
            return GetBlock(lookUp);
        }

        public double GetBalanceInBlock(Hash address, Hash blockHash)
        {
            return stateDB.GetState(blockHash, address).balance;
        }
        public double GetBalance(Hash address)
        {
            return GetBalanceInBlock(address, currentBlock.hash);
        }
        public object GetPublicField(Hash address, Hash fieldSignature)
        {
            return GetState(Sig2Hash.Field(address, fieldSignature)).value;
        }
        public string GetAddressFromANS(string ANSname)
        {
            return (string)GetState(Sig2Hash.ANS(ANSname))?.value;
        }

        internal SingleState GetStateInBlock(Hash key, Hash blockHash)
        {
            return stateDB.GetState(blockHash, key);
        }
        internal SingleState GetState(Hash key)
        {
            return GetStateInBlock(key, currentBlock.hash);
        }
        internal string GetContract(Hash key)
        {
            return (string)stateDB.GetState(currentBlock.hash, key)?.value;
        }

        /// <summary>
        /// Saves the block into DB. (Not a chain)
        /// </summary>
        /// <param name="block"></param>
        internal void SaveBlock(Block block)
        {
            blockDB.Set(block.hash, block);
            blockHashLookupDB.Set($"{block.blockNo}", block.hash);
        }
        /// <summary>
        /// Pushes the given blcok into current chain.
        /// Input block must be a next block of the `currentBlock`.
        /// </summary>
        internal bool PushBlock(Block block, bool isSync = false)
        {
            if (isSync == false && Thread.VolatileRead(ref syncLock) == 1)
                return false;
            if (block == null) 
                throw new ArgumentNullException(nameof(block));

            try
            {
                lock (blockLock)
                {
                    // In case that new block is came from another branch:
                    if (currentBlock != null &&
                        block.prevBlockHash != currentBlock.hash)
                    {
                        //throw new ArgumentException($"input block is not in sequence. prev({currentBlock.blockNo}) -> input({block.blockNo})");
                        //var brancedBlock = TrackBranchedBlock(currentBlock, block);
                        return false;
                    }

                    SaveBlock(block);
                    ApplyTransactions(block);

                    // CONFIRMED
                    currentBlock = block;
                    
                    onBlockConfirmed?.Invoke(block);

                    Console.WriteLine("   * Block accepted: " + currentBlock.blockNo);

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
        /// Force reverts the current chain to specific block.
        /// </summary>
        internal void RevertTo(Block block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            lock (blockLock)
            {
                currentBlock = block;
            }
        }

        /// <summary>
        /// Mark status as `IN_SYNC` to prevent push blocks
        /// </summary>
        internal void SetSyncLock()
        {
            Thread.VolatileWrite(ref syncLock, 1);
        }
        internal void ProcessSyncingAndUnsetSyncLock(int startBlockNo, int targetBlockNo)
        {
            RevertTo(GetBlock(startBlockNo));

            var start = startBlockNo + 1;
            for (int i = start; i <= targetBlockNo; i++)
            {
                if (PushBlock(GetBlock(i), true) == false)
                    throw new InvalidOperationException("SyncError");
            }

            Thread.VolatileWrite(ref syncLock, 0);
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
            var changes = new ChangeSet(this);

            if (txs.Length > 0)
            {
                changes.Begin();
                ApplyPaymentTransaction(txs.First(), changes);
                changes.Commit();
            }

            foreach (var tx in txs.Skip(1))
            {
                tx.receiverAddr = ANSLookup.Resolve(this, tx.receiverAddr);

                if (IsValidTransactionForBlock(newBlock.prevBlockHash, tx) == false)
                    throw new BlockValidationException("Invalid tx: " + tx.hash);

                var fee = -tx.fee; // Initial fee which can be changed 
                changes.Begin();

                if (tx.type == TransactionType.Payment)
                    ApplyPaymentTransaction(tx, changes);
                else if (tx.type == TransactionType.RegisterANS)
                    ApplyRegisterANSTransaction(tx, changes);
                else if (tx.type == TransactionType.Deploy)
                    ApplyDeployTransaction(newBlock, tx, changes, out fee);
                else if (tx.type == TransactionType.Call)
                    ApplyCallTransaction(tx, changes, out fee);
                else
                    throw new BlockValidationException("Unknown txtype: " + tx.type);

                changes.UpdateBalance(tx.senderAddr, -fee);
                changes.Commit();
            }

            stateDB.PushState(
                currentBlock == null ? Hash.ZeroAddress : currentBlock.hash,
                newBlock.hash, changes.ToArray());
        }

        // APPLIES SINGLE TRANSACTIONS
        //   EVERY TRANSACTIONS MUST BE VALID HERE
        //
        //   Do not check twice
        private void ApplyPaymentTransaction(Transaction tx, ChangeSet changes)
        {
            if (tx.senderAddr != Consensus.RewardSenderAddress)
                changes.UpdateBalance(tx.senderAddr, -tx._out);

            changes.UpdateBalance(tx.receiverAddr, tx._out);
        }

        private void ApplyRegisterANSTransaction(Transaction tx, ChangeSet changes)
        {
            changes.Add(PushStateEntry.Create(
                PushStateFlag.NewAddressOnly,
                new SingleState(StateType.ANSLookup)
                {
                    key = Sig2Hash.ANS(tx.ANSname),
                    value = tx.receiverAddr
                }));
        }
        private void ApplyDeployTransaction(Block newBlock, Transaction tx, ChangeSet changes, out double fee)
        {
            changes.Add(PushStateEntry.Create(
                PushStateFlag.NewAddressOnly,
                new SingleState(StateType.Contract)
                {
                    key = tx.receiverAddr,
                    balance = tx._out,
                    value = tx.contractProgram
                }));

            (var abi, var insts) = BConv.FromBase64(tx.contractProgram);
            var sp = (ChainStateProvider)vm.stateProvider;
            var totalGasUsed = 0;

            sp.SetContext(this, tx.receiverAddr, tx, changes);
            try
            {
                var result = vm.Execute(abi, insts,
                    tx.methodSignature, 1000);
            }
            catch (VMRuntimeException e)
            {
            }

            fee = totalGasUsed;
        }
        private void ApplyCallTransaction(Transaction tx, ChangeSet changes, out double fee)
        {
            var contract = GetContract(tx.receiverAddr);
            (var abi, var insts) = BConv.FromBase64(contract);
            var sp = (ChainStateProvider)vm.stateProvider;
            var totalGasUsed = 0;

            sp.SetContext(this, tx.receiverAddr, tx, changes);
            try
            {
                var result = vm.Execute(abi, insts,
                    tx.methodSignature, tx.callArgs, 1000);

                var events = result.events;
            }
            catch (VMRuntimeException e)
            {
            }

            // tODO: gas?? fee??
            fee = totalGasUsed;
        }
    }
}
    