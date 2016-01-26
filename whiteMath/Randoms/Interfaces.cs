using System.Collections.Generic;
using System.Text;

using whiteStructs.Conditions;

namespace whiteMath.Randoms
{
    /// <summary>
    /// A generic interface providing means
    /// for generating random numbers in the whole range
    /// of numeric type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of generated numbers.</typeparam>
    public interface IRandomUnbounded<T>
    {
        /// <summary>
        /// Returns the next random value from
        /// all of the possible finite values of type <typeparamref name="T"/>.
        /// </summary>
        T Next();
    }

    /// <summary>
    /// A generic interface providing means
    /// for generating random numbers in the specified range.
    /// </summary>
    /// <typeparam name="T">The type of generated numbers.</typeparam>
    public interface IRandomBounded<T>
    {
        /// <summary>
        /// Returns the next random value in the
        /// <c>[min; max)</c> interval.
        /// </summary>
        /// <param name="min">The lower _inclusive_ numeric boundary of random generation.</param>
        /// <param name="max">The upper _exclusive_ numeric boundary of random generation.</param>
        /// <returns>A random number laying in the [min; max) interval.</returns>
        T Next(T min, T max);
    }

    /// <summary>
    /// A generic interface providing means
    /// for generating random numbers either in the whole
    /// numeric range of type <typeparamref name="T"/> or
    /// in a user-specified concrete range.
    /// </summary>
    /// <typeparam name="T">The type of generated numbers.</typeparam>
    public interface IRandomBoundedUnbounded<T> : IRandomBounded<T>, IRandomUnbounded<T> { }

    /// <summary>
    /// A generic interface providing means 
    /// for generating random floating-point numbers in the <c>[0; 1)</c> interval.
    /// </summary>
    /// <typeparam name="T">The type of generated numbers.</typeparam>
    public interface IRandomFloatingPoint<T>
    {
        /// <summary>
        /// Returns the next random value in the
        /// [0; 1) interval.
        /// </summary>
        /// <returns>A value in the [0; 1) interval.</returns>
        T Next_SingleInterval();
    }

    /// <summary>
    /// An interface which provides a method
    /// for generating pseudo-random sequences of
    /// bytes.
    /// </summary>
    public interface IRandomBytes
    {
        /// <summary>
        /// Generates a sequence of bytes
        /// and stores them in a buffer.
        /// </summary>
        /// <param name="buffer">The buffer to store the random sequence of bytes.</param>
        void NextBytes(byte[] buffer);
    }

    // ------------------------------------------------------
    // ------------------------------------------------------

    /// <summary>
    /// This class contains extension methods for <see cref="IRandomBounded&lt;T&gt;"/>.
    /// </summary>
    public static class RandomBounded
    {
        /// <summary>
        /// Generates a pseudo-random string of the desired length containing 
		/// characters from a <see cref="char"/> list. There are no guarantees for 
		/// every character to appear in the string.
        /// </summary>
        /// <param name="gen">
		/// An integer generator which will be used to provide 
		/// random indices for the character list.
		/// </param>
        /// <param name="characters">
		/// A list of characters. The presence of duplicates will increase the character's 
		/// chance of appearing in the result string.
		/// </param>
        /// <param name="length">A non-negative desired length of the string.</param>
		/// <returns>
		/// A random string of the desired <paramref name="length"/> that consists of
		/// characters from a <see cref="char"/> list.
		/// </returns>
        public static string NextString(this IRandomBounded<int> gen, IList<char> characters, int length)
        {
			Condition.ValidateNotNull(gen, nameof(gen));
			Condition.ValidateNotNull(characters, nameof(characters));
			Condition
				.Validate(length >= 0)
				.OrArgumentOutOfRangeException("The length of the string should be non-negative.");

            StringBuilder builder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
                builder.Append(characters[gen.Next(0, characters.Count)]);

            return builder.ToString();
        }
    }
}
