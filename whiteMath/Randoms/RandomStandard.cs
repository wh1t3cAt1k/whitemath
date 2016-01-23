using System;
using System.Diagnostics.Contracts;

namespace whiteMath.Randoms
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
        private Random rnd;

        /// <summary>
        /// Creates a new instance of RandomStandard class using a clock-dependent
        /// unknown seed value. 
        /// </summary>
        public RandomStandard()
        {
            rnd = new Random();
            ___generator_delegate_init();
        }

        [ContractInvariantMethod]
        private void ___invariant()
        {
            Contract.Invariant(this.rnd != null);
        }

        /// <summary>
        /// Creates a new instance of RandomStandard class using an explicitly
        /// provided integer seed value.
        /// </summary>
        /// <param name="seed"></param>
        public RandomStandard(int seed)
        {
            rnd = new Random(seed);
            ___generator_delegate_init();
        }

        /// <summary>
        /// Generates a sequence of random bytes
        /// and stores them in the buffer.
        /// </summary>
        /// <param name="buffer">A byte array to store the random sequence.</param>
        public void NextBytes(byte[] buffer)
        {
            rnd.NextBytes(buffer);
        }

        /// <summary>
        /// Returns the next pseudo-random integer value
        /// in the [int.MinValue; int.MaxValue] interval.
        /// </summary>
        /// <returns>The next integer value in the [int.MinValue; int.MaxValue] interval.</returns>
        public int NextInt()
        {
            return rnd.Next(int.MinValue, 0) + rnd.Next(0, int.MaxValue);
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
            Contract.Requires<ArgumentException>(minValue < maxValue, "The lower inclusive bound should be less than the upper exclusive.");

            return 
                rnd.Next(minValue, maxValue);
        }

        // ---------------------------------------------------------------------
        // --------------------- functionality extended by IRandomExtensions----

        BoundedGenerator<long>          genLongBounded;
        UpperBoundedGenerator<long>     genLongUpperBounded;
        UnboundedGenerator<long>        genLongUnbounded;

        BoundedGenerator<ulong>         genULongBounded;
        UpperBoundedGenerator<ulong>    genULongUpperBounded;
        UnboundedGenerator<ulong>       genULongUnbounded;

        private void ___generator_delegate_init()
        {
            genLongBounded          = RandomFunctionalityExtensions.CreateNextLongBounded(rnd.NextBytes);
            genLongUpperBounded     = RandomFunctionalityExtensions.CreateNextLongUpperBounded(rnd.NextBytes);
            genLongUnbounded        = RandomFunctionalityExtensions.CreateNextLongUnbounded(rnd.NextBytes);

            genULongBounded         = RandomFunctionalityExtensions.CreateNextULongBounded(rnd.NextBytes); 
            genULongUpperBounded    = RandomFunctionalityExtensions.CreateNextULongUpperBounded(rnd.NextBytes);
            genULongUnbounded       = RandomFunctionalityExtensions.CreateNextULongUnbounded(rnd.NextBytes);
        }

        /// <summary>
        /// Returns the next <c>ulong</c> number.
        /// </summary>
        /// <returns>A random <c>ulong</c> number.</returns>
        public ulong NextULong()
        {
            return 
                genULongUnbounded();
        }

        /// <summary>
        /// Returns the next <c>ulong</c> number limited
        /// by an exclusive upper boundary.
        /// </summary>
        /// <param name="maxValue">The exclusive upper boundary of generated numbers.</param>
        /// <returns>A random <c>ulong</c> number which is smaller than <paramref name="maxValue"/>.</returns>
        public ulong NextULong(ulong maxValue)
        {
            Contract.Requires<ArgumentException>(maxValue > 0, "The upper exclusive boundary for generated values should not be equal to zero.");

            return 
                genULongUpperBounded(maxValue);
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
            Contract.Requires<ArgumentException>(maxValue > minValue, "The upper exclusive boundary should be bigger than the lower inclusive.");

            return 
                genULongBounded(minValue, maxValue);
        }

        /// <summary>
        /// Returns the next <c>long</c> value in the [long.MinValue; long.MaxValue]
        /// interval.
        /// </summary>
        /// <returns>The next number in the <c>[long.MinValue; long.MaxValue]</c> interval.</returns>
        public long NextLong()
        {
            return
                genLongUnbounded();
        }

        /// <summary>
        /// Returns the next non-negative <c>long</c> value that is less than
        /// <paramref name="maxValue"/>
        /// </summary>
        /// <param name="maxValue">The upper exclusive bound for generated number.</param>
        /// <returns>A non-negative <c>long</c> number that is less than <paramref name="maxValue"/></returns>
        public long NextLong(long maxValue)
        {
            Contract.Requires<ArgumentOutOfRangeException>(maxValue > 0, "The upper exclusive boundary should be a positive number.");

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
            Contract.Requires<ArgumentException>(maxValue > minValue, "The lower inclusive boundary should be less than the upper exclusive.");

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
            return rnd.NextDouble();
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
            Contract.Requires(min < max);

            return this.NextInt(min, max);
        }

        long IRandomUnbounded<long>.Next()
        {
            return this.NextLong();
        }

        long IRandomBounded<long>.Next(long minValue, long maxValue)
        {
            Contract.Requires(minValue < maxValue);
            
            return this.NextLong(minValue, maxValue);
        }

        ulong IRandomUnbounded<ulong>.Next()
        {
            return this.NextULong();
        }

        ulong IRandomBounded<ulong>.Next(ulong min, ulong max)
        {
            Contract.Requires(min < max);

            return this.NextULong(min, max);
        }

        double IRandomFloatingPoint<double>.Next_SingleInterval()
        {
            return this.NextDouble_SingleInterval();
        }
    }
}
