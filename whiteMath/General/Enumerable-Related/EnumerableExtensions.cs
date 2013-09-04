using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace whiteMath.General
{
    [ContractVerification(true)]
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Constructs a string out of a sequence, enumerating its values through the separator.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="sequence">The calling sequence object.</param>
        /// <param name="separator">A string used to separate adjacent elements.</param>
        /// <param name="toString">
        /// A function that converts <typeparamref name="T"/> values to their string representations. 
        /// If <c>null</c>, standard <c>ToString()</c> function will be used.
        /// </param>
        /// <returns>A string which enumerates the values of the sequence through the separator string specified.</returns>
        public static string ToElementString<T>(this IEnumerable<T> sequence, string separator = "|", Func<T, string> toString = null)
        {
            Contract.Requires<ArgumentNullException>(sequence != null, "sequence");

            StringBuilder res = new StringBuilder();

            foreach (T obj in sequence)
                res.Append(String.Format("{0}{1}", separator, (toString == null ? obj.ToString() : toString(obj))));

            if (res.Length > 0)
                res.Append(separator);

            return res.ToString();
        }

        /// <summary>
        /// Tests whether a sequence is empty, i.e. contains not a single element.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="sequence">The calling sequence object.</param>
        /// <returns>True if the sequence is empty, false otherwise.</returns>
        public static bool IsEmpty<T>(this IEnumerable<T> sequence)
        {
            return 
                sequence.GetEnumerator().MoveNext() == false;
        }

        /// <summary>
        /// Finds the maximum element in a generic sequence using the comparer object specified.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="sequence">The calling sequence object.</param>
        /// <param name="comparer">A comparer for the T type. Its Compare() method should return a positive value in case when the first compared element is bigger than the second.</param>
        /// <returns>The maximum element in the sequence.</returns>
        public static T Max<T>(this IEnumerable<T> sequence, IComparer<T> comparer)
        {
            T max;

            IEnumerator<T> enumerator = sequence.GetEnumerator();

            if (!enumerator.MoveNext())
                throw GeneralExceptions.__SEQUENCE_EMPTY;
            else
                max = enumerator.Current;

            while (enumerator.MoveNext())
                if (comparer.Compare(enumerator.Current, max) > 0)
                    max = enumerator.Current;
            
            return max;
        }

        /// <summary>
        /// Finds the minimum element in a generic sequence using the comparer object specified.
        /// The comparer is expected to return a positive number if the first element is bigger than the second.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="sequence">The calling sequence object.</param>
        /// <param name="comparer">A comparer for the T type. Its Compare() method should return a positive value in case when the first compared element is bigger than the second.</param>
        /// <returns>The minimum element in the sequence.</returns>
        public static T Min<T>(this IEnumerable<T> sequence, IComparer<T> comparer)
        {
            T min;

            IEnumerator<T> enumerator = sequence.GetEnumerator();

            if (!enumerator.MoveNext())
                throw GeneralExceptions.__SEQUENCE_EMPTY;
            else
                min = enumerator.Current;

            while (enumerator.MoveNext())
                if (comparer.Compare(enumerator.Current, min) < 0)
                    min = enumerator.Current;

            return min;
        }

        /// <summary>
        /// Finds the minimum and the maximum elements in a sequence.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the sequence.
        /// </typeparam>
        /// <param name="sequence">
        /// A sequence object in which the minimal and maximal elements would be found.
        /// </param>
        /// <param name="comparer">
        /// An optional <see cref="IComparer&lt;T&gt;"/> for the <typeparamref name="T"/> type. 
        /// </param>
        /// <returns>
        /// A logical point whose X value is equal to the minimum, and Y value is equal to the maximum.</returns>
        public static Point<T> MinMax<T>(this IEnumerable<T> sequence, IComparer<T> comparer = null)
        {
            Contract.Requires<ArgumentNullException>(sequence != null, "sequence");
            Contract.Requires<ArgumentException>(!sequence.IsEmpty(), "The sequence should contain at least one element.");

            if (comparer == null)
            {
                comparer = Comparer<T>.Default;
            }

            T min;
            T max;

            IEnumerator<T> enumerator = sequence.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                throw GeneralExceptions.__SEQUENCE_EMPTY;
            }
            else
            {
                min = max = enumerator.Current;
            }

            while (enumerator.MoveNext())
            {
                if (comparer.Compare(enumerator.Current, min) < 0)
                    min = enumerator.Current;
                else if (comparer.Compare(enumerator.Current, max) > 0)
                    max = enumerator.Current;
            }

            return new Point<T>(min, max);
        }

        // ------------------------------------
        // ------- TO 2D ARRAY ---------------
        // ------------------------------------

        /// <summary>
        /// Creates a single-column two-dimensional array from an enumerable.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">A sequence to be converted to 2D single-column array.</param>
        /// <returns>
        /// A single-column two dimensional array containing the elements of 
        /// <paramref name="enumerable"/> in increasing row order.
        /// </returns>
        public static T[,] To2DArrayColumn<T>(this IEnumerable<T> enumerable)
        {
            Contract.Requires<ArgumentNullException>(enumerable != null, "enumerable");

            int elementCount = enumerable.Count();

            T[,] result = new T[elementCount, 1];

            int rowIndex = 0;

            foreach (T value in enumerable)
            {
                result[rowIndex, 0] = value;
                ++rowIndex;
            }

            return result;
        }

        /// <summary>
        /// Creates a single-row two-dimensional array from an enumerable.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">A sequence to be converted to 2D single-row array.</param>
        /// <returns>
        /// A single-row two dimensional array containing the elements of 
        /// <paramref name="enumerable"/> in increasing column order.
        /// </returns>
        public static T[,] To2DArrayRow<T>(this IEnumerable<T> enumerable)
        {
            Contract.Requires<ArgumentNullException>(enumerable != null, "enumerable");

            int elementCount = enumerable.Count();

            T[,] result = new T[1, elementCount];

            int columnIndex = 0;

            foreach (T value in enumerable)
            {
                result[0, columnIndex] = value;
                ++columnIndex;
            }

            return result;
        }

    }
}
