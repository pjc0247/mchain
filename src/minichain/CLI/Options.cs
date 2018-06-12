using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

namespace minichain
{
    [Verb("miner", HelpText = "Start node as a miner")]
    internal class MinerOptions
    {

    }

    [Verb("node", HelpText = "Start as a full-node")]
    internal class NodeOptions
    {

    }
}
