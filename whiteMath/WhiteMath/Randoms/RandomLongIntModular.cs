using System;

using WhiteMath.ArithmeticLong;

using WhiteStructs.Conditions;

namespace WhiteMath.Randoms
{
    /// <summary>
    /// This class provides high-quality uniform random <c>LongInt&lt;<typeparamref name="B"/>&gt;</c>
    /// generation. 
    /// </summary>
    /// <typeparam name="B">The type specifying the digit base for the <c>LongInt&lt;B&gt;</c> type.</typeparam>
    public class RandomLongIntModular<B> : IRandomBounded<LongInt<B>> where B : IBase, new()
    {
		private IRandomBounded<int> _digitGenerator;
		private Func<LongInt<B>, LongInt<B>, LongInt<B>> _multiply;

		// Used for caching.
		// -
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
        /// <param name="digitGenerator">
        /// A uniform distribution integer generator which will be used 
        /// to produce <c>LongInt&lt;<typeparamref name="B"/>&gt;</c> digits.
        /// If <c>null</c>, a new <c>RandomStandard</c> instance will be used.
        /// </param>
        /// <see cref="RandomStandard"/>
        /// <param name="multiply">
		/// A function taking two <c>LongInt{B}</c> numbers and returning their product.
		/// If <c>null</c>, the simple, <c>O(n^2)</c> method will be used.
        /// </param>
		/// <seealso cref="LongInt{B}.Helper.MultiplySimple"/>
		public RandomLongIntModular(IRandomBounded<int> digitGenerator = null, Func<LongInt<B>, LongInt<B>, LongInt<B>> multiply = null)
        {
			_digitGenerator = digitGenerator ?? new RandomStandard();
			_multiply = multiply ?? LongInt<B>.Helper.MultiplySimple;
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
				.OrArgumentOutOfRangeException("The maximum exclusive bound should be a positive number.");

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
                
				upperBound = _multiply(
					maxExclusive,
					BinarySearchMax(
						(LongInt<B>)1, 
						LongInt<B>.BASE, 
						(x => _multiply(x, maxExclusive) <= basePowered)));
                
                this.lastMaxExclusive = maxExclusive;
                this.lastBound = upperBound;
            }
            else
                upperBound = this.lastBound;

            LongInt<B> result = new LongInt<B>();

			while (true)
			{
				result.Digits.Clear();

				for (int digitIndex = 0; digitIndex < maxExclusive.Length; ++digitIndex)
				{
					result.Digits.Add(_digitGenerator.Next(0, LongInt<B>.BASE));
				}

				result.DealWithZeroes();

				if (result >= upperBound)
				{
					++TotalRejected;
				}
				else
				{
					break;
				}
			}

            // Возвращаем остаток от деления.

            // return result % maxExclusive;

            LongInt<B> divisionResult = BinarySearchMax(
				(LongInt<B>)0, 
				LongInt<B>.BASE, 
				(x => _multiply(x, maxExclusive) <= result));
            
			return result - _multiply(divisionResult, maxExclusive);
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
				.OrArgumentException("The minimum inclusive bound should be less than the maximum exclusive.");

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
		private static LongInt<B> BinarySearchMax<B>(
			LongInt<B> leftInclusive, 
			LongInt<B> rightInclusive, 
			Predicate<LongInt<B>> predicate)
            where B : IBase, new()
        {
			Condition.ValidateNotNull(leftInclusive, nameof(leftInclusive));
			Condition.ValidateNotNull(rightInclusive, nameof(rightInclusive));
			Condition.ValidateNotNull(predicate, nameof(predicate));
			Condition
				.Validate(leftInclusive <= rightInclusive)
				.OrArgumentException();

			LongInt<B> leftInclusiveBoundary = leftInclusive;
			LongInt<B> rightInclusiveBoundary = rightInclusive;

            while (!(leftInclusiveBoundary > rightInclusiveBoundary))
            {
				LongInt<B> midpoint = (leftInclusiveBoundary + rightInclusiveBoundary) / 2;

                if (predicate(midpoint))
				{
                    leftInclusiveBoundary = midpoint + 1;
				}
                else
				{
                    rightInclusiveBoundary = midpoint - 1;
				}
            }

            return leftInclusiveBoundary - 1;
        }
    }
}
