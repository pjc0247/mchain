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

            try
            {
                var peers = miner.peers;

                while (true)
                {
                    var addr = Console.ReadLine();

                    if (addr == "P")
                        miner.SendTransaction(miner.wallet.CreatePaymentTransaction("ASDF", 0.1));
                    else
                        peers.AddPeer(addr);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void UpdateScreen(Miner miner)
        {
            while (true)
            {
                int backupX = Console.CursorLeft;
                int backupY = Console.CursorTop;

                Console.CursorLeft = 60;
                Console.CursorTop = 7;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"PEERS: {miner.peers.alivePeers}");

                Console.CursorLeft = backupX;
                Console.CursorTop = backupY;
                Console.ResetColor();

                Thread.Sleep(1000);
            }
        }
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject);
        }
    }
}
