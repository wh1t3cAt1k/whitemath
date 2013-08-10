using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace whiteMath
{
    /// <summary>
    /// Standard calculator for integers.
    /// </summary>
    public class CalcBigInteger : ICalc<BigInteger>
    {
        public bool isIntegerCalculator     { get { return true; } }

        public BigInteger sum(BigInteger one, BigInteger two)    { return one + two; }
        public BigInteger dif(BigInteger one, BigInteger two)    { return one - two; }
        public BigInteger mul(BigInteger one, BigInteger two)    { return one * two; }
        public BigInteger div(BigInteger one, BigInteger two)    { return one / two; }
        public BigInteger rem(BigInteger one, BigInteger two)    { return one % two; }
        public BigInteger negate(BigInteger one)          { return -one; }

        public BigInteger increment(BigInteger one)       { return ++one; }
        public BigInteger decrement(BigInteger one)       { return --one; }

        public bool mor(BigInteger one, BigInteger two)   { return one > two; }
        public bool eqv(BigInteger one, BigInteger two)   { return one == two; }

        public BigInteger intPart(BigInteger one)                 { return one; }

        public bool isEven(BigInteger one)                 { return one % 2 == 0; }
        public bool isNaN(BigInteger one)                  { return false; }
        public bool isPosInf(BigInteger one)               { return false; }
        public bool isNegInf(BigInteger one)               { return false; }

        public BigInteger getCopy(BigInteger val)                 { return val; }
        public BigInteger zero                             { get { return 0; } }

        public BigInteger fromInt(long equivalent)         { return (BigInteger)equivalent; }
        public BigInteger fromDouble(double equivalent)    { throw new NonFractionalTypeException("BigInteger"); }

        public BigInteger parse(string value) { return BigInteger.Parse(value); }
    }
}
