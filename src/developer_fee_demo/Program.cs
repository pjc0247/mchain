using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using minichain;

namespace developer_fee_demo
{
    class DevFeeMiner : Miner
    {
        /// <summary>
        /// Mining program developer's wallet address
        /// </summary>
        public static readonly string DeveloperAddress = "asdfasdfasdfasdf";
        /// <summary>
        /// How much developer takes the fee (currently 3%)
        /// </summary>
        public static readonly double DeveloperFeeRate = 0.03f;

        protected override Transaction[] PrepareBlockTransactions(int blockNo)
        {
            // -2 for reward transaction and fee transaction
            var txsWithHighestFee =
                txPool.GetTransactionsWithHighestFee(Consensus.MaxTransactionsPerBlock - 2);
            var txs = new List<Transaction>();

            // First transaction is block reward
            //    Block reward is BLOCK_REWARD + TOTAL_FEE
            txs.Add(Transaction.CreateRewardTransaction(blockNo, wallet.address, txsWithHighestFee));
            // Second transaction is developer fee
            //    BLOCK_REWARD * FEE_RATE
            txs.Add(wallet.CreateSignedTransaction(DeveloperAddress, Consensus.CalcBlockReward(blockNo) * DeveloperFeeRate, 0));
            txs.AddRange(txsWithHighestFee);

            return txs.ToArray();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            /* WRITE YOUR MINING PROGRAM HERE */
        }
    }
}
