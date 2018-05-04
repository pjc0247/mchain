using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using minichain;

namespace minichain.test
{
    [TestClass]
    public class Block_test
    {
        [TestMethod]
        public void GenesisBlockHasAlwaysSameHash()
        {
            Assert.AreEqual(Block.GenesisBlock().hash, Block.GenesisBlock().hash);
        }
    }
}
