using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath
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
        private static C calc = new C();

		internal T numerator;
		internal T denominator;

        // todo: make default-value denominator = calc.fromInt(1);

        /// <summary>
        /// The standard constructor for generic Rational numbers.
        /// User should explicitly specify both the numerator and the denumerator.
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        public Rational(T numerator, T denominator)
        {
            numerator = calc.getCopy(numerator);
            denominator = calc.getCopy(denominator);

            normalize();
        }

        /// <summary>
        /// Parameterless constructor for inner purposes.
        /// </summary>
        private Rational() { }

        /// <summary>
		/// Normalizes the number, divides both the numerator
		/// and denominator by their GCD, forces the denominator
		/// to be positive.
        /// </summary>
        private void normalize()
        {
			T greatestCommonDivisor = WhiteMath<T, C>.GreatestCommonDivisor(
				WhiteMath<T, C>.Abs(numerator), 
				WhiteMath<T, C>.Abs(denominator));

			// For infinity checking cases.
			// -
            if (calc.eqv(greatestCommonDivisor, calc.zero)) return;

            numerator = calc.div(numerator, greatestCommonDivisor);
            denominator = calc.div(denominator, greatestCommonDivisor);

            // Negate the denominator if it's negative
			// -
            if (calc.mor(calc.zero, denominator))
            {
                numerator = calc.negate(numerator);
                denominator = calc.negate(denominator);
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
        private static bool checkInf(ref Rational<T, C> number)
        {
            if (calc.eqv(number.denominator, calc.zero))
            {
                if (calc.eqv(number.numerator, calc.zero))
                    number = NaN;
                else if (calc.mor(calc.zero, number.numerator))
                    number = NegativeInfinity;
                else
                    number = PositiveInfinity;

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

            Rational<T, C> result = new Rational<T, C>();

            result.denominator = WhiteMath<T, C>.LowestCommonMultiple(one.denominator, two.denominator, WhiteMath<T, C>.GreatestCommonDivisor(one.denominator, two.denominator));
            result.numerator = calc.sum(calc.mul(one.numerator, calc.div(result.denominator, one.denominator)), calc.mul(two.numerator, calc.div(result.denominator, two.denominator)));

            if (checkInf(ref result))
				
                result.normalize();
            
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

            Rational<T, C> result = new Rational<T, C>();

            result.denominator = WhiteMath<T, C>.LowestCommonMultiple(one.denominator, two.denominator, WhiteMath<T, C>.GreatestCommonDivisor(one.denominator, two.denominator));
            result.numerator = calc.dif(calc.mul(one.numerator, calc.div(result.denominator, one.denominator)), calc.mul(two.numerator, calc.div(result.denominator, two.denominator)));

            if (checkInf(ref result)) result.normalize();
            return result;
        }

        public static Rational<T, C> operator *(Rational<T, C> one, Rational<T, C> two)
        {
            if (one is SpecialRational)
            {
                if (two is SpecialRational || calc.eqv(calc.zero, two.numerator)) return NaN;
                else if (calc.mor(calc.zero, two.numerator))
                    if (one is Positive_Infinity) return NegativeInfinity;
                    else if (one is Negative_Infinity) return PositiveInfinity;

                return one;
            }
            else if (two is SpecialRational) return two * one;

            return new Rational<T, C>(calc.mul(one.numerator, two.numerator), calc.mul(one.denominator, two.denominator));
        }

        public static Rational<T, C> operator /(Rational<T, C> one, Rational<T, C> two)
        {
            if (one is SpecialRational)
            {
                if (two is SpecialRational || calc.eqv(two.numerator, calc.zero)) return NaN;

                else if (calc.mor(calc.zero, two.numerator))
                {
                    if (one is Positive_Infinity) return NegativeInfinity;
                    else if (one is Negative_Infinity) return PositiveInfinity;
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

            if (calc.eqv(two.numerator, calc.zero)) { Rational<T, C> tmp = new Rational<T, C>(one.numerator, calc.zero); checkInf(ref tmp); return tmp; }
            return new Rational<T, C>(calc.mul(one.numerator, two.denominator), calc.mul(one.denominator, two.numerator));
        }

        public static Rational<T,C> operator -(Rational<T,C> one)
        {
            if (one is SpecialRational)
            {
                if (one is Positive_Infinity) 
                    return NegativeInfinity;
                
                else if (one is Negative_Infinity) 
                    return PositiveInfinity;
                
                else return one;
            }

            return new Rational<T, C>(calc.negate(one.numerator), one.denominator);
        }

        ///-----------------------------------
        ///----LOGICAL OPERATORS--------------
        ///-----------------------------------

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

            T denomLcm = WhiteMath<T, C>.LowestCommonMultiple(one.denominator, two.denominator, WhiteMath<T, C>.GreatestCommonDivisor(one.denominator, two.denominator));
            return calc.mor(calc.mul(one.numerator, calc.div(denomLcm, one.denominator)), calc.mul(two.numerator, calc.div(denomLcm, two.denominator)));
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

        public static implicit operator Rational<T, C>(double num)
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

            // Find the 'commma'
			// -
            string separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            int separatorIndex = numberString.IndexOf(separator);

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
            return calc.div(obj.numerator, obj.denominator);
        }

        //-----------------------------------
        //-----INHERITED OVERRIDING----------
        //-----------------------------------

        /// <summary>
        /// Gets the exact deep copy of the current number.
        /// </summary>
        public object Clone()
        {
            return new Rational<T, C>(calc.getCopy(this.numerator), calc.getCopy(this.denominator));
        }

        /// <summary>
        /// Gets the hash code for current number.
        /// Still works stupidly (though reasonable), needs to be fixed.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return numerator.GetHashCode() + denominator.GetHashCode();
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
                return (obj as Rational<T, C>).numerator.Equals(this.numerator) && (obj as Rational<T, C>).denominator.Equals(this.denominator);
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
                return numerator.ToString() + "/" + denominator.ToString();
            else if (formatType == NumberFormat.IntegerPair)
                return String.Format("[{0}; {1}]", numerator, denominator);
            else
                return String.Format("[{0}/{1}]", numerator, denominator);
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
