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
    /// <remarks>
    /// The speed of number generation might not be very high as the class uses number 
    /// multiplication/division along with rejection sampling, though optimized.
    /// </remarks>
    /// <typeparam name="B">The type specifying the digit base for the <c>LongInt&lt;B&gt;</c> type.</typeparam>
    [ContractVerification(true)]
    [Obsolete("This implementation is slower than RandomLongIntModular.")]
    public class RandomLongIntModularUnoptimized<B> : IRandomBounded<LongInt<B>> where B : IBase, new()
    {
        private IRandomBounded<int>                         intGenerator;           // integer generator
        private Func<LongInt<B>, LongInt<B>, LongInt<B>>    multiplication;         // multiplication function

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
        public RandomLongIntModularUnoptimized(IRandomBounded<int> intGenerator = null, Func<LongInt<B>, LongInt<B>, LongInt<B>> multiplication = null)
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

            // Мы будем генерировать ВСЕ цифры от 0 до BASE - 1.
            // НО:
            // Мы отбрасываем верхнюю границу, где приведение по модулю maxExclusive 
            // даст нам лишнюю неравномерность.
            // Мы можем использовать только интервал, содержащий целое число чисел maxExclusive.
            // Тогда спокойно приводим по модулю и наслаждаемся равномерностью.

            // Например, если мы генерируем цифирки по основанию 10, и хотим число от [0; 12),
            // то нам нужно отбрасывать начиная с floor(10^2 / 12) * 12 = 96. 

            LongInt<B> upperBound = (LongInt<B>.CreatePowerOfBase(maxExclusive.Length) / maxExclusive) * maxExclusive;
            
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

            return result % maxExclusive;
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
    }
}
