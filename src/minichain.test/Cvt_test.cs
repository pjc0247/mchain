using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using minichain;

namespace minichain.test
{
    [TestClass]
    public class Cvt_test
    {
        [TestMethod]
        public void ToDoubleAndBack()
        {
            var original = "hello";
            var doubles = Cvt.StringToDouble(original);
            var back = Cvt.DoubleToString(doubles);
            Assert.AreEqual(original, back);

            original = "hello world!";
            doubles = Cvt.StringToDouble(original);
            back = Cvt.DoubleToString(doubles);
            Assert.AreEqual(original, back);

            original = "hello world! 한글가나다!@#!~＠★";
            doubles = Cvt.StringToDouble(original);
            back = Cvt.DoubleToString(doubles);
            Assert.AreEqual(original, back);
        }
    }
}
