using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    public class RpcPacketBase : PacketBase
    {
    }

    public class RpcUnlock : RpcPacketBase
    {
        public string password;
    }
    public class RpcSendFund : RpcPacketBase
    {
        public string receiverAddr;
        public double amount;
    }
    public class RpcQueryBalance : RpcPacketBase
    {
    }
    public class RpcQueryWallet : RpcPacketBase
    {
    }

    public class RpcResponse : RpcPacketBase
    {
        public object result;
    }
}
