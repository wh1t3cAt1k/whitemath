using System.Collections;
using System.Collections.Generic;

namespace whiteMath.General
{
    /// <summary>
    /// This class is used to return a default element
    /// when the index specified is out of the bounds.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultList<T>: IList<T>
    {
        private IList<T> parent;

        public T DefaultElement { get; private set; }

        // --------------------------
        // ----- CTORS --------------
        // --------------------------

        /// <summary>
        /// Constructs a new instance of DefaultList.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="defaultValue"></param>
        public DefaultList(IList<T> list, T defaultValue)
        {
            this.parent = list;
            this.DefaultElement = DefaultElement;
        }

        // --------------------------
        // ----- INDEXERS -----------
        // --------------------------

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    return DefaultElement;
                else
                    return parent[index];
            }
            set
            {
                parent[index] = value;
            }
        }

        // --------------------------
        // ----- PROPERTIES ---------
        // --------------------------
        
        public int Count { get { return parent.Count; } }
        public bool IsReadOnly { get { return parent.IsReadOnly; } }
        
        // --------------------------
        // ------ METHODS -----------
        // --------------------------

        public void Add(T item)
        {
            parent.Add(item);
        }

        public void Clear()
        {
            parent.Clear();
        }
        
        public bool Contains(T key)
        {
            return parent.Contains(key);
        }

        public void CopyTo(T[] arr, int index)
        {
            parent.CopyTo(arr, index);
        }

        public int IndexOf(T value)
        {
            return parent.IndexOf(value);
        }

        public void Insert(int index, T value)
        {
            parent.Insert(index, value);
        }

        public bool Remove(T key)
        {
            return parent.Remove(key);
        }

        public void RemoveAt(int index)
        {
            parent.RemoveAt(index);
        }

        // --------------------------
        // ------ ENUMERATORS -------
        // --------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ClassicEnumerator<T>(this);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ClassicEnumerator<T>(this);
        }
    }
}
