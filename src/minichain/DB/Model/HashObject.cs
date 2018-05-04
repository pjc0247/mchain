using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    public class HashObject
    {
        public virtual string hash { get; protected set; }

        /// <summary>
        /// For internal use
        /// </summary>
        internal static HashObject Empty()
        {
            return new HashObject()
            {
                hash = Hash.Calc("0")
            };
        }
    }
}
