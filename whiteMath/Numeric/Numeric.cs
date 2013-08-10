using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using whiteMath.General;

namespace whiteMath
{
    public struct Numeric<T, C>: IComparable<Numeric<T,C>>, IEquatable<Numeric<T,C>> where C:ICalc<T>, new()
    {
        private T value;                     // the value within the number
        private static C calc = new C();     // the calculator used with numbers

        /// <summary>
        /// Returns the calculator for the instantiated numeric class.
        /// </summary>
        public static C Calculator { get { return calc; } }

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
        /// <see cref="ICalc&lt;T&gt;.intPart"/>
        public Numeric<T, C> IntegerPart
        {
            get { return calc.intPart(this); }
        }

        /// <summary>
        /// Returns the fractional part for the current Numeric object.
        /// </summary>
        /// <see cref="ICalcExtensionMethods.fracPart&lt;T&gt;"/>
        public Numeric<T, C> FractionalPart
        {
            get { return calc.fracPart(this); }
        }

        // --------------------------------------
        // --------------------------------------
        // --------------------------------------

        /// <summary>
        /// Gets a deep copy of current number.
        /// </summary>
        public Numeric<T, C> Copy { get { return calc.getCopy(this); } }

        /// <summary>
        /// Gets a zero number for T type.
        /// </summary>
        public static Numeric<T, C> Zero { get { return calc.zero; } }

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
        public bool Even { get { return calc.isEven(this.value); } }


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
            return calc.sum(one, two);
        }

        public static Numeric<T, C> operator -(Numeric<T, C> one, Numeric<T, C> two)
        {
            return calc.dif(one, two);
        }

        public static Numeric<T, C> operator *(Numeric<T, C> one, Numeric<T, C> two)
        {
            return calc.mul(one, two);
        }

        public static Numeric<T, C> operator /(Numeric<T, C> one, Numeric<T, C> two)
        {
            return calc.div(one, two);
        }

        public static Numeric<T, C> operator %(Numeric<T, C> one, Numeric<T, C> two)
        {
            return calc.rem(one, two);
        }

        public static Numeric<T, C> operator -(Numeric<T, C> one)
        {
            return calc.negate(one);
        }

        public static Numeric<T, C> operator ++(Numeric<T, C> one)
        {
            return calc.increment(one);
        }

        public static Numeric<T, C> operator --(Numeric<T, C> one)
        {
            return calc.decrement(one);
        }

        // --------------------------------------
        // ---------------- NaN and Infinities checking
        // --------------------------------------

        public static bool isNaN(Numeric<T, C> one)
        {
            return calc.isNaN(one);
        }

        public static bool isPositiveInfinity(Numeric<T, C> one)
        {
            return calc.isPosInf(one);
        }

        public static bool isNegativeInfinity(Numeric<T, C> one)
        {
            return calc.isNegInf(one);
        }

        public static bool isInfinity(Numeric<T, C> one)
        {
            return calc.isPosInf(one) || calc.isNegInf(one);
        }

        // --------------------------------------
        // ---------------- Conversion operators
        // --------------------------------------

        public static explicit operator Numeric<T, C>(long num)
        {
            return calc.fromInt(num);
        }

        public static explicit operator Numeric<T, C>(double num)
        {
            return calc.fromDoubleSafe(num);
        }

        // --------------------------------------
        // ---------------- Comparison operators
        // --------------------------------------

        public static bool operator >(Numeric<T, C> one, Numeric<T, C> two)
        {
            return calc.mor(one, two);
            // directly
        }

        public static bool operator <(Numeric<T, C> one, Numeric<T, C> two)
        {
            return calc.mor(two, one);
            // a<b <==> b>a
        }

        public static bool operator ==(Numeric<T, C> one, Numeric<T, C> two)
        {
            return calc.eqv(one, two);
            // directly
        }

        public static bool operator !=(Numeric<T, C> one, Numeric<T, C> two)
        {
            return !calc.eqv(one, two);
            // a!=b  <==> !(a==b)
        }

        public static bool operator >=(Numeric<T, C> one, Numeric<T, C> two)
        {
            return !calc.mor(two, one);
            // a>=b  <==> !(b>a)
        }

        public static bool operator <=(Numeric<T, C> one, Numeric<T, C> two)
        {
            return !calc.mor(one, two);
            // a<=b  <==> !(a>b)
        }

        // Equals, hashcode and toString

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
                return calc.eqv(this.value, (T)obj);
            
            else if (obj is Numeric<T,C>)
                return calc.eqv(this.value, ((Numeric<T,C>)obj).value);
            
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
            return calc.eqv(this.value, obj.value);
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
        private static readonly Numeric<T, C> ___2     = __CONST_1 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_3     = ___2 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_4     = __CONST_3 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_5     = __CONST_4 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_6     = __CONST_5 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_7     = __CONST_6 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_8     = __CONST_7 + __CONST_1;
        private static readonly Numeric<T, C> __CONST_9     = __CONST_8 + __CONST_1;

        public static Numeric<T, C> _0 { get { return calc.zero; } }
        public static Numeric<T, C> _1 { get { return __CONST_1.Copy; } }
        public static Numeric<T, C> _2 { get { return ___2.Copy; } }
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
            return _compinstnc.Invoke(this, another);
        }

        // Members

        private static Comparison<Numeric<T, C>> _compinstnc = 
            delegate(Numeric<T, C> one, Numeric<T, C> two)
                {
                    if (one < two)
                        return -1;
                    else if (one == two)
                        return 0;
                    else return 1;
                };

        private static IComparer<Numeric<T, C>> _comparerinstnc = _compinstnc.CreateComparer();

        // ------------ для T -------------------------

        private static Comparison<T> _tcompinstnc =
            delegate(T one, T two)
            {
                if (calc.mor(one, two))
                    return 1;
                else if (calc.eqv(one, two))
                    return 0;
                else
                    return -1;
            };

        private static IComparer<T> _tcomparerinstnc = _tcompinstnc.CreateComparer();

        // -----------
        // Getters
        // -----------

        /// <summary>
        /// Gets the comparison delegate for this numeric class
        /// implicitly using calculator's methods mor() and eqv().
        /// </summary>
        public static Comparison<Numeric<T, C>> NumericComparison
        {
            get { return _compinstnc; }
        }

        /// <summary>
        /// Gets the comparison delegate for the underlying (hidden)
        /// T type using its calculator's methods mor() and eqv().
        /// </summary>
        public static Comparison<T> TComparison
        {
            get { return _tcompinstnc; }
        }

        /// <summary>
        /// Gets the comparer for this numeric class.
        /// implicitly using calculator's methods mor() and eqv().
        /// </summary>
        public static IComparer<Numeric<T, C>> NumericComparer
        {
            get { return _comparerinstnc; }
        }

        /// <summary>
        /// Gets the comparer for the underlying (hidden)
        /// T type using its calculator's methods mor() and eqv().
        /// </summary>
        public static IComparer<T> TComparer
        {
            get { return _tcomparerinstnc; }
        }

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
        /// <typeparam name="C">The calculator for the number type.</typeparam>
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
        /// <typeparam name="C">The calculator for the number type.</typeparam>
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
