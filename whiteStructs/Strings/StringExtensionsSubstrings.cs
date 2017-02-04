using System;
using System.Collections.Generic;

using WhiteStructs.Conditions;

namespace WhiteStructs.Strings
{
    public static class StringExtensionsSubstrings
    {
        public enum SubstringSearchOptions
        {
            Default, InvariantCultureIgnoreCase, CurrentCultureIgnoreCase
        }

		private static bool AreCharactersEqual(char a, char b, SubstringSearchOptions options)
        {
			if (options == SubstringSearchOptions.Default)
			{
				return a.Equals(b);
			}
			else if (options == SubstringSearchOptions.CurrentCultureIgnoreCase)
			{
				return char.ToLower(a).Equals(char.ToLower(b));
			}
			else if (options == SubstringSearchOptions.InvariantCultureIgnoreCase)
			{
				return char.ToLowerInvariant(a).Equals(char.ToLowerInvariant(b));
			}
			else
			{
				// TODO: need common enum fattened exception 
				// for both WhiteMath and WhiteStructs.
				// -
				throw new Exception();
			}
        }

        /// <summary>
        /// This method will calculate the Z-function for the caller string.
        /// The result of the Z-function is a vector; the i-th element of this
        /// vector is the length of the greatest common prefix of the string with 
		/// the SPL string, where the SPL is a suffix of the string starting 
		/// at element <c>i</c>.
        /// </summary>
        /// <param name="text">The caller string object.</param>
        /// <param name="options">The options of searching for the substring.</param>
        /// <returns>The Z-function for the string.</returns>
		public static int[] CalculateZFunction(
			this string text, 
			SubstringSearchOptions options = SubstringSearchOptions.Default)
        {
			int textLength = text.Length;
            int[] result = new int[textLength];

			if (textLength == 0)
			{
				return result;
			}

            result[0] = text.Length;
 
            int left = 0;
            int right = 1;

			for (int characterIndex = 1; characterIndex < textLength; ++characterIndex)
            {
                if (characterIndex >= right)
                {
					// Unable to find the rightmost Z-block covering the 
					// current index. Need to calculate all over again.
					// -
                    int offset = 0;

					while (
						characterIndex + offset < textLength
						&& AreCharactersEqual(
							text[characterIndex + offset],
							text[offset],
							options))
					{
						++offset;
					}

                    result[characterIndex] = offset;

                    left = characterIndex;
                    right = characterIndex + offset;
                }
                else
                {
					// There is an intersection.
					// -
                    int equivalentIndex = characterIndex - left;

                    if (result[equivalentIndex] < right - characterIndex)
                    {
						// More symbols are left till the end of the Z-block
						// than the value of the Z-function. No need for further checks.
						// -
                        result[characterIndex] = result[equivalentIndex];
                        continue;
                    }

                    int offset = 0;

					while (right + offset < textLength
						&& AreCharactersEqual(
							text[right - characterIndex + offset],
							text[right + offset],
							options))
					{
						++offset;
					}

                    result[characterIndex] = right - characterIndex + offset;

                    left = characterIndex;
                    right += offset;
                }
            }

            return result;
        }

        /// <summary>
        /// Searches for the first occurence of a specified sample string in the calling string
        /// using the specified substring search options. Returns the index of the first occurence 
		/// or a negative value when unsuccessful.
        /// </summary>
        /// <param name="text">The calling string object.</param>
        /// <param name="sample">The sample string to search for.</param>
        /// <param name="options">The options of searching.</param>
        /// <returns>The first index of the occurence or a negative value when unsuccessful</returns>
		public static int IndexOfUsingZFunction(
			this string text, 
			string sample, 
			SubstringSearchOptions options = SubstringSearchOptions.Default)
        {
			Condition.ValidateNotNull(text, nameof(text));
			Condition.ValidateNotNull(sample, nameof(sample));

            IList<int> indices = IndicesOfUsingZFunction(text, sample, options);

			if (indices.Count > 0)
			{
				return indices[0];
			}

			return -1;
        }

        /// <summary>
        /// Searches for all occurences of a specified sample string in the calling string 
        /// using the specified substring search options and returns the indices of the occurences.
        /// The indices of the sample are returned in the form of an int[] array.
        /// </summary>
        /// <param name="text">The calling string object.</param>
        /// <param name="sample">The sample string to search for.</param>
        /// <param name="options">The options of searching.</param>
        /// <returns>A list of indices in the <paramref name="text"/> where the specified sample string occurs.</returns>
		public static IList<int> IndicesOfUsingZFunction(
			this string text, 
			string sample, 
			SubstringSearchOptions options = SubstringSearchOptions.Default)
        {
			Condition.ValidateNotNull(text, nameof(text));
			Condition.ValidateNotNull(sample, nameof(sample));

			const string UNICODE_NONCHARACTER = "\uFFFF";

			string combination = $"{sample}{UNICODE_NONCHARACTER}{text}";
			int[] zFunctionVector = combination.CalculateZFunction(options);

            IList<int> indices = new List<int>();

			for (int characterIndex = sample.Length + 1; characterIndex < zFunctionVector.Length; ++characterIndex)
            {
				if (zFunctionVector[characterIndex] == sample.Length)
				{
					indices.Add(characterIndex - sample.Length - 1);
				}
            }

            return indices;
        }
    }
}
