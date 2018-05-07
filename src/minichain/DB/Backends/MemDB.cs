using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace minichain
{
    public class MemDB : IStorageBackend
    {
        private Dictionary<string, string> mem = new Dictionary<string, string>();
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

                Console.WriteLine($"[DB::READ] {key}: \r\n       {mem[key]}");
                return Serializer.Deserialize<T>(mem[key]);
            }
        }
        public void Write(string key, object value)
        {
            lock (memLock)
            {
                mem[key] = Serializer.Serialize(value);

                if (value is SingleState ss)
                    Console.WriteLine($"[DB::WRITE] {key}: {ss.balance}");
                else
                    Console.WriteLine($"[DB::WRITE] {key}: \r\n       {mem[key]}");
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
