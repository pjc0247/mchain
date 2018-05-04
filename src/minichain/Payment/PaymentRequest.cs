using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    public class PaymentRequest
    {
        public string paymentId { get; private set; }

        public double value;
        public string dTag;

        public int firstConfirmedAt;

        public PaymentRequest()
        {
            paymentId = UniqID.Generate();
        }
    }
}
