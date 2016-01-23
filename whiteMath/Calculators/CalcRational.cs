using System;

using whiteMath.RationalNumbers;

namespace whiteMath.Calculators
{
    public class CalcRational<T,C>: ICalc<Rational<T,C>> where C: ICalc<T>, new()
    {
        private static C calc = new C();

        public bool isIntegerCalculator { get { return false; } }

        public Rational<T, C> zero { get { return new Rational<T, C>(calc.zero, calc.fromInt(1)); } }

        // ----------------------------

        public Rational<T, C> intPart(Rational<T, C> num)
        {
            return calc.div(num.numerator, num.denominator);
        }

        // ----------------------------

        public bool isEven(Rational<T, C> num)
        {
            return false;
        }

        public bool isNegInf(Rational<T, C> num)
        {
            return Rational<T,C>.ReferenceEquals(num, Rational<T,C>.NegativeInfinity);
        }

        public bool isPosInf(Rational<T, C> num)
        {
            return Rational<T,C>.ReferenceEquals(num, Rational<T,C>.PositiveInfinity);
        }

        public bool isNaN(Rational<T, C> num)
        {
            return Rational<T,C>.ReferenceEquals(num, Rational<T,C>.NaN);
        }

        // ----------------------------

        public Rational<T, C> fromInt(long num)
        {
            return new Rational<T, C>(calc.fromInt(num), calc.fromInt(1));
        }

        public Rational<T, C> fromDouble(double num)
        {
            return (Rational<T,C>)num;
        }

        public Rational<T, C> getCopy(Rational<T, C> num)
        {
            return num.Clone() as Rational<T, C>;
        }

        // -------------------------------

        public bool eqv(Rational<T, C> one, Rational<T, C> two)
        {
            return one == two;
        }

        public bool mor(Rational<T, C> one, Rational<T, C> two)
        {
            return one > two;
        }

        // -------------------------------

        public Rational<T, C> increment(Rational<T, C> one)
        {
            one.numerator = calc.sum(one.numerator, one.denominator);
            return one;
        }

        public Rational<T, C> decrement(Rational<T,C> one)
        {
            one.numerator = calc.dif(one.numerator, one.denominator);     // вычтем из числителя знаменатель
            return one;
        }

        // ---------------------------------

        public Rational<T, C> negate(Rational<T, C> one)
        {
            return -one;
        }

        public Rational<T, C> rem(Rational<T, C> one, Rational<T,C> two)
        {
            throw new NotSupportedException("Rational numbers do not support the remainder operations.");
        }

        public Rational<T, C> div(Rational<T, C> one, Rational<T, C> two)
        {
            return one / two;
        }

        public Rational<T, C> mul(Rational<T, C> one, Rational<T, C> two)
        {
            return one * two;
        }

        public Rational<T, C> dif(Rational<T, C> one, Rational<T, C> two)
        {
            return one - two;
        }

        public Rational<T, C> sum(Rational<T, C> one, Rational<T, C> two)
        {
            return one + two;
        }

        // ----------------------------- String

        public Rational<T, C> parse(string value)
        {
            return Rational<T, C>.Parse(value);
        }
    }
}
