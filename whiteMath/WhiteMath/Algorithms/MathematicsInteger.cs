﻿using System;

using WhiteMath.Calculators;

using WhiteStructs.Conditions;

namespace WhiteMath.Mathematics
{
    public partial class Mathematics<T, C> where C : ICalc<T>, new()
    {
        /// <summary>
        /// Returns the value of the Jacobi symbol for
        /// the two numbers, that is, the multiplication product
        /// of Legendre symbols with numerators all equal to the Jacobi symbol's
        /// numerator and denominators taken from the factorization
        /// of Jacobi symbol's denominator.
        /// </summary>
        /// <param name="numerator">The numerator of the Jacobi symbol.</param>
        /// <param name="denominator">The denominator of the Jacobi symbol. Should be odd and positive.</param>
        /// <returns>The value of the Jacobi symbol for <paramref name="numerator"/> and <paramref name="denominator"/>.</returns>
        public static int JacobiSymbol(T numerator, T denominator)
        {
			Condition
				.Validate(Calculator.IsIntegerCalculator)
				.OrException(new NonIntegerTypeException(typeof(T).Name));
			Condition
				.Validate(denominator > Numeric<T, C>.Zero && !Numeric<T,C>.Calculator.IsEven(denominator))
				.OrArgumentException("The denominator of the Jacobi symbol should be odd and positive.");

            bool minus = false;

            Numeric<T, C> x = numerator;
            Numeric<T, C> y = denominator;

            if(y == Numeric<T, C>._1)
                return 1;

            if (Mathematics<T, C>.GreatestCommonDivisor(x, y) != Numeric<T, C>._1)
                return 0;

            if (x < Numeric<T,C>.Zero)
            {
                // Надо домножить на (-1)^((y-1)/2)
                // Эта величина равна -1 тогда и только тогда, когда floor(y/2) - четное число.

                if ((y / Numeric<T, C>._2).IsEven)
                    minus ^= true;

                x = -x;
            }

			// Neither the numerator
			// nor the denominator have negative
			// values at this step.

            while (true)
            {
                // (x; y) = (x mod y; y) ------------------

                if (x > y)
                    x = x % y;

                // ---------- избавляемся от четности -----

                int t = 0;

                while(x.IsEven)
                {
                    // Надо домножить на (-1)^((y^2 - 1)/8)
                    // Эта величина равна -1 тогда и только тогда, когда y имеет остатки 3 или 5 при делении на 8

                    ++t;
                    x /= Numeric<T, C>._2;
                }

                if (t % 2 != 0)
                {
                    Numeric<T, C> rem = y % Numeric<T,C>._8;

                    if (rem == Numeric<T, C>._3 || rem == Numeric<T, C>._5)
                        minus ^= true;
                }

                // ----------------------------------------
                // --- Если x - единица, то надо вернуться.
                // ----------------------------------------

                if (x == Numeric<T, C>._1)
                    return (minus ? -1 : 1);

                // ----------------------------------------------------
                // -- x и y на этом этапе гарантированно взаимно просты
                // -- и нечетны, поэтому можно использовать свойство (8) из твоей тетрадочки
                // ----------------------------------------------------

                if (x < y)
                {
                    if (x % Numeric<T, C>._4 == Numeric<T,C>._3 &&
                        y % Numeric<T, C>._4 == Numeric<T,C>._3)
                        minus ^= true;

                    Numeric<T, C> tmp = x;
                    x = y;
                    y = tmp;
                }
            }
        }

