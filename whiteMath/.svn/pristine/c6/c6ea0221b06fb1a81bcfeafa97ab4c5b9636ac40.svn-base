using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;
using System.Diagnostics.Contracts;

namespace whiteMath.Randoms
{
    /// <summary>
    /// Represents a random number generator wrapper built
    /// on a cryptographically strong <see cref="RandomNumberGenerator"/> instance.
    /// </summary>
    [ContractVerification(true)]
    // 
    public class RandomCryptographic:
        IRandomBoundedUnbounded<int>,
        IRandomBoundedUnbounded<uint>,
        IRandomBoundedUnbounded<long>,
        IRandomBoundedUnbounded<ulong>
    {
        private RandomNumberGenerator gen;

        private UpperBoundedGenerator<int>      ___nextIntUB;
        private UpperBoundedGenerator<uint>     ___nextUIntUB;
        private UpperBoundedGenerator<long>     ___nextLongUB;
        private UpperBoundedGenerator<ulong>    ___nextULongUB;

        private UnboundedGenerator<int>         ___nextIntNB;
        private UnboundedGenerator<uint>        ___nextUIntNB;
        private UnboundedGenerator<long>        ___nextLongNB;
        private UnboundedGenerator<ulong>       ___nextULongNB;

        private BoundedGenerator<int>           ___nextIntB;
        private BoundedGenerator<uint>          ___nextUIntB;
        private BoundedGenerator<long>          ___nextLongB;
        private BoundedGenerator<ulong>         ___nextULongB;
        
        /// <summary>
        /// Initializes an instance of <see cref="RandomCryptographic"/>
        /// with a single instance of cryptographically strong <see cref="RandomNumberGenerator"/>.
        /// </summary>
        /// <param name="gen">
        /// An instance of cryptographically strong <see cref="RandomNumberGenerator"/>.
        /// If <c>null</c>, an instance built from <see cref="RNGCryptoServiceProvider.Create()"/> will be used.
        /// </param>
        public RandomCryptographic(RandomNumberGenerator gen = null)
        {
            if (gen == null)
                gen = RNGCryptoServiceProvider.Create();

            this.gen = gen;

            // initialize methods

            this.___nextIntB = RandomFunctionalityExtensions.CreateNextIntBounded(this.gen.GetBytes);
            this.___nextIntNB = RandomFunctionalityExtensions.CreateNextIntUnbounded(this.gen.GetBytes);
            this.___nextIntUB = RandomFunctionalityExtensions.CreateNextIntUpperBounded(this.gen.GetBytes);

            this.___nextLongB = RandomFunctionalityExtensions.CreateNextLongBounded(this.gen.GetBytes);
            this.___nextLongNB = RandomFunctionalityExtensions.CreateNextLongUnbounded(this.gen.GetBytes);
            this.___nextLongUB = RandomFunctionalityExtensions.CreateNextLongUpperBounded(this.gen.GetBytes);

            this.___nextUIntB = RandomFunctionalityExtensions.CreateNextUIntBounded(this.gen.GetBytes);
            this.___nextUIntNB = RandomFunctionalityExtensions.CreateNextUIntUnbounded(this.gen.GetBytes);
            this.___nextUIntUB = RandomFunctionalityExtensions.CreateNextUIntUpperBounded(this.gen.GetBytes);

            this.___nextULongB = RandomFunctionalityExtensions.CreateNextULongBounded(this.gen.GetBytes);
            this.___nextULongNB = RandomFunctionalityExtensions.CreateNextULongUnbounded(this.gen.GetBytes);
            this.___nextULongUB = RandomFunctionalityExtensions.CreateNextULongUpperBounded(this.gen.GetBytes);
        }

        // ---------------------------------------------------------
        // ------------ FUNCTIONALITY ------------------------------

        /// <summary>
        /// Returns the next <c>uint</c> number.
        /// </summary>
        /// <returns>A random <c>uint</c> number.</returns>
        public uint NextUInt()
        {
            return
                ___nextUIntNB();
        }

        /// <summary>
        /// Returns the next <c>uint</c> number limited
        /// by an exclusive upper boundary.
        /// </summary>
        /// <param name="maxValue">The exclusive upper boundary of generated numbers.</param>
        /// <returns>A random <c>uint</c> number which is smaller than <paramref name="maxValue"/>.</returns>
        public uint NextUInt(uint maxValue)
        {
            Contract.Requires<ArgumentException>(maxValue > 0, "The upper exclusive boundary for generated values should not be equal to zero.");

            return
                ___nextUIntUB(maxValue);
        }

        /// <summary>
        /// Returns the next <c>uint</c> number in the <c>[<paramref name="minValue"/>; <paramref name="maxValue"/>)</c>
        /// interval.
        /// </summary>
        /// <param name="minValue">The lower inclusive boundary of generated values.</param>
        /// <param name="maxValue">The upper exclusive boundary of generated values.</param>
        /// <returns>A random <c>uint</c> number which is bigger than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>.</returns>
        public uint NextUInt(uint minValue, uint maxValue)
        {
            Contract.Requires<ArgumentException>(maxValue > minValue, "The upper exclusive boundary should be bigger than the lower inclusive.");

            return
                ___nextUIntB(minValue, maxValue);
        }



        /// <summary>
        /// Returns the next <c>ulong</c> number.
        /// </summary>
        /// <returns>A random <c>ulong</c> number.</returns>
        public ulong NextULong()
        {
            return
                ___nextULongNB();
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
                ___nextULongUB(maxValue);
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
                ___nextULongB(minValue, maxValue);
        }

        /// <summary>
        /// Returns the next <c>int</c> value in the [int.MinValue; int.MaxValue]
        /// interval.
        /// </summary>
        /// <returns>The next number in the <c>[int.MinValue; int.MaxValue]</c> interval.</returns>
        public int NextInt()
        {
            return
                ___nextIntNB();
        }

        /// <summary>
        /// Returns the next non-negative <c>int</c> value that is less than
        /// <paramref name="maxValue"/>
        /// </summary>
        /// <param name="maxValue">The upper exclusive bound for generated number.</param>
        /// <returns>A non-negative <c>int</c> number that is less than <paramref name="maxValue"/></returns>
        public int NextInt(int maxValue)
        {
            Contract.Requires<ArgumentOutOfRangeException>(maxValue > 0, "The upper exclusive boundary should be a positive number.");

            return
                ___nextIntUB(maxValue);
        }

        /// <summary>
        /// Returns the next <c>int</c> value in the <c>[minValue; maxValue)</c>
        /// interval.
        /// </summary>
        /// <param name="minValue">The lower inclusive bound for generated number.</param>
        /// <param name="maxValue">The upper exclusive bound for generated number.</param>
        /// <returns>The next <c>int</c> value in the <c>[minValue; maxValue)</c> interval.</returns>
        public int NextInt(int minValue, int maxValue)
        {
            Contract.Requires<ArgumentException>(maxValue > minValue, "The lower inclusive boundary should be less than the upper exclusive.");

            return
                ___nextIntB(minValue, maxValue);
        }


        /// <summary>
        /// Returns the next <c>long</c> value in the [long.MinValue; long.MaxValue]
        /// interval.
        /// </summary>
        /// <returns>The next number in the <c>[long.MinValue; long.MaxValue]</c> interval.</returns>
        public long NextLong()
        {
            return
                ___nextLongNB();
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
                ___nextLongUB(maxValue);
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
                ___nextLongB(minValue, maxValue);
        }

        // -----------------------------------------------------------
        // ------------ explicit interface implementation ------------

        int IRandomUnbounded<int>.Next()                         { return ___nextIntNB(); }
        int IRandomBounded<int>.Next(int min, int max)         { return ___nextIntB(min, max); }

        uint IRandomUnbounded<uint>.Next()                       { return ___nextUIntNB(); }
        uint IRandomBounded<uint>.Next(uint min, uint max)     { return ___nextUIntB(min, max); }

        long IRandomUnbounded<long>.Next()                       { return ___nextLongNB(); }
        long IRandomBounded<long>.Next(long min, long max)     { return ___nextLongB(min, max); }

        ulong IRandomUnbounded<ulong>.Next()                     { return ___nextULongNB(); }
        ulong IRandomBounded<ulong>.Next(ulong min, ulong max) { return ___nextULongB(min, max); }
    }
}
