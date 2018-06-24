using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using minichain;

namespace bad_node_demo
{
    /// <summary>
    /// A simple packet that allows
    /// attackers to know each other.
    /// </summary>
    class PktHelloBadNode : BroadcastPacket
    {
        public string addr;
    }

    /// <summary>
    /// An attacker node that denies every blocks from 
    /// non-attacker miner.
    /// </summary>
    class BadNode : Miner
    {
        // List of bad friends.
        private HashSet<string> badNodeAddrs = new HashSet<string>();

        private Thread broadcaster;

        public BadNode()
        {
            Subscribe<PktHelloBadNode>(OnHelloBadNode);

            broadcaster = new Thread(BadNodeBroadcaster);
            broadcaster.Start();
        }

        private void BadNodeBroadcaster()
        {
            while (true)
            {
                SendPacketToAllPeers(new PktHelloBadNode()
                {
                    addr = wallet.address
                });
                Thread.Sleep(4000);
            }
        }
        private void OnHelloBadNode(Peer sender, PktHelloBadNode pkt)
        {
            if (badNodeAddrs.Add(pkt.addr))
                Console.WriteLine("Discovered another bad node!!  " + sender.address);
        }

        protected override void OnNewBlock(Peer sender, PktBroadcastNewBlock pkt)
        {
            // Check the block is mined by alias
            if (badNodeAddrs.Contains(pkt.block.minerAddr) == false) return;

            base.OnNewBlock(sender, pkt);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "51_ATTACK_DEMO";
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            var miner = new BadNode();
            miner.Start();

            try
            {
                var peers = miner.peers;

                while (true)
                {
                    var addr = Console.ReadLine();

                    peers.ConnectPeer(addr);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject);
        }
    }
}
