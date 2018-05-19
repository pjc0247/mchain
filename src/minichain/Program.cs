using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace minichain
{
    class Program
    { 
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Title = "minichain";
            Copyright.PrintLogo();

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            var miner = new Miner();
            Console.WriteLine("==============THIS IS A YOUR WALLET DATA==============");
            Console.WriteLine(miner.wallet.Export());
            miner.Start();

            Console.Title = "minichain: " + miner.peers.listeningPort;

            try
            {
                var peers = miner.peers;
                var ctxAddr = "";

                while (true)
                {
                    var addr = Console.ReadLine();

                    if (addr == "P")
                        miner.SendTransaction(miner.wallet.CreatePaymentTransaction(ctxAddr, 1.1));
                    else if (addr == "C")
                    {
                        Console.WriteLine("CreateCC");
                        var tx = miner.wallet.CreateDeployTransaction(
                                "0xmH4sIAAAAAAAEALVSS0vDQBjMah5Nn9TXQUSK51Kseir00PZURPQgXkRKkq42ut0teQj+V0VExAci/gPdWbPQgB7dwmQ73+zkm29jEMMwvuTCE6u8IKE/DXl4PW0FIqLNxgmN4lDwbru1jV+zMUhZkka0y2maRB5rNo5Sn4XBPr05FleUd3nKmAXDYubT6w9h6waCywNBEjtTmkzEOLbNWiY5UMTpGXRKuwgwJdSxg5ttYdNzQCoCOrOSM1BHbYAjwcLuF0EcXnAPESwqG7ohRgG0XZBQ6HU6oyARkXoJTOBhu3CTJd/H35LxX2NziA5XQbilzHHI4yRKg0T65YfjWrnALiK4aNYtAkqAMgB2bhVQQxQY/GFuBmJMHTGjkcfH5oIe3+EMvL4e61OGz5fQtHPtsZSORj8DVUOtw7K9s7tH0K+6yw95FudXUF/GPOVkL5jwPTbyiKVl75lM2awCLieUMUFsrXjLFOpbU5el6NccXdD0S0YbinY1/ZzRa3jDOhifFHXxab64AcYjJV18nG9wU+WUi5R1/WG+3gDTJhVdvM+KcLO3JFQHEy/kLflp8PicRqSqlXe5ODVN387HKX0DIRr0dckDAAA=",
                                "A::_ctor");
                        miner.SendTransaction(tx);
                        ctxAddr = tx.receiverAddr;
                    }
                    else if (addr == "CC")
                    {
                        miner.SendTransaction(
                            miner.wallet.CreateCallTransaction(
                                ctxAddr, "A::bb", new object[] {1, 2}));
                    }
                    else if (addr == "DD")
                    {
                        var r = miner.chain.GetPublicField(ctxAddr,
                            minivm.ABISignature.Field("A", "global_a"));

                        Console.WriteLine(r);
                    }
                    else if (addr == "qq")
                    {
                        var tx = miner.wallet.CreateRegisterANSTransaction(
                            "AAAAA", "*zuzu");
                        miner.SendTransaction(tx);
                    }
                    else if (addr == "ww")
                    {
                        var tx = miner.wallet.CreatePaymentTransaction(
                            "*zuzu", 1);
                        miner.SendTransaction(tx);
                    }
                    else
                        peers.AddPeer(addr);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject);
        }
    }
}
