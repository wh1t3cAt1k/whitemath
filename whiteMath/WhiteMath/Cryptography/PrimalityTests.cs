using System;

using WhiteMath.Mathematics;
using WhiteMath.Calculators;
using WhiteMath.Randoms;
using WhiteMath.ArithmeticLong;

using WhiteStructs.Conditions;

namespace WhiteMath.Cryptography
{
    /// <summary>
    /// This class provides methods for testing whether a number is prime.
    /// </summary>
    public static class PrimalityTests
    {
        /*
         * if n < 1,373,653, it is enough to test a = 2 and 3;
         * if n < 9,080,191, it is enough to test a = 31 and 73;
         * if n < 4,759,123,141, it is enough to test a = 2, 7, and 61;
         * if n < 2,152,302,898,747, it is enough to test a = 2, 3, 5, 7, and 11;
         * if n < 3,474,749,660,383, it is enough to test a = 2, 3, 5, 7, 11, and 13;
         * if n < 341,550,071,728,321, it is enough to test a = 2, 3, 5, 7, 11, 13, and 17.
         */

        /// <summary>
        /// Performs a Miller's deterministic prime test on a number.
        /// Goes over all numbers under the <c>floor(ln^2(<paramref name="number"/>))</c> boundary.
        /// </summary>
        /// <typeparam name="B">
        /// A class specifying the digit base of <c>LongInt&lt;<typeparamref name="B"/></c> type.
        /// The base should be an integer power of two.
        /// </typeparam>
        /// <remarks>This test relies upon Riemann's Generalized Hypothesis, which still remains unproven. Use with caution.</remarks>
        /// <param name="number">A number, bigger than 1, to test for primality.</param>
        /// <returns><c>true</c>, if the number is prime, <c>false</c> otherwise.</returns>
		public static bool IsPrimeMiller<B>(this LongInt<B> number)
            where B: IBase, new()
        {
			Condition.ValidateNotNull(number, nameof(number));
			Condition
				.Validate(LongInt<B>.IsBasePowerOfTwo)
			    .OrArgumentException("The digit base of the number should be a strict power of two.");
			Condition
				.Validate(number > 1)
				.OrArgumentOutOfRangeException("The tested number should be bigger than 1.");

			if (number.IsEven)
			{
				return number == 2;
			}

			if (number == 3)
			{
				return true;
			}

			// We will factorize (number - 1) as 2^s * t
			// -
            LongInt<B> t;
            long s;

            LongInt<B> numDecremented = number - 1;

            MillerRabinFactorize(numDecremented, out t, out s);

            LongInt<B> upperBound = Mathematics<LongInt<B>, CalcLongInt<B>>.Min(
				number.LengthInBinaryPlaces * number.LengthInBinaryPlaces, 
				numDecremented);

			for (LongInt<B> i = 2; i <= upperBound; i++)
			{
				if (!IsMillerRabinPrimeWitness(i, number, t, s))
				{
					return false;
				}
			}
            return true;
        }

        /// <summary>
        /// Performs a Wilson's deterministic primality test of a number.
        /// Usually takes an enormous amount of time and is never used in practice.
        /// </summary>
		/// <typeparam name="B">
		/// A class specifying the digit base of <see cref="LongInt{B}"/> type.
		/// </typeparam>
        /// <param name="number">A number, bigger than 1, to test for primality.</param>
        /// <returns>
        /// <c>true</c>, if the number is prime, <c>false</c> otherwise. 
		/// However, if the <paramref name="number"/> is big enough, 
		/// you should not expect this method to return anything, as
        /// it is going to run until the heat death of the Universe,
		/// at which moment the primality of any number will not be of
		/// interest to you or your computer.
        /// </returns>
		/// <remarks>
		/// The time complexity of the algorithm is <c>O(n!)</c>, 
		/// where <c>n</c> is the magnitude of the number.
		/// </remarks>
		public static bool IsPrimeWilsonTheorem<B>(this LongInt<B> number)
            where B: IBase, new()
        {
			Condition.ValidateNotNull(number, nameof(number));
			Condition
				.Validate(number > 1)
				.OrArgumentOutOfRangeException("The tested number should be bigger than 1");

			LongInt<B> result = 1;

			for (LongInt<B> i = 2; i < number - 1; ++i)
			{
				result = result * i % number;
			}

			return result == 1;
        }

