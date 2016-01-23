using System;

using whiteMath.ArithmeticLong;

using whiteStructs.Conditions; 

namespace whiteMath.Randoms
{
    /// <summary>
    /// This class provides quick pseudo-random number generation
    /// for <c>LongInt&lt;B&gt;</c> type, but sometimes seriously violates
    /// uniformity. 
    /// </summary>
    /// <typeparam name="B">The type specifying the digit base for the <c>LongInt&lt;B&gt;</c> type.</typeparam>
    public class RandomLongIntQuick<B>: IRandomBounded<LongInt<B>> where B: IBase, new()
    {
        private IRandomBounded<int> intGenerator;

        /// <summary>
        /// Initializes the <c>RandomLongIntQuick&lt;<typeparamref name="B"/>&gt;</c> instance
        /// with a bounded <c>int</c> random number generator.
        /// </summary>
        /// <param name="intGenerator">
        /// A uniform integer generator which will be used to produce 
        /// <c>LongInt&lt;<typeparamref name="B"/>&gt;</c> digits. 
        /// If <c>null</c>, a new instance of <c>RandomStandard</c> will be used.
        /// </param>
        /// <see cref="RandomStandard"/>
        public RandomLongIntQuick(IRandomBounded<int> intGenerator = null)
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
			Condition.ValidateNotNull(maxInclusive, nameof(maxInclusive));
			Condition
				.Validate(!maxInclusive.Negative)
				.OrArgumentOutOfRangeException("The maximum inclusive bound should not be negative.");

            LongInt<B> result = new LongInt<B>();

            result.Digits.Clear();
            result.Digits.AddRange(new int[maxInclusive.Length]);

            // Флаг, сигнализирующий о том, что мы пока генерируем
            // только цифры нашей верхней границы.
            // Как только сгенерировали что-то меньшее, все остальное
            // можно генерировать в пределах от 0 до BASE-1.

            bool flag = true;

            for (int i = result.Length - 1; i >= 0; i--)
            {
                if (flag)
                {
                    result[i] = intGenerator.Next(0, maxInclusive[i] + 1);

                    if (result[i] < maxInclusive[i])
                        flag = false;
                }
                else
                    result[i] = intGenerator.Next(0, LongInt<B>.BASE);
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
			Condition.ValidateNotNull(maxExclusive, nameof(maxExclusive));
			Condition
				.Validate(maxExclusive > 0)
				.OrArgumentOutOfRangeException("The upper exclusive bound should be strictly positive.");

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
			Condition.ValidateNotNull(minInclusive, nameof(minInclusive));
			Condition.ValidateNotNull(maxExclusive, nameof(maxExclusive));
			Condition
				.Validate(minInclusive < maxExclusive) 
				.OrArgumentException("The minimum inclusive bound should be less than the upper exclusive one.");

            return minInclusive + NextInclusive(maxExclusive - minInclusive - 1);
        }
    }
}
