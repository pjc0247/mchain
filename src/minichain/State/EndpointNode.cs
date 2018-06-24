using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Discovery;

namespace minichain
{
    public class EndpointNode : NodeBase
    {
        protected delegate void NewBlockDiscoveredDelegate(Block block);

        protected NewBlockDiscoveredDelegate onNewBlockDiscoveredByOther;

        public NodeConfig cfg { get; private set; }

        public Wallet wallet { get; private set; }
        public ChainState chain { get; private set; }

        public NodeState state { get; private set; }

        protected TransactionPool txPool { get; private set; }

        private RpcServer rpcServer;
        private Discoverer discoverer;
        private Thread discoverThread;

        private int syncTargetBlockNo = 0;

        /// <summary>
        /// Creates a node with default configuration.
        /// </summary>
        public EndpointNode() : this (new NodeConfig())
        {
        }
        /// <summary>
        /// Creates a node with a given configuration.
        /// </summary>
        public EndpointNode(NodeConfig _cfg)
        {
            cfg = _cfg;
            rpcServer = new RpcServer(this, cfg.rpcListenPort);
            if (cfg.useRpcServer)
                rpcServer.Start();

            chain = new ChainState();
            wallet = new Wallet(chain);
            txPool = new TransactionPool(cfg.maxTxpoolSize);

            Subscribe<PktBroadcastNewBlock>(OnNewBlock);
            Subscribe<PktNewTransaction>(OnNewTransaction);
            Subscribe<PktRequestPeers>(OnRequestPeers);
            Subscribe<PktResponsePeers>(OnResponsePeers);
            Subscribe<PktRequestBlock>(OnRequestBlock);
            Subscribe<PktResponseBlock>(OnResponseBlock);

            discoverer = new Discoverer("minichain", peers.listeningPort.ToString());
            discoverer.onFoundPeer += OnFoundLocalPeer;
            discoverer.BeginDiscovery(-1);
            discoverThread = new Thread(DiscoverWorker);
            discoverThread.Start();
        }
        public override void Stop()
        {
            base.Stop();

            discoverThread.Join();
        }

        public void SendTransaction(Transaction tx)
        {
            if (Transaction.IsValidTransaction(tx) == false)
                throw new ArgumentException(nameof(tx));
            if (chain.IsValidTransactionForBlock(chain.currentBlock.hash, tx))
                throw new InvalidOperationException("Tx is not valid for current state");

            txPool.AddTransaction(tx);
            SendPacketToAllPeers(new PktNewTransaction()
            {
                sentBlockNo = chain.currentBlock.blockNo,
                tx = tx
            });
        }

        private void OnFoundLocalPeer(Int64 id, string payload, IPEndPoint ep)
        {
            if (payload == peers.listeningPort.ToString())
                return;

            peers.ConnectPeer(ep.Address.ToString().Split(':')[0] + ":" + payload);
        }

        /// <summary>
        /// Periodically requests other peers to current peers.
        /// </summary>
        private void DiscoverWorker()
        {
            while (isAlive)
            {
                // Already reached to MaxPeers, Skip discovery
                if (peers.alivePeers >= PeerPool.MaxPeers)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                SendPacketToAllPeers(new PktRequestPeers() { });
                Thread.Sleep(5000);
            }
        }

        private void OnRequestPeers(Peer sender, PktRequestPeers pkt)
        {
            SendPacket(sender, new PktResponsePeers()
            {
                addrs = peers.GetPeerAddresses()
            });
        }
        private void OnResponsePeers(Peer sender, PktResponsePeers pkt)
        {
            if (pkt.addrs == null) return;

            foreach (var addr in pkt.addrs)
                peers.ConnectPeer(addr);
        }

        private void OnRequestBlock(Peer sender, PktRequestBlock pkt)
        {
            var block = chain.GetBlock(pkt.blockNo);
            if (block == null) return;

            SendPacket(sender, new PktResponseBlock()
            {
                block = block
            });
        }
        private void OnResponseBlock(Peer sender, PktResponseBlock pkt)
        {
            if (pkt.block == null) return;
            if (Block.IsValidBlockLight(pkt.block, pkt.block.nonce) == false)
                return;

            Console.WriteLine($"    {pkt.block.blockNo} / {syncTargetBlockNo}");

            if (chain.GetBlock(pkt.block.hash) == null)
            {
                chain.SaveBlock(pkt.block);

                SendPacket(sender, new PktRequestBlock()
                {
                    blockNo = pkt.block.blockNo - 1
                });
            }
            else
            {
                state = NodeState.SyncProcessing;
                Console.WriteLine("===SYNC-PROCESSING====");

                chain.ProcessSyncingAndUnsetSyncLock(pkt.block.blockNo, syncTargetBlockNo);

                Console.WriteLine("===SYNC-DONE====");

                state = NodeState.OK;
            }
        }

        protected void PublishBlock(Block block)
        {
            SendPacketToAllPeers(new PktBroadcastNewBlock()
            {
                block = block
            });
        }
        protected virtual void OnNewBlock(Peer sender, PktBroadcastNewBlock pkt)
        {
            if (state != NodeState.OK) return;

            // Peer sent invalid block.
            if (Block.IsValidBlockLight(pkt.block, pkt.block.nonce) == false) return;
            // My block is longer than received
            if (chain.currentBlock.blockNo >= pkt.block.blockNo) return;

            // In case we need to sync blocks
            if (chain.currentBlock.blockNo + 1 < pkt.block.blockNo)
            {
                chain.SaveBlock(pkt.block);
                StartSyncing(sender, pkt.block.blockNo);
                return;
            }

            if (chain.PushBlock(pkt.block))
            {
                onNewBlockDiscoveredByOther?.Invoke(pkt.block);
                txPool.RemoveTransactions(pkt.block.txs);
            }
        }
        protected virtual void OnNewTransaction(Peer sender, PktNewTransaction pkt)
        {
            // Peer sent invalid transaction
            if (pkt.tx == null) return;
            // Future transaction which cannot be processed at this time
            if (chain.currentBlock.blockNo < pkt.sentBlockNo) return;
            if (chain.currentBlock.blockNo >= pkt.sentBlockNo + 10) return;
            if (Transaction.IsValidTransaction(pkt.tx) == false) return;

            txPool.AddTransaction(pkt.tx);
        }

        private void StartSyncing(Peer peer, int targetBlockNo)
        {
            Console.WriteLine("===SYNC====");

            chain.SetSyncLock();

            state = NodeState.SyncDownloading;
            syncTargetBlockNo = targetBlockNo;

            peer.SendPacket(new PktRequestBlock()
            {
                blockNo = targetBlockNo - 1
            });
        }
    }
}
