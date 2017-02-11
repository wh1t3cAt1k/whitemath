using System;
using System.Collections.Generic;

using WhiteMath.Randoms;

using WhiteStructs.Conditions;

namespace WhiteMath.General
{
    /// <summary>
    /// This static class provides extension methods
    /// for filling collections with new elements.
    /// </summary>
    public static class CollectionFillingExtensions
    {
        /// <summary>
        /// Adds the same value to a collection the specified amount of times.
        /// </summary>
        /// <remarks>
        /// The filling is made by <c>Add()</c> operation, which means, no existing 
        /// elements of the collection will be modified by this method.
        /// </remarks>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="collection">The collection reference.</param>
        /// <param name="elementCount">A number specifying how many elements should be added to the collection.</param>
        /// <param name="value">The value to fill the collection with.</param>
        public static void FillByAppending<T>(this ICollection<T> collection, int elementCount, T value)
        {
			Condition.ValidateNotNull(collection, nameof(collection));
			Condition.ValidateNonNegative(elementCount, "The element count should be non-negative.");

			for (int i = 0; i < elementCount; ++i)
			{
				collection.Add(value);
			}
        }

        /// <summary>
        /// Adds elements to a collection by using a random number generator
        /// to create random numbers in the interval <c>[<paramref name="min"/>; <paramref name="max"/>)</c>.
        /// </summary>
        /// <remarks>
        /// The filling is made by <c>Add()</c> operation, which means, no existing 
        /// elements of the collection will be modified by this method.
        /// </remarks>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="collection">The collection reference.</param>
        /// <param name="elementCount">A number specifying how many elements should be added to the collection.</param>
        /// <param name="randomGenerator">A random generator.</param>
        /// <param name="min">The lower inclusive bound of numbers to be generated.</param>
        /// <param name="max">The upper exclusive bound of numbers to be generated.</param>
        public static void FillByAppending<T>(this ICollection<T> collection, int elementCount, IRandomBounded<T> randomGenerator, T min, T max)
        {
			Condition.ValidateNotNull(collection, nameof(collection));
			Condition.ValidateNotNull(randomGenerator, nameof(randomGenerator));
			Condition.ValidateNonNegative(elementCount, "The element count should be non-negative.");

			for (int i = 0; i < elementCount; ++i)
			{
				collection.Add(randomGenerator.Next(min, max));
			}
        }

        /// <summary>
        /// Adds elements to a collection by incorporating current element's zero-based order number.
        /// </summary>
        /// <remarks>
        /// The filling is made by <c>Add()</c> operation, which means, no existing 
        /// elements of the collection will be modified by this method.
        /// </remarks>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="collection">The collection reference.</param>
        /// <param name="elementCount">A number specifying how many elements should be added to the collection.</param>
        /// <param name="function">A function that maps integer indices to <typeparamref name="T"/> values.</param>
        public static void FillByAppending<T>(this ICollection<T> collection, int elementCount, Func<int, T> function)
        {
			Condition.ValidateNotNull(collection, nameof(collection));
			Condition.ValidateNotNull(function, nameof(function));
			Condition.ValidateNonNegative(elementCount, "The element count should be non-negative.");

			for (int i = 0; i < elementCount; i++)
			{
				collection.Add(function(i));
			}
        }

        /// <summary>
        /// Adds elements to a collection by incorporating previously added element's value.
        /// The first element to add should be explicitly provided.
        /// </summary>
        /// <remarks>
        /// The filling is made by <c>Add()</c> operation, which means, no existing 
        /// elements of the collection will be modified by this method.
        /// </remarks>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="collection">The collection reference.</param>
        /// <param name="elementCount">A number specifying how many elements should be added to the collection.</param>
        /// <param name="function">The function that maps <typeparamref name="T"/>--><typeparamref name="T"/></param>
        /// <param name="firstElement">
        /// The first element to be added. The next element would be 
        /// created using <c><paramref name="function"/></c> 
        /// with this element as an argument.
        /// </param>
        public static void FillByAppending<T>(this ICollection<T> collection, int elementCount, Func<T, T> function, T firstElement)
        {
			Condition.ValidateNotNull(collection, nameof(collection));
			Condition.ValidateNotNull(function, nameof(function));
			Condition.ValidateNonNegative(elementCount, "The element count should be non-negative.");

            T current;
            T last = firstElement;

            collection.Add(last);

            for (int i = 1; i < elementCount; i++)
            {
                current = function(last);
                collection.Add(current);
                last = current;
            }
        }

        /// <summary>
        /// Adds elements to a collection incorporating both current element's zero-based order number 
        /// and the value of previously added element. 
        /// The first element to add should be explicitly provided.
        /// </summary>
        /// <remarks>
        /// The filling is made by <c>Add()</c> operation, which means, no existing 
        /// elements of the collection will be modified by this method.
        /// </remarks>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="collection">The collection reference.</param>
        /// <param name="elementCount">A number specifying how many elements should be added to the collection.</param>
        /// <param name="function">The function mapping (int, T) --> T.</param>
        /// <param name="firstElement">
        /// The first element to be added to the collection. 
        /// The next element would be created using the function passed with this element as an argument.</param>
        public static void FillByAppending<T>(this ICollection<T> collection, int elementCount, Func<T, int, T> function, T firstElement)
        {
			Condition.ValidateNotNull(collection, nameof(collection));
			Condition.ValidateNotNull(function, nameof(function));
			Condition.ValidateNonNegative(elementCount, "The element count should be non-negative.");

            T current;
            T last = firstElement;

            collection.Add(last);

            for (int i = 1; i < elementCount; i++)
            {
                current = function(last, i);
                collection.Add(current);
                last = current;
            }
        }
    }
}
