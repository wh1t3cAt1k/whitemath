using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteStructs.Strings
{
    public static class SubstringSearching
    {
        public enum SubstringSearchOptions
        {
            Default, InvariantCultureIgnoreCase, CurrentCultureIgnoreCase
        }

        private static bool _CHAR_EQUAL(char a, char b, SubstringSearchOptions opt)
        {
            if (opt == SubstringSearchOptions.Default)
                return a.Equals(b);
            
            else if (opt == SubstringSearchOptions.CurrentCultureIgnoreCase)
                return char.ToLower(a).Equals(char.ToLower(b));

            else 
                return char.ToLowerInvariant(a).Equals(char.ToLowerInvariant(b));
        }

        /// <summary>
        /// This method will calculate the Z-function for the caller string.
        /// The result of the Z-function is a vector; the i-th element of this
        /// vector is the length of the greatest common prefix of the string with the string SPL,
        /// where the SPL is a suffix of the string starting at element i.
        /// </summary>
        /// <param name="str">The caller string object.</param>
        /// <param name="options">The options of searching for the substring.</param>
        /// <returns>The Z-function for the string.</returns>
        public static int[] calculateZFunction(this string str, SubstringSearchOptions options = SubstringSearchOptions.Default)
        {

            int length = str.Length;
            int[] result = new int[length];

            if(length == 0)
                return result;

            result[0] = str.Length;
 
            int left = 0;
            int right = 1;

            for (int current = 1; current < length; current++)
            {
                // Не находится самого правого Z-блока, покрывающего текущий индекс.
                // Надо все вычислять с самого начала.

                if (current >= right)
                {
                    int offset = 0;

                    while (current + offset < length && _CHAR_EQUAL(str[current + offset], str[offset], options))
                        offset++;

                    result[current] = offset;

                    left = current;
                    right = current + offset;
                }

                // перекрытие есть
                else
                {
                    int equivalentIndex = current - left;

                    // До конца Z-блока осталось больше символов, чем значение Z-функции.
                    // Можно не проверять.

                    if (result[equivalentIndex] < right - current)
                    {
                        result[current] = result[equivalentIndex];
                        continue;
                    }

                    int offset = 0;

                    while (right + offset < length && _CHAR_EQUAL(str[right - current + offset], str[right + offset], options))
                        offset++;

                    result[current] = right - current + offset;

                    left = current;
                    right += offset;
                }
            }

            return result;
        }

        /// <summary>
        /// Searches for the first occurence of a specified sample string in the calling string
        /// using the specified substring search options and returns the index of the occurence or a negative value when unsuccessful.
        /// </summary>
        /// <param name="str">The calling string object.</param>
        /// <param name="sample">The sample string to search for.</param>
        /// <param name="options">The options of searching.</param>
        /// <returns>The index of the occurence or a negative value when unsuccessful</returns>
        public static int indexOfString_ZFunction(this string str, string sample, SubstringSearchOptions options = SubstringSearchOptions.Default)
        {
            List<int> indices = indicesOfString_ZFunction(str, sample, options);

            if (indices.Count > 0)
                return indices[0];
            else
                return -1;
        }

        /// <summary>
        /// Searches for all occurences of a specified sample string in the calling string 
        /// using the specified substring search options and returns the indices of the occurences.
        /// 
        /// The indexes of the sample are returned in the form of an int[] array.
        /// </summary>
        /// <param name="str">The calling string object.</param>
        /// <param name="sample">The sample string to search for.</param>
        /// <param name="options">The options of searching.</param>
        /// <returns>A list of indices in the <paramref name="str"/> where the specified sample string occurs.</returns>
        public static List<int> indicesOfString_ZFunction(this string str, string sample, SubstringSearchOptions options = SubstringSearchOptions.Default)
        {
            string  tmp         = sample + "\uFFFF" + str;
            int[]   zFunction   = tmp.calculateZFunction(options);

            List<int> indices = new List<int>();

            for (int i = sample.Length + 1; i < zFunction.Length; i++)
            {
                if (zFunction[i] == sample.Length)
                    indices.Add(i - sample.Length - 1);
            }

            return indices;
        }
    }
}