        /// <summary>
        /// Performs an Extended Euclidean algorithms on two positive numbers <c>one, two</c>,
        /// calculates their GCD and finds such integer <c>x, y</c> which satisfy Bezout's identity <c>one*x + two*y = 1</c>.
        /// </summary>
        /// <param name="one">The first number.</param>
        /// <param name="two">The second number.</param>
        /// <param name="x">The first coefficient in Bezout's identity <c>one*x + two*y = 1</c>.</param>
        /// <param name="y">The second coefficient in Bezout's identity <c>one*x + two*y = 1</c>.</param>
        /// <returns>The greatest common divisor of <paramref name="one"/> and <paramref name="two"/>.</returns>
        public static T ExtendedEuclideanAlgorithm(T one, T two, out T x, out T y)
        {
			Condition
				.Validate(Calculator.IsIntegerCalculator)
				.OrException(new NonIntegerTypeException(typeof(T).Name));
			Condition
				.Validate(one != Numeric<T,C>.Zero && two != Numeric<T,C>.Zero)
				.OrArgumentOutOfRangeException("None of the numbers should be zero.");

			/*
            Contract.Ensures(Contract.Result<T>() > Numeric<T,C>.Zero);
            Contract.Ensures((Numeric<T, C>)one % Contract.Result<T>() == Numeric<T, C>.Zero);
            Contract.Ensures((Numeric<T, C>)two % Contract.Result<T>() == Numeric<T, C>.Zero);
            */

            // Uncomment only on tests
            // because int often overflows here.
            // Contract.Ensures((Numeric<T, C>)one * Contract.ValueAtReturn<T>(out x) + (Numeric<T, C>)two * Contract.ValueAtReturn<T>(out y) == Contract.Result<T>());

            if (Calculator.GreaterThan(two, one))
                return ExtendedEuclideanAlgorithm(two, one, out y, out x);

            bool oneLessThanZero = Calculator.GreaterThan(Calculator.Zero, one);
            bool twoLessThanZero = Calculator.GreaterThan(Calculator.Zero, two);

            if (oneLessThanZero || twoLessThanZero)
            {
                if (oneLessThanZero && twoLessThanZero)
                {
                    T res = ExtendedEuclideanAlgorithm(Calculator.Negate(one), Calculator.Negate(two), out x, out y);
                    
                    x = Calculator.Negate(x);
                    y = Calculator.Negate(y);

                    return res;
                }
                else if (oneLessThanZero)
                {
                    T res = ExtendedEuclideanAlgorithm(Calculator.Negate(one), two, out x, out y);
                    x = Calculator.Negate(x);

                    return res;
                }
                else
                {
                    T res = ExtendedEuclideanAlgorithm(one, Calculator.Negate(two), out x, out y);
                    y = Calculator.Negate(y);

                    return res;
                }
            }

            Numeric<T, C> a = one;
            Numeric<T, C> b = two;

            Numeric<T, C>
                //
                tmp,
                curX = Numeric<T, C>.Zero,
                curY = Numeric<T, C>._1,
                lastX = Numeric<T, C>._1,
                lastY = Numeric<T, C>.Zero;

            while (b != Numeric<T, C>.Zero)
            {
                Numeric<T, C> quotient = a / b;

                tmp = a;
                
                a = b;
                b = tmp % b;

                tmp = curX;

                curX = lastX - quotient * curX;
                lastX = tmp;

                tmp = curY;

                curY = lastY - quotient * curY;
                lastY = tmp;
            }

            x = lastX;
            y = lastY;

            return a;
        }

        /// <summary>
        /// Finds a multiplicative inverse of a positive number on a coprime module.
        /// </summary>
		/// <param name="number">A positive number coprime to and less than '<paramref name="modulus"/>'.</param>
		/// <param name="modulus">A positive number coprime to and bigger than '<paramref name="number"/>'.</param>
        /// <exception>
		/// If <paramref name="number"/> and <paramref name="modulus"/> are not coprime, 
		/// an <see cref="ArgumentException"/> is thrown.
        /// </exception>
		/// <returns>A number which, multiplied by <paramref name="number"/>, results in <see cref="Numeric{T,C}._1"/></returns>
        public static T MultiplicativeInverse(T number, T modulus)
        {
			Condition
				.Validate((Numeric<T, C>)modulus > number)
				.OrArgumentOutOfRangeException("The modulus should be larger than the number.");	
			Condition
				.Validate(number > Numeric<T,C>.Zero)
				.OrArgumentOutOfRangeException("The number should be positive.");
			
			/*
            Contract.Ensures((Numeric<T,C>)number * Contract.Result<T>() % module == Numeric<T,C>._1);
			*/

            T gcd, x, y;

            gcd = ExtendedEuclideanAlgorithm(number, modulus, out x, out y);

			Condition
				.Validate(gcd == Numeric<T,C>._1)
				.OrArgumentException("The number and the modulus are not coprime.");

            // Since the result might be negative,
            // add modulus until it is positive.

			while (Calculator.GreaterThan(Calculator.Zero, x))
			{
				x = Calculator.Add(x, modulus);
			}

            return x;
        }

