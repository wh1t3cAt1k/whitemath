using System;
using System.Linq;
using System.Globalization;

using whiteMath.Algorithms;
using whiteMath.Calculators;

namespace whiteMath.RationalNumbers
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

		private enum SpecialNumberType
		{
			None = 0,
			NaN = 1,
			PositiveInfinity = 2,
			NegativeInfinity = 3
		}

		private SpecialNumberType specialNumberType = SpecialNumberType.None;

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

		/// <summary>
		/// Gets a value indicating whether this instance is not a number.
		/// </summary>
		/// <value><c>true</c>, if this instance is NaN; otherwise, <c>false</c>.</value>
		public bool IsNaN
		{
			get
			{
				return this.specialNumberType == SpecialNumberType.NaN;
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
				return this.specialNumberType == SpecialNumberType.PositiveInfinity;
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
				return this.specialNumberType == SpecialNumberType.NegativeInfinity;
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
		/// Parameterless constructor for inner purposes.
		/// TODO: delete after the deletion of the SpecialRational subclass.
		/// </summary>
		private Rational()
		{
		}

        /// <summary>
        /// The standard constructor for generic Rational numbers.
        /// User should explicitly specify both the numerator and the denumerator.
        /// </summary>
        public Rational(T numerator, T denominator)
        {
			this.Numerator = calc.getCopy(numerator);
            this.Denominator = calc.getCopy(denominator);

            this.Normalize();
        }

        /// <summary>
		/// Normalizes the number, divides both the numerator
		/// and denominator by their GCD, forces the denominator
		/// to be positive.
        /// </summary>
        private void Normalize()
        {
			if (!this.IsNormalNumber) return;

			T greatestCommonDivisor = WhiteMath<T, C>.GreatestCommonDivisor(
				WhiteMath<T, C>.Abs(this.Numerator), 
				WhiteMath<T, C>.Abs(this.Denominator));

            this.Numerator = calc.div(this.Numerator, greatestCommonDivisor);
            this.Denominator = calc.div(this.Denominator, greatestCommonDivisor);

            // Negate the denominator if it's negative
			// -
            if (calc.mor(calc.zero, this.Denominator))
            {
                this.Numerator = calc.negate(this.Numerator);
                this.Denominator = calc.negate(this.Denominator);
            }
        }

        /// <summary>
		/// If the number's denominator is zero, changes the number
		/// into a negative/positive infinity or a NaN (when the 
		/// numerator is also zero).
        /// </summary>
		/// <returns>
		/// <c>true</c>, if the number kept its normal status,
		/// <c>false</c> if it became an infinity or a NaN.
		/// </returns>
        private static bool CheckForInfinity(ref Rational<T, C> number)
        {
            if (calc.eqv(number.Denominator, calc.zero))
            {
				if (calc.eqv(number.Numerator, calc.zero))
				{
					number = NaN;
				}
				else if (calc.mor(calc.zero, number.Numerator))
				{
					number = NegativeInfinity;
				}
				else
				{
					number = PositiveInfinity;
				}

				number.Numerator = calc.zero;
				number.Denominator = calc.zero;

                return false;
            }

            return true;
        }

        ///-----------------------------------
        ///----ARITHMETIC OPERATORS-----------
        ///-----------------------------------

        public static Rational<T, C> operator +(Rational<T, C> one, Rational<T, C> two)
        {
            if (one is SpecialRational)
            {
                if (two is SpecialRational)
                {
                    if (one.GetType().Equals(two.GetType())) return one;
                    else return NaN;
                }

                return one;
            }
            else if (two is SpecialRational) return two + one;

			T resultDenominator = WhiteMath<T, C>.LowestCommonMultiple(
				one.Denominator, 
				two.Denominator, 
				WhiteMath<T, C>.GreatestCommonDivisor(one.Denominator, two.Denominator));
			
			T resultNumerator = calc.sum(
				calc.mul(one.Numerator, calc.div(resultDenominator, one.Denominator)), 
				calc.mul(two.Numerator, calc.div(resultDenominator, two.Denominator)));

			Rational<T, C> result = new Rational<T, C>(resultNumerator, resultDenominator);

			if (CheckForInfinity(ref result))
			{
				result.Normalize();
			}

            return result;
        }

        public static Rational<T, C> operator -(Rational<T, C> one, Rational<T, C> two) 
        {
            if (one is SpecialRational || two is SpecialRational)
            {
				if (one is NotANumber || two is NotANumber)
				{
					return NaN;
				}
                else if (one is Positive_Infinity)
                {
                    if (two is SpecialRational)
                    {
						if (two is Negative_Infinity)
						{
							return one;
						}
						else
						{
							return NaN;
						}
                    }

                    return one;
                }
                else if (two is Positive_Infinity)
                {
                    if (one is SpecialRational)
                    {
						if (one is Negative_Infinity)
						{
							return one;
						}
						else
						{
							return NaN;
						}
                    }

                    return NegativeInfinity;
                }
                else if (one is Negative_Infinity)
                {
                    if (two is SpecialRational)
                    {
						if (two is Positive_Infinity)
						{
							return one;
						}
						else
						{
							return NaN;
						}
                    }

                    return one;
                }
                else if (two is Negative_Infinity)
                {
                    if (one is SpecialRational)
                    {
						if (one is Positive_Infinity)
						{
							return one;
						}
						else
						{
							return NaN;
						}
                    }

                    return PositiveInfinity;
                }
            }


            T resultDenominator = WhiteMath<T, C>.LowestCommonMultiple(
				one.Denominator, 
				two.Denominator, 
				WhiteMath<T, C>.GreatestCommonDivisor(one.Denominator, two.Denominator));
            
			T resultNumerator = calc.dif(
				calc.mul(one.Numerator, calc.div(resultDenominator, one.Denominator)), 
				calc.mul(two.Numerator, calc.div(resultDenominator, two.Denominator)));

			Rational<T, C> result = new Rational<T, C>(resultNumerator, resultDenominator);

			if (CheckForInfinity(ref result))
			{
				result.Normalize();
			}

            return result;
        }

        public static Rational<T, C> operator *(Rational<T, C> one, Rational<T, C> two)
        {
			if (one is SpecialRational)
			{
				if (two is SpecialRational || calc.eqv(calc.zero, two.Numerator))
				{
					return NaN;
				}
				else if (calc.mor(calc.zero, two.Numerator))
				{
					if (one is Positive_Infinity)
					{
						return NegativeInfinity;
					}
					else if (one is Negative_Infinity)
					{
						return PositiveInfinity;
					}
				}
				return one;
			}
			else if (two is SpecialRational)
			{
				return two * one;
			}

            return new Rational<T, C>(
				calc.mul(one.Numerator, two.Numerator), 
				calc.mul(one.Denominator, two.Denominator));
        }

        public static Rational<T, C> operator /(Rational<T, C> one, Rational<T, C> two)
        {
            if (one is SpecialRational)
            {
				if (two is SpecialRational || calc.eqv(two.Numerator, calc.zero))
				{
					return NaN;
				}
                else if (calc.mor(calc.zero, two.Numerator))
                {
					if (one is Positive_Infinity)
					{
						return NegativeInfinity;
					}
					else if (one is Negative_Infinity)
					{
						return PositiveInfinity;
					}
                }

                return one;
            }
            else if (two is SpecialRational)
            {
                if (one is SpecialRational || two is NotANumber) return NaN;

				// We get zero when dividing by an infinity.
				// -
                return new Rational<T, C>(calc.zero, calc.fromInt(1));
            }

            if (calc.eqv(two.Numerator, calc.zero)) 
			{
				// TODO: this is madness. Refactor.
				// -
				Rational<T, C> tmp = new Rational<T, C>(one.Numerator, calc.zero); 
				CheckForInfinity(ref tmp); 
				return tmp; 
			}

            return new Rational<T, C>(
				calc.mul(one.Numerator, two.Denominator), 
				calc.mul(one.Denominator, two.Numerator));
        }

        public static Rational<T,C> operator -(Rational<T,C> one)
        {
            if (one is SpecialRational)
            {
				if (one is Positive_Infinity)
				{
					return NegativeInfinity;
				}
				else if (one is Negative_Infinity)
				{
					return PositiveInfinity;
				}
				else
				{
					return one;
				}
            }

            return new Rational<T, C>(calc.negate(one.Numerator), one.Denominator);
        }

        public static bool operator ==(Rational<T, C> one, Rational<T, C> two)
        {
            return one.Equals(two);
        }

        public static bool operator !=(Rational<T, C> one, Rational<T, C> two)
        {
            return !(one == two);
        }

        public static bool operator >(Rational<T, C> one, Rational<T, C> two)
        {
            if (one is NotANumber || two is NotANumber) return false;
            else if (one is Positive_Infinity || two is Negative_Infinity) return true;
            else if (two is Positive_Infinity || one is Negative_Infinity) return false;

            T denomLcm = WhiteMath<T, C>.LowestCommonMultiple(one.Denominator, two.Denominator, WhiteMath<T, C>.GreatestCommonDivisor(one.Denominator, two.Denominator));
            return calc.mor(calc.mul(one.Numerator, calc.div(denomLcm, one.Denominator)), calc.mul(two.Numerator, calc.div(denomLcm, two.Denominator)));
        }

        public static bool operator <(Rational<T, C> one, Rational<T, C> two)
        {
            if (one is NotANumber || two is NotANumber) return false;
            else if (one is Positive_Infinity || two is Negative_Infinity) return false;
            else if (two is Positive_Infinity || one is Negative_Infinity) return true;

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
            return new Rational<T, C>(num, calc.fromInt(1));
        }

        public static explicit operator Rational<T, C>(double num)
        {
            string numberString = num.ToString();

            T numeratorMultiplier;

			if (numberString.First().Equals('-'))
			{
				numberString = numberString.Substring(1);
				numeratorMultiplier = calc.fromInt(-1);
			}
			else
			{
				numeratorMultiplier = calc.fromInt(1);
			}

            T denominator = calc.fromInt(1);

            int exponentSymbolIndex = numberString.IndexOf("e", StringComparison.OrdinalIgnoreCase);

            if (exponentSymbolIndex > 0)
            {
                int exponent = int.Parse(numberString.Substring(exponentSymbolIndex + 1));

				if (exponent >= 0)
				{
					numeratorMultiplier = WhiteMath<T, C>.PowerInteger(calc.fromInt(10), exponent);
				}
				else
				{
					denominator = WhiteMath<T, C>.PowerInteger(calc.fromInt(10), -exponent);
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

				denominator = calc.mul(
					denominator, 
					WhiteMath<T, C>.PowerInteger(calc.fromInt(10), numberOfCharactersAfterSeparator));

				numberString = numberString.Replace(
					System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, 
					"");
            }

            T numerator = calc.mul(calc.parse(numberString), numeratorMultiplier);

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
            return calc.div(obj.Numerator, obj.Denominator);
        }

        /// <summary>
        /// Gets the exact deep copy of the current number.
        /// </summary>
        public object Clone()
        {
            return new Rational<T, C>(calc.getCopy(this.Numerator), calc.getCopy(this.Denominator));
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
            if (!(obj is Rational<T, C>)) return false;
            else if (this is SpecialRational || obj is SpecialRational)
            {
                if (this is Positive_Infinity && obj is Positive_Infinity ||
                    this is Negative_Infinity && obj is Negative_Infinity)
                    return true;
                
                return false;
            }
            else
			{
                return (obj as Rational<T, C>).Numerator.Equals(this.Numerator) && (obj as Rational<T, C>).Denominator.Equals(this.Denominator);
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
				calc.parse(split[0]), 
				calc.parse(split[1]));

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
