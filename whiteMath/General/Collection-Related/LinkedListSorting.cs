using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath.General
{
    /// <summary>
    /// This class provides extension methods for sorting linked lists.
    /// </summary>
    public static class LinkedListSorting
    {
        /// <summary>
        /// Returns a boolean flag determining whether the linked list object passed
        /// is sorted according to the comparer passed.
        /// 
        /// If the comparer is null, then the default comparer will be used (if exists).
        /// </summary>
        /// <typeparam name="T">The type of values stored in the list.</typeparam>
        /// <param name="list">The calling list object.</param>
        /// <param name="comparer">The comparer for the <typeparamref name="T"/> type. If null value is passed, then the default comparer will be used (if exists).</param>
        /// <returns>A boolean flag determining whether the linked list object passed
        /// is sorted according to the comparer passed.</returns>
        public static bool IsSorted<T>(this LinkedList<T> list, IComparer<T> comparer = null)
        {
            if (comparer == null)
                comparer = Comparer<T>.Default;

            if (list.Count < 2)
                return true;

            LinkedListNode<T> current = list.First.Next;

            for (; current != null; current = current.Next)
                if (comparer.Compare(current.Value, current.Previous.Value) < 0)
                    return false;

            return true;
        }

        /// <summary>
        /// Performs the in-place linked list sorting using an insertion sort algorithm.
        /// The time is O(n^2).
        /// </summary>
        /// <typeparam name="T">The values stored in the list.</typeparam>
        /// <param name="list">The calling linked list to be sorted.</param>
        /// <param name="comparer">The comparer for the <typeparamref name="T"/> type. If null is passed, the default comparer will be used (if exists).</param>
        public static void InsertionSort<T>(this LinkedList<T> list, IComparer<T> comparer = null)
        {
            if (comparer == null)
                comparer = Comparer<T>.Default;

            if(list.Count<2)
                return;

            LinkedListNode<T> current = list.First.Next;      // начиная с индекса 1
            LinkedListNode<T> next;                           // следующий за первым. может быть null.

            for (; current != null; current = next)
            {
                next = current.Next;  // следующий. безопасно, current здесь не равен null никогда.

                LinkedListNode<T> backward = current.Previous;
                list.Remove(current);

                for (; backward != null; backward = backward.Previous)
                    if (comparer.Compare(backward.Value, current.Value) <= 0)
                        break;

                if (backward == null)
                    list.AddFirst(current);
                else
                    list.AddAfter(backward, current);
            }

            return;
        }        

        // ---------------------------------------
        // ------- ARRAY-DELEGATIVE METHODS ------
        // ---------------------------------------
        
        public delegate void ListSortMethod<T>(IList<T> list, IComparer<T> comparer);
        public delegate void ArraySortMethod<T>(T[] list, IComparer<T> comparer);

        public static void Sort_UsingListSortMethod<T>(this LinkedList<T> list, ListSortMethod<LinkedListNode<T>> sortMethod, IComparer<T> comparer = null)
        {
            LinkedListNode<T>[] arr = list.GetNodes();
            IComparer<LinkedListNode<T>> nodeComparer = comparer.GetLinkedListNodeComparer();

            // Сортируем массив узлов.

            sortMethod(arr, nodeComparer);

            // Восстанавливаем порядок в связном списке на основе массива узлов.

            list.Reorder_As_In_NodeList(arr);

            return;
        }

        public static void Sort_UsingArraySortMethod<T>(this LinkedList<T> list, ArraySortMethod<LinkedListNode<T>> sortMethod, IComparer<T> comparer = null)
        {
            LinkedListNode<T>[] arr = list.GetNodes();
            IComparer<LinkedListNode<T>> nodeComparer = comparer.GetLinkedListNodeComparer();

            // Сортируем массив узлов.

            sortMethod(arr, nodeComparer);

            // Восстанавливаем порядок в связном списке на основе массива узлов.

            list.Reorder_As_In_NodeList(arr);

            return;
        }
    }
}
