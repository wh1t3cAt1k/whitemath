using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath
{
    public partial class Rational<T,C>: ICloneable where C: ICalc<T>, new()
    {
        public static readonly Rational<T, C> PositiveInfinity = new Positive_Infinity();
        public static readonly Rational<T, C> NegativeInfinity = new Negative_Infinity();
        public static readonly Rational<T, C> NaN = new NotANumber();

        //----------------------------------------------

        private class Infinities : Rational<T, C> { }

        private class Positive_Infinity: Infinities
        {
            public override bool Equals(object obj)
            {
                if(obj is Positive_Infinity) return true;
                return false;
            }

            public override int GetHashCode()
            {
                return 0;
            }

            public override string ToString()
            {
                return "+infinity";
            }
        }

        private class Negative_Infinity : Infinities
        {
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return 0;
            }

            public override string ToString()
            {
                return "-infinity";
            }
        }

        private class NotANumber : Infinities
        {
            public override bool Equals(object obj)
            {
                return false;
            }

            public override int GetHashCode()
            {
                return 0;
            }

            public override string ToString()
            {
                return "NaN";
            }
        }
    }
}
