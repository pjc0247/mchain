using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using minichain;

namespace minichain.test
{
    [TestClass]
    public class Chain_test
    {
        [TestMethod]
        public void StartAsGenesis()
        {
            var chain = new ChainState();

            Assert.AreEqual(
                Block.GenesisBlock().hash,
                chain.currentBlock.hash);
        }
        [TestMethod]
        public void CannotPushSameBlock()
        {
            var chain = new ChainState();

            Assert.AreEqual(false, chain.PushBlock(Block.GenesisBlock()));
        }

        [TestMethod]
        public void GetByBlockNo()
        {
            var chain = new ChainState();
            var blocks = new List<Block>();

            for (int i = 0; i < 3; i++)
            {
                var block = CreateEmptyBlock(UniqID.Generate(), chain.currentBlock);
                blocks.Add(block);
                chain.PushBlock(block);
            }

            Assert.AreEqual(Block.GenesisBlock().hash, chain.GetBlock(0).hash);
            Assert.AreEqual(blocks[0].hash, chain.GetBlock(1).hash);
            Assert.AreEqual(blocks[1].hash, chain.GetBlock(2).hash);
            Assert.AreEqual(blocks[2].hash, chain.GetBlock(3).hash);
        }

        [TestMethod]
        public void RevertTo()
        {
            var chain = new ChainState();
            var wallet = new Wallet(chain);

            var sender = wallet.address;
            var receiverA = UniqID.Generate();

            var blocks = new List<Block>();

            for (int i=0;i<3;i++)
            {
                var block = CreateEmptyBlock(sender, chain.currentBlock);
                blocks.Add(block);
                chain.PushBlock(block);
            }

            Assert.AreEqual(3 * Consensus.CalcBlockReward(1), chain.GetBalance(sender));

            chain.PushBlock(CreatePaymentBlock(sender, chain.currentBlock, wallet, receiverA, 2.0));
            Assert.AreEqual(
                4 * Consensus.CalcBlockReward(1) - 2.0, 
                chain.GetBalance(sender));

            chain.RevertTo(blocks[2]);
            Assert.AreEqual(blocks[2].hash, chain.currentBlock.hash);
            Assert.AreEqual(3 * Consensus.CalcBlockReward(1), chain.GetBalance(sender));
        }
        private Block CreateEmptyBlock(string minerAddress, Block prevBlock)
        {
            return new Block(minerAddress, prevBlock, new Transaction[] {
                    Transaction.CreateRewardTransaction(0, minerAddress, new Transaction[] { })
                }, "0");
        }
        private Block CreatePaymentBlock(string minerAddress, Block prevBlock, Wallet wallet, string receiverAddress, udouble value)
        {
            return new Block(minerAddress, prevBlock, new Transaction[] {
                    Transaction.CreateRewardTransaction(0, minerAddress, new Transaction[] { }),
                    wallet.CreatePaymentTransaction(receiverAddress, value)
                }, "0");
        }
    }
}
