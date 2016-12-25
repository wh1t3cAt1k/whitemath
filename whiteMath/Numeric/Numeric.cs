using System;
using System.Collections.Generic;

using whiteMath.Calculators;
using whiteMath.General;

namespace whiteMath
{
    public struct Numeric<T, C> : IComparable<Numeric<T, C>>, IEquatable<Numeric<T, C>> 
		where C : ICalc<T>, new()
    {
        private T value;

		/// <summary>
		/// Returns the Calculator for the instantiated numeric class.
		/// </summary>
		public static C Calculator { get; private set; } = new C();

		private Numeric(T value)
        {
            this.value = value;
        }

        // --------------------------------------
        // --------------------------------------
        // --------------------------------------

        /// <summary>
        /// Returns the integer part for the current Numeric object.
        /// </summary>
        /// <see cref="ICalc&lt;T&gt;.IntegerPart"/>
        public Numeric<T, C> IntegerPart
        {
            get { return Calculator.IntegerPart(this); }
        }

        /// <summary>
        /// Returns the fractional part for the current Numeric object.
        /// </summary>
        /// <see cref="ICalcExtensionMethods.FractionalPart&lt;T&gt;"/>
        public Numeric<T, C> FractionalPart
        {
            get { return Calculator.FractionalPart(this); }
        }

        // --------------------------------------
        // --------------------------------------
        // --------------------------------------

        /// <summary>
        /// Gets a deep copy of current number.
        /// </summary>
        public Numeric<T, C> Copy { get { return Calculator.GetCopy(this); } }

        /// <summary>
        /// Gets a zero number for T type.
        /// </summary>
        public static Numeric<T, C> Zero { get { return Calculator.Zero; } }

        // --------------------------------------
        // ---------BOOLEAN TESTS ---------------
        // --------------------------------------

        /// <summary>
        /// Tests whether the current number is even.
        /// </summary>
        /// <exception cref="NonIntegerTypeException">
        /// This exception will be thrown if <typeparamref name="T"/> is 
        /// non-integer type.
        /// </exception>
        public bool IsEven { get { return Calculator.IsEven(this.value); } }

        // --------------------------------------
        // ---------CONVERSION OPERATORS---------
        // --------------------------------------

        public static implicit operator Numeric<T, C>(T val)
        {
            return new Numeric<T, C>(val);
        }

        public static implicit operator T(Numeric<T, C> obj)
        {
            return obj.value;
        }

        // --------------------------------------
        // --------------------------------------
        // --------------------------------------

        public static Numeric<T, C> operator +(Numeric<T, C> one, Numeric<T, C> two)
        {
            return Calculator.Add(one, two);
        }

        public static Numeric<T, C> operator -(Numeric<T, C> one, Numeric<T, C> two)
        {
            return Calculator.Subtract(one, two);
        }

        public static Numeric<T, C> operator *(Numeric<T, C> one, Numeric<T, C> two)
        {
            return Calculator.Multiply(one, two);
        }

        public static Numeric<T, C> operator /(Numeric<T, C> one, Numeric<T, C> two)
        {
            return Calculator.Divide(one, two);
        }

        public static Numeric<T, C> operator %(Numeric<T, C> one, Numeric<T, C> two)
        {
            return Calculator.Modulo(one, two);
        }

        public static Numeric<T, C> operator -(Numeric<T, C> one)
        {
            return Calculator.Negate(one);
        }

        public static Numeric<T, C> operator ++(Numeric<T, C> one)
        {
            return Calculator.Increment(one);
        }

        public static Numeric<T, C> operator --(Numeric<T, C> one)
        {
            return Calculator.Decrement(one);
        }

        // --------------------------------------
        // ---------------- NaN and Infinities checking
        // --------------------------------------

        public static bool IsNaN(Numeric<T, C> one)
        {
            return Calculator.IsNaN(one);
        }

		public static bool IsPositiveInfinity(Numeric<T, C> one)
        {
            return Calculator.IsPositiveInfinity(one);
        }

		public static bool IsNegativeInfinity(Numeric<T, C> one)
        {
            return Calculator.IsNegativeInfinity(one);
        }

        public static bool IsInfinity(Numeric<T, C> one)
        {
			return Calculator.IsInfinity(one);
        }

        // --------------------------------------
        // ---------------- Conversion operators
        // --------------------------------------

        public static explicit operator Numeric<T, C>(long num)
        {
            return Calculator.FromInteger(num);
        }

        public static explicit operator Numeric<T, C>(double num)
        {
            return Calculator.FromDoubleSafe(num);
        }

        // --------------------------------------
        // ---------------- Comparison operators
        // --------------------------------------

        public static bool operator >(Numeric<T, C> one, Numeric<T, C> two)
        {
            return Calculator.GreaterThan(one, two);
        }

        public static bool operator <(Numeric<T, C> one, Numeric<T, C> two)
        {
            return Calculator.GreaterThan(two, one);
        }

        public static bool operator ==(Numeric<T, C> one, Numeric<T, C> two)
        {
            return Calculator.Equal(one, two);
        }

        public static bool operator !=(Numeric<T, C> one, Numeric<T, C> two)
        {
            return !Calculator.Equal(one, two);
        }

        public static bool operator >=(Numeric<T, C> one, Numeric<T, C> two)
        {
            return !Calculator.GreaterThan(two, one);
        }

        public static bool operator <=(Numeric<T, C> one, Numeric<T, C> two)
        {
            return !Calculator.GreaterThan(one, two);
        }

        /// <summary>
        /// Checks whether the value of the current <c>Numeric&lt;<typeparamref name="T"/>,<typeparamref name="C"/>&gt;</c> 
        /// object is equal to the value of another <c>Numeric&lt;<typeparamref name="T"/>,<typeparamref name="C"/>&gt;</c> object or
        /// <typeparamref name="T"/> object.
        /// </summary>
        /// <param name="obj">The object to test for equality with.</param>
        /// <returns>True if values are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
			if (obj is T) {
				return Calculator.Equal (this.value, (T)obj);
			} else if (obj is Numeric<T,C>)
                return Calculator.Equal(this.value, ((Numeric<T,C>)obj).value);
            
            return false;
        }

