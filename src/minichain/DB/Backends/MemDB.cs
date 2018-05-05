using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace minichain
{
    public class MemDB : IStorageBackend
    {
        private Dictionary<string, object> mem = new Dictionary<string, object>();
        private object memLock = new object();

        public MemDB()
        {

        }

        public T Read<T>(string key)
        {
            lock (memLock)
            {
                if (mem.ContainsKey(key) == false)
                    return default(T);

                return (T)mem[key];
            }
        }
        public void Write(string key, object value)
        {
            lock (memLock)
            {
                mem[key] = value;

                if (value is SingleState ss)
                    Console.WriteLine($"[DB::WRITE] {key}: {ss.balance}");
                else
                    Console.WriteLine($"[DB::WRITE] {key}: {value}");
            }
        }

        public void Stash(string key)
        {
            lock (memLock)
            {
                if (mem.ContainsKey(key))
                    mem.Remove(key);

                Console.WriteLine($"[DB::STASH] {key}");
            }
        }
    }
}
