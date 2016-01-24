using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using whiteStructs.Conditions;

namespace whiteMath.General
{
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
            StringBuilder res = new StringBuilder();

            foreach (T obj in sequence)
                res.Append(String.Format("{0}{1}", separator, (toString == null ? obj.ToString() : toString(obj))));

            if (res.Length > 0)
                res.Append(separator);

            return res.ToString();
        }

        /// <summary>
        /// Tests whether a sequence contains no elements.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="sequence">The calling sequence object.</param>
		/// <returns><c>true</c>, if the sequence is empty, <c>false</c> otherwise.</returns>
        public static bool IsEmpty<T>(this IEnumerable<T> sequence)
        {
			Condition.ValidateNotNull(sequence, nameof(sequence));

            return !sequence.GetEnumerator().MoveNext();
        }

		/// <summary>
		/// Determines if the specified sequence contains exactly one element.
		/// </summary>
		/// <returns><c>true</c> if is the specified sequence has exactly one element; otherwise, <c>false</c>.</returns>
		/// <param name="sequence">The calling sequence object.</param>
		public static bool IsSingleton<T>(this IEnumerable<T> sequence)
		{
			Condition.ValidateNotNull(sequence, nameof(sequence));

			IEnumerator<T> enumerator = sequence.GetEnumerator();

			return
				enumerator.MoveNext() && !enumerator.MoveNext();
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
			Condition.ValidateNotNull(sequence, nameof(sequence));
			Condition.ValidateNotEmpty(sequence, Messages.SequenceShouldContainAtLeastOneElement);

            T max;

            IEnumerator<T> enumerator = sequence.GetEnumerator();

			enumerator.MoveNext();
			max = enumerator.Current;

			while (enumerator.MoveNext())
			{
				if (comparer.Compare(enumerator.Current, max) > 0)
				{
					max = enumerator.Current;
				}
			}

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
			Condition.ValidateNotNull(sequence, nameof(sequence));
			Condition.ValidateNotEmpty(sequence, Messages.SequenceShouldContainAtLeastOneElement);

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
        /// Finds the smallest and the largest elements in a sequence.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the sequence.
        /// </typeparam>
        /// <param name="sequence">
        /// A sequence in which the minimal and maximal elements would be found.
        /// </param>
        /// <param name="comparer">
        /// An optional <see cref="IComparer&lt;T&gt;"/> for the <typeparamref name="T"/> type. 
        /// </param>
        /// <returns>
        /// A logical point whose X value is equal to the minimum, and Y value is equal to the maximum.</returns>
        public static Point<T> MinMax<T>(this IEnumerable<T> sequence, IComparer<T> comparer = null)
        {
			Condition.ValidateNotNull(sequence, nameof(sequence));
			Condition.ValidateNotEmpty(sequence, Messages.SequenceShouldContainAtLeastOneElement);

            if (comparer == null)
            {
                comparer = Comparer<T>.Default;
            }

            T min;
            T max;

            IEnumerator<T> enumerator = sequence.GetEnumerator();

			enumerator.MoveNext();
			min = max = enumerator.Current;
            
            while (enumerator.MoveNext())
            {
				if (comparer.Compare(enumerator.Current, min) < 0)
				{
					min = enumerator.Current;
				}
				else if (comparer.Compare(enumerator.Current, max) > 0)
				{
					max = enumerator.Current;
				}
            }

            return new Point<T>(min, max);
        }

		/// <summary>
		/// Performs an action upon each element of a sequence.
		/// </summary>
		/// <param name="sequence">
		/// A sequence whose elements will be each passed as an 
		/// argument to the action.
		/// </param>
		/// <param name="action">
		/// An action to be performed upon each element of the 
		/// incoming sequence.
		/// </param>
		/// <typeparam name="T">
		/// The type of elements in the sequence.
		/// </typeparam>
		public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
		{
			foreach (T element in sequence)
			{
				action(element);
			}
		}

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
			Condition.ValidateNotNull(enumerable, nameof(enumerable));

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
			Condition.ValidateNotNull(enumerable, nameof(enumerable));

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
