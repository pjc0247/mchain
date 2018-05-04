using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    public class SingleState : HashObject
    {
        public string key { get; set; }
        public double value { get; set; }

        public override string hash
        {
            get { return Hash.Calc2(key, value.ToString()); }
        }
    }
}
