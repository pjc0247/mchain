using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    internal class Sig2Hash
    {
        public static Hash ANS(Hash name)
        {
            return Hash.Calc2("ANS", name);
        }

        public static Hash Field(Hash contractAddr, Hash fieldSignature)
        {
            return Hash.Calc2(contractAddr, fieldSignature);
        }
    }
}
