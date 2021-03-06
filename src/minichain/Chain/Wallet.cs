﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace minichain
{
    public class WalletState : SingleState
    {
        public Hash address => key;

        public WalletState() : base(StateType.Wallet)
        {
        }
    }

    /// <summary>
    /// A wallet structure for Importing/Exporting.
    /// </summary>
    public class WalletParameter
    {
        public string address;
        public string publicKey;
        public string privateKey;
    }

    public class Wallet : WalletState
    {
        private string publicKey;
        private string privateKey;

        private ChainState chain;

        public Wallet(ChainState _chain)
        {
            RSA.GenerateKeyPair(out publicKey, out privateKey);

            chain = _chain;
            key = Hash.Calc(publicKey);
        }

        /// <summary>
        /// Imports wallet from json string
        /// </summary>
        public void Import(string json)
        {
            if (string.IsNullOrEmpty(json))
                throw new ArgumentException(nameof(json));

            var p = JsonConvert.DeserializeObject<WalletParameter>(json);

            if (IsValidKeyPair(p.privateKey, p.publicKey))
            {
                key = p.address;
                privateKey = p.privateKey;
                publicKey = p.publicKey;
            }
            else
                throw new ArgumentException("Invalid key pair");
        }


        /// <summary>
        /// Validates the input key pair
        /// </summary>
        /// <returns>returns true if valid</returns>
        private bool IsValidKeyPair(string privateKey, string publicKey)
        {
            var sign = RSA.SignWithPrivateKey(privateKey, "teststring");
            return RSA.VerifyWithPrivateKey(publicKey, "teststring", sign);
        }

        /// <summary>
        /// Exports current wallet to json string
        /// </summary>
        public string Export()
        {
            var p = new WalletParameter()
            {
                address = address,
                privateKey = privateKey,
                publicKey = publicKey
            };

            return JsonConvert.SerializeObject(p, Formatting.Indented);
        }

        public double GetBalanceInBlock(Hash blockHash)
        {
            return chain.GetBalanceInBlock(address, blockHash);
        }
        public double GetBalance()
        {
            return chain.GetBalance(address);
        }

        /// <summary>
        /// Creates a payment transaction signed by current wallet
        /// </summary>
        public Transaction CreatePaymentTransaction(Hash receiverAddr, double amount, double fee = 0)
        {
            var tx = new Transaction()
            {
                type = TransactionType.Payment,

                _out = amount,

                senderAddr = address,
                receiverAddr = receiverAddr,

                fee = fee
            };
            Sign(tx);
            return tx;
        }

        /// <summary>
        /// Creates a deploy transaction signed by current wallet
        /// </summary>
        public Transaction CreateDeployTransaction(Hash contractProgram, string ctorSignature, double fee = 0)
        {
            var contractAddr = Hash.Calc2(
                UniqID.Generate(),
                Hash.Calc2($"{chain.currentBlock.blockNo}", contractProgram));

            var tx = new Transaction()
            {
                type = TransactionType.Deploy,

                contractProgram = contractProgram,
                methodSignature = ctorSignature,

                senderAddr = address,
                receiverAddr = contractAddr,

                fee = fee
            };
            Sign(tx);
            return tx;
        }

        /// <summary>
        /// Creates a call transaction signed by current wallet
        /// </summary>
        public Transaction CreateCallTransaction(Hash contractAddr, string methodSignature, object[] args, double fee = 0)
        {
            var tx = new Transaction()
            {
                type = TransactionType.Call,

                methodSignature = methodSignature,
                callArgs = args,

                senderAddr = address,
                receiverAddr = contractAddr,

                fee = fee
            };
            Sign(tx);
            return tx;
        }

        public Transaction CreateRegisterANSTransaction(Hash targetAddress, string name, double fee = 0)
        {
            var tx = new Transaction()
            {
                type = TransactionType.RegisterANS,

                senderAddr = address,
                receiverAddr = targetAddress,
                ANSname = name,

                fee = fee
            };
            Sign(tx);
            return tx;
        }

        /// <summary>
        /// Signs a single transaction
        /// </summary>
        public void Sign(Transaction tx)
        {
            if (tx == null)
                throw new ArgumentNullException(nameof(tx));

            tx.Sign(privateKey, publicKey);
        }
    }
}
