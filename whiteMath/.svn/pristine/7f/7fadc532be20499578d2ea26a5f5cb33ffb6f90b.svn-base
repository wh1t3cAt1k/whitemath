using System;

using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Text;

namespace whiteMath.General
{
    /// <summary>
    /// Creates the wrapper to access the elements of some particular list, but in reverse order.
    /// e.g. accessing element number [i] in the reverse list is exactly the same as accessing element number[parent.Count - i - 1] in the parent list.
    /// e.g. adding to the end of a reverse list is equivalent to adding to the beginning of the parent list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    class ReverseList<T>: IList<T>
    {
        private IList<T> parent;

        // -----------------------
        // ---- CTORS ------------
        // -----------------------

        public ReverseList(IList<T> list)
        {
            this.parent = list;
            return;
        }

        // -----------------------
        // ---- ENUMERATORS ------
        // -----------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ClassicEnumerator<T>(this);
        }

        // -----------------------

        public int Count { get { return parent.Count; } }

        public bool IsReadOnly { get { return parent.IsReadOnly; } }

        // -----------------------

        public T this[int i] 
        {
            get { return parent[_ic(i)]; }
            set { parent[_ic(i)] = value; }
        }

        // -----------------------

        public void Add(T item)
        {
            parent.Insert(0, item);
        }

        public bool Contains(T key)
        {
            return parent.Contains(key);
        }

        public void Clear()
        {
            parent.Clear();
        }

        public int IndexOf(T key)
        {
            return _ic(parent.IndexOf(key));
        }

        public void Insert(int i, T item)
        {
            parent.Insert(_ic(i), item);
        }

        public bool Remove(T key)
        {
            for (int i = 0; i < this.Count; i++)
                if (this[i].Equals(key))
                {
                    parent.RemoveAt(_ic(i));
                    return true;
                }

            return false;
        }

        public void RemoveAt(int i)
        {
            parent.RemoveAt(_ic(i));
        }

        public void CopyTo(T[] arr, int index)
        {
            for (int i = 0; i < this.Count; i++)
                arr[i + index] = this[i];
        }

        // -----------------------

        /// <summary>
        /// Index conversion.
        /// </summary>
        private int _ic(int i)
        {
            return parent.Count - i - 1;
        }
    }
}
