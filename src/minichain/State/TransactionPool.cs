using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    /// <summary>
    /// Management pending transactions
    /// </summary>
    public class TransactionPool
    {
        public int count => pendingTxs.Count;

        private Dictionary<string, Transaction> pendingTxs = new Dictionary<string, Transaction>();

        public TransactionPool()
        {
        }

        public void AddTransaction(Transaction tx)
        {
            lock (pendingTxs)
            {
                // Already has same tx in pool.
                if (pendingTxs.ContainsKey(tx.hash) &&
                    pendingTxs[tx.hash].version >= tx.version)
                    return;

                pendingTxs.Add(tx.hash, tx);
            }
        }
        public void AddTransactions(Transaction[] txs)
        {
            lock (pendingTxs)
            {
                foreach (var tx in txs)
                    AddTransaction(tx);
            }
        }
        public void RemoveTransactions(Transaction[] txs)
        {
            lock (pendingTxs)
            {
                foreach (var tx in txs)
                    pendingTxs.Remove(tx.hash);
            }
        }
        public Transaction[] GetTransactionsWithHighestFee(int n)
        {
            lock (pendingTxs)
            {
                var txs = pendingTxs
                    .Select(x => x.Value)
                    .OrderByDescending(x => x.fee)
                    .Take(n).ToArray();

                for (int i = 0; i < txs.Length; i++)
                    pendingTxs.Remove(txs[i].hash);

                return txs;
            }
        }
    }
}
