using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using minichain;

namespace minichain.test
{
    [TestClass]
    public class Hash_test
    {
        [TestMethod]
        public void Equality()
        {
            Assert.AreEqual(Hash.ZeroAddress, Hash.ZeroAddress);
            Assert.AreEqual(Hash.Calc("1"), Hash.Calc("1"));
            Assert.AreNotEqual(Hash.Calc("2"), Hash.Calc("1"));
        }

        [TestMethod]
        public void HasSameLength()
        {
            Assert.AreEqual(Hash.ZeroAddress.Length, Hash.Calc("2").Length);
            Assert.AreEqual(Hash.Calc("1").Length, Hash.Calc("2").Length);
            Assert.AreEqual(Hash.Calc("1").Length, Hash.Calc("3").Length);
            Assert.AreEqual(Hash.Calc("3").Length, Hash.Calc("4").Length);
        }
    }
}
