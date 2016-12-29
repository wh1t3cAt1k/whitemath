using System;

using whiteStructs.Conditions;

namespace WhiteMath.Randoms
{
    /// <summary>
    /// Represents a wrapper for a standard <c>Random</c> class from the C# library
    /// to provide additional functionality (e.g. generating <c>long</c> values) and to implement 
    /// <c>IRandomBounded&lt;T&gt;</c>, <c>IRandomUnbounded&lt;T&gt;</c> and <c>IRandomFloatingPoint&lt;T&gt;</c> interfaces 
    /// for compatibility with whiteMath library methods.
    /// </summary>
    public class RandomStandard: 
        IRandomBoundedUnbounded<int>, 
        IRandomBoundedUnbounded<long>, 
        IRandomBoundedUnbounded<ulong>,
        IRandomFloatingPoint<double>,
        IRandomBytes
    {
		private Random _libraryGenerator;

        /// <summary>
        /// Creates a new instance of RandomStandard class using a clock-dependent
        /// unknown seed value. 
        /// </summary>
        public RandomStandard()
        {
            _libraryGenerator = new Random();
            InitializeGeneratorDelegates();
        }

        /// <summary>
        /// Creates a new instance of RandomStandard class using an explicitly
        /// provided integer seed value.
        /// </summary>
        /// <param name="seed"></param>
        public RandomStandard(int seed)
        {
            _libraryGenerator = new Random(seed);
            InitializeGeneratorDelegates();
        }

        /// <summary>
        /// Generates a sequence of random bytes
        /// and stores them in the buffer.
        /// </summary>
        /// <param name="buffer">A byte array to store the random sequence.</param>
        public void NextBytes(byte[] buffer)
        {
            _libraryGenerator.NextBytes(buffer);
        }

        /// <summary>
        /// Returns the next pseudo-random integer value
        /// in the [int.MinValue; int.MaxValue] interval.
        /// </summary>
        /// <returns>The next integer value in the [int.MinValue; int.MaxValue] interval.</returns>
        public int NextInt()
        {
            return _libraryGenerator.Next(int.MinValue, 0) + _libraryGenerator.Next(0, int.MaxValue);
        }

        /// <summary>
        /// Returns the next pseudo-random integer value
        /// in the [minValue; maxValue) interval.
        /// </summary>
        /// <param name="minValue">The lower inclusive bound of the number to be generated.</param>
        /// <param name="maxValue">The upper exclusive bound of the number to be generated.</param>
        /// <returns>The next integer value in the [min; max) interval.</returns>
        public int NextInt(int minValue, int maxValue)
        {
			Condition.Validate(minValue < maxValue)
				.OrArgumentException("The lower inclusive bound should be less than the upper exclusive.");

            return _libraryGenerator.Next(minValue, maxValue);
        }

        // ---------------------------------------------------------------------
        // --------------------- functionality extended by IRandomExtensions----

        BoundedGenerator<long> genLongBounded;
        UpperBoundedGenerator<long> genLongUpperBounded;
        UnboundedGenerator<long> genLongUnbounded;

        BoundedGenerator<ulong> genULongBounded;
        UpperBoundedGenerator<ulong> genULongUpperBounded;
        UnboundedGenerator<ulong> genULongUnbounded;

		private void InitializeGeneratorDelegates()
        {
            genLongBounded = RandomFunctionalityExtensions.CreateNextLongBounded(_libraryGenerator.NextBytes);
            genLongUpperBounded = RandomFunctionalityExtensions.CreateNextLongUpperBounded(_libraryGenerator.NextBytes);
            genLongUnbounded = RandomFunctionalityExtensions.CreateNextLongUnbounded(_libraryGenerator.NextBytes);

            genULongBounded = RandomFunctionalityExtensions.CreateNextULongBounded(_libraryGenerator.NextBytes); 
            genULongUpperBounded = RandomFunctionalityExtensions.CreateNextULongUpperBounded(_libraryGenerator.NextBytes);
            genULongUnbounded = RandomFunctionalityExtensions.CreateNextULongUnbounded(_libraryGenerator.NextBytes);
        }

        /// <summary>
        /// Returns the next <c>ulong</c> number.
        /// </summary>
        /// <returns>A random <c>ulong</c> number.</returns>
        public ulong NextULong() => genULongUnbounded();

        /// <summary>
        /// Returns the next <c>ulong</c> number limited
        /// by an exclusive upper boundary.
        /// </summary>
        /// <param name="maxValue">The exclusive upper boundary of generated numbers.</param>
        /// <returns>A random <c>ulong</c> number which is smaller than <paramref name="maxValue"/>.</returns>
        public ulong NextULong(ulong maxValue)
        {
			Condition.Validate(maxValue > 0)
				.OrArgumentOutOfRangeException("The upper exclusive boundary for generated values should not be equal to zero.");

            return genULongUpperBounded(maxValue);
        }

        /// <summary>
        /// Returns the next <c>ulong</c> number in the <c>[<paramref name="minValue"/>; <paramref name="maxValue"/>)</c>
        /// interval.
        /// </summary>
        /// <param name="minValue">The lower inclusive boundary of generated values.</param>
        /// <param name="maxValue">The upper exclusive boundary of generated values.</param>
        /// <returns>A random <c>ulong</c> number which is bigger than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>.</returns>
        public ulong NextULong(ulong minValue, ulong maxValue)
        {
			Condition.Validate(maxValue > minValue)
				.OrArgumentOutOfRangeException("The upper exclusive boundary should be bigger than the lower inclusive.");

            return genULongBounded(minValue, maxValue);
        }

		/// <summary>
		/// Returns the next <c>long</c> value in the [long.MinValue; long.MaxValue]
		/// interval.
		/// </summary>
		/// <returns>The next number in the <c>[long.MinValue; long.MaxValue]</c> interval.</returns>
		public long NextLong() => genLongUnbounded();

        /// <summary>
        /// Returns the next non-negative <c>long</c> value that is less than
        /// <paramref name="maxValue"/>
        /// </summary>
        /// <param name="maxValue">The upper exclusive bound for generated number.</param>
        /// <returns>A non-negative <c>long</c> number that is less than <paramref name="maxValue"/></returns>
        public long NextLong(long maxValue)
        {
			Condition.Validate(maxValue > 0)
 				.OrArgumentOutOfRangeException("The upper exclusive boundary should be a positive number.");

            return
                genLongUpperBounded(maxValue);
        }

        /// <summary>
        /// Returns the next <c>long</c> value in the <c>[minValue; maxValue)</c>
        /// interval.
        /// </summary>
        /// <param name="minValue">The lower inclusive bound for generated number.</param>
        /// <param name="maxValue">The upper exclusive bound for generated number.</param>
        /// <returns>The next <c>long</c> value in the <c>[minValue; maxValue)</c> interval.</returns>
        public long NextLong(long minValue, long maxValue)
        {
			Condition.Validate(maxValue > minValue)
 				.OrArgumentOutOfRangeException("The lower inclusive boundary should be less than the upper exclusive.");

            return
                genLongBounded(minValue, maxValue);
        }

        // ---------------------------------------------------------------------
        // --------------------- end functionality extended --------------------
        // ---------------------------------------------------------------------

        /// <summary>
        /// Returns the next pseudo-random double value
        /// in the [0; 1) interval.
        /// </summary>
        /// <returns>The next double value in the [0; 1) interval.</returns>
        public double NextDouble_SingleInterval()
        {
            return _libraryGenerator.NextDouble();
        }

        /// <summary>
        /// Returns the next pseudo-random double value
        /// in the (-double.MaxValue; double.MaxValue) interval.
        /// </summary>
        /// <returns>The next double value in the (-double.MaxValue; double.MaxValue) interval.</returns>
        public double NextDouble()
        {
            int negative = NextInt() < 0 ? 1 : 0;

            return negative * NextDouble(0, double.MaxValue);
        }

        /// <summary>
        /// Return the next pseudo-random double value
        /// in the [min; max) interval.
        /// </summary>
        /// <param name="min">The lower inclusive bound of the number to be generated.</param>
        /// <param name="max">The upper exclusive bound of the number to be generated.</param>
        /// <returns>The next pseudo-random double value in the [min; max) interval.</returns>
        public double NextDouble(double min, double max)
        {
            double length = max - min;

            return min + NextDouble_SingleInterval() * length;
        }

        // ---------------------------------------------------
        // ------- explicit interface implementations --------
        // ---------------------------------------------------

        int IRandomUnbounded<int>.Next()
        {
            return this.NextInt();
        }

        int IRandomBounded<int>.Next(int min, int max)
        {
			Condition.Validate(min < max).OrArgumentOutOfRangeException();

            return this.NextInt(min, max);
        }

        long IRandomUnbounded<long>.Next()
        {
            return this.NextLong();
        }

        long IRandomBounded<long>.Next(long minValue, long maxValue)
        {
			Condition.Validate(minValue < maxValue).OrArgumentOutOfRangeException();
            
            return this.NextLong(minValue, maxValue);
        }

        ulong IRandomUnbounded<ulong>.Next()
        {
            return this.NextULong();
        }

        ulong IRandomBounded<ulong>.Next(ulong min, ulong max)
        {
			Condition.Validate(min < max).OrArgumentOutOfRangeException();

            return this.NextULong(min, max);
        }

        double IRandomFloatingPoint<double>.NextInUnitInterval()
        {
            return this.NextDouble_SingleInterval();
        }
    }
}
