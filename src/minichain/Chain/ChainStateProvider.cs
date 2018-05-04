using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using minivm;

namespace minichain
{
    internal class ChainStateProvider : IStateProvider
    {
        public int blockNo => state.currentBlock.blockNo;

        private ChainState state;

        public ChainStateProvider(ChainState _state)
        {
            state = _state;
        }

        public object GetState(string key)
        {
            throw new NotImplementedException();
        }
        public void SetState(string key, object value)
        {
            throw new NotImplementedException();
        }
    }
}
