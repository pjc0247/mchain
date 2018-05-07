using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using minichain;

namespace minichain.test
{
    [TestClass]
    public class TxPool_test
    {
        [TestMethod]
        public void GetWithHigestFee()
        {
            var pool = new TransactionPool();
            var tx1 = new Transaction()
            {
                fee  = 1
            };
            var tx2 = new Transaction()
            {
                fee = 2
            };
            var tx3 = new Transaction()
            {
                fee = 3
            };
            var tx4 = new Transaction()
            {
                fee = 4
            };

            pool.AddTransaction(tx1);
            pool.AddTransaction(tx2);
            pool.AddTransaction(tx3);
            pool.AddTransaction(tx4);

            var txs = pool.GetTransactionsWithHighestFee(null, null, 3);

            Assert.AreEqual(3, txs.Length);
            Assert.AreEqual(4, txs[0].fee);
            Assert.AreEqual(3, txs[1].fee);
            Assert.AreEqual(2, txs[2].fee);
        }

        [TestMethod]
        public void CannotAddDuplicatedTransaction()
        {
            var pool = new TransactionPool();
            var tx = new Transaction()
            {
            };

            pool.AddTransaction(tx);
            pool.AddTransaction(tx);
            pool.AddTransaction(tx);

            Assert.AreEqual(pool.count, 1);
        }

        [TestMethod]
        public void CanUpdateTransaction()
        {
            var pool = new TransactionPool();
            var txV1 = new Transaction()
            {
                version = 0, fee = 0
            };

            pool.AddTransaction(txV1);

            // update transaction
            txV1.version = 1;
            txV1.fee = 1;
            pool.AddTransaction(txV1);

            Assert.AreEqual(pool.count, 1);
            Assert.AreEqual(
                pool.GetTransactionsWithHighestFee(null, null, 1)[0].fee,
                1);
        }
    }
}
