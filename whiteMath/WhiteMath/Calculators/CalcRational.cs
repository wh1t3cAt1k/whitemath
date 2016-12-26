using System;

using WhiteMath.RationalNumbers;

namespace WhiteMath.Calculators
{
    public class CalcRational<T,C>: ICalc<Rational<T,C>> where C: ICalc<T>, new()
    {
        private static C calc = new C();

        public bool IsIntegerCalculator { get { return false; } }

        public Rational<T, C> Zero { get { return new Rational<T, C>(calc.Zero, calc.FromInteger(1)); } }

        // ----------------------------

        public Rational<T, C> IntegerPart(Rational<T, C> num)
        {
            return calc.Divide(num.Numerator, num.Denominator);
        }

        // ----------------------------

        public bool IsEven(Rational<T, C> num)
        {
            return false;
        }

        public bool IsNegativeInfinity(Rational<T, C> num)
        {
			return num.IsNegativeInfinity;
        }

        public bool IsPositiveInfinity(Rational<T, C> num)
        {
			return num.IsPositiveInfinity;
        }

        public bool IsNaN(Rational<T, C> num)
        {
			return num.IsNaN;
        }

        // ----------------------------

        public Rational<T, C> FromInteger(long num)
        {
            return new Rational<T, C>(calc.FromInteger(num), calc.FromInteger(1));
        }

        public Rational<T, C> FromDouble(double num)
        {
            return (Rational<T,C>)num;
        }

        public Rational<T, C> GetCopy(Rational<T, C> num)
        {
            return num.Clone() as Rational<T, C>;
        }

        // -------------------------------

        public bool Equal(Rational<T, C> one, Rational<T, C> two)
        {
            return one == two;
        }

        public bool GreaterThan(Rational<T, C> one, Rational<T, C> two)
        {
            return one > two;
        }

        // -------------------------------

        public Rational<T, C> Increment(Rational<T, C> one)
        {
            one.Numerator = calc.Add(one.Numerator, one.Denominator);
            return one;
        }

        public Rational<T, C> Decrement(Rational<T,C> one)
        {
            one.Numerator = calc.Subtract(one.Numerator, one.Denominator);     // вычтем из числителя знаменатель
            return one;
        }

        // ---------------------------------

        public Rational<T, C> Negate(Rational<T, C> one)
        {
            return -one;
        }

        public Rational<T, C> Modulo(Rational<T, C> one, Rational<T,C> two)
        {
            throw new NotSupportedException("Rational numbers do not support the remainder operations.");
        }

        public Rational<T, C> Divide(Rational<T, C> one, Rational<T, C> two)
        {
            return one / two;
        }

        public Rational<T, C> Multiply(Rational<T, C> one, Rational<T, C> two)
        {
            return one * two;
        }

        public Rational<T, C> Subtract(Rational<T, C> one, Rational<T, C> two)
        {
            return one - two;
        }

        public Rational<T, C> Add(Rational<T, C> one, Rational<T, C> two)
        {
            return one + two;
        }

        // ----------------------------- String

        public Rational<T, C> Parse(string value)
        {
            return Rational<T, C>.Parse(value);
        }
    }
}
