using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSocketSharp;
using WebSocketSharp.Server;

using Newtonsoft.Json;

namespace minichain
{
    internal class RpcServer
    {
        public EndpointNode node { get; private set; }

        private WebSocketServer ws;
        private Dictionary<string, Action<string>> handlers = new Dictionary<string, Action<string>>();

        public RpcServer(EndpointNode _node, int port)
        {
            node = _node;

            ws = new WebSocketServer(port);
            ws.ReuseAddress = true;
            ws.AddWebSocketService("/", () => new RpcSession(this));
        }

        public void Start()
        {
            ws.Start();
        }
        public void Stop()
        {
            if (ws.IsListening)
                ws.Stop();
        }

        public void RegisterHandler<T>(string type, Action<T> callback)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentException(nameof(type));

            handlers[type] = (json) => {
                callback(JsonConvert.DeserializeObject<T>(json));
            };
        }
        public void ProcessPacket(RpcSession receiver, string json)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            if (data == null || data.ContainsKey("type") == false) return;
            var type = (string)data["type"];

            if (handlers.ContainsKey(type) == false) return;

            handlers[type]?.Invoke(json);
        }
    }
}
