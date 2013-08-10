using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using whiteMath.ArithmeticLong;

namespace whiteMath
{
    /// <summary>
    /// This class provides generic mathematic algorithms for different purposes.
    /// It makes heavy use of <see cref="Numeric&lt;T,C&gt;"/> class
    /// so that many of the algorithms work for arbitrary numeric types which have a valid calculator 
    /// (i.e. an <see cref="ICalc&lt;T&gt;"/> instance). For various algorithms various restrictions are
    /// applied, though, e.g. a GCD algorithm would require the <typeparamref name="T"/> type to be an integer type.
    /// </summary>
    /// <typeparam name="T">The type of numbers passed to mathematic functions.</typeparam>
    /// <typeparam name="C">A calculator type for the <typeparamref name="T"/> type.</typeparam>
    public partial class WhiteMath<T, C> where C : ICalc<T>, new()
    {
        private static C calc = Numeric<T,C>.Calculator;

        /// <summary>
        /// The signum function.
        /// Let 'zero' be the calculator's zero() method result/
        /// Signum of the number equals:
        /// 
        /// a) 1, if (number > zero);
        /// b) 0, if (number == zero);
        /// c) -1, if (number &lt; zero);
        /// </summary>
        /// <param name="number">The number to evaluate.</param>
        /// <returns>
        /// a) 1, if (number > zero);
        /// b) 0, if (number == zero);
        /// c) -1, if (number &lt; zero);
        ///</returns>
        public static int Sign(T number)
        {
            if (calc.mor(number, calc.zero))
                return 1;

            else if (calc.mor(calc.zero, number))
                return -1;
            
            else
                return 0;
        }

        /// HERON SQUARE ROOT
        /// 
        /// <summary>
        /// Performs a square root calculation using simple Heron algorithm.
        /// Works only for "positive" (number >= calculator.zero()) numbers.
        /// Calculator should have reasonable fromInt() method implemented, because of the formula:
        /// 
        /// x_{n+1} = 1/2 * (x_{n} + a/x_{n})
        /// 
        /// Iteration would stop when the absolute difference between the numbers
        /// is less than epsilon parameter or when the iteration step results in no change.
        /// 
        /// Restrictions:
        /// 
        /// 1. The calculator should have reasonable fromInt() method implemented and return a correct
        /// equivalent for "2".
        /// 2. Suitable for floating-point numbers.
        /// 
        /// Speed:
        /// 
        /// Due to genericity, the x_{0} is evaluated equal to the passed number.
        /// About log(N) iterations is needed.
        /// 
        /// </summary>
        /// <param name="number">The number whose square root is to be found.</param>
        /// <param name="epsilon">The precision of the calculation.</param>
        /// <returns>The result of square root computation.</returns>
        public static T SquareRootHeron(T number, T epsilon)
        {
            if (calc.mor(calc.zero, number))
                throw new ArgumentException("The number passed: "+number.ToString()+" is a forbidden negative value.");

            Numeric<T,C> twoEquivalent = calc.fromInt(2);

            Numeric<T,C> xOld;
            Numeric<T,C> xNew;

            xOld = number;

            while(true)
            {
                xNew = (xOld + number / xOld) / twoEquivalent;
                if (xNew == xOld || calc.mor(epsilon, Abs(xNew - xOld))) break;
                xOld = xNew;
            }

            return xNew;
        }

        /// POWER INTEGER
        /// 
        /// <summary>
        /// Performs the quick mathematical power operation.
        /// Works only for integer exponent values.
        /// </summary>
        /// <param name="number">The number to raise to the power.</param>
        /// <param name="power">The exponent of the power.</param>
        /// <returns>The number raised to the integer power.</returns>
        public static T PowerInteger(T number, long power)
        {
            if (power == 0) return calc.fromInt(1);
            else if (power < 0)
            {
                if (!calc.mor(number, calc.zero))
                    throw new ArgumentException("Cannot raise a non-positive number to a negative power.");
                return calc.div(calc.fromInt(1), PowerInteger(number, -power));
            }

            // Ноль в любой степени будет ноль:

            if (calc.eqv(number, calc.zero))
                return calc.zero;

            Numeric<T,C> res = Numeric<T,C>._1;              // результат возведения в степень
            Numeric<T,C> copy = calc.getCopy(number);        // изменяемая копия (переданное число может быть ссылочным типом)

            while (power > 0)
            {
                // Если остаток от деления на 2 равен 1
                if ((power & 1) == 1)
                    res *= copy;
            
                copy *= copy;
                power >>= 1;
            }

            return res;
        }

        /// POWER INTEGER
        /// 
        /// <summary>
        /// Performs the quick mathematical power operation.
        /// Works only for integer exponent values.
        /// 
        /// WARNING! The power value here should be an integer number,
        /// that is, the calculator method 'integerPower(power)' should
        /// return the same value as power. Otherwise, the result
        /// would be unpredictable AND INCORRECT.
        /// </summary>
        /// <param name="number">The number to raise to the power.</param>
        /// <param name="power">The INTEGER exponent of the power.</param>
        /// <returns>The number raised to the integer power.</returns>
        public static T PowerInteger_Generic(T number, T power)
        {
            power = calc.intPart(power);

            if (power == Numeric<T,C>.Zero) return calc.fromInt(1);
            
            else if (power < Numeric<T,C>.Zero)
            {
                if (number <= Numeric<T,C>.Zero)
                    throw new ArgumentException("Cannot raise a non-positive number to a negative power.");
                return calc.div(calc.fromInt(1), PowerInteger_Generic(number, calc.negate(power)));
            }

            // Ноль в любой степени будет ноль:

            if (calc.eqv(number, calc.zero))
                return calc.zero;

            Numeric<T, C> res = calc.fromInt(1);              // результат возведения в степень
            Numeric<T, C> copy = calc.getCopy(number);        // изменяемая копия (переданное число может быть ссылочным типом)

            T two = calc.fromInt(2);
            T one = calc.fromInt(1);

            while (power > Numeric<T,C>.Zero)
            {
                // Если остаток от деления на 2 равен 1
                if (calc.eqv(WhiteMath<T,C>.Modulus(power, two), one))
                    res *= copy;

                copy *= copy;
                power = calc.intPart(calc.div(power, two));
            }

            return res;
        }
    }
}
