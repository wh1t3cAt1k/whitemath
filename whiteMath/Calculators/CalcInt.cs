namespace whiteMath.Calculators
{
    /// <summary>
    /// Standard calculator for integers.
    /// </summary>
    public class CalcInt : ICalc<int>
    {
        public bool IsIntegerCalculator     { get { return true; } }

        public int Add(int one, int two)    { return one + two; }
        public int Subtract(int one, int two)    { return one - two; }
        public int Multiply(int one, int two)    { return one * two; }
        public int Divide(int one, int two)    { return one / two; }
        public int Modulo(int one, int two)    { return one % two; }
        public int Negate(int one)          { return -one; }

        public int Increment(int one)       { return ++one; }
        public int Decrement(int one)       { return --one; }

        public bool GreaterThan(int one, int two)   { return one > two; }
        public bool Equal(int one, int two)   { return one == two; }

        public int IntegerPart(int one)                 { return one; }

        public bool IsEven(int one)                 { return one % 2 == 0; }
        public bool IsNaN(int one)                  { return false; }
        public bool IsPositiveInfinity(int one)               { return false; }
        public bool IsNegativeInfinity(int one)               { return false; }

        public int GetCopy(int val)                 { return val; }
        public int Zero                             { get { return 0; } }

        public int FromInteger(long equivalent)         { return (int)equivalent; }
        public int FromDouble(double equivalent)    { throw new NonFractionalTypeException("int"); }

        public int Parse(string value) { return int.Parse(value); }
    }
}
