using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using minichain;

namespace minichain.test
{
    [TestClass]
    public class StateDB_test
    {
        [TestMethod]
        public void PushState()
        {
            var sdb = new StateDB(new MemDB());

            var addr_a = UniqID.Generate();
            var addr_b = UniqID.Generate();

            var state = sdb.PushState(Hash.ZeroAddress, Hash.Calc("1"), new PushStateEntry[]
            {
                PushStateEntry.Create(PushStateFlag.None, 
                     new SingleState(StateType.Wallet)
                     {
                         key = addr_a,
                         balance = 100
                     }),
                PushStateEntry.Create(PushStateFlag.None,
                     new SingleState(StateType.Wallet)
                     {
                         key = addr_b,
                         balance = 200
                     }),
            });

            Assert.AreEqual(100, sdb.GetState(Hash.Calc("1"), addr_a).balance);
            Assert.AreEqual(200, sdb.GetState(Hash.Calc("1"), addr_b).balance);

            state = sdb.PushState(state, Hash.Calc("2"), new PushStateEntry[]
            {
                PushStateEntry.Create(PushStateFlag.None,
                     new SingleState(StateType.Wallet)
                     {
                         key = addr_a,
                         balance = 200
                     })
            });

            Assert.AreEqual(200, sdb.GetState(Hash.Calc("2"), addr_a).balance);
            Assert.AreEqual(200, sdb.GetState(Hash.Calc("2"), addr_b).balance);
        }

        [TestMethod]
        public void GetPreviousData()
        {
            var sdb = new StateDB(new MemDB());

            var addr_a = UniqID.Generate();

            var stateA = sdb.PushState(Hash.ZeroAddress, Hash.Calc("1"), new PushStateEntry[]
            {
                PushStateEntry.Create(PushStateFlag.None,
                     new SingleState(StateType.Wallet)
                     {
                         key = addr_a,
                         balance = 100
                     })
            });
            var stateB = sdb.PushState(stateA, Hash.Calc("2"), new PushStateEntry[]
            {
                PushStateEntry.Create(PushStateFlag.None,
                     new SingleState(StateType.Wallet)
                     {
                         key = addr_a,
                         balance = 200
                     })
            });
            var stateC = sdb.PushState(stateB, Hash.Calc("3"), new PushStateEntry[]
            {
                PushStateEntry.Create(PushStateFlag.None,
                     new SingleState(StateType.Wallet)
                     {
                         key = addr_a,
                         balance = 300
                     })
            });

            Assert.AreEqual(100, sdb.GetState(Hash.Calc("1"), addr_a).balance);
            Assert.AreEqual(200, sdb.GetState(Hash.Calc("2"), addr_a).balance);
            Assert.AreEqual(300, sdb.GetState(Hash.Calc("3"), addr_a).balance);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NoDuplicatedEntry()
        {
            var sdb = new StateDB(new MemDB());

            var prev = sdb.PushState(Hash.ZeroAddress, Hash.Calc("1"), new PushStateEntry[]
            {
                PushStateEntry.Create(PushStateFlag.NewAddressOnly,
                    new SingleState(StateType.Wallet)
                    {
                        key = Hash.Calc("RANDOM_ADDRESS"),
                        balance = 0
                    })
            });

            sdb.PushState(prev, Hash.Calc("2"), new PushStateEntry[]
            {
                PushStateEntry.Create(PushStateFlag.NewAddressOnly,
                    new SingleState(StateType.Wallet)
                    {
                        key = Hash.Calc("RANDOM_ADDRESS"),
                        balance = 0
                    })
            });
        }
    }
}
