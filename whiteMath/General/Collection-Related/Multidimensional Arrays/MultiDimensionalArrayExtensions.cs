using System;
using System.Collections.Generic;
using System.Linq;

using System.Diagnostics.Contracts;

namespace whiteMath.General
{
    /// <summary>
    /// This class provides extension methods for multidimensional arrays.
    /// </summary>
    [ContractVerification(true)]
    public static class MultiDimensionalArrayExtensions
    {
        // -------------------------------------
        // ---- COMPARING ARRAYS ---------------
        // -------------------------------------

        /// <summary>
        /// Returns whether two arrays are
        /// equal in size within each dimension.
        /// </summary>
        /// <param name="matrix">The first array.</param>
        /// <param name="another">The second array.</param>
        /// <returns><c>true</c> if the sizes are equal, <c>false</c> otherwise.</returns>
        public static bool IsSizeEqualTo(this Array first, Array second)
        {
            Contract.Requires<ArgumentNullException>(first != null, "first");
            Contract.Requires<ArgumentNullException>(second != null, "second");

            return 
                first.Rank == second.Rank &&
                Enumerable
                    // For all integers in [0, first.Rank)
                    // -
                    .Range(0, first.Rank)
                    // Dimension sizes should be equal.
                    // -
                    .All(dimensionNumber => (first.GetLength(dimensionNumber) == second.GetLength(dimensionNumber)));
        }

        /// <summary>
        /// Returns whether the two arrays are
        /// of the same size and contain equal elements
        /// for every combination of indices. 
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="first">The first array.</param>
        /// <param name="second">The second array.</param>
        /// <param name="equalityComparer">
        /// An equality comparer for the type <typeparamref name="T"/>.
        /// If <c>null</c>, a default comparer will be used.
        /// </param>
        /// <param name="NaNEqualsNaN">
        /// If this flag is set to <c>true</c>, then any two <c>NaN</c> values
        /// (i.e. values not equal to themselves in terms of <see cref="equalityComparer"/>)
        /// WILL be considered equal even though it is against the IEEE standard. 
        /// </param>
        /// <returns>
        /// <c>true</c> if two arrays are of the same size and contain equal
        /// elements for every combination of indices, <c>false</c> otherwise.
        /// </returns>
        [Pure]
        public static bool IsElementwiseEqualTo<T>(
            this Array first, 
            Array second, 
            IEqualityComparer<T> equalityComparer = null,
            bool NaNEqualsNaN = false)
        {
            Contract.Requires<ArgumentNullException>(first != null, "first");
            Contract.Requires<ArgumentNullException>(second != null, "second");

            if (equalityComparer == null)
            {
                equalityComparer = EqualityComparer<T>.Default;
            }

            if (NaNEqualsNaN)
            {
                equalityComparer = new NaNForceEqualityComparer<T>(equalityComparer);
            }

            return
                first.IsSizeEqualTo(second) &&
                first.Cast<T>().SequenceEqual(second.Cast<T>(), equalityComparer);
        }
    }
}
