using System.Diagnostics;

using WhiteStructs.Conditions;

namespace WhiteMath.Randoms
{
	/// <remarks>
	/// This generator is not thread-safe.
	/// </remarks>
    public class RandomMersenneTwister: 
        IRandomBoundedUnbounded<int>, 
        IRandomBoundedUnbounded<uint>, 
        IRandomUnitInterval<double>        
    {
		// Period parameters.
		// -
        private const int N = 624;
        private const int M = 397;
        private const uint MATRIX_A = 0x9908b0df; /* constant vector a */
        private const uint UPPER_MASK = 0x80000000; /* most significant w-r bits */
        private const uint LOWER_MASK = 0x7fffffff; /* least significant r bits */

        // Tempering parameters.
		// -
        private const uint TEMPERING_MASK_B = 0x9d2c5680;
        private const uint TEMPERING_MASK_C = 0xefc60000;

		private static readonly uint[] MAGIC_TABLE = { 0x0, MATRIX_A };

        private static uint TEMPERING_SHIFT_U(uint y) => (y >> 11);
        private static uint TEMPERING_SHIFT_S(uint y) => (y << 7);
        private static uint TEMPERING_SHIFT_T(uint y) => (y << 15);
        private static uint TEMPERING_SHIFT_L(uint y) => (y >> 18);

		private uint[] _stateVector = new uint[N];

		private short _stateVectorCurrentIndex;

        /// <summary>
        /// Initializes the generator with an optional seed (if <c>null</c>,
		/// a system clock dependent value will be used).
        /// </summary>
        /// <param name="seed">
		/// An optional seed value. If it is <c>null</c>, 
		/// a system clock dependent value will be used.
		/// </param>
        public RandomMersenneTwister(uint? seed = null)
        {
            if (seed == null)
            {				
				unchecked
				{
					seed = (uint)(Stopwatch.GetTimestamp());
				}
            }

			unchecked 
			{
				/* setting initial seeds to mt[N] using the generator 
				 * Line 25 of Table 1 in [KNUTH 1981, The Art of 
				 * Computer Programming vol. 2 (2nd ed.), pp102]
				 */
				_stateVector[0] = seed.Value & 0xffffffffU;

				for (_stateVectorCurrentIndex = 1; _stateVectorCurrentIndex < N; ++_stateVectorCurrentIndex)
				{
					_stateVector[_stateVectorCurrentIndex] = 
						(69069 * _stateVector[_stateVectorCurrentIndex - 1]) & 0xffffffffU;
				}
			}
        }

		private void RefreshStateVector()
		{
			uint temp;
			short stateVectorIndex = 0;

			for (; stateVectorIndex < N - M; ++stateVectorIndex)
			{
				temp = 
					(_stateVector[stateVectorIndex] & UPPER_MASK) 
					| (_stateVector[stateVectorIndex + 1] & LOWER_MASK);
				
				_stateVector[stateVectorIndex] = 
					_stateVector[stateVectorIndex + M] ^ (temp >> 1) ^ MAGIC_TABLE[temp & 0x1];
			}

			for (; stateVectorIndex < N - 1; ++stateVectorIndex)
			{
				temp = 
					(_stateVector[stateVectorIndex] & UPPER_MASK) 
					| (_stateVector[stateVectorIndex + 1] & LOWER_MASK);
				
				_stateVector[stateVectorIndex] = 
					_stateVector[stateVectorIndex + (M - N)] ^ (temp >> 1) ^ MAGIC_TABLE[temp & 0x1];
			}

			temp = (_stateVector[N - 1] & UPPER_MASK) | (_stateVector[0] & LOWER_MASK);

			_stateVector[N - 1] = _stateVector[M - 1] ^ (temp >> 1) ^ MAGIC_TABLE[temp & 0x1];
			_stateVectorCurrentIndex = 0;
		}

        /// <summary>
        /// Generates the next unsigned integer.
        /// </summary>
        /// <returns>A random unsigned integer in [uint.MinValue; uint.MaxValue) interval.</returns>
        public uint NextUInt()
        {
            // mag01[x] = x * MATRIX_A  for x = 0,1
			// -
			// We generate N words at a time.
			// -
            if (_stateVectorCurrentIndex >= N) 
            {
				RefreshStateVector();
            }

            uint result = _stateVector[_stateVectorCurrentIndex++];

            result ^= TEMPERING_SHIFT_U(result);
            result ^= TEMPERING_SHIFT_S(result) & TEMPERING_MASK_B;
            result ^= TEMPERING_SHIFT_T(result) & TEMPERING_MASK_C;
            result ^= TEMPERING_SHIFT_L(result);

            return result;
        }

		public uint NextUInt(uint minInclusiveValue, uint maxExclusiveValue) 
        {
			Condition
				.Validate(minInclusiveValue < maxExclusiveValue)
				.OrArgumentException("The minimum inclusive value should be smaller than the maximum exclusive value.");
            
            return (minInclusiveValue + NextUInt() % maxExclusiveValue) % maxExclusiveValue;
        }

        public int NextInt()
        {
            return (int)this.NextUInt();
        }

		public int NextInt(int minInclusiveValue, int maxExclusiveValue)
        {
			Condition
				.Validate(minInclusiveValue < maxExclusiveValue)
				.OrArgumentException("The minimum inclusive value should be smaller than the maximum exclusive value.");
			
            return minInclusiveValue + (int)NextUInt(0, (uint)(maxExclusiveValue - minInclusiveValue));
        }

        public double NextDouble()
        {
            return (double)this.NextUInt() / ((ulong)uint.MaxValue + 1);
        }

		#region Explicit Interface Implementations

        double IRandomUnitInterval<double>.NextInUnitInterval()
        {
            return this.NextDouble();
        }

        int IRandomUnbounded<int>.Next()
        {
            return this.NextInt();
        }

        int IRandomBounded<int>.Next(int minValue, int maxValue)
        {
            return this.NextInt(minValue, maxValue);
        }

        uint IRandomUnbounded<uint>.Next()
        {
            return this.NextUInt();
        }

        uint IRandomBounded<uint>.Next(uint minValue, uint maxValue)
        {
            return this.NextUInt(minValue, maxValue);
        }

		#endregion
    }
}
