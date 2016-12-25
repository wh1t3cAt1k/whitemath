using System.Numerics;

namespace whiteMath.Calculators
{
    /// <summary>
    /// Standard calculator for integers.
    /// </summary>
    public class CalcBigInteger : ICalc<BigInteger>
    {
        public bool IsIntegerCalculator     { get { return true; } }

        public BigInteger Add(BigInteger one, BigInteger two)    { return one + two; }
        public BigInteger Subtract(BigInteger one, BigInteger two)    { return one - two; }
        public BigInteger Multiply(BigInteger one, BigInteger two)    { return one * two; }
        public BigInteger Divide(BigInteger one, BigInteger two)    { return one / two; }
        public BigInteger Modulo(BigInteger one, BigInteger two)    { return one % two; }
        public BigInteger Negate(BigInteger one)          { return -one; }

        public BigInteger Increment(BigInteger one)       { return ++one; }
        public BigInteger Decrement(BigInteger one)       { return --one; }

        public bool GreaterThan(BigInteger one, BigInteger two)   { return one > two; }
        public bool Equal(BigInteger one, BigInteger two)   { return one == two; }

        public BigInteger IntegerPart(BigInteger one)                 { return one; }

        public bool IsEven(BigInteger one)                 { return one % 2 == 0; }
        public bool IsNaN(BigInteger one)                  { return false; }
        public bool IsPositiveInfinity(BigInteger one)               { return false; }
        public bool IsNegativeInfinity(BigInteger one)               { return false; }

        public BigInteger GetCopy(BigInteger val)                 { return val; }
        public BigInteger Zero                             { get { return 0; } }

        public BigInteger FromInteger(long equivalent)         { return (BigInteger)equivalent; }
        public BigInteger FromDouble(double equivalent)    { throw new NonFractionalTypeException("BigInteger"); }

        public BigInteger Parse(string value) { return BigInteger.Parse(value); }
    }
}
