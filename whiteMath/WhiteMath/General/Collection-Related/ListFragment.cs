﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace WhiteMath.General
{
    /// <summary>
    /// Represents the list fragment determined by the parent list and a set of indices.
    /// Logically considered incontinuous.
    /// </summary>
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
        public T this[int index] 
		{ 
			get 
			{ 
				return list[indices[index]]; 
			} 
			set 
			{ 
				list[indices[index]] = value; 
			} 
		}

        /// <summary>
        /// Returns true if the list fragment is read only.
        /// </summary>
        public bool IsReadOnly => list.IsReadOnly;

        /// <summary>
        /// Returns the amount of elements in the list fragment.
        /// </summary>
        public int Count => indices.Length;

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
        public void CopyTo(T[] array, int offset)
        {
            for (int i = 0; i < Count; i++)
                array[offset + i] = this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public IEnumerator<T> GetEnumerator() => new ClassicEnumerator<T>(this);

        public bool Remove(T key)
        {
            throw new NotSupportedException("The operation is not supported by list fragments.");
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException("The operation is not supported by list fragments.");
        }

        public void Clear()
        {
            throw new NotSupportedException("The operation is not supported by list fragments.");
        }

        public void Add(T key)
        {
            throw new NotSupportedException("The operation is not supported by list fragments.");
        }

        public void Insert(int index, T value)
        {
            throw new NotSupportedException("The operation is not supported by list fragments.");
        }

        /// <summary>
        /// Returns the list fragment containing all parent list elements with odd indices.
        /// </summary>
		public static ListFragment<T> GetOdds(IList<T> list)
        {
            int[] indices = new int[list.Count / 2];

            for (int i = 1; i < list.Count; i += 2)
                indices[i / 2] = i;

            return new ListFragment<T>(list, indices);
        }

        /// <summary>
        /// Returns the list fragment containing all parent list elements with even indices.
        /// </summary>
		public static ListFragment<T> GetEvens(IList<T> list)
        {
            int[] indices = new int[list.Count / 2 + list.Count % 2];

            for (int i = 0; i < list.Count; i += 2)
                indices[i / 2] = i;

            return new ListFragment<T>(list, indices);
        }

        /// <summary>
        /// Returns the list fragment containing all the elements of the parent list
        /// in the order of Bit-Reverse-Permutation.
        /// Frequently used in iterational FFT and other algorithms.
        /// </summary>
		public static ListFragment<T> GetBitReversed(IList<T> list)
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
                r = GetNextReversed(r, list.Count);
                indicesNew[i] = r;
            }

            return new ListFragment<T>(list, indicesNew);
        }

		private static int GetNextReversed(int previous, int length)
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
