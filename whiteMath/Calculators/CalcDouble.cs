namespace whiteMath.Calculators
{
    public class CalcDouble : ICalc<double>
    {
        public bool isIntegerCalculator { get { return false; } }

        public double sum(double one, double two) { return one + two; }
        public double dif(double one, double two) { return one - two; }
        public double mul(double one, double two) { return one * two; }
        public double div(double one, double two) { return one / two; }
        public double rem(double one, double two) { throw new NonIntegerTypeException("double"); }

        public double increment(double one) { return ++one; }
        public double decrement(double one) { return --one; }

        public bool mor(double one, double two) { return one > two; }
        public bool eqv(double one, double two) { return one == two; }

        public double negate(double one)    { return -one; }
        public double intPart(double one)   { return Math.Truncate(one); }
        public bool isEven(double one)      { throw new NonIntegerTypeException("double"); }

        public bool isNaN(double one) { return double.IsNaN(one); }
        public bool isPosInf(double one) { return double.IsPositiveInfinity(one); }
        public bool isNegInf(double one) { return double.IsNegativeInfinity(one); }

        public double getCopy(double val) { return val; }
        public double zero { get { return 0.0; } }

        public double fromInt(long equivalent) { return equivalent; }
        public double fromDouble(double equivalent) { return equivalent; }

        public double parse(string value) { return double.Parse(value); }
    }
}
