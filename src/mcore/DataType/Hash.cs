using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public partial struct Hash
{
    public static readonly Hash ZeroAddress;

    private static readonly int ValidAddrLength = -1;

    public int Length => addr.Length;
    public string str => addr;

    private string addr;

    static Hash()
    {
        ValidAddrLength = Hash.Calc("0").addr.Length;
        ZeroAddress = new Hash(new string('0', ValidAddrLength));
    }
    public static bool IsValidAddress(string addr)
    {
        if (string.IsNullOrEmpty(addr))
            return false;
        if (ValidAddrLength == -1)
            return true;
        if (addr.Length != ValidAddrLength)
            return false;

        return Regex.IsMatch(addr, @"[a-zA-Z0-9]");
    }

    public Hash(string _addr)
    {
        if (string.IsNullOrEmpty(_addr))
            _addr = Hash.ZeroAddress;
        if (IsValidAddress(_addr) == false)
            throw new ArgumentException(nameof(_addr) + " / " + _addr);

        addr = _addr;
    }

    public static implicit operator string(Hash d)
    {
        return d.addr;
    }
    public static implicit operator Hash(string d)
    {
        return new Hash(d);
    }
}
