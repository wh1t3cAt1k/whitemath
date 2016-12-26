using System;
using System.Linq;
using System.Globalization;

using WhiteMath.Mathematics;
using WhiteMath.Calculators;

namespace WhiteMath.RationalNumbers
{
    /// <summary>
    /// The struct representing the rational number as a pair of integer-like numbers.
    /// For example:
    /// <example>Rational&lt;LongInt, CalcLongInt&gt;</example>
    /// <example>Rational&lt;int, CalcInt&gt;</example>
    /// </summary>
    /// <typeparam name="T">The integer-like type of numerator and denominator.</typeparam>
    public partial class Rational<T, C> : ICloneable where C : ICalc<T>, new()
    {
		private static C calc = Numeric<T, C>.Calculator;

		public enum SpecialNumberType
		{
			None = 0,
			NaN = 1,
			PositiveInfinity = 2,
			NegativeInfinity = 3
		}

		private SpecialNumberType _specialNumberType = SpecialNumberType.None;

		public T Numerator
		{
			get;
			internal set;
		}

		public T Denominator
		{
			get;
			internal set;
		}

		public bool IsNegative
		{
			get
			{
				return 
					calc.IsNegative(this.Numerator) !=
					calc.IsNegative(this.Denominator);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is not a number.
		/// </summary>
		/// <value><c>true</c>, if this instance is NaN; otherwise, <c>false</c>.</value>
		public bool IsNaN
		{
			get
			{
				return this._specialNumberType == SpecialNumberType.NaN;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is a positive infinity.
		/// </summary>
		/// <value><c>true</c> if this instance is a positive infinity; otherwise, <c>false</c>.</value>
		public bool IsPositiveInfinity
		{
			get
			{
				return this._specialNumberType == SpecialNumberType.PositiveInfinity;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is a negative infinity.
		/// </summary>
		/// <value><c>true</c> if this instance is a negative infinity; otherwise, <c>false</c>.</value>
		public bool IsNegativeInfinity
		{
			get
			{
				return this._specialNumberType == SpecialNumberType.NegativeInfinity;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is a positive or a negative infinity.
		/// </summary>
		/// <value><c>true</c> if this instance is an infinity; otherwise, <c>false</c>.</value>
		public bool IsInfinity
		{
			get
			{
				return this.IsPositiveInfinity || this.IsNegativeInfinity;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is a normal number, i.e. not NaN or
		/// an infinity.
		/// </summary>
		/// <value><c>true</c> if this instance is normal number; otherwise, <c>false</c>.</value>
		public bool IsNormalNumber
		{
			get
			{
				return !this.IsInfinity && !this.IsNaN;
			}
		}

        /// <summary>
		/// Initializes a new instance of the <see cref="Rational{T, C}"/> class
		/// using explicitly provided numerator and denominator values.
        /// </summary>
        public Rational(T numerator, T denominator)
        {
			this.Numerator = calc.GetCopy(numerator);
            this.Denominator = calc.GetCopy(denominator);

			this.CheckSpecial();
            this.Normalize();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="WhiteMath.RationalNumbers.Rational{T, C}"/> class
		/// using a special number type provided.
		/// </summary>
		/// <param name="specialNumberType">Special number type.</param>
		public Rational(SpecialNumberType specialNumberType)
		{
			if (specialNumberType == SpecialNumberType.None)
			{
				throw new ArgumentException("This constructor is intended only for constructing NaN or infinities.");
			}

			this._specialNumberType = specialNumberType;
			this.Numerator = calc.Zero;
			this.Denominator = calc.Zero;
		}

        /// <summary>
		/// Normalizes the number:
		/// <list type="bullet">
		/// <item>If the number is NaN or infinity, does nothing;</item>
		/// <item>Else, if the number's numerator is zero, forces the denominator into 1.</item>
		/// <item>Else, divides both the numerator and denominator by their GCD, forces the 
		/// denominator to be positive.</item>
		/// </list>
        /// </summary>
        private void Normalize()
        {
			if (!this.IsNormalNumber) 
				return;

			if (calc.Equal(this.Numerator, calc.Zero))
			{
				// We only want to have a single representation ]
				// for zero.
				// -
				this.Denominator = calc.FromInteger(1);
			}
			else
			{
				T greatestCommonDivisor = Mathematics<T, C>.GreatestCommonDivisor(
					                         Mathematics<T, C>.Abs(this.Numerator), 
					                         Mathematics<T, C>.Abs(this.Denominator));

				this.Numerator = calc.Divide(this.Numerator, greatestCommonDivisor);
				this.Denominator = calc.Divide(this.Denominator, greatestCommonDivisor);

				// Negate the denominator if it's negative
				// -
				if (calc.GreaterThan(calc.Zero, this.Denominator))
				{
					this.Numerator = calc.Negate(this.Numerator);
					this.Denominator = calc.Negate(this.Denominator);
				}
			}
        }

        /// <summary>
		/// If the number's denominator is zero, changes the number
		/// into a negative/positive infinity or a NaN (when the 
		/// numerator is also zero).
		/// For non-zero denominator, but zero numerator,
		/// changes the denominator to one.
        /// </summary>
		/// <returns>
		/// <c>true</c>, if the number kept its normal status,
		/// <c>false</c> if it became an infinity or a NaN.
		/// </returns>
        private bool CheckSpecial()
        {
			if (calc.Equal(this.Denominator, calc.Zero))
			{
				if (calc.Equal(this.Numerator, calc.Zero))
				{
					this._specialNumberType = SpecialNumberType.NaN;
				}
				else if (calc.GreaterThan(calc.Zero, this.Numerator))
				{
					this._specialNumberType = SpecialNumberType.NegativeInfinity;
				}
				else
				{
					this._specialNumberType = SpecialNumberType.PositiveInfinity;
				}

				this.Numerator = calc.Zero;
				this.Denominator = calc.Zero;

				return false;
			}

            return true;
        }

        ///-----------------------------------
        ///----ARITHMETIC OPERATORS-----------
        ///-----------------------------------

        public static Rational<T, C> operator +(Rational<T, C> first, Rational<T, C> second)
        {
			if (first.IsNormalNumber && second.IsNormalNumber)
			{
				T resultDenominator = Mathematics<T, C>.LowestCommonMultiple(
					                      first.Denominator, 
					                      second.Denominator, 
					                      Mathematics<T, C>.GreatestCommonDivisor(first.Denominator, second.Denominator));

				T resultNumerator = calc.Add(
					                    calc.Multiply(first.Numerator, calc.Divide(resultDenominator, first.Denominator)), 
					                    calc.Multiply(second.Numerator, calc.Divide(resultDenominator, second.Denominator)));

				Rational<T, C> result = new Rational<T, C>(resultNumerator, resultDenominator);

				return result;
			}
			else if (!first.IsNormalNumber && !second.IsNormalNumber)
			{
				if (first._specialNumberType == second._specialNumberType)
				{
					return new Rational<T, C>(first._specialNumberType);
				}
				else
				{
					return new Rational<T, C>(SpecialNumberType.NaN);
				}
			}
			else if (!first.IsNormalNumber && second.IsNormalNumber)
			{
				return new Rational<T, C>(first._specialNumberType);
			}
			else
			{
				return second + first;
			}
        }

        public static Rational<T, C> operator -(Rational<T, C> first, Rational<T, C> second) 
        {
			if (first.IsNormalNumber && second.IsNormalNumber)
			{
				T resultDenominator = Mathematics<T, C>.LowestCommonMultiple(
					                      first.Denominator, 
					                      second.Denominator, 
					                      Mathematics<T, C>.GreatestCommonDivisor(first.Denominator, second.Denominator));

				T resultNumerator = calc.Subtract(
					                    calc.Multiply(first.Numerator, calc.Divide(resultDenominator, first.Denominator)), 
					                    calc.Multiply(second.Numerator, calc.Divide(resultDenominator, second.Denominator)));

				Rational<T, C> result = new Rational<T, C>(resultNumerator, resultDenominator);

				result.CheckSpecial();
				result.Normalize();

				return result;
			}
			else if (
				first.IsNaN ||
				second.IsNaN ||
				first._specialNumberType == second._specialNumberType)
			{
				return new Rational<T, C>(SpecialNumberType.NaN);
			}
			else if (first.IsPositiveInfinity)
			{
				return new Rational<T, C>(SpecialNumberType.PositiveInfinity);
			}
			else
			{
				return new Rational<T, C>(SpecialNumberType.NegativeInfinity);
			}
        }

        public static Rational<T, C> operator *(Rational<T, C> first, Rational<T, C> second)
        {
			if (first.IsNormalNumber && second.IsNormalNumber)
			{
				return new Rational<T, C>(
					calc.Multiply(first.Numerator, second.Numerator), 
					calc.Multiply(first.Denominator, second.Denominator));
			}
			else if (
				first.IsNaN ||
				!first.IsNormalNumber && !second.IsNormalNumber)
			{
				return new Rational<T, C>(SpecialNumberType.NaN);
			}
			else if (first.IsInfinity && second.IsNormalNumber)
			{
				if (second.IsNegative)
				{
					return (first.IsPositiveInfinity ? 
						new Rational<T, C>(SpecialNumberType.NegativeInfinity) :
						new Rational<T, C>(SpecialNumberType.PositiveInfinity));
				}
				else
				{
					return new Rational<T, C>(first._specialNumberType);
				}
			}
			else
			{
				return second * first;
			}
        }

        public static Rational<T, C> operator /(Rational<T, C> first, Rational<T, C> second)
        {
			if (first.IsNormalNumber && second.IsNormalNumber)
			{
				return new Rational<T, C>(
					calc.Multiply(first.Numerator, second.Denominator), 
					calc.Multiply(first.Denominator, second.Numerator));
			}
			else if (
				first.IsNaN ||
				second.IsNaN ||
				first.IsInfinity && second.IsInfinity)
			{
				return new Rational<T, C>(SpecialNumberType.NaN);
			}
			else if (first.IsInfinity && second.IsNormalNumber)
			{
				if (second.IsNegative)
				{
					return first.IsPositiveInfinity ?
						new Rational<T, C>(SpecialNumberType.NegativeInfinity) :
						new Rational<T, C>(SpecialNumberType.PositiveInfinity);
				}
				else
				{
					return new Rational<T, C>(first._specialNumberType);
				}
			}
			else if (first.IsNormalNumber && second.IsInfinity)
			{
				return new Rational<T, C>(calc.Zero, calc.FromInteger(1));
			}
			else
			{
				throw new InvalidProgramException();
			}
        }

        public static Rational<T,C> operator -(Rational<T,C> number)
        {
			if (number.IsNormalNumber)
			{
				return new Rational<T, C>(calc.Negate(number.Numerator), number.Denominator);
			}
			else if (number.IsInfinity)
			{
				return (number.IsPositiveInfinity ?
					new Rational<T, C>(SpecialNumberType.NegativeInfinity) :
					new Rational<T, C>(SpecialNumberType.PositiveInfinity));
			}
			else if (number.IsNaN)
			{
				return new Rational<T, C>(SpecialNumberType.NaN);
			}
			else
			{
				throw new InvalidProgramException();
			}
        }

        public static bool operator ==(Rational<T, C> first, Rational<T, C> second)
        {
			if (!first.IsNormalNumber && !second.IsNormalNumber && !first.IsNaN)
			{
				return first._specialNumberType == second._specialNumberType;
			}
			else if (first.IsNaN || first.IsNormalNumber != second.IsNormalNumber)
			{
				return false;
			}
			else if (first.IsNormalNumber && second.IsNormalNumber)
			{
				return 
					calc.Equal(first.Numerator, second.Numerator) && 
					calc.Equal(first.Denominator, second.Denominator);
			}
			else
			{
				throw new InvalidProgramException();
			}
        }
			
        public static bool operator !=(Rational<T, C> first, Rational<T, C> second)
        {
            return !(first == second);
        }

        public static bool operator >(Rational<T, C> one, Rational<T, C> two)
        {
			/*
            if (one is NotANumber || two is NotANumber) return false;
            else if (one is Positive_Infinity || two is Negative_Infinity) return true;
            else if (two is Positive_Infinity || one is Negative_Infinity) return false;
			*/

            T denomLcm = Mathematics<T, C>.LowestCommonMultiple(one.Denominator, two.Denominator, Mathematics<T, C>.GreatestCommonDivisor(one.Denominator, two.Denominator));
            return calc.GreaterThan(calc.Multiply(one.Numerator, calc.Divide(denomLcm, one.Denominator)), calc.Multiply(two.Numerator, calc.Divide(denomLcm, two.Denominator)));
        }

        public static bool operator <(Rational<T, C> one, Rational<T, C> two)
        {
			/*
            if (one is NotANumber || two is NotANumber) return false;
            else if (one is Positive_Infinity || two is Negative_Infinity) return false;
            else if (two is Positive_Infinity || one is Negative_Infinity) return true;
			*/

            return two > one;
        }

        public static bool operator >=(Rational<T, C> one, Rational<T, C> two)
        {
            return !(two > one);
        }

        public static bool operator <=(Rational<T, C> one, Rational<T, C> two)
        {
            return !(one > two);
        }

        ///-----------------------------------
        ///-------------CONVERSION OPERATORS--
        ///-----------------------------------

        public static implicit operator Rational<T, C>(T num)
        {
            return new Rational<T, C>(num, calc.FromInteger(1));
        }

        public static explicit operator Rational<T, C>(double num)
        {
            string numberString = num.ToString();

            T numeratorMultiplier;

			if (numberString.First().Equals('-'))
			{
				numberString = numberString.Substring(1);
				numeratorMultiplier = calc.FromInteger(-1);
			}
			else
			{
				numeratorMultiplier = calc.FromInteger(1);
			}

            T denominator = calc.FromInteger(1);

            int exponentSymbolIndex = numberString.IndexOf("e", StringComparison.OrdinalIgnoreCase);

            if (exponentSymbolIndex > 0)
            {
                int exponent = int.Parse(numberString.Substring(exponentSymbolIndex + 1));

				if (exponent >= 0)
				{
					numeratorMultiplier = Mathematics<T, C>.PowerInteger(calc.FromInteger(10), exponent);
				}
				else
				{
					denominator = Mathematics<T, C>.PowerInteger(calc.FromInteger(10), -exponent);
				}

                numberString = numberString.Substring(0, exponentSymbolIndex);
            }

			// Trim all the leading and trailing zeroes
			// -
			numberString.Trim('0');

            // Find the decimal separator
			// -
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            int separatorIndex = numberString.IndexOf(decimalSeparator);

            if (separatorIndex > 0)
            {
                int numberOfCharactersAfterSeparator = numberString.Length - separatorIndex - 1;

				denominator = calc.Multiply(
					denominator, 
					Mathematics<T, C>.PowerInteger(calc.FromInteger(10), numberOfCharactersAfterSeparator));

				numberString = numberString.Replace(
					System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, 
					"");
            }

            T numerator = calc.Multiply(calc.Parse(numberString), numeratorMultiplier);

            return new Rational<T,C>(numerator, denominator);
        }

        /// <summary>
        /// Divides the numerator by the denominator using the 
        /// <typeparamref name="C"/> calculator's division operation
        /// and returns the result.
        /// 
        /// If <typeparamref name="T"/> is integer type, may result in
        /// a loss of result's fractional part.
        /// </summary>
        /// <param name="obj">
        /// The number whose <typeparamref name="T"/> type equivalent
        /// is to be found.
        /// </param>
        /// <returns>
        /// The result of dividing the numerator by the denominator using 
        /// <typeparamref name="C"/> calculator's division function.
        /// </returns>
        public static explicit operator T(Rational<T, C> obj)
        {
            return calc.Divide(obj.Numerator, obj.Denominator);
        }

        /// <summary>
        /// Gets the exact deep copy of the current number.
        /// </summary>
        public object Clone()
        {
            return new Rational<T, C>(calc.GetCopy(this.Numerator), calc.GetCopy(this.Denominator));
        }

        /// <summary>
        /// Gets the hash code for current number.
        /// </summary>
        public override int GetHashCode()
        {
			// TODO: since the numerator and the denominator are mutable,
			// it is not a good idea to implement it this way.
			// -
            return Numerator.GetHashCode() + Denominator.GetHashCode();
        }

        /// <summary>
        /// Checks if two Rationals store the same numeric value.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
			if (!(obj is Rational<T, C>))
			{
				return false;
			}
			else
			{
				return (this == (obj as Rational<T, C>));
			}
		}
        // -------------------------------------- String representation

        /// <summary>
        /// Used by the overloaded ToString() method, provides one of the following number formats:
        /// 1. IntegerPair:		[num; denom]
        /// 2. Num_Div_Denom:	num/denom
        /// 3. Both:			[num/denom]
        /// </summary>
        public enum NumberFormat
        {
            IntegerPair, Num_Div_Denom, Both
        }

        /// <summary>
        /// Returns the string representation of current Rational number.
        /// </summary>
        /// <returns>The string value containing representation of the number.</returns>
        public override string ToString()
        {
            return ToString(NumberFormat.Both);
        }

        public string ToString(NumberFormat formatType)
        {
			if (formatType == NumberFormat.Num_Div_Denom)
			{
				return string.Format("{0}/{1}", this.Numerator, this.Denominator);
			}
			else if (formatType == NumberFormat.IntegerPair)
			{
				return string.Format("[{0}; {1}]", this.Numerator, this.Denominator);
			}
			else
			{
				return string.Format("[{0}/{1}]", this.Numerator, this.Denominator);
			}
        }

        // --------------------------------- PARSE

        /// <summary>
        /// Parses the string containing the rational number.
        /// Syntax:
        /// 
        /// 1. [numerator; denominator]
        /// 2. [numerator / denominator]
        /// 3. numerator / denominator
        /// 
        /// Numerator and denominator should be written in the format in accordance 
        /// to their own Parse methods.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Rational<T, C> Parse(string value)
        {
			// TODO: should be rewritten using regular expressions.
			// TODO: add checks for the incorrect arguments.
			// -

            bool outerNegationSign = false;
            
            value = value.Replace(" ", "");

            if (value[0] == '-')
            {
                value = value.Substring(1);
                outerNegationSign = true;
            }

            // Destroy the outer parentheses
			// -
            if (value[0] == '[' && value[value.Length - 1] == ']')
                value = value.Substring(1, value.Length - 2);

            string[] split;

            if (value.Contains(';'))
                split = value.Split(';');
            else
                split = value.Split('/');

            Rational<T, C> result = new Rational<T, C>(
				calc.Parse(split[0]), 
				calc.Parse(split[1]));

			if (outerNegationSign)
			{
				return -result;
			}
			else
			{
				return result;
			}
        }
    }
}
