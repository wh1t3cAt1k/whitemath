using System.Collections;
using System.Collections.Generic;

namespace WhiteMath.General
{
    /// <summary>
    /// This class is used to return a default element
    /// when the specified index is out of the bounds.
    /// </summary>
    public class ListWithDefault<T>: IList<T>
    {
        private IList<T> _source;

        public T DefaultElement { get; private set; }

        /// <summary>
        /// Constructs a new instance of DefaultList.
        /// </summary>
        public ListWithDefault(IList<T> list, T defaultValue)
        {
            this._source = list;
			this.DefaultElement = defaultValue;
        }

        public T this[int index]
        {
            get
            {
				if (index < 0 || index >= Count)
				{
					return DefaultElement;
				}
				else
				{
					return _source[index];
				}
            }
            set
            {
                _source[index] = value;
            }
        }
        
        public int Count => _source.Count;

        public bool IsReadOnly => _source.IsReadOnly;

		public void Add(T item) => _source.Add(item);

		public void Clear() => _source.Clear();

		public bool Contains(T key) => _source.Contains(key);

		public void CopyTo(T[] arr, int index) => _source.CopyTo(arr, index);

		public int IndexOf(T value) => _source.IndexOf(value);

		public void Insert(int index, T value) => _source.Insert(index, value);

		public bool Remove(T key) => _source.Remove(key);

		public void RemoveAt(int index) => _source.RemoveAt(index);

		public IEnumerator<T> GetEnumerator() => new ClassicEnumerator<T>(this);

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