        /// <summary>
        /// Performs a deterministic primality test on a LongInt number.
        /// Usually takes an enormous amount of time and is never used in practice.
        /// </summary>
        /// <typeparam name="B">A class specifying the digit base of <c>LongInt&lt;<typeparamref name="B"/></c> type.</typeparam>
        /// <param name="num">A number, bigger than 1, to test for primality.</param>
        /// <returns>
        /// True if the number is prime, false otherwise. Nevertheless, if the <paramref name="num"/> is big enough, you aren't to expect this method to return anything. 
        /// It will run until the Universe collapses.
        /// </returns>
		public static bool IsPrimeTrialDivision<B>(this LongInt<B> num)
            where B: IBase, new()
        {
			Condition.ValidateNotNull(num, nameof(num));
			Condition
				.Validate(num > 1)
				.OrArgumentOutOfRangeException("The tested number should be bigger than 1.");

			LongInt<B> numberRoot = LongInt<B>.Helper.SquareRootInteger(num);

			for (LongInt<B> i = 2; i <= numberRoot + 1; ++i)
			{
				if (num % i == 0)
				{
					return false;
				}
			}

            return true;
        }

        /// <summary>
        /// Performs a Solovay-Strassen stochastic primality test of a long integer number
        /// and returns the probability that the number is composite. 
        /// </summary>
        /// <typeparam name="B">A class specifying the digit base of <c>LongInt&lt;<typeparamref name="B"/></c> type.</typeparam>
        /// <param name="number">A number, bigger than 1, to test for primality.</param>
        /// <param name="generator">
        /// A bounded <c>LongInt&lt;<typeparamref name="B"/></c> random generator. 
        /// For probability estimations to be correct, this generator should guarantee uniform distribution 
        /// for any given interval.
        /// </param>
        /// <param name="countRounds">
        /// A positive number of testing rounds. Recommended to be more than <c>log2(<paramref name="number"/>)</c>.
        /// </param>
        /// <returns>
        /// The probability that the <paramref name="number"/> is composite. 
        /// Equals to <c>2^(-<paramref name="countRounds"/>)</c>, which is
		/// worse than for <see cref="CalculateCompositeProbabilityMillerRabin{B}"/>.
        /// </returns>
		public static double CalculateCompositeProbabilitySolovayStrassen<B>(
			this LongInt<B> number, 
			IRandomBounded<LongInt<B>> generator, 
			long countRounds)
            where B : IBase, new()
        {
			Condition.ValidateNotNull(number, nameof(number));
			Condition.ValidateNotNull(generator, nameof(generator));
			Condition
				.Validate(number > 1)
				.OrArgumentOutOfRangeException("The tested number should be bigger than 1.");
			Condition
				.Validate(countRounds > 0)
				.OrArgumentOutOfRangeException("The number of rounds should be positive.");

			if (number.IsEven)
			{
				return (number == 2 ? 0 : 1);
			}
			else if (number == 3)
			{
				return 0;
			}
            
			LongInt<B> numberHalf = (number - 1) / 2;

            for (long i = 0; i < countRounds; i++)
            {
				LongInt<B> randomNumber = generator.Next(2, number);

                int jacobiSymbol = Mathematics<LongInt<B>, CalcLongInt<B>>.JacobiSymbol(randomNumber, number);

				// The Jacobi symbol being equal to zero means that the numbers are not
				// coprime, meaning that the original number is composite.
				// -
				if (jacobiSymbol == 0)
				{
					return 1;
				}

                LongInt<B> powered = LongInt<B>.Helper.PowerIntegerModular(randomNumber, numberHalf, number);

				if (jacobiSymbol == 1 && powered != 1 
					|| jacobiSymbol == -1 && powered != number - 1)
				{
					return 1;
				}
            }

            return Math.Pow(2, -countRounds);
        }

