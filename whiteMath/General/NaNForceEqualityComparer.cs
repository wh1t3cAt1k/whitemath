using System.Collections.Generic;

namespace whiteMath.General
{
    /// <summary>
    /// Represents a wrapper around an arbitrary equality comparer,
    /// modifying it for the <c>NaN</c> case: the wrapping comparer will
    /// return whatever the underlying comparer returns, except for the
    /// case when both compared objects are <c>NaN</c>s (by definition, not equal to 
    /// anything, even themselves). In this case, the wraping equality comparer
    /// will return <c>true</c>.
    /// </summary>
    /// <typeparam name="T">The type of elements to be compared for equality.</typeparam>
    public class NaNForceEqualityComparer<T>: IEqualityComparer<T>
    {
        /// <summary>
        /// Gets the parent equality comparer associated with
        /// the current object.
        /// </summary>
        public IEqualityComparer<T> ParentEqualityComparer { get; private set; }

        /// <summary>
        /// Constructs a <see cref="NaNForceEqualityComparer&lt;T&gt;"/> using
        /// an <see cref="IEqualityComparer&lt;T&gt;"/> comparer object.
        /// </summary>
        /// <param name="parent">
        /// An <see cref="IEqualityComparer&lt;T&gt;"/> comparer object
        /// which will be used for equality comparison in every case except
        /// when both compared values are <c>NaN</c>'s (not equal to themselves - in
        /// terms of the parent comparer).
        /// In that case, the constructed comparer will return <c>true</c>.
        /// </param>
        public NaNForceEqualityComparer(IEqualityComparer<T> parent)
        {
            this.ParentEqualityComparer = parent;
        }

        /// <summary>
        /// Compares two <typeparamref name="T"/> objects and returns whatever 
        /// the <see cref="ParentEqualityComparer"/> returns, except for the case when
        /// <see cref="ParentEqualityComparer"/> returns <c>false</c> for 
        /// self-comparison of both <paramref name="first"/> and <paramref name="second"/>.
        /// Since that means that both values are <c>NaN</c> (by definition), the 
        /// comparer will intentionally return <c>true</c> (because that's what it does by design).
        /// </summary>
        /// <param name="first">The first object to be compared for equality.</param>
        /// <param name="second">The second object to be compared for equality.</param>
        /// <returns>
        /// The result of comparison by <see cref="ParentEqualityComparer"/> if at least one of the 
        /// objects is not <c>NaN</c>, or <c>true</c> otherwise.
        /// </returns>
        public bool Equals(T first, T second)
        {
            if (!this.ParentEqualityComparer.Equals(first, first) &&
                !this.ParentEqualityComparer.Equals(second, second))
            {
                return true;
            }

            return this.ParentEqualityComparer.Equals(first, second);
        }

        /// <summary>
        /// Returns whatever <see cref="ParentEqualityComparer"/>
        /// returns in the <see cref="GetHashCode"/> for this value.
        /// </summary>
        /// <param name="value">The value whose hash code is to be calculated.</param>
        /// <returns>
        /// Whatever <see cref="ParentEqualityComparer"/> returns in the <see cref="GetHashCode"/> 
        /// for this value.
        /// </returns>
        public int GetHashCode(T value)
        {
            return this.ParentEqualityComparer.GetHashCode(value);
        }
    }
}
