using System;

using WhiteMath.Calculators;

using WhiteStructs.Conditions;

namespace WhiteMath.Mathematics
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
    public partial class Mathematics<T, C> where C : ICalc<T>, new()
    {
        private static C Calculator = Numeric<T,C>.Calculator;

        /// <summary>
        /// The signum function.
        /// Let 'zero' be the calculator's zero() method result.
        /// The sign of the number will be equal to:
        /// 
		/// a) 1, if (number &gt; zero);
        /// b) 0, if (number == zero);
        /// c) -1, if (number &lt; zero);
        /// </summary>
        /// <param name="number">The number to evaluate.</param>
        /// <returns>
		/// a) 1, if (number &gt; zero);
        /// b) 0, if (number == zero);
        /// c) -1, if (number &lt; zero);
        ///</returns>
        public static int Sign(T number)
        {
			if (Calculator.GreaterThan(number, Calculator.Zero))
			{
				return 1;
			}
			else if (Calculator.GreaterThan(Calculator.Zero, number))
			{
				return -1;
			}
			else
			{
				return 0;
			}
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
			Condition.Validate(!Calculator.GreaterThan(Calculator.Zero, number)).OrArgumentException();

			if (Calculator.GreaterThan(Calculator.Zero, number))
			{
				throw new ArgumentException(Messages.ArgumentShouldBeNonNegative);
			}

			Numeric<T,C> two = Numeric<T, C>._2;

            Numeric<T,C> xOld;
            Numeric<T,C> xNew;

            xOld = number;

            while(true)
            {
                xNew = (xOld + number / xOld) / two;

				if (xNew == xOld || Calculator.GreaterThan(epsilon, Abs(xNew - xOld)))
				{
					break;
				}

                xOld = xNew;
            }

            return xNew;
        }

        /// <summary>
        /// Performs the quick mathematical power operation.
        /// Works only for integer exponent values.
        /// </summary>
        /// <param name="number">The number to raise to the power.</param>
        /// <param name="power">The exponent of the power.</param>
        /// <returns>The number raised to the integer power.</returns>
        public static T PowerInteger(T number, long power)
        {
			if (power == 0)
			{
				return Calculator.FromInteger(1);
			}
            else if (power < 0)
            {
				Condition.Validate(Calculator.GreaterThan(number, Calculator.Zero)).OrArgumentException(Messages.CannotRaiseNonPositiveArgumentToNegativePower);
                return Calculator.Divide(Calculator.FromInteger(1), PowerInteger(number, -power));
            }

			if (Calculator.Equal(number, Calculator.Zero))
			{
				return Calculator.Zero;
			}

            Numeric<T,C> result = Numeric<T,C>._1;
            Numeric<T,C> numberCopy = Calculator.GetCopy(number);

            while (power > 0)
            {
				if ((power & 1) == 1)
				{
					result *= numberCopy;
				}
            
                numberCopy *= numberCopy;
                power >>= 1;
            }

            return result;
        }
			
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
			T powerCopy = Calculator.GetCopy(power);
			powerCopy = Calculator.IntegerPart(powerCopy);

			if (Calculator.Equal(powerCopy, Calculator.Zero))
			{
				return Calculator.FromInteger(1);
			}
			else if (Calculator.GreaterThan(Calculator.Zero, powerCopy))
            {
				Condition.Validate(Calculator.GreaterThan(number, Calculator.Zero)).OrArgumentException(Messages.CannotRaiseNonPositiveArgumentToNegativePower);
				return Calculator.Divide(Calculator.FromInteger(1), PowerInteger_Generic(number, Calculator.Negate(power)));
            }

			if (Calculator.Equal(number, Calculator.Zero))
			{
				return Calculator.Zero;
			}

			Numeric<T, C> result = Numeric<T, C>._1;
            Numeric<T, C> numberCopy = Calculator.GetCopy(number);

			T two = Numeric<T, C>._2;
			T one = Numeric<T, C>._1;

			while (powerCopy > Numeric<T,C>.Zero)
            {
				if (Calculator.Equal(Mathematics<T,C>.Modulus(powerCopy, two), one))
				{
					result *= numberCopy;
				}

                numberCopy *= numberCopy;
				powerCopy = Calculator.IntegerPart(Calculator.Divide(powerCopy, two));
            }

            return result;
        }
    }
}
