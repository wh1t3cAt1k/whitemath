using System;
using System.Security.Cryptography;

using WhiteStructs.Conditions;

namespace WhiteMath.Randoms
{
    /// <summary>
    /// Represents a random number generator wrapper built
    /// on a cryptographically strong <see cref="RandomNumberGenerator"/> 
	/// instance.
    /// </summary>
    public class RandomCryptographic:
        IRandomBoundedUnbounded<int>,
        IRandomBoundedUnbounded<uint>,
        IRandomBoundedUnbounded<long>,
        IRandomBoundedUnbounded<ulong>
    {
		private RandomNumberGenerator _generator;

		private UpperBoundedGenerator<int> _getNextIntUpperBounded;
		private UpperBoundedGenerator<uint> _getNextUnsignedIntUpperBounded;
		private UpperBoundedGenerator<long> _getNextLongUpperBounded;
		private UpperBoundedGenerator<ulong> _getNextUnsignedLongUpperBounded;

		private UnboundedGenerator<int> _getNextIntUnbounded;
		private UnboundedGenerator<uint> _getNextUnsignedIntUnbounded;
		private UnboundedGenerator<long> _getNextLongUnbounded;
		private UnboundedGenerator<ulong> _getNextUnsignedLongUnbounded;

		private BoundedGenerator<int> _getNextIntBounded;
		private BoundedGenerator<uint> _getNextUnsignedIntBounded;
		private BoundedGenerator<long> _getNextLongBounded;
		private BoundedGenerator<ulong> _getNextUnsignedLongBounded;
        
        /// <summary>
        /// Initializes an instance of <see cref="RandomCryptographic"/>
        /// with a single instance of cryptographically strong <see cref="RandomNumberGenerator"/>.
        /// </summary>
        /// <param name="generator">
        /// An instance of cryptographically strong <see cref="RandomNumberGenerator"/>.
        /// If <c>null</c>, an instance built from <see cref="RNGCryptoServiceProvider.Create"/> will be used.
        /// </param>
		public RandomCryptographic(RandomNumberGenerator generator = null)
        {
			if (generator == null)
			{
				generator = RandomNumberGenerator.Create();
			}

            _generator = generator;

            // initialize methods

            this._getNextIntBounded = RandomFunctionalityExtensions.CreateNextIntBounded(this._generator.GetBytes);
            this._getNextIntUnbounded = RandomFunctionalityExtensions.CreateNextIntUnbounded(this._generator.GetBytes);
            this._getNextIntUpperBounded = RandomFunctionalityExtensions.CreateNextIntUpperBounded(this._generator.GetBytes);

            this._getNextLongBounded = RandomFunctionalityExtensions.CreateNextLongBounded(this._generator.GetBytes);
            this._getNextLongUnbounded = RandomFunctionalityExtensions.CreateNextLongUnbounded(this._generator.GetBytes);
            this._getNextLongUpperBounded = RandomFunctionalityExtensions.CreateNextLongUpperBounded(this._generator.GetBytes);

            this._getNextUnsignedIntBounded = RandomFunctionalityExtensions.CreateNextUIntBounded(this._generator.GetBytes);
            this._getNextUnsignedIntUnbounded = RandomFunctionalityExtensions.CreateNextUIntUnbounded(this._generator.GetBytes);
            this._getNextUnsignedIntUpperBounded = RandomFunctionalityExtensions.CreateNextUIntUpperBounded(this._generator.GetBytes);

            this._getNextUnsignedLongBounded = RandomFunctionalityExtensions.CreateNextULongBounded(this._generator.GetBytes);
            this._getNextUnsignedLongUnbounded = RandomFunctionalityExtensions.CreateNextULongUnbounded(this._generator.GetBytes);
            this._getNextUnsignedLongUpperBounded = RandomFunctionalityExtensions.CreateNextULongUpperBounded(this._generator.GetBytes);
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
                _getNextUnsignedIntUnbounded();
        }

        /// <summary>
        /// Returns the next <c>uint</c> number limited
        /// by an exclusive upper boundary.
        /// </summary>
        /// <param name="maxValue">The exclusive upper boundary of generated numbers.</param>
        /// <returns>A random <c>uint</c> number which is smaller than <paramref name="maxValue"/>.</returns>
        public uint NextUInt(uint maxValue)
        {
			Condition.ValidatePositive(maxValue, "The upper exclusive boundary for generated values should be positive.");

            return
                _getNextUnsignedIntUpperBounded(maxValue);
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
			Condition
				.Validate(maxValue > minValue)
				.OrArgumentException("The upper exclusive boundary should be bigger than the lower inclusive boundary.");

            return
                _getNextUnsignedIntBounded(minValue, maxValue);
        }



        /// <summary>
        /// Returns the next <c>ulong</c> number.
        /// </summary>
        /// <returns>A random <c>ulong</c> number.</returns>
        public ulong NextULong()
        {
            return
                _getNextUnsignedLongUnbounded();
        }

        /// <summary>
        /// Returns the next <c>ulong</c> number limited
        /// by an exclusive upper boundary.
        /// </summary>
        /// <param name="maxValue">The exclusive upper boundary of generated numbers.</param>
        /// <returns>A random <c>ulong</c> number which is smaller than <paramref name="maxValue"/>.</returns>
        public ulong NextULong(ulong maxValue)
        {
			Condition
				.Validate(maxValue > 0)
				.OrArgumentOutOfRangeException("The upper exclusive boundary for generated values should be positive.");

            return
                _getNextUnsignedLongUpperBounded(maxValue);
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
			Condition
				.Validate(maxValue > minValue)
				.OrArgumentException("The upper exclusive boundary should be bigger than the lower inclusive boundary.");

            return
                _getNextUnsignedLongBounded(minValue, maxValue);
        }

        /// <summary>
        /// Returns the next <c>int</c> value in the [int.MinValue; int.MaxValue]
        /// interval.
        /// </summary>
        /// <returns>The next number in the <c>[int.MinValue; int.MaxValue]</c> interval.</returns>
        public int NextInt()
        {
            return
                _getNextIntUnbounded();
        }

        /// <summary>
        /// Returns the next non-negative <c>int</c> value that is less than
        /// <paramref name="maxValue"/>
        /// </summary>
        /// <param name="maxValue">The upper exclusive bound for generated number.</param>
        /// <returns>A non-negative <c>int</c> number that is less than <paramref name="maxValue"/></returns>
        public int NextInt(int maxValue)
        {
			Condition.ValidatePositive(maxValue, "The upper exclusive boundary should be a positive number.");

            return
                _getNextIntUpperBounded(maxValue);
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
			Condition
				.Validate(maxValue > minValue)
				.OrArgumentException("The upper exclusive boundary should be bigger than the lower inclusive boundary.");

            return
                _getNextIntBounded(minValue, maxValue);
        }


        /// <summary>
        /// Returns the next <c>long</c> value in the [long.MinValue; long.MaxValue]
        /// interval.
        /// </summary>
        /// <returns>The next number in the <c>[long.MinValue; long.MaxValue]</c> interval.</returns>
        public long NextLong()
        {
            return
                _getNextLongUnbounded();
        }

        /// <summary>
        /// Returns the next non-negative <c>long</c> value that is less than
        /// <paramref name="maxValue"/>
        /// </summary>
        /// <param name="maxValue">The upper exclusive bound for generated number.</param>
        /// <returns>A non-negative <c>long</c> number that is less than <paramref name="maxValue"/></returns>
        public long NextLong(long maxValue)
        {
			Condition.ValidatePositive(maxValue, "The upper exclusive boundary should be a positive number.");

            return
                _getNextLongUpperBounded(maxValue);
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
			Condition
				.Validate(maxValue > minValue)
				.OrArgumentException("The upper exclusive boundary should be bigger than the lower inclusive boundary.");
			
            return
                _getNextLongBounded(minValue, maxValue);
        }

		#region Explicit Interface Implementation

		int IRandomUnbounded<int>.Next() 
			=> this.NextInt();

		int IRandomBounded<int>.Next(int minInclusive, int maxExclusive) 
			=> this.NextInt(minInclusive, maxExclusive);

		uint IRandomUnbounded<uint>.Next() 
			=> this.NextUInt();

		uint IRandomBounded<uint>.Next(uint minInclusive, uint maxExclusive) 
			=> this.NextUInt(minInclusive, maxExclusive);

		long IRandomUnbounded<long>.Next() 
			=> this.NextLong();

		long IRandomBounded<long>.Next(long minInclusive, long maxExclusive) 
			=> this.NextLong(minInclusive, maxExclusive);

		ulong IRandomUnbounded<ulong>.Next()
			=> this.NextULong();

		ulong IRandomBounded<ulong>.Next(ulong minInclusive, ulong maxExclusive) 
			=> this.NextULong(minInclusive, maxExclusive);

		#endregion
    }
}