        /// <summary>
        /// Finds the greatest common divisor of two integer-like numbers
        /// using the simple Euclid algorithm.
        /// 
        /// The calculator for the numbers is recommended to provide reasonable implementation
        /// of the remainder operation (%) - otherwise, the function
        /// Modulus from whiteMath class will be used.
        /// 
        /// Will work with floating-point type numbers only if they are integers;
        /// In case of division errors the result will be rounded and thus not guaranteed.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static T GreatestCommonDivisor(T one, T two)
        {
			Condition
				.Validate(one != Numeric<T,C>.Zero && two != Numeric<T,C>.Zero)
				.OrArgumentException("None of the arguments may be zero.");
            
			/*
            Contract.Ensures(Contract.Result<T>() > Numeric<T,C>.Zero);
            Contract.Ensures((Numeric<T,C>)one % Contract.Result<T>() == Numeric<T,C>.Zero);
            Contract.Ensures((Numeric<T,C>)two % Contract.Result<T>() == Numeric<T,C>.Zero);
			*/

            // T может быть ссылочным типом, поэтому необходимо
            // предостеречь объекты от изменения
            // а также отсечь знаки по необходимости
            
            T oneTmp;
            T twoTmp;

            if (Calculator.GreaterThan(Calculator.Zero, one))
                oneTmp = Calculator.Negate(one);
            else
                oneTmp = Calculator.GetCopy(one);

            if (Calculator.GreaterThan(Calculator.Zero, two))
                twoTmp = Calculator.Negate(two);
            else
                twoTmp = Calculator.GetCopy(two);

            try
            {
                while (!Calculator.Equal(oneTmp, Calculator.Zero) && !Calculator.Equal(twoTmp, Calculator.Zero))
                {
                    if (Calculator.GreaterThan(oneTmp, twoTmp))
                        oneTmp = Calculator.Modulo(oneTmp, twoTmp);
                    else
                        twoTmp = Calculator.Modulo(twoTmp, oneTmp);
                }
            }
            catch   // calculator throws exception - working with fp's.
            {
                oneTmp = Calculator.IntegerPart(oneTmp);
                twoTmp = Calculator.IntegerPart(twoTmp);

                while (!Calculator.Equal(oneTmp, Calculator.Zero) && !Calculator.Equal(twoTmp, Calculator.Zero))
                {
                    if (Calculator.GreaterThan(oneTmp, twoTmp))
                        oneTmp = Mathematics<T,C>.Round(Mathematics<T,C>.Modulus(oneTmp, twoTmp));
                    else
                        twoTmp = Mathematics<T,C>.Round(Mathematics<T,C>.Modulus(twoTmp, oneTmp));
                }
            }

            return Calculator.Add(oneTmp, twoTmp);
        }

        /// <summary>
        /// Finds the lowest common multiple of two integer-like numbers from the equation:
        /// A * B = gcd(A,B) * lcm(A,B)
        /// </summary>
        /// <param name="one">The first number.</param>
        /// <param name="two">The second number.</param>
        /// <returns></returns>
        public static T LowestCommonMultiple(T one, T two)
        {
            return LowestCommonMultiple(one, two, GreatestCommonDivisor(one, two));
        }

        /// <summary>
        /// Finds the lowest common multiple of two integer-like numbers from the equation:
        /// A * B = gcd(A,B) * lcm(A,B)
        /// </summary>
        /// <param name="one">The first number.</param>
        /// <param name="two">The second number.</param>
        /// <param name="greatestCommonDivisor">The greatest common divisor for the numbers. Optional, if nothing is specified, it will be calculated.</param>
        /// <returns></returns>
        public static T LowestCommonMultiple(T one, T two, T greatestCommonDivisor)
        {
            return Calculator.Divide(Calculator.Multiply(one, two), greatestCommonDivisor);
        }

        /// <summary>
        /// Returns the exact factorial of an integer number.
        /// Uses simple iteration.
        /// It is recommended that you use long integers (ex. LongInt) to avoid overflow exceptions.
        /// </summary>
        /// <param name="integer"></param>
        /// <returns></returns>
        public static T Factorial(T integer)
        {
            if (Calculator.GreaterThan(Calculator.Zero, integer))
                throw new ArgumentException("The argument for the factorial function should be a positive integer.");

            T tmp = Calculator.FromInteger(1);

            for (T counter = Calculator.FromInteger(2); !Calculator.GreaterThan(counter, integer); counter = Calculator.Increment(counter))
                tmp = Calculator.Multiply(tmp, counter);

            return tmp;
        }

