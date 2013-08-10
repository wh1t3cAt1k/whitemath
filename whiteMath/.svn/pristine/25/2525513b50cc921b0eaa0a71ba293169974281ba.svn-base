using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace whiteMath.General
{
    /// <summary>
    /// Represents the list fragment determined by the parent list and a set of indices.
    /// Logically considered incontinuous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListFragment<T> : IList<T>
    {
        IList<T> list;
        int[] indices;

        public ListFragment(IList<T> list, params int[] indices)
        {
            this.list = list;
            this.indices = indices;
        }

        /// <summary>
        /// The indexer for the list fragment.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index] { get { return list[indices[index]]; } set { list[indices[index]] = value; } }

        /// <summary>
        /// Returns true if the list fragment is read only.
        /// </summary>
        public bool IsReadOnly { get { return list.IsReadOnly; } }

        /// <summary>
        /// Returns the amount of elements in the list fragment.
        /// </summary>
        public int Count { get { return indices.Length; } }

        // -------------------------------------------

        /// <summary>
        /// Returns the logical index of the element that is equal to the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int IndexOf(T key)
        {
            for (int i = 0; i < Count; i++)
                if (this[i].Equals(key))
                    return i;

            return -1;
        }

        /// <summary>
        /// Returns true if the list fragment contains the element equal to the key specified.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(T key)
        {
            for (int i = 0; i < Count; i++)
                if (this[i].Equals(key))
                    return true;

            return false;
        }

        /// <summary>
        /// Copies all the elements from the list fragment to an array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        public void CopyTo(T[] array, int offset)
        {
            for (int i = 0; i < Count; i++)
                array[offset + i] = this[i];
        }

        // ----------------------------- Enumerators

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ClassicEnumerator<T>(this);
        }

        // ----------------------------- NOT SUPPORTED

        public bool Remove(T key)
        {
            throw new NotImplementedException("The operation is not supported by list fragments.");
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException("The operation is not supported by list fragments.");
        }

        public void Clear()
        {
            throw new NotImplementedException("The operation is not supported by list fragments.");
        }

        public void Add(T key)
        {
            throw new NotImplementedException("The operation is not supported by list fragments.");
        }

        public void Insert(int index, T value)
        {
            throw new NotImplementedException("The operation is not supported by list fragments.");
        }

        // --------------------------- Static miracles

        /// <summary>
        /// Returns the list fragment containing all parent list elements with odd indices.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ListFragment<T> getOdds(IList<T> list)
        {
            int[] indices = new int[list.Count / 2];

            for (int i = 1; i < list.Count; i += 2)
                indices[i / 2] = i;

            return new ListFragment<T>(list, indices);
        }

        /// <summary>
        /// Returns the list fragment containing all parent list elements with even indices.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ListFragment<T> getEvens(IList<T> list)
        {
            int[] indices = new int[list.Count / 2 + list.Count % 2];

            for (int i = 0; i < list.Count; i += 2)
                indices[i / 2] = i;

            return new ListFragment<T>(list, indices);
        }

        /// <summary>
        /// Returns the list fragment containing all the elements of the parent list
        /// in the order of Bit-Reverse-Permutation.
        /// 
        /// Frequently used in iterational FFT and other algorithms.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ListFragment<T> getBitReversed(IList<T> list)
        {
            if (list.Count == 0)
                return new ListFragment<T>(list);
            else if (list.Count == 1)
                return new ListFragment<T>(list, 0);
            else if (list.Count == 2)
                return new ListFragment<T>(list, 0, 1);

            int r = 0;
            int[] indicesNew = new int[list.Count];

            indicesNew[0] = 0;
            for (int i = 1; i < list.Count; i++)
            {
                r = nextReversed(r, list.Count);
                indicesNew[i] = r;
            }

            return new ListFragment<T>(list, indicesNew);
        }

        private static int nextReversed(int previous, int length)
        {
            do
            {
                length >>= 1;
                previous ^= length;
            } while ((previous & length) == 0);

            return previous;
        }
    }
}
