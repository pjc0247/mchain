using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    /// <summary>
    /// Basic implementation for payment validation node.
    /// </summary>
    public class PaymentNode : EndpointNode
    {
        public static readonly int RequiredConfirmations = 6;

        public delegate void PaymentApprovalDelegate(string paymentId);

        public PaymentApprovalDelegate onPaymentApproval { get; set; }

        private Dictionary<string, PaymentRequest> pendingRequests = new Dictionary<string, PaymentRequest>();
        private Dictionary<string, PaymentRequest> awaitingConfirmations = new Dictionary<string, PaymentRequest>();

        public PaymentNode() : base()
        {
            chain.onBlockConfirmed += OnBlockConfirmed;
        }

        public PaymentRequest AddPaymentRequest(double value)
        {
            var req = new PaymentRequest()
            {
                dTag = UniqID.Generate(),
                value = value
            };

            pendingRequests.Add(req.dTag, req);

            return req;
        }

        private void OnBlockConfirmed(Block block)
        {
            ProcessAwaitingRequests();

            foreach (var tx in block.txs)
            {
                if (wallet.address != tx.receiverAddr)
                    continue;
                if (pendingRequests.ContainsKey(tx.dTag) == false)
                    continue;

                var req = pendingRequests[tx.dTag];
                if (req.value == tx._out)
                {
                    req.firstConfirmedAt = block.blockNo;
                    awaitingConfirmations[req.dTag] = req;
                }
            }
        }
        private void ProcessAwaitingRequests()
        {
            var blockNo = chain.currentBlock.blockNo;

            foreach (var req in awaitingConfirmations)
            {
                if (blockNo - req.Value.firstConfirmedAt >= RequiredConfirmations)
                {
                    pendingRequests.Remove(req.Key);
                    awaitingConfirmations.Remove(req.Key);

                    onPaymentApproval?.Invoke(req.Value.paymentId);
                }
            }
        }
    }
}
