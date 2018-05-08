using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using minichain;

namespace minichain.test
{
    [TestClass]
    public class MemDB_test
    {
        [TestMethod]
        public void SaveAndLoad()
        {
            var db = new MemDB();

            db.Write("a", "HELLO");
            db.Write("b", "BYE");

            Assert.AreEqual("HELLO", db.Read<string>("a"));
            Assert.AreEqual("BYE", db.Read<string>("b"));
        }

        [TestMethod]
        public void StashAndRead()
        {
            var db = new MemDB();

            db.Write("a", "HELLO");
            db.Stash("a");

            Assert.AreEqual(null, db.Read<string>("a"));
        }
    }
}
