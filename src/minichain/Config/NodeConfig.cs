using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    // YOU CAN OVERRIDE THIS CLASS TO
    //   WRITE EXTENDED CONFIG FOR YOUR 
    //   CUSTOM NODE.
    //
    // SEE `MinerNodeConfig.cs`.

    public class NodeConfig
    {
        // RPC
        public bool useRpcServer = true;
        public int rpcListenPort = 4044;

        // TX_POOL
        public int maxTxpoolSize = 1024;
    }
}
