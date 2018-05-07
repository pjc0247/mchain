using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSocketSharp;
using WebSocketSharp.Server;

namespace minichain
{
    internal class RpcServer
    {
        public EndpointNode node { get; private set; }

        public RpcServer(EndpointNode _node, int port)
        {
            node = _node;

            var ws = new WebSocketServer(port);
            ws.ReuseAddress = true;
            ws.AddWebSocketService("/", () => new RpcSession(this));
            ws.Start();
        }
    }
}
