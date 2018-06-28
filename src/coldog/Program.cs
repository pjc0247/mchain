using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using CommandLine;
using minichain;

namespace coldog
{
    [Verb("create", HelpText = "Create a new wallet.")]
    class CreateWalletOptions
    {
        [Option('o', "output", Required = true, HelpText = "output path.")]
        public string outPath { get; set; }
    }

    [Verb("transfer", HelpText = "Create a trasfer transaction.")]
    class TransferOptions
    { 
        [Option('i', "wallet", Required = false, HelpText = "input wallet file.")]
        public string inputWallet { get; set; }
        [Option('o', "tx", Required = false, HelpText = "output tx file.")]
        public string outPath { get; set; }

        [Option('r', "addr", Required = true, HelpText = "output tx file.")]
        public string receiverAddress { get; set; }
        [Option('v', "amount", Required = true, HelpText = "output tx file.")]
        public double amount { get; set; }
    }

    [Verb("broadcast", HelpText = "Broadcast a transaction")]
    class BroadcastOptions
    {
        [Option('i', "input", Required = false, HelpText = "input transaction file.")]
        public string inPath { get; set; }
    }

    class Program
    {
        private static EndpointNode node;

        static int Main(string[] args)
        {
            node = new EndpointNode();

            return Parser.Default.ParseArguments<CreateWalletOptions, TransferOptions, BroadcastOptions>(args)
                .MapResult(
                (CreateWalletOptions x) => CreateWallet(x),
                (TransferOptions x) => Transfer(x),
                (BroadcastOptions x) => Broadcast(x),
                errs => 1);
        }

        private static int CreateWallet(CreateWalletOptions opts)
        {
            File.WriteAllText(opts.outPath, node.wallet.Export());

            return 0;
        }
        private static int Transfer(TransferOptions opts)
        {
            node.wallet.Import(File.ReadAllText(opts.inputWallet));

            var tx = node.wallet.CreatePaymentTransaction(opts.receiverAddress, opts.amount);

            File.WriteAllText(opts.outPath, tx.Export());

            return 0;
        }
        private static int Broadcast(BroadcastOptions opts)
        {
            if (File.Exists(opts.inPath) == false)
            {
                Console.WriteLine($"{opts.inPath} file does not exists!");
                return -1;
            }

            // TODO: connect to peers

            var tx = Transaction.Import(File.ReadAllText(opts.inPath));
            node.SendTransaction(tx);

            return 0;
        }
    }
}