        /// <summary>
        /// Checks whether the value of the current <c>Numeric&lt;<typeparamref name="T"/>,<typeparamref name="C"/>&gt;</c>
        /// object is equal to the value of another <c>Numeric&lt;<typeparamref name="T"/>,<typeparamref name="C"/>&gt;</c> object.
        /// </summary>
        /// <param name="obj">The object to test for equality with.</param>
        /// <returns>True if values are equal, false otherwise.</returns>
        public bool Equals(Numeric<T, C> obj)
        {
            return Calculator.Equal(this.value, obj.value);
        }

        /// <summary>
        /// Returns the hash code of the value stored.
        /// </summary>
        /// <returns>The hash code of the value stored.</returns>
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        /// <summary>
        /// Returns the standard string representation of the value stored.
        /// </summary>
        /// <returns>The string representation of the value stored.</returns>
        public override string ToString()
        {
            return value.ToString();
        }

        // ----------------------------------
        // ---------- SOME OF THE CONSTANTS -
        // ----------------------------------

        private static readonly Numeric<T, C> __CONST_1     = (Numeric<T,C>)1;
		private static readonly Numeric<T, C> __CONST_2     = __CONST_1 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_3     = __CONST_2 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_4     = __CONST_3 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_5     = __CONST_4 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_6     = __CONST_5 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_7     = __CONST_6 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_8     = __CONST_7 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_9     = __CONST_8 + __CONST_1;