        /// <summary>
        /// Performs a Fermat stochastic primality test of a long integer number.
        /// </summary>
		/// <typeparam name="B">
		/// A class specifying the digit base of 
		/// the <see cref="LongInt{B}"/> type.
		/// </typeparam>
        /// <param name="number">A number to test for primality. Should be more than 1.</param>
        /// <param name="generator">
		/// A bounded <see cref="LongInt{B}"/> random generator. For probability 
		/// estimations to be correct, this generator should guarantee uniform 
		/// distribution for any given interval.
        /// </param>
        /// <param name="countRounds">A positive number of testing rounds.</param>
        /// <returns>
		/// If this method returns false, the number is guaranteed to be composite. 
		/// Otherwise the, number is probably prime, but it's not always the case.
		/// </returns>
		public static bool IsPrimeFermat<B>(
			this LongInt<B> number, 
			IRandomBounded<LongInt<B>> generator, 
			long countRounds)
            where B: IBase, new()
        {
			Condition.ValidateNotNull(number, nameof(number));
			Condition.ValidateNotNull(generator, nameof(generator));
			Condition
				.Validate(number > 1)
				.OrArgumentOutOfRangeException("The number to test should be bigger than 1.");
			Condition
				.Validate(countRounds > 0)
				.OrArgumentOutOfRangeException("The number of rounds should be positive");

			if (number.IsEven)
			{
				return number == 2;
			}
			else if (number == 3)
			{
				return true;
			}

            LongInt<B> numDecremented = number - 1;

            for (long i = 0; i < countRounds; i++)
            {
                LongInt<B> test = generator.Next(2, number);

				if (LongInt<B>.Helper.PowerIntegerModular(test, numDecremented, number) != 1)
				{
					return false;
				}
            }

            return true;
        }

        /// <summary>
        /// Performs a Miller-Rabin stochastic primality test of a long integer number
        /// and returns the probability that the number is composite. 
        /// </summary>
        /// <typeparam name="B">A class specifying the digit base of <c>LongInt&lt;<typeparamref name="B"/></c> type.</typeparam>
        /// <param name="number">The number to test for primality. Should be more than 1.</param>
        /// <param name="generator">
        /// A bounded <c>LongInt&lt;<typeparamref name="B"/></c> random generator. 
        /// For probability estimations to be correct, this generator should guarantee uniform distribution 
        /// for any given interval.
        /// </param>
        /// <param name="countRounds">
        /// A positive number of testing rounds. Recommended to be more than <c>log2(<paramref name="number"/>)</c>.
        /// </param>
        /// <returns>The probability that the <paramref name="number"/> is composite.</returns>
		public static double CalculateCompositeProbabilityMillerRabin<B>(
			this LongInt<B> number, 
			IRandomBounded<LongInt<B>> generator, 
			long countRounds) 
            where B: IBase, new()
        {
			Condition.ValidateNotNull(number, nameof(number));
			Condition.ValidateNotNull(generator, nameof(generator));
			Condition
				.Validate(number > 1)
				.OrArgumentOutOfRangeException("The number to test should be bigger than 1.");
			Condition
				.Validate(countRounds > 0)
				.OrArgumentOutOfRangeException("The number of rounds should be positive");

			if (number.IsEven)
			{
				return (number == 2 ? 0 : 1);
			}
			else if (number == 3)
			{
				return 0;
			}

			// We will factorize (number - 1) as 2^s * t
			// -
            LongInt<B> t;
            long s;

            MillerRabinFactorize(number - 1, out t, out s);

            for (int i = 0; i < countRounds; i++)
            {
                LongInt<B> x = generator.Next(2, number - 1);

                if (!IsMillerRabinPrimeWitness(x, number, t, s))
                    return 1;
            }

            return Math.Pow(4, -countRounds);
        }

        /// <summary>
        /// Factorizes an even number as 2^s * t.
        /// </summary>
		private static void MillerRabinFactorize<B>(LongInt<B> number, out LongInt<B> t, out long s)
            where B: IBase, new()
        {
			Condition.Validate(number.IsEven).OrException(new Exception());

            s = 0;

            while (number.IsEven)
            {
                number >>= 1;
                ++s;
            }

            t = number;
        }

        /// <returns>
        /// <c>true</c>, if the number is a primality witness.
		/// A value of <c>false</c> indicates deterministically
		/// that the number is composite.
        /// </returns>
		private static bool IsMillerRabinPrimeWitness<B>(
			LongInt<B> candidate, 
			LongInt<B> number, 
			LongInt<B> t, 
			long s)
            where B: IBase, new()
        {
            candidate = LongInt<B>.Helper.PowerIntegerModular(candidate, t, number);

			if (candidate == 1 || candidate == number - 1)
			{
				return true;
			}

            for (int j = 0; j < s - 1; j++)
            {
                candidate = candidate * candidate % number;

				if (candidate == 1)
				{
					// Not a single bracket group will be divisible.
					// The number is composite.
					// -
					return false;
				}
				else if (candidate == number - 1)
				{
					// This bracket group is divisible, thus, the
					// candidate number is a prime witness indeed.
					// -
					return true;
				}
            }

            // Not a single bracket group is divisible.
			// The number is composite.
			// -
            return false;
        }
    }
}
