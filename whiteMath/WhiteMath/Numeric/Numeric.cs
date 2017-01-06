using System;
using System.Collections.Generic;

using WhiteMath.Calculators;
using WhiteMath.General;

namespace WhiteMath
{
    public struct Numeric<T, C> : IComparable<Numeric<T, C>>, IEquatable<Numeric<T, C>> 
		where C : ICalc<T>, new()
    {
		private readonly T _value;

		/// <summary>
		/// Returns the Calculator for the instantiated numeric class.
		/// </summary>
		public static C Calculator { get; private set; } = new C();

		private Numeric(T value)
        {
            this._value = value;
        }

		// --------------------------------------
		// --------------------------------------
		// --------------------------------------

		/// <summary>
		/// Returns the integer part for the current Numeric object.
		/// </summary>
		/// <see cref="ICalc&lt;T&gt;.IntegerPart"/>
		public Numeric<T, C> IntegerPart => Calculator.IntegerPart(this);

		/// <summary>
		/// Returns the fractional part for the current Numeric object.
		/// </summary>
		/// <see cref="ICalcExtensionMethods.FractionalPart&lt;T&gt;"/>
		public Numeric<T, C> FractionalPart => Calculator.FractionalPart(this);

		// --------------------------------------
		// --------------------------------------
		// --------------------------------------

		/// <summary>
		/// Gets a deep copy of current number.
		/// </summary>
		public Numeric<T, C> Copy => Calculator.GetCopy(this);

        /// <summary>
        /// Gets a zero number for T type.
        /// </summary>
        public static Numeric<T, C> Zero => Calculator.Zero;

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
		public bool IsEven => Calculator.IsEven(this);

		// --------------------------------------
		// ---------CONVERSION OPERATORS---------
		// --------------------------------------

		public static implicit operator Numeric<T, C>(T value)
			=> new Numeric<T, C>(value);

		public static implicit operator T(Numeric<T, C> number)
			=> number._value;

		public static Numeric<T, C> operator +(Numeric<T, C> first, Numeric<T, C> second)
			=> Calculator.Add(first, second);

		public static Numeric<T, C> operator -(Numeric<T, C> first, Numeric<T, C> second)
			=> Calculator.Subtract(first, second);

		public static Numeric<T, C> operator *(Numeric<T, C> first, Numeric<T, C> second)
			=> Calculator.Multiply(first, second);

		public static Numeric<T, C> operator /(Numeric<T, C> first, Numeric<T, C> second)
			=> Calculator.Divide(first, second);

		public static Numeric<T, C> operator %(Numeric<T, C> first, Numeric<T, C> second)
			=> Calculator.Modulo(first, second);

		public static Numeric<T, C> operator -(Numeric<T, C> number)
			=> Calculator.Negate(number);

		public static Numeric<T, C> operator ++(Numeric<T, C> number)
			=> Calculator.Increment(number);

		public static Numeric<T, C> operator --(Numeric<T, C> number)
			=> Calculator.Decrement(number);

		public static bool IsNaN(Numeric<T, C> number)
			=> Calculator.IsNaN(number);

		public static bool IsPositiveInfinity(Numeric<T, C> number)
			=> Calculator.IsPositiveInfinity(number);
      
		public static bool IsNegativeInfinity(Numeric<T, C> number)
			=> Calculator.IsNegativeInfinity(number);

		public static bool IsInfinity(Numeric<T, C> number)
			=> Calculator.IsInfinity(number);

		public static explicit operator Numeric<T, C>(long number)
			=> Calculator.FromInteger(number);

		public static explicit operator Numeric<T, C>(double number)
			=> Calculator.FromDoubleSafe(number);

		public static bool operator >(Numeric<T, C> first, Numeric<T, C> second)
        	=> Calculator.GreaterThan(first, second);

		public static bool operator <(Numeric<T, C> first, Numeric<T, C> second)
			=> Calculator.GreaterThan(second, first);

		public static bool operator ==(Numeric<T, C> first, Numeric<T, C> second)
			=> Calculator.Equal(first, second);

		public static bool operator !=(Numeric<T, C> first, Numeric<T, C> second)
			=> !Calculator.Equal(first, second);

		public static bool operator >=(Numeric<T, C> first, Numeric<T, C> second)
			=> !Calculator.GreaterThan(second, first);

		public static bool operator <=(Numeric<T, C> first, Numeric<T, C> second)
			=> !Calculator.GreaterThan(first, second);

        /// <summary>
        /// Checks whether the value of the current <c>Numeric&lt;<typeparamref name="T"/>,<typeparamref name="C"/>&gt;</c> 
        /// object is equal to the value of another <c>Numeric&lt;<typeparamref name="T"/>,<typeparamref name="C"/>&gt;</c> object or
        /// <typeparamref name="T"/> object.
        /// </summary>
        /// <param name="obj">The object to test for equality with.</param>
        /// <returns>True if values are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
			if (obj is T)
			{
				return Calculator.Equal(this._value, (T)obj);
			}
			else if (obj is Numeric<T, C>)
			{
				return Calculator.Equal(this._value, ((Numeric<T, C>)obj)._value);
			}
			else
			{
				return false;
			}
        }

