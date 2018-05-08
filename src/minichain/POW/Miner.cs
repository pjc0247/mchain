using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    public class Miner : EndpointNode
    {
        private string solution;
        private AutoResetEvent ev = new AutoResetEvent(false);

        private Thread miningThread;
        private long hashCounter = 0;

        private new MinerNodeConfig cfg;

        public Miner() : this(new MinerNodeConfig())
        {
        }
        public Miner(MinerNodeConfig _cfg) : base(_cfg)
        {
            cfg = _cfg;
        }

        public void Start()
        {
            onNewBlockDiscoveredByOther += OnNewBlockDiscovered;

            isAlive = true;

            miningThread = new Thread(MinerHQ);
            miningThread.Start();
        }
        public override void Stop()
        {
            base.Stop();

            ev.Set();
            miningThread.Join();
        }

        protected virtual Transaction[] PrepareBlockTransactions(string prevBlockHash, int blockNo)
        {
            // -1 for reward transaction
            var txsWithHighestFee = 
                txPool.GetTransactionsWithHighestFee(chain, prevBlockHash, Consensus.MaxTransactionsPerBlock - 1);
            var txs = new List<Transaction>();

            // First transaction is block reward
            //    Block reward is BLOCK_REWARD + TOTAL_FEE
            txs.Add(Transaction.CreateRewardTransaction(blockNo, wallet.address, txsWithHighestFee));
            txs.AddRange(txsWithHighestFee);

            return txs.ToArray();
        }

        private void MinerHQ()
        {
            while (isAlive)
            {
                if (state != NodeState.OK)
                {
                    Thread.Sleep(100);
                    continue;
                }

                var workingBlockNo = chain.currentBlock.blockNo;
                var txs = PrepareBlockTransactions(chain.currentBlock.hash, workingBlockNo + 1);
                var vblock = new Block(wallet.address, chain.currentBlock, txs, "");

                var startTime = DateTime.Now;
                PrepareWorkers(vblock, Math.Min(cfg.maxMiningThreads, Environment.ProcessorCount));

                if (ev.WaitOne())
                {
                    var elapsed = (DateTime.Now - startTime).TotalSeconds;
                    /*
                    Console.WriteLine(
                        $"   * HASHRATE: {(int)(Interlocked.Read(ref hashCounter) / elapsed)}/s");
                    */

                    if (chain.currentBlock.blockNo != workingBlockNo)
                    {
                        txPool.AddTransactions(txs.Skip(1).ToArray());
                        continue;
                    }

                    var solutionCopy = solution;
                    if (Block.IsValidBlockLight(vblock, solutionCopy) == false)
                        continue;

                    chain.PushBlock(new Block(wallet.address, chain.currentBlock, txs, solution));

                    PublishBlock(chain.currentBlock);

                    /*
                    Console.WriteLine(
                        $"   * FindBlock#{chain.currentBlock.blockNo}, elapsed {elapsed} sec(s)\r\n" +
                        $"        nonce: {solution} \r\n" +
                        $"        prevBlock: {chain.currentBlock.prevBlockHash} \r\n" + 
                        $"        txs: {chain.currentBlock.txs.Length}");
                    */
                }
            }
        }

        private void OnNewBlockDiscovered(Block block)
        {
            ev.Set();
        }

        private void PrepareWorkers(Block vblock, int nThread)
        {
            /*
            Console.WriteLine(
                "------------------------------------------------------------\r\n" +
                "Preparing job, block#" + vblock.blockNo + " with " + nThread + " thread(s).");
            */

            Interlocked.Exchange(ref hashCounter, 0);
            for (int i = 0; i < nThread; i++)
            {
                var t = new Thread(() =>
                {
                    Worker(vblock, 100000000 * i);
                });
                t.Start();
            }
        }
        private void Worker(Block vblock, int start)
        {
            var limit = 10000;

            while(isAlive && vblock.blockNo == chain.currentBlock.blockNo + 1)
            {
                var nonce = Solver.FindSolution(vblock, start, limit);
                // DoubleCheck
                if (vblock.blockNo != chain.currentBlock.blockNo + 1)
                    return;

                Interlocked.Add(ref hashCounter, limit);
                if (Block.IsValidNonce(vblock, nonce))
                {
                    solution = nonce;
                    ev.Set();
                }

                start += limit;
            }
        }
    }
}
