﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    public enum StateType
    {
        Empty,

        Wallet,

        Contract,
        Field,

        ANSLookup
    }

    public class SingleState : HashObject
    {
        public StateType type { get; set; }

        public Hash key { get; set; }
        public double balance { get; set; }
        public object value { get; set; }

        /// <summary>
        /// THIS IS NOT AN ADDRESS
        /// </summary>
        public override Hash hash
        {
            get { return Hash.Calc3(key, balance.ToString(), value?.ToString()); }
        }

        public SingleState(StateType _type)
        {
            type = _type;
        }
    }
}
