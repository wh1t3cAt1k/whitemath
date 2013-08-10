using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics.Contracts;

namespace whiteMath.General
{
    /// <summary>
    /// Represents a structure which is returned from the functions
    /// which can succeed in calculating some value or fail to do that.
    /// </summary>
    /// <typeparam name="T">The type of result which can be potentially calculated by a function.</typeparam>
    [ContractVerification(true)]
    public struct PotentialResult<T>
    {
        /// <summary>
        /// Gets a flag which signalizes whether the <see cref="Value"/> of the 
        /// current object was calculated successfully and can be used.
        /// </summary>
        public bool Success { get; private set; }

        private T ___value;

        /// <summary>
        /// Gets the value calculated by the function.
        /// If <see cref="Success"/> field is <c>false</c>, this
        /// property will throw an <see cref="InvalidOperationException"/>.
        /// </summary>
        public T Value 
        {
            get 
            {
                Contract.Requires<InvalidOperationException>(this.Success);

                return 
                    ___value;
            }
        }

        /// <summary>
        /// Creates a <see cref="PotentialResult&lt;T&gt;"/> object
        /// from a successfully calculated value.
        /// </summary>
        /// <param name="value">The value to store in the <see cref="PotentialResult&lt;T&gt;"/>.</param>
        /// <returns>A <see cref="PotentialResult&lt;T&gt;"/> object with a usable value and <c>Success</c> field set to <c>true</c>.</returns>
        public static PotentialResult<T> CreateSuccess(T value)
        {
            PotentialResult<T> res = new PotentialResult<T>();

            res.___value = value;
            res.Success = true;

            return res;
        }

        /// <summary>
        /// Creates a <see cref="PotentialResult&lt;T&gt;"/> object
        /// which signalizes a failure.
        /// </summary>
        /// <returns>A <see cref="PotentialResult&lt;T&gt;"/> object with a non-usable value and <c>Success</c> field set to <c>false</c>.</returns>
        public static PotentialResult<T> CreateFailure()
        {
            return new PotentialResult<T>();
        }
    }
}
