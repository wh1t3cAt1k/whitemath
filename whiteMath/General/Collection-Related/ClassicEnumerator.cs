using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace whiteMath.General
{
    /// <summary>
    /// The classic enumerator for IList<typeparamref name="T"/>.
    /// </summary>
    internal class ClassicEnumerator<T> : IEnumerator<T>
    {
        int curInd = -1;
        IList<T> list;

        /// <summary>
        /// Creates a <see cref="ClassicEnumerator&lt;T&gt;"/> for an <see cref="IList&lt;T&gt;"/>.
        /// </summary>
        public ClassicEnumerator(IList<T> list)
        {
            this.list = list;
        }

        /// <summary>
        /// Gets the element to which the enumerator currently points.
        /// </summary>
        object IEnumerator.Current
        {
            get { return Current; }
        }

        /// <summary>
        /// Gets the element to which the enumerator currently points.
        /// </summary>
        public T Current
        {
            get { return list[curInd]; }
        }

        /// <summary>
        /// Moves the enumerator so that it points to the next element of the collection.
        /// </summary>
        public bool MoveNext()
        {
            curInd++;

            if (curInd < list.Count)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Disposes the enumerator so that it becomes stale (unusable),
        /// freeing all references to any objects.
        /// </summary>
        public void Dispose()
        {
            Reset();
            list = null;
        }

        /// <summary>
        /// Resets the enumerator so that it points to the
        /// 'before-the-first' element of the collection.
        /// </summary>
        public void Reset()
        {
            curInd = -1;
        }
    }
}
