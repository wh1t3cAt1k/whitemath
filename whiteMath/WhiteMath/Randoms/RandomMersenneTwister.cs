using System;
using WhiteMath.General;

namespace WhiteMath.Randoms
{
    public class RandomMersenneTwister: 
        IRandomBounded<int>, IRandomUnbounded<int>,
        IRandomBounded<uint>, IRandomUnbounded<uint>,
        IRandomFloatingPoint<double>        
    {
        /* Period parameters */
        private const int N = 624;
        private const int M = 397;
        private const uint MATRIX_A = 0x9908b0df; /* constant vector a */
        private const uint UPPER_MASK = 0x80000000; /* most significant w-r bits */
        private const uint LOWER_MASK = 0x7fffffff; /* least significant r bits */

        /* Tempering parameters */
        private const uint TEMPERING_MASK_B = 0x9d2c5680;
        private const uint TEMPERING_MASK_C = 0xefc60000;

        private static uint TEMPERING_SHIFT_U(uint y) { return (y >> 11); }
        private static uint TEMPERING_SHIFT_S(uint y) { return (y << 7); }
        private static uint TEMPERING_SHIFT_T(uint y) { return (y << 15); }
        private static uint TEMPERING_SHIFT_L(uint y) { return (y >> 18); }

        private uint[] mt = new uint[N]; /* the array for the state vector  */

        private short mti;

        private static uint[] mag01 = { 0x0, MATRIX_A };

        /// <summary>
        /// Initializes the generator with a non-zero seed.
        /// The seed number itself should be as 'random' as possible - consider
        /// using system clock or another generator for that.
        /// </summary>
        /// <param name="seed">A non-zero seed value. If it is zero, a clock-dependent value will be used.</param>
        public RandomMersenneTwister(uint seed)
        {
            if (seed == 0)
            {
                // Test query performance counter. If it's not available, use less-precision value from the clock.
                long x = -1;
				
                // if (NativeMethods.QueryPerformanceCounter(ref x) <= 0)
				unchecked
				{
					seed = (uint)(DateTime.Now.Ticks);
				}
                // else
                //    seed = (uint)x;
            }

			unchecked 
			{
				/* setting initial seeds to mt[N] using         */
				/* the generator Line 25 of Table 1 in          */
				/* [KNUTH 1981, The Art of Computer Programming */
				/*    Vol. 2 (2nd Ed.), pp102]                  */
				mt[0] = seed & 0xffffffffU;
         
				for (mti = 1; mti < N; ++mti)
					mt[mti] = (69069 * mt[mti - 1]) & 0xffffffffU;
			}
        }

        /// <summary>
        /// Initializes the generator with a clock-dependent seed value.
        /// </summary>
        public RandomMersenneTwister(): this(0) 
        { }

        /// <summary>
        /// Generates the next unsigned integer.
        /// </summary>
        /// <returns>A random unsigned integer in [uint.MinValue; uint.MaxValue) interval.</returns>
        public uint NextUInt()
        {
            uint y;

            /* mag01[x] = x * MATRIX_A  for x=0,1 */
            if (mti >= N) /* generate N words at one time */
            {
                short kk = 0;

                for (; kk < N - M; ++kk)
                {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1];
                }

                for (; kk < N - 1; ++kk)
                {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1];
                }

                y = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
                mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1];

                mti = 0;
            }

            y = mt[mti++];
            y ^= TEMPERING_SHIFT_U(y);
            y ^= TEMPERING_SHIFT_S(y) & TEMPERING_MASK_B;
            y ^= TEMPERING_SHIFT_T(y) & TEMPERING_MASK_C;
            y ^= TEMPERING_SHIFT_L(y);

            return y;
        }

        public uint NextUInt(uint minValue, uint maxValue) 
        {
            (minValue < maxValue).Assert(new ArgumentOutOfRangeException("The minimum value is greater than or equal to the maximum value specified."));
            
            return (minValue + NextUInt() % maxValue) % maxValue;
        }

        public int NextInt()
        {
            return (int)this.NextUInt();
        }

        public int NextInt(int minValue, int maxValue)
        {
            (minValue < maxValue).Assert(GeneralExceptions.__MINVALUE_EXCEED_EQUAL_MAXVALUE);

            return minValue + (int)NextUInt(0, (uint)(maxValue - minValue));
        }

        public void NextBytes(byte[] buffer)
        {
            int bufLen = buffer.Length;

            if (buffer == null)
            {
                throw new ArgumentNullException();
            }

            for (int idx = 0; idx < bufLen; ++idx)
            {
                buffer[idx] = (byte)this.NextInt(0, 256);
            }
        }

        public double NextDouble()
        {
            return (double)this.NextUInt() / ((ulong)uint.MaxValue + 1);
        }

        // ------------------------------------
        // ---------- explicit interface ------
        // ---------- implementations ---------

        double IRandomFloatingPoint<double>.NextInUnitInterval()
        {
            return this.NextDouble();
        }

        // ------------- as integer number generator ----------------

        int IRandomUnbounded<int>.Next()
        {
            return this.NextInt();
        }

        int IRandomBounded<int>.Next(int minValue, int maxValue)
        {
            return this.NextInt(minValue, maxValue);
        }

        // ------------- as unsigned integer generator --------------

        uint IRandomUnbounded<uint>.Next()
        {
            return this.NextUInt();
        }

        uint IRandomBounded<uint>.Next(uint minValue, uint maxValue)
        {
            return this.NextUInt(minValue, maxValue);
        }
    }
}
