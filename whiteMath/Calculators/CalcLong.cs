using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath
{
    /// <summary>
    /// Standard calculator for integers.
    /// </summary>
    public class CalcLong : ICalc<long>
    {
        public bool isIntegerCalculator     { get { return true; } }

        public long sum(long one, long two)    { return one + two; }
        public long dif(long one, long two)    { return one - two; }
        public long mul(long one, long two)    { return one * two; }
        public long div(long one, long two)    { return one / two; }
        public long rem(long one, long two)    { return one % two; }
        public long negate(long one)          { return -one; }

        public long increment(long one)       { return ++one; }
        public long decrement(long one)       { return --one; }

        public bool mor(long one, long two)   { return one > two; }
        public bool eqv(long one, long two)   { return one == two; }

        public long intPart(long one)                 { return one; }

        public bool isEven(long one)                 { return one % 2 == 0; }
        public bool isNaN(long one)                  { return false; }
        public bool isPosInf(long one)               { return false; }
        public bool isNegInf(long one)               { return false; }

        public long getCopy(long val)                 { return val; }
        public long zero                             { get { return 0; } }

        public long fromInt(long equivalent)         { return (int)equivalent; }
        public long fromDouble(double equivalent)    { throw new NonFractionalTypeException("int"); }

        public long parse(string value) { return int.Parse(value); }
    }
}
