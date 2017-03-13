using System;
using System.Collections.Generic;

using WhiteMath.Random;

namespace WhiteMath.General
{
    public static class ListFillingExtensions
    {
        /// <summary>
        /// Overwrites the entire list with the value specified.
        /// </summary>
        /// <remarks>
        /// The filling is made by assign operation, which means, no new elements
        /// will be added to the collection using <c>Add()</c> method.
        /// </remarks>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list reference.</param>
        /// <param name="value">The value to fill the list with.</param>
        public static void FillByAssign<T>(this IList<T> list, T value)
        {
			for (int i = 0; i < list.Count; ++i)
			{
				list[i] = value;
			}
        }

        /// <summary>
        /// Overwrites the entire list using a random number generator
        /// which would be used to generate random numbers in the interval
        /// [<paramref name="min"/>; <paramref name="max"/>).
        /// </summary>
        /// <remarks>
        /// The filling is made by assign operation, which means, no new elements
        /// will be added to the collection using <c>Add()</c> method.
        /// </remarks>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list reference.</param>
        /// <param name="randomGenerator">The random generator to generate random numbers for the list.</param>
        /// <param name="min">The minimum inclusive value to generate.</param>
        /// <param name="max">The maximum exclusive value to generate.</param>
        public static void FillByAssign<T>(this IList<T> list, IRandomBounded<T> randomGenerator, T min, T max)
        {
			for (int i = 0; i < list.Count; ++i)
			{
				list[i] = randomGenerator.Next(min, max);
			}
        }

        /// <summary>
        /// Overwrites the entire list by incorporating current element's index.
        /// </summary>
        /// <remarks>
        /// The filling is made by assign operation, which means, no new elements
        /// will be added to the collection using <c>Add()</c> method.
        /// </remarks>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list reference.</param>
        /// <param name="function">The function mapping integer indices to <typeparamref name="T"/> values.</param>
        public static void FillByAssign<T>(this IList<T> list, Func<int, T> function)
        {
			for (int i = 0; i < list.Count; ++i)
			{
				list[i] = function(i);
			}
        }

        /// <summary>
        /// Overwrites the entire list by incorporating the previous element's value.
        /// The first element of the list should be thus explicitly provided.
        /// </summary>
        /// <remarks>
        /// The filling is made by assign operation, which means, no new elements
        /// will be added to the collection using <c>Add()</c> method.
        /// </remarks>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list reference.</param>
        /// <param name="function">The function mapping <typeparamref name="T"/>--><typeparamref name="T"/></param>
        /// <param name="firstElement">
        /// The first element of the list. The next element would be 
        /// created using <c><paramref name="function"/></c> 
        /// and this element as an argument.
        /// </param>
        public static void FillByAssign<T>(this IList<T> list, Func<T, T> function, T firstElement)
        {
			if (list.Count == 0)
			{
				return;
			}

            list[0] = firstElement;

			for (int i = 1; i < list.Count; ++i)
			{
				list[i] = function(list[i - 1]);
			}
        }

        /// <summary>
        /// Overwrites the entire list by incorporating both current element's index and the value of
        /// the previous element. The first element of the list should be explicitly provided.
        /// </summary>
        /// <remarks>
        /// The filling is made by assign operation, which means, no new elements
        /// will be added to the collection using <c>Add()</c> method.
        /// </remarks>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list reference.</param>
        /// <param name="function">The function mapping the current index AND the previous list element --> to the CURRENT list element.</param>
        /// <param name="firstElement">The first element of the list. The next element would be created using the function passed with this element as an argument.</param>
        public static void FillByAssign<T>(this IList<T> list, Func<T, int, T> function, T firstElement)
        {
			if (list.Count == 0)
			{
				return;
			}

            list[0] = firstElement;

			for (int i = 1; i < list.Count; ++i)
			{
				list[i] = function(list[i - 1], i);
			}
        }

    }
}
