using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using minichain;

namespace minichain.test
{
    [TestClass]
    public class RSA_test
    {
        [TestMethod]
        public void EncryptAndDecrypt()
        {
            string publicKey, privateKey;

            RSA.GenerateKeyPair(out publicKey, out privateKey);

            var sign = RSA.SignWithPrivateKey(privateKey, "HELLO");
            var ret = RSA.VerifyWithPrivateKey(publicKey, "HELLO", sign);

            Assert.AreEqual(ret, true);
        }
    }
}
