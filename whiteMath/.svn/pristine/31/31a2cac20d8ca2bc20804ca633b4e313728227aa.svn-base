using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath.Numeric
{
    /// <summary>
    /// This class provides a simple way of checking monotonic increasing 
    /// or decreasing of a discrete stream of values.
    /// </summary>
    /// <typeparam name="T">A numeric type of values in the stream.</typeparam>
    /// <typeparam name="C">A calculator for the <typeparamref name="T"/> type.</typeparam>
    public class StreamMonotonicityChecker<T, C> where C: ICalc<T>, new()
    {
        public enum MonotonicityPropertyType 
        { 
            MonotonicIncrease, 
            MonotonicNonDecrease,
            MonotonicNonIncrease,
            MonotonicDecrease
        }

        /// <summary>
        /// Returns <c>true</c> if the monotonicity property still holds.
        /// </summary>
        public bool MonotonicityPropertyHolds { get; private set; }

        /// <summary>
        /// Gets the last value which was passed to the instance of the class
        /// to be checked for not violating monotonicity of the stream.
        /// 
        /// Returns <c>null</c> if no values were passed previously.
        /// </summary>
        public Numeric<T, C>? LastPassedValue { get; private set; }

        /// <summary>
        /// Returns the type of monotonicity property which the current object
        /// was initialized with.
        /// </summary>
        public MonotonicityPropertyType MonotonicityProperty { get; private set; }

        /// <summary>
        /// Constructs an instance of the class using the specified
        /// monotonicity check type.
        /// </summary>
        /// <param name="checkType">
        /// The type of monotonicity check, e.g. check for 
        /// non-decrease of values, increase etc..
        /// </param>
        public StreamMonotonicityChecker(MonotonicityPropertyType checkType)
        {
            this.MonotonicityProperty = checkType;
        }

        /// <summary>
        /// Returns <c>true</c> if, after insertion of the value specified,
        /// the monotonicity property will remain unbroken.
        /// </summary>
        /// <param name="value">The value to be tested.</param>
        /// <param name="insertIfKeeps">
        /// If this flag is <c>true</c>, the value will be automatically inserted into the object
        /// if it doesn't break the monotonicity property.</param>
        /// <returns>
        /// A flag signalizing whether, after insertion of the value specified,
        /// the monotonicity property will remain unbroken.
        /// </returns>
        public bool WillKeepMonotonicity(Numeric<T, C> value, bool insertIfKeeps = true)
        {
            // If the monotonicity property is already broken,
            // then there is no need to even check.
            // -
            if (!this.MonotonicityPropertyHolds)
            {
                return false;
            }

            bool result = true;

            if (LastPassedValue.HasValue)
            {
                if (
                    this.MonotonicityProperty == MonotonicityPropertyType.MonotonicDecrease && 
                    (!(value < this.LastPassedValue.Value))
                    ||
                    this.MonotonicityProperty == MonotonicityPropertyType.MonotonicIncrease && 
                    (!(value > this.LastPassedValue.Value))
                    ||
                    this.MonotonicityProperty == MonotonicityPropertyType.MonotonicNonDecrease &&
                    (value < this.LastPassedValue.Value)
                    ||
                    this.MonotonicityProperty == MonotonicityPropertyType.MonotonicNonIncrease &&
                    (value > this.LastPassedValue.Value))
                {
                    result = false;
                }
            }

            if (result && insertIfKeeps)
            {
                this.LastPassedValue = value;
            }

            return result;
        }

        /// <summary>
        /// Inserts a value into the checker and returns the flag signalizing
        /// whether the monotonicity property still holds.
        /// </summary>
        /// <param name="value">The value to be inserted.</param>
        /// <returns>True if, after the insertion of the value passed, the monotonicity property still holds.</returns>
        public bool InsertValue(Numeric<T, C> value)
        {
            // Checking the condition already has a side-effect of 
            // inserting the value if it keeps the monotonicity property.
            // -
            if (!this.WillKeepMonotonicity(value, true))
            {
                // If it doesn't, we still insert it,
                // but the monotonicity property is broken forever.
                // -
                this.MonotonicityPropertyHolds = false;
                this.LastPassedValue = value;

                return false;
            }

            return true;
        }
    }
}
