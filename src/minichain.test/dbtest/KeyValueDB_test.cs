using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using minichain;

namespace minichain.test
{
    [TestClass]
    public class KeyValueDB_test
    {
        [TestMethod]
        public void SaveAndLoad()
        {
            var db = new KeyValueDB();

            db.Set("a", "HELLO");
            db.Set("b", "BYE");

            Assert.AreEqual("HELLO", db.Get("a"));
            Assert.AreEqual("BYE", db.Get("b"));
        }
    }
}
