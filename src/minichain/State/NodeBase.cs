using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    /// <summary>
    /// Base class which provides basic Send/Recv methods.
    /// </summary>
    public class NodeBase
    {
        public bool isAlive { get; protected set; }

        public PeerPool peers { get; private set; }

        private Dictionary<Type, Action<Peer, object>> subscribers = new Dictionary<Type, Action<Peer, object>>();
        private ConcurrentSet<string> processedPackets = new ConcurrentSet<string>();

        public NodeBase()
        {
            peers = new PeerPool(this);
        }

        public virtual void Stop()
        {
            isAlive = true;
        }

        public void Subscribe<T>(Action<Peer, T> callback)
        {
            subscribers[typeof(T)] = (peer, pkt) =>
            {
                callback(peer, (T)pkt);
            };
        }
        public void ProcessPacket(Peer sender, PacketBase packet)
        {
            if (packet == null || string.IsNullOrEmpty(packet.pid)) return;
            // Since we're currently using p2p networking,
            //   same packet can be delivered more than once
            if (processedPackets.TryAdd(packet.pid) == false) return;

            if (subscribers.ContainsKey(packet.GetType()))
            {
                try
                {
                    subscribers[packet.GetType()]?.Invoke(sender, packet);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            if (packet is BroadcastPacket bpacket)
                BroadcastPacket(bpacket);
        }

        /// <summary>
        /// Send `BraodcastPacket` to all peers.
        /// This method includes `ttl` handling.
        /// </summary>
        private void BroadcastPacket(BroadcastPacket packet)
        {
            if (packet.ttl == 0) return;

            packet.ttl -= 1;
            SendPacketToAllPeers(packet);
        }
        protected void SendPacket(Peer sender, PacketBase packet)
        {
            sender.SendPacket(packet);
        }
        protected void SendPacketToAllPeers(PacketBase packet)
        {
            peers.SendPacketToAllPeers(packet);
        }
    }
}
