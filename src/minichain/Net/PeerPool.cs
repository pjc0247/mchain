﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using WebSocketSharp;
using WebSocketSharp.Server;

namespace minichain
{
    public class PeerPool
    {
        public delegate void PeerConnectedDelegate(Peer peer);
        public delegate void PeerLostDelegate(Peer peer);

        public static int MaxPeers = 4;

        public int alivePeers => peers.Count;
        public int listeningPort { get; }

        public PeerConnectedDelegate onPeerConnected;
        public PeerLostDelegate onPeerLost;

        private string[] localAddress;
        private string externalAddress;

        private NodeBase nodeBase;
        private ConcurrentDictionary<Peer, int> peers = new ConcurrentDictionary<Peer, int>();

        public PeerPool(NodeBase _nodeBase)
        {
            nodeBase = _nodeBase;

            listeningPort = new Random().Next(10000, 60000);
            var ws = new WebSocketServer(listeningPort);
            ws.AddWebSocketService("/", () => new Peer(this));
            ws.Start();

            localAddress = ExternalAddress.GetMyLocalIps();
            externalAddress = ExternalAddress.GetMyExternalIp();

            Console.WriteLine("RUNNING on port " + listeningPort);

            foreach (var addr in HardCodedSeeds.Addrs)
                ConnectPeer(addr);

            onPeerConnected += p =>
            {
                Console.WriteLine("[PEER] New peer " + p.address);
            };
        }

        public void ConnectPeer(string addr)
        {
            if (peers.Count >= MaxPeers) return;
            if (peers.Any(x => x.Key.address == addr)) return;

            if (addr.StartsWith("ws://") == false &&
                addr.StartsWith("wss://") == false)
                addr = "ws://" + addr;

            var uri = new Uri(addr);
            if ((uri.Host == "127.0.0.1" || uri.Host == "localhost" ||
                localAddress.Contains(uri.Host) || uri.Host == externalAddress) &&
                uri.Port == listeningPort)
            {
                return;
            }

            var ws = new WebSocket(addr);
            var peer = new Peer(this, ws);
            ws.Connect();
        }
        public void AddPeer(Peer peer)
        {
            peers.TryAdd(peer, 0);

            onPeerConnected?.Invoke(peer);
        }
        public void RemovePeer(Peer peer)
        {
            peers.TryRemove(peer, out _);

            onPeerLost?.Invoke(peer);
        }

        public void SendPacketToAllPeers(PacketBase packet)
        {
            foreach (var kv in peers)
            {
                var peer = kv.Key;
                try
                {
                    peer.SendRawPacket(packet.ToJson());
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        public string[] GetPeerAddresses()
        {
            return peers.Select(x => x.Key.address).ToArray();
        }

        public void ProcessPacket(Peer sender, PacketBase pkt)
        {
            nodeBase.ProcessPacket(sender, pkt);
        }

        public Peer[] Pick(int n)
        {
            return peers.Take(n).Select(x => x.Key).ToArray();
        }
    }
}