        /// <summary>
        /// Checks whether the first number is some natural integer power of
        /// the argument passed. The exact value of the power is either null (if false)
        /// or int.
        /// </summary>
        /// <param name="one">The argument to test if it is the natural power of the second.</param>
        /// <param name="two">The second argument.</param>
        /// <param name="powerValue">The value of the integer power. Contains either null or the value of the integer power.</param>
        /// <returns>True if the first argument is the natural power of the second, false otherwise.</returns>
        public static bool IsNaturalIntegerPowerOf(Numeric<T,C> one, Numeric<T,C> two, out int? powerValue)
        {
            // first, the power is zero.
            powerValue = 0;

            // check if the first argument is zero, then the assertion is true.
            // the power value is ZERO!
            if (one == Numeric<T, C>._1)
                return true;

            // get the number copy
            Numeric<T, C> twoCopy = two.Copy;

            // second condition for the overflow proof.
            while (one >= twoCopy && twoCopy>=Numeric<T,C>.Zero)
            {
                // first increase the power value.
                powerValue++;

                if (one == twoCopy)
                    return true;

                // now multiply.
                twoCopy *= two;
            }

            // now the twoCopy finally is more than one.
            // that means... BIG BADA BOOM!!!!

            powerValue = null;
            return false;
        }

        // ---------------------------------------------------------------
        // --------------- integer powers --------------------------------

        /// <summary>
        /// Performs a fast modular integral modular exponentiation.
        /// </summary>
        /// <param name="number">An integer number to be exponentiated.</param>
        /// <param name="power">A non-negative integer exponent.</param>
        /// <param name="modulus">An integer modulus of the operation.</param>
        /// <returns>
		/// The result of raising the <paramref name="number"/> to 
		/// power <paramref name="power"/> modulo <paramref name="modulus"/>.
		/// </returns>
        public static T PowerIntegerModular(T number, ulong power, T modulus)
        {
			Condition
				.Validate(Calculator.IsIntegerCalculator)
				.OrException(new NonIntegerTypeException(typeof(T).Name));
            
            Numeric<T, C> result = Numeric<T, C>._1;
            Numeric<T, C> numberNumeric = number;

            while (power > 0)
            {
				if ((power & 1) == 1)
				{
					result = (result * numberNumeric) % modulus;
				}

                power >>= 1;
                numberNumeric = (numberNumeric * numberNumeric) % modulus;
            }

            return result;
        }


        // ---------------------------------------------------------------
        // --------------- integer square roots --------------------------

        /// <summary>
        /// Returns the integer part of the square root
        /// of the number.
        /// </summary>
        /// <remarks>This method works only for integer numeric types.</remarks>
        /// <param name="number">A non-negative number for which the integer part of its square root is to be found.</param>
        /// <returns>The integer part of the square root of the <paramref name="number"/>.</returns>
        [Obsolete("This method works very slowly. Consider using SquareRootInteger instead.")]
        public static T SquareRootIntegerSimple(T number)
        {
			Condition
				.Validate(Calculator.IsIntegerCalculator)
				.OrException(new NonIntegerTypeException(typeof(T).Name));
            Condition
				.Validate((Numeric<T,C>)number >= Numeric<T,C>.Zero)
				.OrArgumentOutOfRangeException("The number passed to this method should not be negative.");

            Numeric<T, C> numberNumeric = number;
            Numeric<T, C> reduction = Numeric<T, C>._1;
            Numeric<T, C> result = Numeric<T, C>._0;

            while (numberNumeric > Numeric<T, C>._0)
            {
                ++result;

                numberNumeric -= reduction;
                reduction += Numeric<T,C>._2;
            }

			if (numberNumeric < Numeric<T, C>._0)
			{
				--result;
			}

            return result;
        }

        /// <summary>
        /// Returns the integer part of the square root
        /// of the number.
        /// </summary>
        /// <remarks>This method works only for integer numeric types.</remarks>
        /// <param name="number">A strictly positive number for which the integer part of its square root is to be found.</param>
        /// <param name="firstEstimate">
        /// A first estimate of the square root. 
        /// WARNING! This number should be more than or equal to the real 
        /// square root of the <paramref name="number"/>, otherwise the behaviour 
        /// of this method is undefined.
        /// </param>
        /// <returns>The integer part of the square root of the <paramref name="number"/>.</returns>
        public static T SquareRootInteger(T number, T firstEstimate)
        {
			Condition
				.Validate(Calculator.IsIntegerCalculator)
				.OrException(new NonIntegerTypeException(typeof(T).Name));
			Condition
				.Validate((Numeric<T,C>)number >= Numeric<T,C>.Zero)
				.OrArgumentOutOfRangeException("The number passed to this method should not be negative.");

			Numeric<T, C> previousEstimate;
			Numeric<T, C> currentEstimate;

			currentEstimate = firstEstimate;

            while (true)
            {
                previousEstimate = currentEstimate;
                currentEstimate = (previousEstimate + number / previousEstimate) / Numeric<T, C>._2;

				if (currentEstimate == previousEstimate)
				{
					return currentEstimate;
				}
				else if (currentEstimate > previousEstimate)
				{
					return previousEstimate;
				}
            }
        }
    }
}
