using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using WebSocketSharp;
using WebSocketSharp.Server;

namespace minichain
{
    class RpcSession : WebSocketBehavior
    {
        private RpcServer server;

        private bool isUnlocked = false;

        public RpcSession(RpcServer _server)
        {
            server = _server;
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                //server.ProcessPacket(this, e.Data);

                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(e.Data);
                if (data == null || data.ContainsKey("type") == false) return;
                var type = (string)data["type"];

                if (type == "unlock") OnUnlock(JsonConvert.DeserializeObject<RpcUnlock>(e.Data));
                else if (isUnlocked == false) AbortSession();
                else if (type == "query_balance") OnQueryBalance(JsonConvert.DeserializeObject<RpcQueryBalance>(e.Data));
                else if (type == "query_wallet") OnQueryWallet(JsonConvert.DeserializeObject<RpcQueryWallet>(e.Data));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SendPacket(RpcPacketBase pkt)
        {
            Send(pkt.ToJson());
        }
        private void AbortSession()
        {
            Context.WebSocket.Close();
        }
        private void OnUnlock(RpcUnlock pkt)
        {
            // fixme
            if (pkt.password != "asdfasdf")
                AbortSession();
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[[RPC]] Account unlocked");
                Console.ResetColor();
                isUnlocked = true;
            }
        }
        private void OnQueryBalance(RpcQueryBalance pkt)
        {
            var balance = server.node.wallet.GetBalance();

            SendPacket(new RpcResponse()
            {
                pid = pkt.pid,
                result = balance
            });
        }
        private void OnQueryWallet(RpcQueryWallet pkt)
        {
            var wallet = server.node.wallet;

            SendPacket(new RpcResponse()
            {
                pid = pkt.pid,
                result = new WalletState()
                {
                    key = wallet.address,
                    balance = wallet.balance
                }
            });
        }
    }
}
