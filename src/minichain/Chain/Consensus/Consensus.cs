using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    public class Consensus
    {
        public static int TrustedConfirmations = 30;

        public static int MaxTransactionsPerBlock = 16;
        public static string RewardSenderAddress = Hash.ZeroAddress;

        public static double CalcBlockReward(int blockNo)
        {
            // You can implement dynamic block rewards
            //   like code below:
#if JUST_AN_EXAMPLE
            if (blockNo < 100)
                return 1;
            else if (blockNo < 200)
                return 0.5;
            else if (blockNo < 300)
                return 0.25;
            else return 0; // No reward after block#300
#endif

            // But, we just use static reward at this time.
            return 1;
        }
        public static int CalcBlockDifficulty(int blockNo)
        {
            // You can implement dynamic block difficulty
            //   like code below:
#if JUST_AN_EXAMPLE
            if (blockNo < 100)
                return 5;
            else if (blockNo < 200)
                return 6;
            else if (blockNo < 300)
                return 7;
            else return 8;
#endif

            // But, we just use static difficulty at this time.
            return 5;
        }
    }
}