        /// <summary>
        /// Checks whether the value of the current <c>Numeric&lt;<typeparamref name="T"/>,<typeparamref name="C"/>&gt;</c>
        /// object is equal to the value of another <c>Numeric&lt;<typeparamref name="T"/>,<typeparamref name="C"/>&gt;</c> object.
        /// </summary>
        /// <param name="obj">The object to test for equality with.</param>
        /// <returns>True if values are equal, false otherwise.</returns>
        public bool Equals(Numeric<T, C> obj)
        {
            return Calculator.Equal(this._value, obj._value);
        }

		/// <summary>
		/// Returns the hash code of the value stored.
		/// </summary>
		/// <returns>The hash code of the value stored.</returns>
		public override int GetHashCode() => _value.GetHashCode();

        /// <summary>
        /// Returns the standard string representation of the value stored.
        /// </summary>
        /// <returns>The string representation of the value stored.</returns>
        public override string ToString()
        {
            return _value.ToString();
        }

		#region Constants

        private static readonly Numeric<T, C> __CONST_1 = (Numeric<T, C>)1;
		private static readonly Numeric<T, C> __CONST_2 = __CONST_1 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_3 = __CONST_2 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_4 = __CONST_3 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_5 = __CONST_4 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_6 = __CONST_5 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_7 = __CONST_6 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_8 = __CONST_7 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_9 = __CONST_8 + __CONST_1;

		public static Numeric<T, C> _0 => Calculator.Zero;
		public static Numeric<T, C> _1 => __CONST_1.Copy;
		public static Numeric<T, C> _2 => __CONST_2.Copy;
		public static Numeric<T, C> _3 => __CONST_3.Copy;
        public static Numeric<T, C> _4 => __CONST_4.Copy;
        public static Numeric<T, C> _5 => __CONST_5.Copy;
        public static Numeric<T, C> _6 => __CONST_6.Copy;
		public static Numeric<T, C> _7 => __CONST_7.Copy;
        public static Numeric<T, C> _8 => __CONST_8.Copy;
        public static Numeric<T, C> _9 => __CONST_9.Copy;
        
		#endregion

        // ----------------------------------
        // ---------- COMPARING -------------
        // ----------------------------------

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

		private static Comparison<Numeric<T, C>> _comparison = (one, two) =>
		{
        	if (one < two) return -1;
			if (one == two) return 0;
            
			return 1;
        };

		private static IComparer<Numeric<T, C>> _comparer = _comparison.CreateComparer();

		private static Comparison<T> _underlyingTypeComparison =
            delegate(T one, T two)
            {
                if (Calculator.GreaterThan(one, two))
                    return 1;
                else if (Calculator.Equal(one, two))
                    return 0;
                else
                    return -1;
            };

		private static IComparer<T> _underlyingTypeComparer = _underlyingTypeComparison.CreateComparer();

		/// <summary>
		/// Gets the comparison delegate for this numeric class
		/// implicitly using Calculator's methods mor() and eqv().
		/// </summary>
		public static Comparison<Numeric<T, C>> NumericComparison => _comparison;

		/// <summary>
		/// Gets the comparison delegate for the underlying (hidden)
		/// T type using its Calculator's methods mor() and eqv().
		/// </summary>
		public static Comparison<T> UnderlyingTypeComparison => _underlyingTypeComparison;

		/// <summary>
		/// Gets the comparer for this numeric class.
		/// implicitly using Calculator's methods mor() and eqv().
		/// </summary>
		public static IComparer<Numeric<T, C>> NumericComparer => _comparer;

		/// <summary>
		/// Gets the comparer for the underlying (hidden)
		/// T type using its Calculator's methods mor() and eqv().
		/// </summary>
		public static IComparer<T> UnderlyingTypeComparer => _underlyingTypeComparer;
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
        public static Numeric<T, C>[] ConvertToNumericArray<T, C>(this IList<T> list) 
			where C: ICalc<T>, new()
        {
			Numeric<T, C>[] result = new Numeric<T, C>[list.Count];

			for (int elementIndex = 0; elementIndex < list.Count; ++elementIndex)
			{
				result[elementIndex] = list[elementIndex];
			}

            return result;
        }

        /// <summary>
        /// Converts a list of Numeric objects to the equivalent array of unboxed T objects.
        /// </summary>
        /// <typeparam name="T">The type of numbers in array.</typeparam>
        /// <typeparam name="C">The Calculator for the number type.</typeparam>
        /// <param name="list">The list object containing Numeric elements.</param>
        /// <returns>An array of T objects.</returns>
		public static T[] ConvertToUnderlyingTypeArray<T, C>(this IList<Numeric<T, C>> list) 
			where C : ICalc<T>, new()
        {
			T[] result = new T[list.Count];

			for (int elementIndex = 0; elementIndex < result.Length; ++elementIndex)
			{
				result[elementIndex] = list[elementIndex];
			}

            return result;
        }
    }
}
