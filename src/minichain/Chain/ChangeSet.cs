using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    class ChangeSet
    {
        private HashSet<PushStateEntry> changes = new HashSet<PushStateEntry>();
        private HashSet<PushStateEntry> pendingChanges;

        private ChainState chainState;

        public ChangeSet(ChainState _chainState)
        {
            chainState = _chainState;
        }

        public void Begin()
        {
            pendingChanges = new HashSet<PushStateEntry>();
        }
        public void Commit()
        {
            foreach (var c in pendingChanges)
                changes.Add(c);
            pendingChanges = null;
        }
        /// <summary>
        /// Reverts all pending changes.
        /// </summary>
        public void Stash()
        {
            pendingChanges = null;
        }

        public void Add(PushStateEntry entry)
        {
            if (pendingChanges.Add(entry) == false)
                throw new InvalidOperationException(nameof(entry));
        }
        public void UpdateBalance(Hash addr, double delta)
        {
            if (delta == 0) return;
            
            // 1. from pending changes (current transaction)
            var wallet = pendingChanges
                .FirstOrDefault(x => x.state.key == addr)
                ?.state;

            // 2. from changes (other transactions)
            if (wallet == null)
            {
                wallet = changes
                    .FirstOrDefault(x => x.state.key == addr)
                    ?.state;
            }

            // 3. from chain
            if (wallet == null)
                wallet = chainState.GetState(addr);

            if (delta < 0 &&
                wallet.balance < delta)
                throw new BlockValidationException("Insufficient funds");

            wallet.balance += delta;
            pendingChanges.Add(PushStateEntry.Create(
                PushStateFlag.None, wallet));
        }

        public PushStateEntry[] ToArray()
        {
            return changes.ToArray();
        }
    }
}
