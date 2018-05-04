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
                    else if (addr == "C")
                    {
                        miner.SendTransaction(
                            miner.wallet.CreateDeployTransaction(
                                "0xmH4sIAAAAAAAEALVS3UsbQRy81fvI5cN8ICqllFCKiISjtW+BPCR5CqLtg/RFSrhcFnN42Q13e4p/pxYppVQRKaX0b9CdzW25Q/vYC0yWmdm5nd+eQQzDeJAP/vFUVyQM5iELz+ZewGPaaX+icRJy1nvnvcWv0x6mkUhj2mM0FbEfddof00kUBvv04oifUtZjaRRZCCxnOf3BCLFuwJncEIjEmVMx49PENuuZ5UARx5/hU95VgCmhiRXSbAuLvgNSEfCZtUKA2moDHAkWVs8YkvCE+ahgUXmgC2KUQNslCaV+tzsOBI/VSxCCDNtFmpQmE5gqxv8am0N0uQbKtbLEEUtEnAZC5hWH41qFwi5O5+KwbhlQAVQBNcAaoA5AutsEtNAMef94lxnwKXX4gsY+m5orepofFuD1bVl/5CyKEjo4Z36U0vF4OV8143XU2yM4u7rX33IjNitxA8x7YmnxVyaihr319wqIrQ0/84YXuOwhZwmPqLeIQybU/Snjfd74EuMdzvyQeSdUDCIenB5yUtLeu7z31dNQVxtvM6P61ElZ0zcZbSi6oukfGY2S9mswE1LV4ve8+AaMT2pa/JaJmxC3l+KaFr/mxZ1lbF2L1/kqu/i8D3wx8xb8nDS050uhRVPTVwW6penLfLnKI7bVLjw/BAAA",
                                "A::_ctor"));
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
