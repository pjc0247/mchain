using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace minichain
{
    internal class RSA
    {
        public static void GenerateKeyPair(out string publicKey, out string privateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);
            }
        }
        public static string SignWithPrivateKey(string privateKey, string message)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                byte[] buffer;
                rsa.FromXmlString(privateKey);
                buffer = rsa.SignData(Encoding.UTF8.GetBytes(message), new SHA1CryptoServiceProvider());
                return Convert.ToBase64String(buffer);
            }
        }
        public static bool VerifyWithPrivateKey(string publicKey, string message, string sign)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                return rsa.VerifyData(
                    Encoding.UTF8.GetBytes(message), new SHA1CryptoServiceProvider(), 
                    Convert.FromBase64String(sign));
            }
        }
    }
}
