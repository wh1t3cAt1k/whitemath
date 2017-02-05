using System.Collections.Generic;

namespace WhiteStructs.Collections
{
	public static class ListExtensions
	{
		public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> list)
			=> new ReadOnlyListWrapper<T>(list);
	}
}
