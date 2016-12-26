using System.Collections.Generic;

namespace WhiteMath.General
{
    /// <summary>
    /// This class provides extension methods for sorting linked lists.
    /// </summary>
    public static class LinkedListSorting
    {
        /// <summary>
        /// Returns a boolean flag determining whether the linked list object passed
        /// is sorted according to the comparer passed.
        /// If the comparer is null, then the default comparer will be used (if exists).
        /// </summary>
        /// <typeparam name="T">The type of values stored in the list.</typeparam>
        /// <param name="list">The calling list object.</param>
        /// <param name="comparer">The comparer for the <typeparamref name="T"/> type. If null value is passed, then the default comparer will be used (if exists).</param>
        /// <returns>A boolean flag determining whether the linked list object passed
        /// is sorted according to the comparer passed.</returns>
        public static bool IsSorted<T>(this LinkedList<T> list, IComparer<T> comparer = null)
        {
			comparer = comparer ?? Comparer<T>.Default;

			if (list.Count < 2)
			{
				return true;
			}

            LinkedListNode<T> current = list.First.Next;

			for (; current != null; current = current.Next)
			{
				if (comparer.Compare(current.Value, current.Previous.Value) < 0)
				{
					return false;
				}
			}

            return true;
        }

        /// <summary>
        /// Performs the in-place linked list sorting using an insertion sort algorithm.
        /// The worst-case complexity is O(n^2).
        /// </summary>
        /// <typeparam name="T">The values stored in the list.</typeparam>
        /// <param name="list">The calling linked list to be sorted.</param>
        /// <param name="comparer">The comparer for the <typeparamref name="T"/> type. If null is passed, the default comparer will be used (if exists).</param>
        public static void InsertionSort<T>(this LinkedList<T> list, IComparer<T> comparer = null)
        {
			comparer = comparer ?? Comparer<T>.Default;

			if (list.Count < 2)
			{
				return;
			}

			LinkedListNode<T> currentNode = list.First.Next;
			LinkedListNode<T> nextNode;

            for (; currentNode != null; currentNode = nextNode)
            {
				// This is safe, the Current is never null here.
				// -
				nextNode = currentNode.Next;

				LinkedListNode<T> previousNode = currentNode.Previous;
                list.Remove(currentNode);

				for (; previousNode != null; previousNode = previousNode.Previous)
				{
					if (comparer.Compare(previousNode.Value, currentNode.Value) <= 0)
					{
						break;
					}
				}

				if (previousNode == null)
				{
					list.AddFirst(currentNode);
				}
				else
				{
					list.AddAfter(previousNode, currentNode);
				}
            }

            return;
        }
        
        public delegate void ListSortMethod<T>(IList<T> list, IComparer<T> comparer);
        public delegate void ArraySortMethod<T>(T[] list, IComparer<T> comparer);

		public static void SortUsingListSortMethod<T>(
			this LinkedList<T> list, 
			ListSortMethod<LinkedListNode<T>> sortMethod, 
			IComparer<T> comparer = null)
        {
			comparer = comparer ?? Comparer<T>.Default;

            LinkedListNode<T>[] arr = list.GetNodes();
            IComparer<LinkedListNode<T>> nodeComparer = comparer.GetLinkedListNodeComparer();

            sortMethod(arr, nodeComparer);

            list.ReorderAsInList(arr);
        }

		public static void SortUsingArraySortMethod<T>(
			this LinkedList<T> list, 
			ArraySortMethod<LinkedListNode<T>> sortMethod, 
			IComparer<T> comparer = null)
        {
			comparer = comparer ?? Comparer<T>.Default; 

			LinkedListNode<T>[] nodeArray = list.GetNodes();
            IComparer<LinkedListNode<T>> nodeComparer = comparer.GetLinkedListNodeComparer();

            sortMethod(nodeArray, nodeComparer);

            list.ReorderAsInList(nodeArray);
        }
    }
}
