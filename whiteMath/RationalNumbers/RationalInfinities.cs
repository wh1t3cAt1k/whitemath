using System;

using whiteMath.Calculators;

namespace whiteMath.RationalNumbers
{
    public partial class Rational<T,C>: ICloneable where C: ICalc<T>, new()
    {
        public static readonly Rational<T, C> PositiveInfinity = new Positive_Infinity();
        public static readonly Rational<T, C> NegativeInfinity = new Negative_Infinity();
        public static readonly Rational<T, C> NaN = new NotANumber();

        //----------------------------------------------

		/// <summary>
		/// Gets a value indicating whether this instance is not an infinity or a NaN.
		/// </summary>
		/// <value><c>true</c> if this instance is a normal number; otherwise, <c>false</c>.</value>
		public bool IsNormalNumber 
		{ 
			get
			{
				return !(this is SpecialRational); 
			} 
		}

        private class SpecialRational : Rational<T, C> { }

        private class Positive_Infinity: SpecialRational
        {
            public override bool Equals(object obj)
            {
                if (obj is Positive_Infinity) return true;
                else return false;
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

        private class Negative_Infinity : SpecialRational
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

        private class NotANumber : SpecialRational
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
