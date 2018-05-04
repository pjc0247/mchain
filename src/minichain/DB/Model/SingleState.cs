using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    public enum StateType
    {
        Wallet,

        Contract,
        Field
    }

    public class SingleState : HashObject
    {
        public StateType type { get; set; }

        public string key { get; set; }
        public double balance { get; set; }
        public object value { get; set; }

        /// <summary>
        /// THIS IS NOT AN ADDRESS
        /// </summary>
        public override string hash
        {
            get { return Hash.Calc3(key, balance.ToString(), value?.ToString()); }
        }

        public SingleState(StateType _type)
        {
            type = _type;
        }
    }
}
