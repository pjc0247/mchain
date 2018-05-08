using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    public class BlockValidationException : Exception
    {
        public BlockValidationException(string msg ):base(msg)
        {
        }
    }
}
