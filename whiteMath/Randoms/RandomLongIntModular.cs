using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

using whiteMath.ArithmeticLong;

namespace whiteMath.Randoms
{
    /// <summary>
    /// This class provides high-quality uniform random <c>LongInt&lt;<typeparamref name="B"/>&gt;</c>
    /// generation. 
    /// </summary>
    /// <typeparam name="B">The type specifying the digit base for the <c>LongInt&lt;B&gt;</c> type.</typeparam>
    [ContractVerification(true)]
    public class RandomLongIntModular<B> : IRandomBounded<LongInt<B>> where B : IBase, new()
    {
        private IRandomBounded<int>                         intGenerator;           // integer generator
        private Func<LongInt<B>, LongInt<B>, LongInt<B>>    multiplication;         // multiplication function

        // ----- caching area.

        private LongInt<B> lastMaxExclusive;
        private LongInt<B> lastBound;

        /// <summary>
        /// Gets the total amount of generated numbers that 
        /// were discarded during rejection sampling.
        /// </summary>
        public int TotalRejected { get; private set; }

        /// <summary>
        /// Resets the <c>TotalRejected</c>
        /// counter, setting its value to zero.
        /// </summary>
        /// <see cref="TotalRejected"/>
        public void ResetRejectionCounter()
        {
            this.TotalRejected = 0;
        }

        /// <summary>
        /// Initializes the <c>RandomLongIntModular&lt;<typeparamref name="B"/>&gt;</c> instance
        /// with an integer digit generator and a delegate used to multiply <c>LongInt&lt;<typeparamref name="B"/>&gt;</c> numbers.
        /// </summary>
        /// <param name="intGenerator">
        /// A uniform distribution integer generator which will be used 
        /// to produce <c>LongInt&lt;<typeparamref name="B"/>&gt;</c> digits.
        /// If <c>null</c>, a new <c>RandomStandard</c> instance will be used.
        /// </param>
        /// <see cref="RandomStandard"/>
        /// <param name="multiplication">
        /// A function taking two <c>LongInt&lt;<typeparamref name="B"/>&gt;</c> numbers 
        /// and returning their multiplication product.
        /// If <c>null</c>, the simple, O(n^2) multiplication method will be used.
        /// </param>
        /// <see cref="LongInt&lt;B&gt;.Helper.MultiplySimple"/>
        public RandomLongIntModular(IRandomBounded<int> intGenerator = null, Func<LongInt<B>, LongInt<B>, LongInt<B>> multiplication = null)
        {
            if (intGenerator == null)
                intGenerator = new RandomStandard();

            if (multiplication == null)
                multiplication = LongInt<B>.Helper.MultiplySimple;

            this.intGenerator = intGenerator;
            this.multiplication = multiplication;
        }

        /// <summary>
        /// Returns the next non-negative <c>LongInt&lt;<typeparamref name="B"/>&gt;</c>
        /// number which is less than <paramref name="maxExclusive"/>.
        /// </summary>
        /// <param name="maxExclusive">The upper exclusive bound of generated numbers.</param>
        /// <returns>
        /// A non-negative <c>LongInt&lt;<typeparamref name="B"/>&gt;</c> number which
        /// is less than <paramref name="maxExclusive"/>.
        /// </returns>
        public LongInt<B> Next(LongInt<B> maxExclusive)
        {
            Contract.Requires<ArgumentNullException>(maxExclusive != null, "maxExclusive");
            Contract.Requires<ArgumentOutOfRangeException>(maxExclusive > 0, "The maximum exclusive bound should be a positive number.");

            LongInt<B> basePowered = LongInt<B>.CreatePowerOfBase(maxExclusive.Length);
            LongInt<B> upperBound;

            if (this.lastMaxExclusive != maxExclusive)
            {
                // Мы будем генерировать ВСЕ цифры от 0 до BASE - 1.
                // НО:
                // Мы отбрасываем верхнюю границу, где приведение по модулю maxExclusive 
                // даст нам лишнюю неравномерность.
                // Мы можем использовать только интервал, содержащий целое число чисел maxExclusive.
                // Тогда спокойно приводим по модулю и наслаждаемся равномерностью.

                // Например, если мы генерируем цифирки по основанию 10, и хотим число от [0; 12),
                // то нам нужно отбрасывать начиная с floor(10^2 / 12) * 12 = 96. 
                
                upperBound = Max_BinarySearch((LongInt<B>)1, LongInt<B>.BASE, (res => LongInt<B>.Helper.MultiplyFFTComplex(res, maxExclusive) <= basePowered)) * maxExclusive;
                
                // upperBound = multiplication(basePowered / maxExclusive, maxExclusive);

                this.lastMaxExclusive = maxExclusive;
                this.lastBound = upperBound;
            }
            else
                upperBound = this.lastBound;

            LongInt<B> result = new LongInt<B>();

        REPEAT:

            result.Digits.Clear();

            for (int i = 0; i < maxExclusive.Length; i++)
                result.Digits.Add(intGenerator.Next(0, LongInt<B>.BASE));

            result.DealWithZeroes();

            if (result >= upperBound)
            {
                ++TotalRejected;
                goto REPEAT;
            }

            // Возвращаем остаток от деления.

            // return result % maxExclusive;

            LongInt<B> divisionResult = Max_BinarySearch((LongInt<B>)0, LongInt<B>.BASE, (res => LongInt<B>.Helper.MultiplyFFTComplex(res, maxExclusive) <= result));
            
            return result - LongInt<B>.Helper.MultiplyFFTComplex(divisionResult, maxExclusive);
        }

