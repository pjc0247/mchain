﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    public enum TransactionType
    {
        /// <summary>
        /// Send funds with _in & _out patameters.
        /// </summary>
        Payment,

        /// <summary>
        /// Create a new contract
        /// </summary>
        Deploy,

        /// <summary>
        /// Execute a single method from contract
        /// </summary>
        Call
    }

    public class TransactionHeader : HashObject
    {
        public TransactionType type;

        /// To give a chance user can modify the transaction
        ///    already distributed but not in chain.
        public int version;

        /// This will be used to validate the `sign`.
        public string publicKey;
        /// RSA encrypted sign to validate this transaction.
        public string encryptedSign;

        public string senderAddr, receiverAddr;

        public double fee;
        public double _in, _out;
        public string dTag;

        public bool isSigned =>
            !string.IsNullOrEmpty(publicKey) && !string.IsNullOrEmpty(encryptedSign);
    }

    public class TransactionBody : TransactionHeader
    {
        // TransactionType.Deploy
        public string contractProgram;

        // TransactionType.Call
        public string methodSignature;
        public object[] callArgs;
    }

    public class Transaction : TransactionBody
    {
        internal static Transaction EmptyTransaction()
        {
            return new Transaction() { hash = "0000000000000000" };
        }
        public static bool IsValidTransaction(Transaction tx)
        {
            if (tx.senderAddr == Consensus.RewardSenderAddress) return false;
            if (tx._out == 0) return false;
            if (tx.isSigned == false) return false;

            if (Hash.Calc(tx.publicKey) != tx.senderAddr)
                return false;
            if (RSA.VerifyWithPrivateKey(tx.publicKey, tx.GetTransactionSigniture(), tx.encryptedSign) == false)
                return false;

            return true;
        }
        /// <summary>
        /// Validates the transaction is actually included in the block.
        /// </summary>
        public static bool IsValidTransactionDeep(Transaction tx, 
            BlockHeader blockHeader, string[] merkleRoute)
        {
            if (IsValidTransaction(tx) == false) return false;

            var accHash = tx.hash;
            foreach (var hash in merkleRoute)
                accHash = Hash.Calc2(accHash, hash);

            return accHash == blockHeader.merkleRootHash;
        }

        /// <summary>
        /// Creates a reward transaction.
        /// This always be located at txs[0] to mined block.
        /// </summary>
        /// <param name="txs">Other transactions which included in this block.</param>
        public static Transaction CreateRewardTransaction(int blockNo, string minerAddr, Transaction[] txs)
        {
            var totalFee = txs.Sum(x => x.fee);

            return new Transaction()
            {
                senderAddr = Consensus.RewardSenderAddress,
                receiverAddr = minerAddr,
                fee = 0,
                _out = Consensus.CalcBlockReward(blockNo) + totalFee
            };
        }

        public Transaction()
        {
            hash = UniqID.Generate();
            type = TransactionType.Payment;
        }

        public string GetTransactionSigniture()
        {
            return Hash.Calc(senderAddr + receiverAddr + _in + _out + fee + version);
        }

        /// <summary>
        /// Signs this transaction, 
        /// Only signed transaction can be accepted in chain.
        /// 
        /// This makes attacker cannot create fake/manipulated transaction.
        /// 
        ///    * ONLY wallet owner can make valid transaction with PRIVATE KEY.
        ///    * EVERYONE can validation the transaction with PUBLIC KEY.
        /// </summary>
        public void Sign(string _privateKey, string _publicKey)
        {
            var original = GetTransactionSigniture();

            encryptedSign = RSA.SignWithPrivateKey(_privateKey, original);
            publicKey = _publicKey;
        }
    }
}
