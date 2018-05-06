using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    internal class ANSLookup
    {
        public static string Resolve(ChainState chain, string address)
        {
            if (address.StartsWith("*"))
            {
                var ret = chain.GetAddressFromANS(address);
                if (string.IsNullOrEmpty(ret))
                    throw new ArgumentException("Cannot find address from ANS");
                return ret;
            }
            return address;
        }
    }
}
