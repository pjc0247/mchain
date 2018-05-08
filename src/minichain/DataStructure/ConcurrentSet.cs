using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    internal class ConcurrentSet<TKEY> : ConcurrentDictionary<TKEY, TKEY>
    {
        public bool TryAdd(TKEY key)
        {
            return TryAdd(key, key);
        }
    }
}
