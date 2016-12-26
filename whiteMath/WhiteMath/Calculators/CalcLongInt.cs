using WhiteMath.ArithmeticLong;

namespace WhiteMath.Calculators
{
    /// <summary>
    /// The default calculator for whiteMath long integer numbers.
    /// </summary>
    public class CalcLongInt<P>: ICalc<LongInt<P>> where P: IBase, new()
    {
        public bool IsIntegerCalculator { get { return true; } }

        public LongInt<P> Add(LongInt<P> one, LongInt<P> two) { return one + two; }
        public LongInt<P> Subtract(LongInt<P> one, LongInt<P> two) { return one - two; }
        public LongInt<P> Multiply(LongInt<P> one, LongInt<P> two) { return one * two; }
        public LongInt<P> Divide(LongInt<P> one, LongInt<P> two) { return one / two; }
        public LongInt<P> Modulo(LongInt<P> one, LongInt<P> two) { return one % two; }

        public LongInt<P> Increment(LongInt<P> one) { return ++one; }
        public LongInt<P> Decrement(LongInt<P> one) { return --one; }

        public LongInt<P> Negate(LongInt<P> num) { return -num; }

        public LongInt<P> IntegerPart(LongInt<P> num) { return num.Clone() as LongInt<P>; }

        public bool GreaterThan(LongInt<P> one, LongInt<P> two) { return one > two; }
        public bool Equal(LongInt<P> one, LongInt<P> two) { return one == two; }

        public bool IsEven(LongInt<P> num) { return num[0] % 2 == 0; }
        public bool IsNaN(LongInt<P> num) { return false; }
        public bool IsPositiveInfinity(LongInt<P> num) { return false; }
        public bool IsNegativeInfinity(LongInt<P> num) { return false; }

        public LongInt<P> GetCopy(LongInt<P> num) { return num.Clone() as LongInt<P>; }
        public LongInt<P> Zero { get { return new LongInt<P>(); } }

        public LongInt<P> FromInteger(long equivalent)      { return new LongInt<P>(equivalent); }
        public LongInt<P> FromDouble(double equivalent) { throw new NonFractionalTypeException("LongInt"); }

        public LongInt<P> Parse(string value) { return LongInt<P>.Parse(value); }
    }
}
