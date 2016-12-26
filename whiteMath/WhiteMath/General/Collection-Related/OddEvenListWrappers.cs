using System;
using System.Collections.Generic;
using System.Collections;

namespace WhiteMath.General
{
    public static class OddEvenListWrappers
    {
        /// <summary>
        /// Gets the list wrapper 'containing' all the odd elements of the parent list.
        /// Provides continuous indexing of odd elements. Does not support any insertion/removal operations.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list</typeparam>
        /// <param name="obj">The parent list to be wrapped</param>
        /// <returns>The list wrapper containing all the odd elements of the parent list</returns>
        public static IList<T> getListOfOdds<T>(this IList<T> obj)
        {
            return new HalfList<T>.OddList(obj);
        }

        /// <summary>
        /// Gets the list wrapper 'containing' all the even elements of the parent list.
        /// Provides continuous indexing of even elements. Does not support any insertion/removal operations.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list</typeparam>
        /// <param name="obj">The parent list to be wrapped</param>
        /// <returns>The list wrapper containing all the odd elements of the parent list</returns>
        public static IList<T> getListOfEvens<T>(this IList<T> obj)
        {
            return new HalfList<T>.EvenList(obj);
        }

        // -------------------------------------------------------------------------------------

        /// <summary>
        /// The class providing the basic interface for EvenList and OddList implementers.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private abstract class HalfList<T>: IList<T>
        {
            protected IList<T> parent = null;
            protected int length = 0;

            public HalfList(IList<T> parent)
            {
                this.parent = parent;
            }

            public abstract T this[int index] { get; set; }

            public int Count { get { return length; } }
            public bool IsReadOnly { get { return parent.IsReadOnly; } }

            // ----------------------- ENUMERATORS

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new ClassicEnumerator<T>(this);
            }

            // ----------------------- OTHER METHODS

            public void CopyTo(T[] array, int offset)
            {
                for (int i = 0; i < length; i++)
                    array[offset + i] = this[i];
            }

            public bool Contains(T key)
            {
                foreach (T obj in this)
                    if (obj.Equals(key))
                        return true;

                return false;
            }

            public int IndexOf(T key)
            {
                for (int i = 0; i < length; i++)
                    if (this[i].Equals(key))
                        return i;

                return -1;
            }

            // ----------------------- NOT SUPPORTED BY THIS KIND OF LIST

            public void Add(T value)
            {
                throw new NotSupportedException();
            }

            public void Insert(int index, T value)
            {
                throw new NotSupportedException();
            }

            public bool Remove(T value)
            {
                throw new NotSupportedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            // ------------------------------ IMPLEMENTER CLASSES

            internal class EvenList: HalfList<T>
            {
                internal EvenList(IList<T> obj): base(obj)
                {
                    this.length = obj.Count / 2 + obj.Count % 2;
                }

                public override T this[int index]
                {
                    get { return parent[index * 2]; }
                    set { parent[index * 2] = value; }
                }
            }

            internal class OddList : HalfList<T>
            {
                internal OddList(IList<T> obj): base(obj)
                {
                    this.length = obj.Count / 2;
                }

                public override T this[int index]
                {
                    get { return parent[index * 2 + 1]; }
                    set { parent[index * 2 + 1] = value; }
                }
            }
        }
    }
}
