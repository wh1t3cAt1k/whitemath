using System.Collections;
using System.Collections.Generic;

namespace WhiteMath.General
{
    /// <summary>
    /// The classic enumerator for IList<typeparamref name="T"/>.
    /// </summary>
    internal class ClassicEnumerator<T> : IEnumerator<T>
    {
		int _currentindex = -1;
		IList<T> _list;

        /// <summary>
        /// Creates a <see cref="ClassicEnumerator&lt;T&gt;"/> for an <see cref="IList&lt;T&gt;"/>.
        /// </summary>
        public ClassicEnumerator(IList<T> list)
        {
            _list = list;
        }

		/// <summary>
		/// Gets the element to which the enumerator currently points.
		/// </summary>
		object IEnumerator.Current => Current;

        /// <summary>
        /// Gets the element to which the enumerator currently points.
        /// </summary>
        public T Current => _list[_currentindex];

        /// <summary>
        /// Moves the enumerator so that it points to the next element of the collection.
        /// </summary>
        public bool MoveNext()
        {
            _currentindex++;

			if (_currentindex < _list.Count)
			{
				return true;
			}
			else
			{
				return false;
			}
        }

        /// <summary>
        /// Disposes the enumerator so that it becomes stale (unusable),
        /// freeing all references to any objects.
        /// </summary>
        public void Dispose()
        {
            Reset();
            _list = null;
        }

        /// <summary>
        /// Resets the enumerator so that it points to the
        /// 'before-the-first' element of the collection.
        /// </summary>
        public void Reset()
        {
            _currentindex = -1;
        }
    }
}
