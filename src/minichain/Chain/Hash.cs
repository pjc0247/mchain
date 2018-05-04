using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace minichain
{
    class Hash
    {
        /// <summary>
        /// Calculates a hash with one value
        /// </summary>
        public static string Calc(string a)
        {
            // I know it is not a good hashing algorithm.
            var sha = SHA1.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(a));

            return string.Join("", hash.Select(x => x.ToString("x2")).ToArray());
        }

        /// <summary>
        /// Calculates a hash with two values
        /// </summary>
        public static string Calc2(string a, string b)
        {
            if (string.Compare(a, b) >= 0)
                return Calc(a + b);
            else
                return Calc(b + a);
        }
    }
}
