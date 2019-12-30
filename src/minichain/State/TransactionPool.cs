using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    /// <summary>
    /// Manages pending transactions
    /// </summary>
    public class TransactionPool
    {
        public int count => pendingTxs.Count;

        private int maxSize;
        private Dictionary<string, Transaction> pendingTxs = new Dictionary<string, Transaction>();

        public TransactionPool(int _maxSize = 1024)
        {
            maxSize = _maxSize;
        }

        public void AddTransaction(Transaction tx)
        {
            if (count >= maxSize) return;

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
            if (count >= maxSize) return;

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

        public Transaction[] GetTransactionsWithHighestFee(ChainState chain, string blockHash, int n)
        {
            lock (pendingTxs)
            {
                var txs = new List<Transaction>();
                var candidates = pendingTxs
                    .Select(x => x.Value)
                    .OrderByDescending(x => x.fee);

                foreach (var tx in candidates)
                {
                    if (txs.Count == n)
                        break;
                    if (chain != null &&
                        chain.IsValidTransactionForBlock(blockHash, tx) == false)
                        continue;

                    txs.Add(tx);
                }

                for (int i = 0; i < txs.Count; i++)
                    pendingTxs.Remove(txs[i].hash);

                return txs.ToArray();
            }
        }
    }
}
