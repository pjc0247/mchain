using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    internal class Sig2Hash
    {
        public static string Field(string contractAddr, string fieldSignature)
        {
            return Hash.Calc2(contractAddr, fieldSignature);
        }
    }
}
