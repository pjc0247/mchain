using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    /// Inefficient, but human-readable state tree database
    /// 
    /// Why we should use state-tree based DB
    ///    * Saves only the changes on the disk.
    ///    * Can easily revert state to specific point.
    ///    * Can read state from specific point.
    /// 
    /// There are 3 components in StateDB
    ///    * Header
    ///    * Indexed path
    ///    * State
    ///  
    /// see more details:
    /// https://github.com/pjc0247/minichain_state_db
    public class StateDB
    {
        class DataHeader
        {
            public Dictionary<string, string> path;

            public static DataHeader EmptyState()
            {
                return new DataHeader()
                {
                    path = new Dictionary<string, string>()
                };
            }
        }

        private IStorageBackend db;

        public StateDB()
        {
            db = new FileDB();
        }
        public StateDB(IStorageBackend _db)
        {
            db = _db;
        }

        private DataHeader ReadHeader(string stateRoot)
        {
            var header = db.Read<DataHeader>($"root/{stateRoot}");
            if (header == null) return DataHeader.EmptyState();
            return header;
        }
        private string WriteHeader(string stateRoot, DataHeader header)
        {
            db.Write($"root/{stateRoot}", header);
            return stateRoot;
        }

        private SingleState[] ReadStateBlob(string index, string uid)
        {
            return db.Read<SingleState[]>($"chain/{index}/{uid}");
        }
        private string WriteStateBlob(string index, SingleState[] wallets)
        {
            var uid = UniqID.Generate();
            db.Write($"chain/{index}/{uid}", wallets);
            return uid;
        }

        private string GetIndexFromHash(string address)
        {
            return address.Substring(0, 2);
        }
        public SingleState GetState(string stateRoot, string address)
        {
            var header = ReadHeader(stateRoot);
            var index = GetIndexFromHash(address);

            // An address that has never appeared in the chain.
            if (header.path.ContainsKey(index) == false)
                goto EmptyAccount;
            var wallets = ReadStateBlob(index, header.path[index]);

            foreach (var wallet in wallets)
            {
                if (wallet.key == address)
                    return wallet;
            }

            // Empty account
            EmptyAccount:
            return new SingleState(StateType.Wallet)
            {
                key = address,
                balance = 0.0
            };
        }

        /// <summary>
        /// Pushes the changes into database.
        /// </summary>
        public string PushState(string prevStateRoot, string stateRoot, PushStateEntry[] changedStates)
        {
            DataHeader header = null;

            if (prevStateRoot == null)
                header = DataHeader.EmptyState();
            else header = ReadHeader(prevStateRoot);

            var changes = new Dictionary<string, Dictionary<string, SingleState>>();
            foreach (var entry in changedStates)
            {
                var state = entry.state;
                var index = GetIndexFromHash(state.key);

                if (changes.ContainsKey(index) == false)
                {
                    if (header.path.ContainsKey(index))
                    {
                        changes[index] = ReadStateBlob(index, header.path[index])
                            .ToDictionary(x => x.key, x => x);
                    }
                    else 
                        changes[index] = new Dictionary<string, SingleState>();
                }

                if ((entry.flag & PushStateFlag.NewAddressOnly) != 0)
                {
                    if (changes[index].ContainsKey(state.key))
                        throw new InvalidOperationException(
                            "PushStateFlag.NewAddressOnly: " +
                            state.key);
                }

                changes[index][state.key] = state;
            }

            foreach (var change in changes)
            {
                var states = change.Value.Select(x => x.Value).ToArray();
                header.path[change.Key] = WriteStateBlob(change.Key, states);
            }

            return WriteHeader(stateRoot, header);
        }
    }
}