        public static Numeric<T, C> _0 { get { return Calculator.Zero; } }
        public static Numeric<T, C> _1 { get { return __CONST_1.Copy; } }
        public static Numeric<T, C> _2 { get { return __CONST_2.Copy; } }
        public static Numeric<T, C> _3 { get { return __CONST_3.Copy; } }
        public static Numeric<T, C> _4 { get { return __CONST_4.Copy; } }
        public static Numeric<T, C> _5 { get { return __CONST_5.Copy; } }
        public static Numeric<T, C> _6 { get { return __CONST_6.Copy; } }
        public static Numeric<T, C> _7 { get { return __CONST_7.Copy; } }
        public static Numeric<T, C> _8 { get { return __CONST_8.Copy; } }
        public static Numeric<T, C> _9 { get { return __CONST_9.Copy; } }
        
        // ----------------------------------
        // ---------- COMPARING -------------
        // ----------------------------------

        // IComparable

        /// <summary>
        /// Compares the current number to another.
        /// </summary>
        /// <param name="another">A value for the current number to compare with.</param>
        /// <returns>
        /// A positive value if the current number is bigger than the number passed,
        /// a zero value if they are equal, and a negative value if the second number is bigger.
        /// </returns>
        public int CompareTo(Numeric<T, C> another)
        {
            return _comparison(this, another);
        }

        // Members

		private static Comparison<Numeric<T, C>> _comparison = (one, two) =>
		{
        	if (one < two) return -1;
			if (one == two) return 0;
            
			return 1;
        };

		private static IComparer<Numeric<T, C>> _comparer = _comparison.CreateComparer();

        // ------------ для T -------------------------

		private static Comparison<T> _rawComparison =
            delegate(T one, T two)
            {
                if (Calculator.GreaterThan(one, two))
                    return 1;
                else if (Calculator.Equal(one, two))
                    return 0;
                else
                    return -1;
            };

		private static IComparer<T> _rawComparer = _rawComparison.CreateComparer();

		// -----------
		// Getters
		// -----------

		/// <summary>
		/// Gets the comparison delegate for this numeric class
		/// implicitly using Calculator's methods mor() and eqv().
		/// </summary>
		public static Comparison<Numeric<T, C>> NumericComparison => _comparison;

		/// <summary>
		/// Gets the comparison delegate for the underlying (hidden)
		/// T type using its Calculator's methods mor() and eqv().
		/// </summary>
		public static Comparison<T> TComparison => _rawComparison;

		/// <summary>
		/// Gets the comparer for this numeric class.
		/// implicitly using Calculator's methods mor() and eqv().
		/// </summary>
		public static IComparer<Numeric<T, C>> NumericComparer => _comparer;

		/// <summary>
		/// Gets the comparer for the underlying (hidden)
		/// T type using its Calculator's methods mor() and eqv().
		/// </summary>
		public static IComparer<T> TComparer => _rawComparer;
    }

    /// <summary>
    /// This class provides static methods for numeric array conversion.
    /// </summary>
    public static class NumericExtensions
    {
        /// <summary>
        /// Converts a list of T to the array of Numeric objects.
        /// </summary>
        /// <typeparam name="T">The type of numbers in Numeric objects.</typeparam>
        /// <typeparam name="C">The Calculator for the number type.</typeparam>
        /// <param name="list">The list object containint T elements.</param>
        /// <returns>An array of Numeric objects.</returns>
        public static Numeric<T, C>[] ConvertToNumericArray<T, C>(this IList<T> list) where C: ICalc<T>, new()
        {
            Numeric<T, C>[] arr = new Numeric<T, C>[list.Count];

            for (int i = 0; i < list.Count; i++)
                arr[i] = list[i];

            return arr;
        }

        /// <summary>
        /// Converts a list of Numeric objects to the equivalent array of unboxed T objects.
        /// </summary>
        /// <typeparam name="T">The type of numbers in array.</typeparam>
        /// <typeparam name="C">The Calculator for the number type.</typeparam>
        /// <param name="list">The list object containing Numeric elements.</param>
        /// <returns>An array of T objects.</returns>
        public static T[] ConvertToUnboxedArray<T, C>(this IList<Numeric<T, C>> list) where C : ICalc<T>, new()
        {
            T[] arr = new T[list.Count];

            for (int i = 0; i < arr.Length; i++)
                arr[i] = list[i];

            return arr;
        }
    }
}
