﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    public class PaymentNodeConfig : NodeConfig
    {
        public int requiredConfirmations = 6;
    }
}
