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
            //Copyright.PrintLogo();

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
                        miner.SendTransaction(miner.wallet.CreatePaymentTransaction("ASDF", 0.1));
                    else if (addr == "C")
                    {
                        var tx = miner.wallet.CreateDeployTransaction(
                                "0xmH4sIAAAAAAAEALVTW4vTQBjN7ObS9LK97KIiIn0QESlhLz4t9KHt0yKrPogvIiVJhzbsdKYkk5X9kSqLiIi4y7KIiD9C50wzkqA+msLpcM6ZM9/3TWIRy7J+qgf/eJobCsbLhCenyyAWKR30X9A0SwQf7gW7+A36k5zJPKVDTnOZhmzQf5ZHLIkf07Pn4oTyIc8ZcxBYL3JG4yPE+rHgakMsM29J5ULMMtduF5ZjTbx8BZ/2bgJsBV2skOY6WIw8kJqAz25VAvRWF+ApcLD6iyFL5jxECw5VBZ0RqwbarSmojQ4Pp7EUqT4EIchwfaQpKYrqatWw/tfYPGKa66G5XpF4xDOZ5rFUedXh+E6lYR8t+CjWR51+A9AEtABbgDagA+gCcI6/DdhBj0j+x6l2LGbUEyuahnxmb5i5Pl2BN/fm/FBTqUroxjsNWU6n0/Wk9bRvIHJv/+ARQSP6kr+rvdiPStxbGLQa+ZyJKGTTkDjG9q2w6ZjbYPaJa8SvZfEOmAN9jVq8LkTMxb37+05JzRiuyoY+3p6J4JlgNFilCZfEN8bLwqhfc1I39EVBW5puGPpLOfaegs5kESY8mFM5ZiI+eSJI03g/l733/yyhZYyfCiMG6T4AE5EtI34siw/BhKRtxA+FeBPiYC12jPi+LAbr2K4Rz8vF7eJ7OQ7lIliJ16RnPO8qo9k29NsKvWPoN+WJNX4BmBrfqZAEAAA=",
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
