namespace whiteMath.Calculators
{
    /// <summary>
    /// Standard calculator for integers.
    /// </summary>
    public class CalcLong : ICalc<long>
    {
        public bool IsIntegerCalculator     { get { return true; } }

        public long Add(long one, long two)    { return one + two; }
        public long Subtract(long one, long two)    { return one - two; }
        public long Multiply(long one, long two)    { return one * two; }
        public long Divide(long one, long two)    { return one / two; }
        public long Modulo(long one, long two)    { return one % two; }
        public long Negate(long one)          { return -one; }

        public long Increment(long one)       { return ++one; }
        public long Decrement(long one)       { return --one; }

        public bool GreaterThan(long one, long two)   { return one > two; }
        public bool Equal(long one, long two)   { return one == two; }

        public long IntegerPart(long one)                 { return one; }

        public bool IsEven(long one)                 { return one % 2 == 0; }
        public bool IsNaN(long one)                  { return false; }
        public bool IsPositiveInfinity(long one)               { return false; }
        public bool IsNegativeInfinity(long one)               { return false; }

        public long GetCopy(long val)                 { return val; }
        public long Zero                             { get { return 0; } }

        public long FromInteger(long equivalent)         { return (int)equivalent; }
        public long FromDouble(double equivalent)    { throw new NonFractionalTypeException("int"); }

        public long Parse(string value) { return int.Parse(value); }
    }
}
