using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using minichain;

namespace minichain.test
{
    [TestClass]
    public class StateDB_test
    {
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
