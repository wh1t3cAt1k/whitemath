using System.Text;
using System.Collections.Generic;

using WhiteStructs.Conditions;

namespace WhiteMath.Randoms
{
	/// <summary>
	/// This class contains various extension methods for <see cref="IRandomBounded{T}"/>.
	/// </summary>
	public static class RandomBoundedExtensions
	{
		/// <summary>
		/// Generates a pseudo-random string of the desired length containing 
		/// characters from a <see cref="char"/> list. There is no guarantee for 
		/// every character to appear in the string.
		/// </summary>
		/// <param name="generator">
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
		public static string NextString(this IRandomBounded<int> generator, IList<char> characters, int length)
		{
			Condition.ValidateNotNull(generator, nameof(generator));
			Condition.ValidateNotNull(characters, nameof(characters));
			Condition
				.Validate(length >= 0)
				.OrArgumentOutOfRangeException("The length of the string should be non-negative.");

			StringBuilder builder = new StringBuilder(length);

			for (int i = 0; i < length; ++i)
			{
				builder.Append(characters[generator.Next(0, characters.Count)]);
			}

			return builder.ToString();
		}
	}
}
