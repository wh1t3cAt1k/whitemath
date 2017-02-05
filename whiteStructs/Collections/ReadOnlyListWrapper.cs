using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._source.Count;
			}
		}

		public T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._source[index];
			}
		}

		public ReadOnlyListWrapper(IList<T> source) 
		{ 
			this._source = source; 
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerator<T> GetEnumerator()
		{
			return this._source.GetEnumerator();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
