using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

using whiteMath.Randoms;
using whiteMath.ArithmeticLong;

namespace whiteMath.Cryptography
{
    /// <summary>
    /// This class provides methods for testing whether a number is prime.
    /// </summary>
    public static class PrimalityTests
    {
        /*
             if n < 1,373,653, it is enough to test a = 2 and 3;
    if n < 9,080,191, it is enough to test a = 31 and 73;
    if n < 4,759,123,141, it is enough to test a = 2, 7, and 61;
    if n < 2,152,302,898,747, it is enough to test a = 2, 3, 5, 7, and 11;
    if n < 3,474,749,660,383, it is enough to test a = 2, 3, 5, 7, 11, and 13;
    if n < 341,550,071,728,321, it is enough to test a = 2, 3, 5, 7, 11, 13, and 17.

         */

        /// <summary>
        /// Performs a Miller's deterministic prime test on a number.
        /// Goes over all numbers under the <c>floor(ln^2(<paramref name="num"/>))</c> boundary.
        /// </summary>
        /// <typeparam name="B">
        /// A class specifying the digit base of <c>LongInt&lt;<typeparamref name="B"/></c> type.
        /// The base should be an integer power of two.
        /// </typeparam>
        /// <remarks>This test relies upon Riemann's Generalized Hypothesis, which still remains unproven. Use with caution.</remarks>
        /// <param name="num">A number, bigger than 1, to test for primality.</param>
        /// <returns>True if the number is prime according to the test, false otherwise.</returns>
        public static bool IsPrime_Miller<B>(this LongInt<B> num)
            where B: IBase, new()
        {
            Contract.Requires<ArgumentException>(LongInt<B>.BASE_is_power_of_two, "The digit base of the number should be a strict power of two.");
            Contract.Requires<ArgumentNullException>(num != null, "num");
            Contract.Requires<ArgumentOutOfRangeException>(num > 1, "The tested number should be bigger than 1.");

            if (num.IsEven)
            {
                if (num == 2)
                    return true;
                else
                    return false;
            }
            else if (num == 3)
                return true;

            // Представим число num - 1 в виде
            //
            //      num - 1 = 2^s * t

            LongInt<B> t;
            long s;

            LongInt<B> numDecremented = num - 1;

            ___millerRabinFactorize(numDecremented, out t, out s);

            LongInt<B> upperBound = WhiteMath<LongInt<B>, CalcLongInt<B>>.Min(num.LengthInBinaryPlaces * num.LengthInBinaryPlaces, numDecremented);

            for(LongInt<B> i=2; i<=upperBound; i++)
                if(!___millerRabinIsPrimeWitness(i, num, t, s))
                    return false;

            return true;
        }

