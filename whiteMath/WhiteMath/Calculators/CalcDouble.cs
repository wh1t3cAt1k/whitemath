using System;

namespace WhiteMath.Calculators
{
    public class CalcDouble : ICalc<double>
    {
        public bool IsIntegerCalculator { get { return false; } }

        public double Add(double one, double two) { return one + two; }
        public double Subtract(double one, double two) { return one - two; }
        public double Multiply(double one, double two) { return one * two; }
        public double Divide(double one, double two) { return one / two; }
        public double Modulo(double one, double two) { throw new NonIntegerTypeException("double"); }

        public double Increment(double one) { return ++one; }
        public double Decrement(double one) { return --one; }

        public bool GreaterThan(double one, double two) { return one > two; }
        public bool Equal(double one, double two) { return one == two; }

        public double Negate(double one)    { return -one; }
        public double IntegerPart(double one)   { return Math.Truncate(one); }
		public bool IsEven(double one)      { throw new NonIntegerTypeException("double"); }

        public bool IsNaN(double one) { return double.IsNaN(one); }
        public bool IsPositiveInfinity(double one) { return double.IsPositiveInfinity(one); }
        public bool IsNegativeInfinity(double one) { return double.IsNegativeInfinity(one); }

        public double GetCopy(double val) { return val; }
        public double Zero { get { return 0.0; } }

        public double FromInteger(long equivalent) { return equivalent; }
        public double FromDouble(double equivalent) { return equivalent; }

        public double Parse(string value) { return double.Parse(value); }
    }
}
