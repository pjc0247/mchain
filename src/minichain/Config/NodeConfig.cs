using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    public class NodeConfig
    {
        // RPC
        public bool useRpcServer = true;
        public int rpcListenPort = 4044;

        // TX_POOL
        public int maxTxpoolSize = 1024;
    }
}
