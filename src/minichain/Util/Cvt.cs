using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    public class Cvt
    {
        public static double[] StringToDouble(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var padded = bytes.Concat(Enumerable.Repeat((byte)'\0',
                sizeof(double) - (bytes.Length % sizeof(double))
                )).ToArray();
            var vs = new double[(int)Math.Ceiling((double)bytes.Length / sizeof(double))];
            var offset = 0;

            for (int i=0;i<vs.Length;i++)
            {
                vs[i] = BitConverter.ToDouble(padded, offset);
                offset += sizeof(double);
            }

            return vs;
        }

        public static string DoubleToString(double[] vs)
        {
            var b = new byte[sizeof(double) * vs.Length];
            var offset = 0;

            foreach (var v in vs)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(v), 0, b, offset, sizeof(double));
                offset += sizeof(double);
            }

            return Encoding.UTF8.GetString(b).TrimEnd('\0');
        }
    }
}