        /// <summary>
        /// Performs a Wilson's deterministic prime test on a number
        /// and returns true if it is prime.
        /// Usually takes an enormous amount of time and is never used in practice.
        /// </summary>
        /// <typeparam name="B">A class specifying the digit base of <c>LongInt&lt;<typeparamref name="B"/></c> type.</typeparam>
        /// <param name="num">A number, bigger than 1, to test for primality.</param>
        /// <returns>
        /// True if the number is prime, false otherwise. Nevertheless, if the <paramref name="num"/> is big enough, you aren't to expect this method to return anything. 
        /// It will run until the Universe collapses.
        /// </returns>
        public static bool IsPrime_WilsonTheorem<B>(this LongInt<B> num)
            where B: IBase, new()
        {
            Contract.Requires<ArgumentNullException>(num != null, "num");
            Contract.Requires<ArgumentOutOfRangeException>(num > 1, "The tested number should be bigger than 1.");

            LongInt<B> res = 1;

            for (LongInt<B> i = 2; i < num - 1; i++)
                res = res * i % num;

            if (res == 1)
                return true;

            return false;
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
        public static bool IsPrime_TrialDivision<B>(this LongInt<B> num)
            where B: IBase, new()
        {
            Contract.Requires<ArgumentNullException>(num != null, "num");
            Contract.Requires<ArgumentOutOfRangeException>(num > 1, "The tested number should be bigger than 1.");

            LongInt<B> sqrtNum = LongInt<B>.Helper.SquareRootInteger(num);

            for (LongInt<B> i = 2; i <= sqrtNum + 1; ++i)
                if (num % i == 0)
                    return false;

            return true;
        }

        /// <summary>
        /// Performs a Solovay-Strassen stochastic primality test of a long integer number
        /// and returns the probability that the number is composite. 
        /// </summary>
        /// <typeparam name="B">A class specifying the digit base of <c>LongInt&lt;<typeparamref name="B"/></c> type.</typeparam>
        /// <param name="num">A number, bigger than 1, to test for primality.</param>
        /// <param name="gen">
        /// A bounded <c>LongInt&lt;<typeparamref name="B"/></c> random generator. 
        /// For probability estimations to be correct, this generator should guarantee uniform distribution 
        /// for any given interval.
        /// </param>
        /// <param name="rounds">
        /// A positive number of testing rounds. Recommended to be more than <c>log2(<paramref name="num"/>)</c>.
        /// </param>
        /// <returns>
        /// The probability that the <paramref name="num"/> is composite. 
        /// Equals to <c>2^(-<paramref name="rounds"/>)</c>, which is
        /// worse than for <see cref="IsPrime_MillerRabin&lt;B&gt;"/>.
        /// </returns>
        public static double IsPrime_SolovayStrassen<B>(this LongInt<B> num, IRandomBounded<LongInt<B>> gen, long rounds)
            where B : IBase, new()
        {
            Contract.Requires<ArgumentNullException>(num != null, "num");
            Contract.Requires<ArgumentNullException>(gen != null, "gen");
            Contract.Requires<ArgumentOutOfRangeException>(num > 1, "The tested number should be bigger than 2.");
            Contract.Requires<ArgumentOutOfRangeException>(rounds > 0, "The number of rounds should be positive.");

            if(num.IsEven)
            {
                if (num == 2)
                    return 0;
                else
                    return 1;
            }
            else if (num == 3)
                return 0;
            
            LongInt<B> half = (num - 1) / 2;

            for (long i = 0; i < rounds; i++)
            {
                LongInt<B> rnd = gen.Next(2, num);

                int jacobiSymbol = WhiteMath<LongInt<B>, CalcLongInt<B>>.JacobiSymbol(rnd, num);

                // Символ Якоби равняется нулю, значит, числа не взаимно простые
                // значит, наше число составное.

                if (jacobiSymbol == 0)
                    return 1;

                // Иначе еще вот что посмотрим.

                LongInt<B> powered = LongInt<B>.Helper.PowerIntegerModular(rnd, half, num);

                if (jacobiSymbol == 1 && powered != 1 ||
                   jacobiSymbol == -1 && powered != num - 1)
                    return 1;
            }

            return Math.Pow(2, -rounds);
        }

        /// <summary>
        /// Performs a Fermat stochastic primality test of a long integer number.
        /// </summary>
        /// <typeparam name="B">A class specifying the digit base of <c>LongInt&lt;<typeparamref name="B"/></c> type.</typeparam>
        /// <param name="num">A number to test for primality. Should be more than 1.</param>
        /// <param name="gen">
        /// A bounded <c>LongInt&lt;<typeparamref name="B"/></c> random generator. 
        /// For probability estimations to be correct, this generator should guarantee uniform distribution 
        /// for any given interval.
        /// </param>
        /// <param name="rounds">A positive number of testing rounds.</param>
        /// <returns>If this method returns false, the number is guaranteed to be composite. Otherwise the number is probably prime, but it's not always the case.</returns>
        public static bool IsPrime_Fermat<B>(this LongInt<B> num, IRandomBounded<LongInt<B>> gen, long rounds)
            where B: IBase, new()
        {
            Contract.Requires<ArgumentNullException>(num != null, "num");
            Contract.Requires<ArgumentNullException>(gen != null, "gen");
            Contract.Requires<ArgumentOutOfRangeException>(num > 1, "The number to test should be bigger than 1.");
            Contract.Requires<ArgumentOutOfRangeException>(rounds > 0, "The number of rounds should be positive.");

            if (num.IsEven)
            {
                if (num == 2)
                    return true;
                else
                    return false;
            }
            else if (num == 3)
                return true;

            LongInt<B> numDecremented = num - 1;

            for (long i = 0; i < rounds; i++)
            {
                LongInt<B> test = gen.Next(2, num);

                if(LongInt<B>.Helper.PowerIntegerModular(test, numDecremented, num) != 1)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Performs a Miller-Rabin stochastic primality test of a long integer number
        /// and returns the probability that the number is composite. 
        /// </summary>
        /// <typeparam name="B">A class specifying the digit base of <c>LongInt&lt;<typeparamref name="B"/></c> type.</typeparam>
        /// <param name="num">The number to test for primality. Should be more than 1.</param>
        /// <param name="gen">
        /// A bounded <c>LongInt&lt;<typeparamref name="B"/></c> random generator. 
        /// For probability estimations to be correct, this generator should guarantee uniform distribution 
        /// for any given interval.
        /// </param>
        /// <param name="rounds">
        /// A positive number of testing rounds. Recommended to be more than <c>log2(<paramref name="num"/>)</c>.
        /// </param>
        /// <returns>The probability that the <paramref name="num"/> is composite.</returns>
        public static double IsPrime_MillerRabin<B>(this LongInt<B> num, IRandomBounded<LongInt<B>> gen, long rounds) 
            where B: IBase, new()
        {
            Contract.Requires<ArgumentNullException>(num != null, "num");
            Contract.Requires<ArgumentNullException>(gen != null, "gen");
            Contract.Requires<ArgumentOutOfRangeException>(num > 1, "The number to test should be bigger than 1.");
            Contract.Requires<ArgumentOutOfRangeException>(rounds > 0, "The number of rounds should be positive.");

            if (num.IsEven)
            {
                if (num == 2)
                    return 0;
                else
                    return 1;
            }
            else if (num == 3)
                return 0;

            // Представим число num - 1 в виде
            //
            //      num - 1 = 2^s * t

            LongInt<B> t;
            long s;

            ___millerRabinFactorize(num - 1, out t, out s);

            // ------------------------
            // А теперь - куча раундов.
            // ------------------------

            for (int i = 0; i < rounds; i++)
            {
                LongInt<B> x = gen.Next(2, num - 1);

                if (!___millerRabinIsPrimeWitness(x, num, t, s))
                    return 1;
            }

            return Math.Pow(4, -rounds);
        }

        /// <summary>
        /// Раскладывает четное число в произведение 2^s * t.
        /// </summary>
        private static void ___millerRabinFactorize<B>(LongInt<B> number, out LongInt<B> t, out long s)
            where B: IBase, new()
        {
            // Число должно быть четным.
            // -------------------------

            Contract.Requires(number.IsEven);

            s = 0;         // показатель степени вряд ли будет больше 9223372036854775807, можно long :-)

            while (number.IsEven)
            {
                number >>= 1;
                s++;
            }

            t = number;
        }

        /// <summary>
        /// Возвращает true, если число - свидетель простоты.
        /// Если false, то тестируемое число - составное.
        /// </summary>
        private static bool ___millerRabinIsPrimeWitness<B>(LongInt<B> x, LongInt<B> num, LongInt<B> t, long s)
            where B: IBase, new()
        {
            x = LongInt<B>.Helper.PowerIntegerModular(x, t, num);

            if (x == 1 || x == num - 1)
                return true;

            for (int j = 0; j < s - 1; j++)
            {
                x = x * x % num;

                if (x == 1)
                    return false;           // ни одна скобка далее не разделится (см. обоснование), так что составное.

                else if (x == num - 1)  // эта скобка разделилась, так что это свидетель простоты. продолжаем.
                    return true;
            }

            // Ни одна скобка 
            // не разделилась. Составное.

            return false;
        }
    }
}
