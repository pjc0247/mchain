﻿using System;
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
        private string contractAddr;
        private HashSet<PushStateEntry> changes;

        public void SetContext(
            ChainState _state,
            string _contractAddr,
            HashSet<PushStateEntry> _changes)
        {
            state = _state;
            contractAddr = _contractAddr;
            changes = _changes;
        }

        public object GetState(string key)
        {
            return state.GetState(key).value;
        }
        public void SetState(string key, object value)
        {
            key = Sig2Hash.Field(contractAddr, key);

            changes.Add(PushStateEntry.Create(PushStateFlag.None,
                new SingleState(StateType.Field)
                {
                    key = key,
                    value = value
                }));
        }
    }
}
