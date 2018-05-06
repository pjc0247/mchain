using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using minichain;

namespace minichain
{
    [TestClass]
    public class udouble_test
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NegativeNotAccepted()
        {
            udouble a = -1;
        }
        [TestMethod]
        public void ZeroIsAccepted()
        {
            udouble a = 0;
        }

        [TestMethod]
        public void BasicMath()
        {
            udouble a = 2;
            udouble b = 1;

            Assert.AreEqual(2 * 1, a * b);
        }

        [TestMethod]
        public void Compare()
        {
            udouble a = 2;
            udouble b = 1;

            Assert.AreEqual(true, a > b);
            Assert.AreEqual(false, a < b);

            udouble c = 2;

            Assert.AreEqual(true, a == c);
        }
    }
}
