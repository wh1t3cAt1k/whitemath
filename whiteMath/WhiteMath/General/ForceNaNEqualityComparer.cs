using System.Collections.Generic;

namespace WhiteMath.General
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
    public class ForceNaNEqualityComparer<T>: IEqualityComparer<T>
    {
        /// <summary>
        /// Gets the souce equality comparer associated with the current object.
        /// </summary>
        public IEqualityComparer<T> SourceEqualityComparer { get; private set; }

        /// <summary>
        /// Constructs a <see cref="ForceNaNEqualityComparer&lt;T&gt;"/> using
        /// an <see cref="IEqualityComparer&lt;T&gt;"/> comparer object.
        /// </summary>
        /// <param name="sourceEqualityComparer">
        /// An <see cref="IEqualityComparer&lt;T&gt;"/> comparer object
        /// which will be used for equality comparison in every case except
        /// when both compared values are <c>NaN</c>'s (not equal to themselves - in
        /// terms of the parent comparer).
        /// In that case, the constructed comparer will return <c>true</c>.
        /// </param>
		public ForceNaNEqualityComparer(IEqualityComparer<T> sourceEqualityComparer)
        {
            SourceEqualityComparer = sourceEqualityComparer;
        }

        /// <summary>
        /// Compares two <typeparamref name="T"/> objects and returns whatever 
        /// the <see cref="SourceEqualityComparer"/> returns, except for the case when
        /// <see cref="SourceEqualityComparer"/> returns <c>false</c> for 
        /// self-comparison of both <paramref name="first"/> and <paramref name="second"/>.
        /// Since that means that both values are <c>NaN</c> (by definition), the 
        /// comparer will intentionally return <c>true</c> (because that's what it does by design).
        /// </summary>
        /// <param name="first">The first object to be compared for equality.</param>
        /// <param name="second">The second object to be compared for equality.</param>
        /// <returns>
        /// The result of comparison by <see cref="SourceEqualityComparer"/> if at least one of the 
        /// objects is not <c>NaN</c>, or <c>true</c> otherwise.
        /// </returns>
		public bool Equals(T first, T second)
			=> SourceEqualityComparer.Equals(first, second)
				|| (
					!SourceEqualityComparer.Equals(first, first)
					&& !SourceEqualityComparer.Equals(second, second));

        /// <summary>
        /// Returns whatever <see cref="SourceEqualityComparer"/>
        /// returns in the <see cref="GetHashCode"/> for this value.
        /// </summary>
        /// <param name="value">The value whose hash code is to be calculated.</param>
        /// <returns>
        /// Whatever <see cref="SourceEqualityComparer"/> returns in the <see cref="GetHashCode"/> 
        /// for this value.
        /// </returns>
        public int GetHashCode(T value) => SourceEqualityComparer.GetHashCode(value);
    }
}
