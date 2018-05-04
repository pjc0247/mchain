using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    public class CappedList<T> : List<T>
    {
        private int? maxCapacity { get; set; }

        public CappedList() { maxCapacity = null; }
        public CappedList(int capacity) { maxCapacity = capacity; }

        public new void Add(T newElement)
        {
            if (Count == (maxCapacity ?? -1)) RemoveAt(0);
            Add(newElement);
        }
    }
}
