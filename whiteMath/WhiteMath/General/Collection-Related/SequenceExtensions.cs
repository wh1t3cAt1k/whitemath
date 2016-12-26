using System;
using System.Collections.Generic;
using System.Linq;

using WhiteMath.Calculators;

namespace WhiteMath.General
{
    public static class SequenceExtensions
    {
        /// <summary>
        /// For ordered sequences, returns a list of pairs with first element being the index,
        /// and second being one of the sequence elements. The order of elements is preserved.
        /// </summary>
        /// <typeparam name="T">The type of elements in the ordered sequence.</typeparam>
        /// <param name="sequence">The calling ordered sequence.</param>
        /// <returns>returns a list of pairs with first element being the index, and second being one of the sequence elements. The order of elements is preserved.</returns>
        public static List<KeyValuePair<int, T>> toListOfIndexValuePairs<T>(this IEnumerable<T> sequence)
        {
            return toListOfIndexValuePairs(sequence, delegate (int index, T obj) { return obj; });
        }

        /// <summary>
        /// For ordered sequences, returns a list of pairs with first element being the index,
        /// and second being the result of selector function applied to the value and current index. The order of elements is preserved.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the source sequence.</typeparam>
        /// <typeparam name="TResult">The type of values in the key-value pairs in the result sequence.</typeparam>
        /// <param name="sequence">The calling ordered sequence.</param>
        /// <param name="selector">The selector function transforming elements from <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// A list of pairs with first element being the index and second being the result of selector function applied to the value and current index. 
        /// The order of elements is preserved.
        /// </returns>
        public static List<KeyValuePair<int, TResult>> toListOfIndexValuePairs<TSource, TResult>(this IEnumerable<TSource> sequence, Func<int, TSource, TResult> selector)
        {
            int index = 0;

            List<KeyValuePair<int, TResult>> result = new List<KeyValuePair<int, TResult>>(sequence.Count());

            foreach (TSource value in sequence)
            {
                result.Add(new KeyValuePair<int, TResult>(index, selector(index++, value)));
            }

            return result;
        }

        /// <summary>
        /// Converts a numeric sequence into a list of index-value points, where index has the same numeric type
        /// as the elements of the sequence.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <typeparam name="C">A calculator for the <typeparamref name="T"/> type.</typeparam>
        /// <param name="sequence">The calling sequence object.</param>
        /// <returns>a list of index-value points, where index has the same numeric type as the elements of the sequence.</returns>
        public static List<Point<T>> toListOfIndexValuePoints<T, C>(this IEnumerable<T> sequence) where C: ICalc<T>, new()
        {
            ICalc<T> calc = Numeric<T,C>.Calculator;
            
            T index = calc.Zero;

            List<Point<T>> result = new List<Point<T>>(sequence.Count());

            foreach(T value in sequence)
            {
                result.Add(new Point<T>(index, value));
                index = calc.Increment(index);
            }

            return result;
        }
    }
}
