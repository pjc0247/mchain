using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSocketSharp;
using WebSocketSharp.Server;

namespace minichain
{
    public class Peer : WebSocketBehavior
    {
        public string address
        {
            get {
                if (ws != null) return ws.Url.ToString();
                else return Context.WebSocket.Url.ToString();
            }
        }

        protected PeerPool pool;

        private WebSocket ws;

        public Peer(PeerPool _pool)
        {
            pool = _pool;
        }
        public Peer(PeerPool _pool, WebSocket _ws)
        {
            ws = _ws;
            pool = _pool;

            ws.OnOpen += (_,__) => OnOpen();
            ws.OnClose += (_, e) => OnClose(e);
            ws.OnMessage += (_, e) => OnMessage(e);
        }

        public void SendPacket(PacketBase pkt)
        {
            SendRawPacket(pkt.ToJson());
        }
        public void SendRawPacket(string json)
        {
            if (ws != null) ws.Send(json);
            else Context.WebSocket.Send(json);
        }

        protected override void OnOpen()
        {
            pool.AddPeer(this);
        }
        protected override void OnClose(CloseEventArgs e)
        {
            pool.RemovePeer(this);
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                var pkt = PacketBase.FromJson(e.Data);
                if (pkt == null) return;

                pool.ProcessPacket(this, pkt);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
