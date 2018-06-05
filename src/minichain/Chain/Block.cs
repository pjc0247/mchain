using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    public class BlockHeader
    {
        /// Block No#
        public int blockNo { get; protected set; }

        /// Address of block miner
        public Hash minerAddr { get; protected set; }

        /// This block hash
        public Hash hash { get; protected set; }
        /// Prev block hash
        public Hash prevBlockHash { get; protected set; }
        /// Merkle root hash
        public Hash merkleRootHash { get; protected set; }

        public string nonce { get; protected set; }
        public int difficulty { get; protected set; }
    }
    public class BlockBody : BlockHeader
    {
        public Transaction[] txs { get; protected set; }
    }

    public class Block : BlockBody
    {
        public static Block GenesisBlock()
        {
            return new Block()
                {
                    difficulty = Consensus.CalcBlockDifficulty(0),
                    txs = new Transaction[] { },
                    hash = GetBlockHash(null, null, "0")
                };
        }
        public static Hash GetBlockHash(string prevBlockHash, string merkleRootHash, string nonce)
        {
            return Hash.Calc(prevBlockHash + merkleRootHash + nonce);
        }

        /// <summary>
        /// Checks the block has valid nonce.
        /// </summary>
        public static bool IsValidNonce(Block block, string nonce)
        {
            var hash = GetBlockHash(block.prevBlockHash, block.merkleRootHash, nonce);

            return hash.str.StartsWith(new string('0', block.difficulty));
        }
        /// <summary>
        /// Lightweight validations with block header.
        /// </summary>
        public static bool IsValidBlockLight(Block block, string nonce)
        {
            if (block == null) return false;

            // Genesis-block is always right;
            if (block.blockNo == 0) return true;

            var totalFee = block.txs.Sum(x => x.fee);

            // 1. txs MUST be non-empty (except genesis-block)
            if (block.txs.Length == 0 || block.txs.Length > Consensus.MaxTransactionsPerBlock)
                return false;
            // 2. Check the reward transaction. (txs[0])
            if (block.txs[0]._out != Consensus.CalcBlockReward(block.blockNo) + totalFee ||
                block.txs[0].senderAddr != Consensus.RewardSenderAddress ||
                block.txs[0].receiverAddr != block.minerAddr) return false;
            // 3. Has valid minerAddress
            if (string.IsNullOrEmpty(block.minerAddr))
                return false;
            // 4. Has proper difficulty
            if (Consensus.CalcBlockDifficulty(block.blockNo) != block.difficulty) return false;

            // 5. Check the nonce with block difficulty
            return IsValidNonce(block, nonce);
        }
        /// <summary>
        /// Fully validates the given block.
        /// </summary>
        public static bool IsValidBlockDeep(Block block, string nonce)
        {
            if (IsValidBlockLight(block, nonce) == false)
                return false;

            // 1. Check 1~n transactions are valid
            //    (txs[0] is a special transaction which is reserved for block reward)
            if (block.txs.Skip(1).Any(x => Transaction.IsValidTransaction(x) == false))
                return false;

            // 2. Check merkleRootHash is valid
            var merkleTree = new MerkleTree(block.txs);
            if (block.merkleRootHash != merkleTree.GetRootHash())
                return false;

            return true;
        }

        public Block()
        {
        }
        public Block(Hash _minerAddr, Block _prev, Transaction[] _txs, string _nonce)
        {
            if (_prev == null)
                throw new ArgumentNullException(nameof(_prev));

            var merkleTree = new MerkleTree(_txs);

            minerAddr = _minerAddr;
            merkleRootHash = merkleTree.GetRootHash();
            nonce = _nonce;
            txs = _txs;

            prevBlockHash = _prev.hash;
            blockNo = _prev.blockNo + 1;

            difficulty = Consensus.CalcBlockDifficulty(blockNo);
            hash = GetBlockHash(prevBlockHash, merkleRootHash, nonce);
        }
    }
}
