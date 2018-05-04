using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    [Flags]
    public enum PushStateFlag
    {
        None,
        NewAddressOnly
    }

    public class PushStateEntry
    {
        public PushStateFlag flag { get; private set; }
        public SingleState state { get; private set; }

        public static PushStateEntry Create(PushStateFlag flag, SingleState state)
        {
            return new PushStateEntry()
            {
                flag = flag,
                state = state
            };
        }
    }
}
