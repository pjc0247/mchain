using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    class Solver
    {
        public static string FindSolution(Block vblock, int start, int limit = 100000)
        {
            for (int i=start; i<start + limit; i++)
            {
                if (Block.IsValidNonce(vblock, i.ToString()))
                    return i.ToString();
            }

            return null;
        }
    }
}
