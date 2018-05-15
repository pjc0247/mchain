using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    /// <summary>
    /// Basic KEY/VALUE storage
    /// </summary>
    public class KeyValueDB
    {
        private string prefix = "";
        private IStorageBackend db;

        public KeyValueDB()
        {
            db = new FileDB();
        }
        public KeyValueDB(string _prefix, IStorageBackend _db)
        {
            prefix = _prefix;
            db = _db;
        }

        public void Set(string key, object value)
        {
            db.Write(prefix + key, value);
        }
        public T Get<T>(string key)
        {
            return db.Read<T>(prefix + key);
        }
        public string Get(string key)
        {
            return db.Read<string>(prefix + key);
        }
    }
}
