using System.Collections;
using System.Collections.Generic;

namespace WhiteStructs.Collections
{
	/// <summary>
	/// A wrapper over an arbitrary <see cref="IList{T}"/> collection
	/// allowing such a list to be passed into methods accepting
	/// a <see cref="IReadOnlyList{T}">read-only list</see>.
	/// </summary>
	public sealed class ReadOnlyListWrapper<T> : IReadOnlyList<T>
	{
		private readonly IList<T> _source;

		public int Count => this._source.Count;
		public T this[int index] => this._source[index];

		public ReadOnlyListWrapper(IList<T> source) 
		{ 
			this._source = source; 
		}

		public IEnumerator<T> GetEnumerator() => this._source.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}
}
