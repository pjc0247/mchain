using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct udouble : IComparable
{
    public double _value;

    public udouble(double val)
    {
        if (val < 0)
            throw new ArgumentException("Value needs to be positive");
        _value = val;
    }

    public int CompareTo(object obj)
    {
        var other = (udouble)obj;

        if (_value == other._value)
            return 0;
        if (_value > other._value)
            return 1;
        return -1;
    }
    public override string ToString()
    {
        return _value.ToString();
    }

    public static implicit operator double(udouble d)
    {
        return d._value;
    }
    public static implicit operator udouble(double d)
    {
        if (d < 0)
            throw new ArgumentOutOfRangeException("Only positive values allowed");
        return new udouble(d);
    }
    public static implicit operator udouble(uint d)
    {
        return new udouble(d);
    }
}