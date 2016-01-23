using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using whiteMath.ArithmeticLong;

namespace whiteMath.Randoms
{
    /// <summary>
    /// <para>
    /// This class provides high-quality uniform random <c>LongInt&lt;<typeparamref name="B"/>&gt;</c>
    /// number generation, but is incredibly slow.
    /// </para>
    /// <para>
    /// In practice, always use <c>RandomLongIntModular&lt;<typeparamref name="B"/>&gt;</c>
    /// to achieve the same distribution quality with lower performance overhead.
    /// </para>
    /// </summary>
    /// <remarks>
    /// The speed of number generation is usually very slow, because the class uses rejection sampling
    /// even without modular reduction. Please use <c>RandomLongIntModular&lt;<typeparamref name="B"/>&gt;</c>
    /// instead.
    /// </remarks>
    /// <see cref="RandomLongIntModularUnoptimized&lt;B&gt;"/>
    /// <typeparam name="B">The type specifying the digit base for the <c>LongInt&lt;B&gt;</c> type.</typeparam>
    [Obsolete("This class is for illustrative purposes only. Please use RandomLongIntModular<B> instead.")]
    public class RandomLongIntRejection<B> : IRandomBounded<LongInt<B>> where B : IBase, new()
    {
        private IRandomBounded<int> intGenerator;           // integer generator

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
        /// Initializes the <c>RandomLongIntRejection&lt;<typeparamref name="B"/>&gt;</c> instance
        /// with an integer digit generator.
        /// </summary>
        /// <param name="intGenerator">
        /// A uniform distribution integer generator which will be used 
        /// to produce <c>LongInt&lt;<typeparamref name="B"/>&gt;</c> digits.
        /// If <c>null</c>, a new <c>RandomStandard</c> instance will be used.
        /// </param>
        /// <see cref="RandomStandard"/>
        public RandomLongIntRejection(IRandomBounded<int> intGenerator = null)
        {
            if (intGenerator == null)
                intGenerator = new RandomStandard();

            this.intGenerator = intGenerator;
        }

        /// <summary>
        /// Returns the next non-negative <c>LongInt&lt;<typeparamref name="B"/>&gt;</c>
        /// number which is not bigger than <paramref name="maxInclusive"/>.
        /// </summary>
        /// <param name="maxInclusive">The upper inclusive bound of generated numbers.</param>
        /// <returns>
        /// A non-negative <c>LongInt&lt;<typeparamref name="B"/>&gt;</c> number which
        /// is not bigger than <paramref name="maxInclusive"/>.
        /// </returns>
        public LongInt<B> NextInclusive(LongInt<B> maxInclusive)
        {
            Contract.Requires<ArgumentNullException>(maxInclusive != null, "maxInclusive");
            Contract.Requires<ArgumentOutOfRangeException>(!maxInclusive.Negative, "The maximum inclusive number should not be negative.");

            // Будем генерировать числа длины такой же, как maxInclusive.
            // Все цифры - от 0 до BASE - 1

            LongInt<B> result = new LongInt<B>();

            result.Digits.Clear();
            result.Digits.AddRange(new int[maxInclusive.Length]);

            bool flag = true;  // есть ли ограничение по цифрам

            REPEAT:

            for (int i = maxInclusive.Length - 1; i >= 0; i--)
            {
                result.Digits[i] = intGenerator.Next(0, LongInt<B>.BASE);

                if (flag)
                {
                    if (result[i] > maxInclusive[i])
                    {
                        ++TotalRejected;
                        goto REPEAT;
                    }

                    else if (result[i] < maxInclusive[i])
                        flag = false;
                }
            }
            
            result.DealWithZeroes();

            return result;
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

            return NextInclusive(maxExclusive - 1);
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

            return minInclusive + this.NextInclusive(maxExclusive - minInclusive - 1);
        }
    }
}
