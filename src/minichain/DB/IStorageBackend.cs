using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    public interface IStorageBackend
    {
        void Write(string key, object value);
        T Read<T>(string key);
        void Stash(string key);
    }
}
