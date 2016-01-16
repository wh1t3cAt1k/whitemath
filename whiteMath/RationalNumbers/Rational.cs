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

        internal T num;          // числитель
        internal T denom;        // знаменатель

        // todo: сделать default-value denominator = calc.fromInt(1);

        /// <summary>
        /// The standard constructor for generic Rational numbers.
        /// User should explicitly specify both the numerator and the denumerator.
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        public Rational(T numerator, T denominator)
        {
            num = calc.getCopy(numerator);
            denom = calc.getCopy(denominator);

            normalize();
        }

        /// <summary>
        /// Parameterless constructor for inner purposes.
        /// </summary>
        public Rational() { }

        /// <summary>
        /// Нормализует число, делит числитель и знаменатель на НОД,
        /// делает числитель положительным.
        /// </summary>
        private void normalize()
        {
            T gcd = WhiteMath<T, C>.GreatestCommonDivisor(WhiteMath<T, C>.Abs(num), WhiteMath<T, C>.Abs(denom));
            if (calc.eqv(gcd, calc.zero)) return;   // for infinity-checking cases

            num = calc.div(num, gcd);
            denom = calc.div(denom, gcd);

            // Если знаменатель меньше нуля
            if (calc.mor(calc.zero, denom))
            {
                num = calc.negate(num);
                denom = calc.negate(denom);
            }
        }

        /// <summary>
        /// Проверяет число на то, не бесконечность ли оно случаем.
        /// Если бесконечность, меняет его, чтобы оно стало истинной бесконечностью.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool checkInf(ref Rational<T, C> obj)
        {
            if (calc.eqv(obj.denom, calc.zero))
            {
                if (calc.eqv(obj.num, calc.zero))
                    obj = NaN;
                else if (calc.mor(calc.zero, obj.num))
                    obj = NegativeInfinity;
                else
                    obj = PositiveInfinity;

                return false;
            }

            return true;
        }

        ///-----------------------------------
        ///----ARITHMETIC OPERATORS-----------
        ///-----------------------------------

        public static Rational<T, C> operator +(Rational<T, C> one, Rational<T, C> two)
        {
            if (one is Infinities)
            {
                if (two is Infinities)
                {
                    if (one.GetType().Equals(two.GetType())) return one;
                    else return NaN;
                }

                return one;
            }
            else if (two is Infinities) return two + one;

            Rational<T, C> tmp = new Rational<T, C>();

            tmp.denom = WhiteMath<T, C>.LowestCommonMultiple(one.denom, two.denom, WhiteMath<T, C>.GreatestCommonDivisor(one.denom, two.denom));
            tmp.num = calc.sum(calc.mul(one.num, calc.div(tmp.denom, one.denom)), calc.mul(two.num, calc.div(tmp.denom, two.denom)));

            if (checkInf(ref tmp)) 
                tmp.normalize();
            
            return tmp;
        }

        public static Rational<T, C> operator -(Rational<T, C> one, Rational<T, C> two) 
        {
            if (one is Infinities || two is Infinities)
            {
                if (one is NotANumber || two is NotANumber) return NaN;
                else if (one is Positive_Infinity)
                {
                    if (two is Infinities)
                    {
                        if (two is Negative_Infinity) return one;
                        else return NaN;
                    }

                    return one;
                }
                else if (two is Positive_Infinity)
                {
                    if (one is Infinities)
                    {
                        if (one is Negative_Infinity) return one;
                        else return NaN;
                    }

                    return NegativeInfinity;
                }
                else if (one is Negative_Infinity)
                {
                    if (two is Infinities)
                    {
                        if (two is Positive_Infinity) return one;
                        else return NaN;
                    }

                    return one;
                }
                else if (two is Negative_Infinity)
                {
                    if (one is Infinities)
                    {
                        if (one is Positive_Infinity) return one;
                        else return NaN;
                    }

                    return PositiveInfinity;
                }
            }

            Rational<T, C> tmp = new Rational<T, C>();

            tmp.denom = WhiteMath<T, C>.LowestCommonMultiple(one.denom, two.denom, WhiteMath<T, C>.GreatestCommonDivisor(one.denom, two.denom));
            tmp.num = calc.dif(calc.mul(one.num, calc.div(tmp.denom, one.denom)), calc.mul(two.num, calc.div(tmp.denom, two.denom)));

            if (checkInf(ref tmp)) tmp.normalize();
            return tmp;
        }

        public static Rational<T, C> operator *(Rational<T, C> one, Rational<T, C> two)
        {
            if (one is Infinities)
            {
                if (two is Infinities || calc.eqv(calc.zero, two.num)) return NaN;
                else if (calc.mor(calc.zero, two.num))
                    if (one is Positive_Infinity) return NegativeInfinity;
                    else if (one is Negative_Infinity) return PositiveInfinity;

                return one;
            }
            else if (two is Infinities) return two * one;

            return new Rational<T, C>(calc.mul(one.num, two.num), calc.mul(one.denom, two.denom));
        }

        public static Rational<T, C> operator /(Rational<T, C> one, Rational<T, C> two)
        {
            if (one is Infinities)
            {
                if (two is Infinities || calc.eqv(two.num, calc.zero)) return NaN;

                else if (calc.mor(calc.zero, two.num))
                {
                    if (one is Positive_Infinity) return NegativeInfinity;
                    else if (one is Negative_Infinity) return PositiveInfinity;
                }

                return one;
            }
            else if (two is Infinities)
            {
                if (one is Infinities || two is NotANumber) return NaN;
                return new Rational<T, C>(calc.zero, calc.fromInt(1));   // нулевое значение при делении на бесконечность
            }

            if (calc.eqv(two.num, calc.zero)) { Rational<T, C> tmp = new Rational<T, C>(one.num, calc.zero); checkInf(ref tmp); return tmp; }
            return new Rational<T, C>(calc.mul(one.num, two.denom), calc.mul(one.denom, two.num));
        }

        /// <summary>
        /// Унарный минус.
        /// </summary>
        /// <param name="one"></param>
        /// <returns></returns>
        public static Rational<T,C> operator -(Rational<T,C> one)
        {
            if (one is Infinities)
            {
                if (one is Positive_Infinity) 
                    return NegativeInfinity;
                
                else if (one is Negative_Infinity) 
                    return PositiveInfinity;
                
                else return one;
            }

            return new Rational<T, C>(calc.negate(one.num), one.denom);
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

            T denomLcm = WhiteMath<T, C>.LowestCommonMultiple(one.denom, two.denom, WhiteMath<T, C>.GreatestCommonDivisor(one.denom, two.denom));
            return calc.mor(calc.mul(one.num, calc.div(denomLcm, one.denom)), calc.mul(two.num, calc.div(denomLcm, two.denom)));
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
            return calc.div(obj.num, obj.denom);
        }

        //-----------------------------------
        //-----INHERITED OVERRIDING----------
        //-----------------------------------

        /// <summary>
        /// Gets the exact deep copy of the current number.
        /// </summary>
        public object Clone()
        {
            return new Rational<T, C>(calc.getCopy(this.num), calc.getCopy(this.denom));
        }

        /// <summary>
        /// Gets the hash code for current number.
        /// Still works stupidly (though reasonable), but will be fixed soon.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return num.GetHashCode() + denom.GetHashCode();
        }

        /// <summary>
        /// Checks if two Rationals store the same numeric value.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Rational<T, C>)) return false;
            else if (this is Infinities || obj is Infinities)
            {
                if (this is Positive_Infinity && obj is Positive_Infinity ||
                    this is Negative_Infinity && obj is Negative_Infinity)
                    return true;
                
                return false;
            }
            else
                return (obj as Rational<T, C>).num.Equals(this.num) && (obj as Rational<T, C>).denom.Equals(this.denom);
        }

        // -------------------------------------- String representation

        /// <summary>
        /// Used by the overloaded ToString() method, provides one of the following number formats:
        /// 
        /// 1. IntegerPair:     [num; denom]
        /// 2. Num_Div_Denom:   num/denom
        /// 3. Both:            [num/denom]
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
                return num.ToString() + "/" + denom.ToString();
            else if (formatType == NumberFormat.IntegerPair)
                return String.Format("[{0}; {1}]", num, denom);
            else
                return String.Format("[{0}/{1}]", num, denom);
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
            bool outerNegative = false;     // внешний флаг минуса перед скобкой
            
            value = value.Replace(" ", ""); // убираем пробелы

            if (value[0] == '-')            // убираем внешний минус
            {
                value = value.Substring(1);
                outerNegative = true;
            }

            // Убираем внешние скобки

            if (value[0] == '[' && value[value.Length - 1] == ']')
                value = value.Substring(1, value.Length - 2);

            string[] split;

            if (value.Contains(';'))
                split = value.Split(';');
            else
                split = value.Split('/');

            Rational<T,C> tmp = new Rational<T, C>(calc.parse(split[0]), calc.parse(split[1]));

            if (outerNegative)
                return -tmp;
            else
                return tmp;
        }
    }
}