        /// <summary>
        /// Returns the next <c>LongInt&lt;<typeparamref name="B"/>&gt;</c>
        /// which lies in the interval <c>[<paramref name="minInclusive"/>; <paramref name="maxExclusive"/>)</c>.
        /// </summary>
        /// <param name="minInclusive">The lower inclusive bound of generated numbers.</param>
        /// <param name="maxExclusive">The upper exclusive bound of generated numbers.</param>
        /// <returns>
        /// A non-negative <c>LongInt&lt;<typeparamref name="B"/>&gt;</c> number 
        /// which lies in the interval <c>[<paramref name="minInclusive"/>; <paramref name="maxExclusive"/>)</c>.
        /// </returns>
        public LongInt<B> Next(LongInt<B> minInclusive, LongInt<B> maxExclusive)
        {
            Contract.Requires<ArgumentNullException>(minInclusive != null, "minInclusive");
            Contract.Requires<ArgumentNullException>(maxExclusive != null, "maxExclusive");
            Contract.Requires<ArgumentException>(minInclusive < maxExclusive, "The minimum inclusive bound should be less than the maximum exclusive.");

            return minInclusive + this.Next(maxExclusive - minInclusive);
        }

        // --------------------------------------------
        // -------------- helper methods --------------

        /// <summary>
        /// Finds the maximum number '<c>k</c>' within a given integer interval of special structure 
        /// such that a predicate holds for this number '<c>k</c>'.
        /// </summary>
        /// <typeparam name="B">The numeric base class for the <see cref="LongInt&lt;B&gt;"/> numbers.</typeparam>
        /// <param name="predicate">
        /// A predicate that holds for all numbers, starting with the leftmost bound and ending with the 
        /// desired (sought-for) number, and only for these.
        /// </param>
        /// <returns>The maximum number within a given interval for which the <paramref name="predicate"/> holds.</returns>
        private static LongInt<B> Max_BinarySearch<B>(LongInt<B> leftInclusive, LongInt<B> rightInclusive, Predicate<LongInt<B>> predicate)
            where B : IBase, new()
        {
            Contract.Requires<ArgumentNullException>(leftInclusive != null, "leftInclusive");
            Contract.Requires<ArgumentNullException>(rightInclusive != null, "rightInclusive");
            Contract.Requires<ArgumentNullException>(predicate != null, "predicate");
            Contract.Requires<ArgumentException>(!(leftInclusive > rightInclusive));

            LongInt<B> lb = leftInclusive;
            LongInt<B> rb = rightInclusive;

            while (!(lb > rb))
            {
                LongInt<B> mid = (lb + rb) / 2;

                if (predicate(mid))
                    lb = mid + 1;
                else
                    rb = mid - 1;
            }

            return lb - 1;
        }
    }
}
