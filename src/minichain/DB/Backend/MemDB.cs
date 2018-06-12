using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace minichain
{
    public class MemDB : IStorageBackend
    {
        private ConcurrentDictionary<string, string> mem = new ConcurrentDictionary<string, string>();

        public MemDB()
        {

        }

        public T Read<T>(string key)
        {
            string json;

            if (mem.TryGetValue(key, out json))
            {
                Console.WriteLine($"[DB::READ] {key}: \r\n       {json}");
                return Serializer.Deserialize<T>(json);
            }
            else
            {
                Console.WriteLine($"[DB::READ] {key}: \r\n       null");
                return default(T);
            }
        }
        public void Write(string key, object value)
        {
            var json = Serializer.Serialize(value);
            Console.WriteLine($"[DB::WRITE] {key}: \r\n       {json}");

            mem.TryAdd(key, json);
        }
        public void Stash(string key)
        {
            Console.WriteLine($"[DB::STASH] {key}");

            mem.TryRemove(key, out _);
        }
    }
}
