using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

public partial struct Hash
{
    /// <summary>
    /// Calculates a hash with one value
    /// </summary>
    public static Hash Calc(string a)
    {
        if (a == null) a = "";

        // I know it is not a good hashing algorithm.
        var sha = SHA1.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(a));

        return string.Join("", hash.Select(x => x.ToString("x2")).ToArray());
    }

    /// <summary>
    /// Calculates a hash with two values
    /// </summary>
    public static Hash Calc2(string a, string b)
    {
        if (string.Compare(a, b) >= 0)
            return Calc(a + b);
        else
            return Calc(b + a);
    }

    public static Hash Calc3(string a, string b, string c)
    {
        return Calc2(Calc2(a, b), c);
    }

    public static Hash Calc(params object[] objs)
    {
        string g = Calc("");

        if (objs == null) return g;

        foreach (var obj in objs)
        {
            if (obj == null) continue;

            g = Calc2(g, obj.ToString());
            g = Calc2(g, obj.GetType().FullName);
        }

        return Calc2(objs.Length.ToString(), g);
    }
}