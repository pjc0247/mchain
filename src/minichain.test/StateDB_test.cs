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

            var state = sdb.PushState(null, "1", new PushStateEntry[]
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

            Assert.AreEqual(100, sdb.GetState("1", addr_a).balance);
            Assert.AreEqual(200, sdb.GetState("1", addr_b).balance);

            state = sdb.PushState(state, "2", new PushStateEntry[]
            {
                PushStateEntry.Create(PushStateFlag.None,
                     new SingleState(StateType.Wallet)
                     {
                         key = addr_a,
                         balance = 200
                     })
            });

            Assert.AreEqual(200, sdb.GetState("1", addr_a).balance);
            Assert.AreEqual(200, sdb.GetState("1", addr_b).balance);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NoDuplicatedEntry()
        {
            var sdb = new StateDB(new MemDB());

            var prev = sdb.PushState(null, "1", new PushStateEntry[]
            {
                PushStateEntry.Create(PushStateFlag.NewAddressOnly,
                    new SingleState(StateType.Wallet)
                    {
                        key = "ASDFASDFASDF",
                        balance = 0
                    })
            });

            sdb.PushState(prev, "2", new PushStateEntry[]
            {
                PushStateEntry.Create(PushStateFlag.NewAddressOnly,
                    new SingleState(StateType.Wallet)
                    {
                        key = "ASDFASDFASDF",
                        balance = 0
                    })
            });
        }
    }
}
