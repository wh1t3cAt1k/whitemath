using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace whiteMath.General
{
    public static class OtherExtensions
    {
        /// <summary>
        /// Transforms the <c>RectangleF</c> object into a <c>Rectangle</c> by rounding
        /// coordinates of its upper-left point and the size to nearby integer values.
        /// </summary>
        /// <param name="obj">A <c>RectangleF</c> object to transform.</param>
        /// <returns>A <c>Rectangle</c> object with rounded coordinates and size.</returns>
        /// <see cref="Rectangle"/>
        public static Rectangle AsRectangle(this RectangleF obj)
        {
            return new Rectangle(
                (int)Math.Round(obj.X, MidpointRounding.AwayFromZero),
                (int)Math.Round(obj.Y, MidpointRounding.AwayFromZero),
                (int)Math.Round(obj.Width, MidpointRounding.AwayFromZero),
                (int)Math.Round(obj.Height, MidpointRounding.AwayFromZero));
        }

        // --------------------------------------------
        // --------- Integer toStrings ----------------

        public static string ToBitString(this long num, bool appendToWholeBytes = true)
        {
            if (num == 0)
            {
                if (appendToWholeBytes)
                    return "|00000000|";
                else
                    return "|0|";
            }

            string str = "|";
            int counter = 0;

            while (num != 0)
            {
                str += (num & 1).ToString();
                num >>= 1;

                counter++;

                if (counter % 8 == 0)
                {
                    counter = 0;
                    str += "|";
                }
            }

            if(counter != 0)
            {
                for (int i = 0; i < 8 - counter; i++)
                    str += "0";

                str += "|";
            }

            return str;
        }

        // --------------------------------------------
        // --------- Different wrapper comparers ------
        // --------------------------------------------

        // --------- LinkedList Nodes comparer --------

        /// <summary>
        /// Creates a comparer for linked list nodes basing on the
        /// comparer for linked list node values.
        /// </summary>
        /// <typeparam name="T">The type of linked list node values.</typeparam>
        /// <param name="valueComparer">The comparer for linked list node values/</param>
        /// <returns>The comparer for linked list nodes basing on the comparer for linked list node values.</returns>
        public static IComparer<LinkedListNode<T>> GetLinkedListNodeComparer<T>(this IComparer<T> valueComparer)
        {
            return new _LLNComparer<T>(valueComparer);
        }

        /// <summary>
        /// Класс, который создает компаратор для узлов связного списка
        /// на основе компаратора для значений этих узлов.
        /// </summary>
        private class _LLNComparer<T> : IComparer<LinkedListNode<T>>
        {
            private IComparer<T> comparer;      // comparer для values

            public _LLNComparer(IComparer<T> comparer)
            {
                this.comparer = comparer;
            }

            public int Compare(LinkedListNode<T> one, LinkedListNode<T> two)
            {
                return comparer.Compare(one.Value, two.Value);
            }
        }

        // --------- KVP comparer ---------------------

        /// <summary>
        /// Creates a KeyValuePair comparer on the basis of provided key comparer object.
        /// </summary>
        /// <typeparam name="TKey">The type of key objects.</typeparam>
        /// <typeparam name="TVal">The type of value objects.</typeparam>
        /// <param name="keyComparer">The comparer for the key type.</param>
        /// <returns>An IComparer for key-value pairs on the basis of provided key comparer object.</returns>
        public static IComparer<KeyValuePair<TKey, TVal>> createKVPComparerOnKey<TKey, TVal>(this IComparer<TKey> keyComparer)
        {
            return new _KVPComparerKey<TKey, TVal>(keyComparer);
        }

        /// <summary>
        /// Creates a KeyValuePair comparer on the basis of provided value comparer object.
        /// </summary>
        /// <typeparam name="TKey">The type of key objects.</typeparam>
        /// <typeparam name="TVal">The type of value objects.</typeparam>
        /// <param name="valueComparer">The comparer for the value type.</param>
        /// <returns>An IComparer for key-value pairs on the basis of provided value comparer object.</returns>
        public static IComparer<KeyValuePair<TKey, TVal>> createKVPComparerOnValue<TKey, TVal>(this IComparer<TVal> valueComparer)
        {
            return new _KVPComparerValue<TKey, TVal>(valueComparer);
        }

        /// <summary>
        /// Класс, который создает компаратор пар ключ-значение
        /// на основе компаратора ключей.
        /// </summary>
        private class _KVPComparerKey<TKey, TVal> : IComparer<KeyValuePair<TKey, TVal>>
        {
            private IComparer<TKey> keyComparer;

            internal _KVPComparerKey(IComparer<TKey> keyComparer)
            {
                this.keyComparer = keyComparer;
            }

            public int Compare(KeyValuePair<TKey, TVal> one, KeyValuePair<TKey, TVal> two)
            {
                return keyComparer.Compare(one.Key, two.Key);
            }
        }

        /// <summary>
        /// Класс, который создает компаратор пар ключ-значение
        /// на основе компаратора значений.
        /// </summary>
        private class _KVPComparerValue<TKey, TVal> : IComparer<KeyValuePair<TKey, TVal>>
        {
            private IComparer<TVal> valueComparer;

            internal _KVPComparerValue(IComparer<TVal> valueComparer)
            {
                this.valueComparer = valueComparer;
            }

            public int Compare(KeyValuePair<TKey, TVal> one, KeyValuePair<TKey, TVal> two)
            {
                return valueComparer.Compare(one.Value, two.Value);
            }
        }

        // --------------------------------------------
        // --------- Reverse comparer -----------------
        // --------------------------------------------

        /// <summary>
        /// Returns the reverse comparer for an IComparer() object.
        /// For example, each time the incoming comparer would tell 
        /// that object A is 'more' than object B (returns positive value),
        /// this comparer will tell the opposite and return the same absolute value, but with negative sign.
        /// </summary>
        /// <typeparam name="T">The type of comparable objects.</typeparam>
        /// <param name="comparer">The IComparer(<typeparamref name="T"/>) object used to compare objects of type <typeparamref name="T"/>.</param>
        /// <returns>The reverse comparer for the IComparer(<typeparamref name="T"/>) object passed.</returns>
        public static IComparer<T> GetReverseComparer<T>(this IComparer<T> comparer)
        {
            return new _ReverseComparer<T>(comparer);
        }

        /// <summary>
        /// Класс, создающий обратный компаратор на основе прямого компаратора.
        /// </summary>
        private class _ReverseComparer<T> : IComparer<T>
        {
            private IComparer<T> parent;

            public _ReverseComparer(IComparer<T> parent)
            {
                this.parent = parent;
            }

            public int Compare(T one, T two)
            {
                return -parent.Compare(one, two);
            }
        }

        // --------------------------------------------
        // --------- Comparer from comparison ---------
        // --------------------------------------------

        /// <summary>
        /// Creates an <see cref="IComparer&lt;T&gt;"/> object from a calling <see cref="Comparison&lt;T&gt;"/> object.
        /// </summary>
        /// <typeparam name="T">The type of comparable values.</typeparam>
        /// <param name="comparison">A comparison delegate comparing values of type <typeparamref name="T"/>.</param>
        /// <returns>An <see cref="IComparer&lt;T&gt;"/> equal in functionality to the comparison object passed.</returns>
        public static IComparer<T> CreateComparer<T>(this Comparison<T> comparison)
        {
            return new ComparisonComparer<T>(comparison);
        }

        /// <summary>
        /// Creates an <see cref="IEqualityComparer&lt;T&gt;"/> object from a calling <see cref="Func&lt;T, T, bool&gt;"/> 
        /// equality comparison functor.
        /// </summary>
        /// <typeparam name="T">
        /// The type of comparable values.
        /// </typeparam>
        /// <param name="equalityComparison">
        /// A comparison delegate comparing values of type <typeparamref name="T"/>, returning
        /// <c>true</c> if two objects should be considered equal, and <c>false</c> otherwise.
        /// </param>
        /// <returns>An <see cref="IEqualityComparer&lt;T&gt;"/> equal in functionality to the functor object passed.</returns>
        public static IEqualityComparer<T> CreateEqualityComparer<T>(this Func<T, T, bool> equalityComparison)
        {
            return new FuncEqualityComparer<T>(equalityComparison);
        }

        /// <summary>
        /// Creates an <see cref="EqualityComparer&lt;T&gt;"/> using a
        /// functor object taking two elements of <typeparamref name="T"/>
        /// type (it should return <c>true</c> if they are equal and <c>false</c> otherwise). 
        /// 
        /// Please not that a default implementation of <see cref="GetHashCode"/>
        /// will be used.
        /// </summary>
        /// <typeparam name="T">The type of compared elements.</typeparam>
        public class FuncEqualityComparer<T> : IEqualityComparer<T>
        {
            Func<T, T, bool> inner;

            public FuncEqualityComparer(Func<T, T, bool> inner)
            {
                this.inner = inner;
            }

            public bool Equals(T first, T second)
            {
                return inner(first, second);
            }

            public int GetHashCode(T element)
            {
                return EqualityComparer<T>.Default.GetHashCode();
            }
        }


        /// <summary>
        /// Class comparing two objects basing on the Comparison delegate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class ComparisonComparer<T>: IComparer<T>
        {
            Comparison<T> inner;

            public ComparisonComparer(Comparison<T> inner)
            {
                this.inner = inner;
            }

            public int Compare(T one, T two)
            {
                return inner.Invoke(one, two);
            }
        }

        // ---------------------------------------------------
        // -------------- STRING EXTENSIONS ------------------
        // ---------------------------------------------------

        /// <summary>
        /// Returns the overall number of a particular char occurences in 
        /// the calling string object.
        /// </summary>
        /// <param name="obj">The calling string object for which the count is performed.</param>
        /// <param name="value">The char value to count.</param>
        /// <returns>The overall number of passed char value occurences in the string.</returns>
        public static int CharacterCount(this string obj, char value)
        {
            int occurences = 0;

            for (int i = 0; i < obj.Length; i++)
                if (obj[i] == value)
                    occurences++;

            return occurences;
        }

        /// <summary>
        /// Returns the substring of a calling string object starting with
        /// startIndex and ending with (exclusive) endIndex.
        /// </summary>
        /// <param name="obj">The calling string object to receive substring from.</param>
        /// <param name="startIndex">The inclusive starting index in the original string.</param>
        /// <param name="endIndex">The exclusive ending index in the original string.</param>
        /// <returns></returns>
        public static string SubstringToIndex(this string obj, int startIndex, int endIndex)
        {
            return obj.Substring(startIndex, endIndex - startIndex);
        }
    }
}
