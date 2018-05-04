using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    /// <summary>
    /// https://en.bitcoin.it/wiki/Satoshi_Client_Node_Discovery#Hard_Coded_.22Seed.22_Addresses
    /// </summary>
    public class HardCodedSeeds
    {
        /// <summary>
        /// We don't have any onging seeds since this is not a production purpose chain.
        /// </summary>
        public static readonly string[] Addrs =
        {
 //           "1.2.3.4:6666",
 //           "5.6.7.8:7777",
        };
    }
}
