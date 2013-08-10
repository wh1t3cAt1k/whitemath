using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath.General
{
    /// <summary>
    /// The generic interface for priority queues
    /// which supports the operations for element inserting
    /// and removing the maximum element.
    /// </summary>
    /// <typeparam name="T">The type of elements in the priority queue.</typeparam>
    public interface IPriorityQueue<T>
    {
        /// <summary>
        /// Gets the total element count in the queue.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Removes all elements from the queue.
        /// </summary>
        void Clear();

        /// <summary>
        /// Returns the instance of the comparer
        /// used to compare elements of type <typeparamref name="T"/>.
        /// </summary>
        IComparer<T> Comparer { get; }

        /// <summary>
        /// Returns the flag determining whether the queue is empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Inserts an element into the queue. 
        /// </summary>
        /// <param name="value">The value to be inserted.</param>
        void Insert(T value);
        
        /// <summary>
        /// Removes the maximum element from the queue and returns its value.
        /// If the sequence is empty, should throw an InvalidOperationException.
        /// </summary>
        /// <returns>The maximum element in the queue.</returns>
        T Pop();
        
        /// <summary>
        /// Returns the value of the maximum element without removing it.
        /// If the sequence is empty, should throw an InvalidOperationException.
        /// </summary>
        /// <returns>The maximum element in the queue.</returns>
        T PeekMax();
    }

    /// <summary>
    /// Class providing different extension methods for priority queues.
    /// </summary>
    public static class PriorityQueueExtensions
    {
        /// <summary>
        /// Removes all of the elements from the priority queue
        /// and forms the sorted (according to the queue IComparer) array.
        /// 
        /// The flag is received determining whether the array should be sorted
        /// in the reverse order.
        /// 
        /// The time is O(N*logN).
        /// </summary>
        /// <param name="queue">The calling priority queue object.</param>
        /// <param name="reverseSorted">The boolean flag determining whether the array should be sorted in the reverse order.</param>
        /// <returns>An array containing all of the queue elements in sorted order.</returns>
        public static T[] ToSortedArray<T>(this IPriorityQueue<T> queue, bool reverseSorted = false)
        {
            T[] arr = new T[queue.Count];

            if (!reverseSorted)
            {
                for (int i = arr.Length - 1; i >= 0; i--)
                    arr[i] = queue.Pop();
            }
            else
            {
                for (int i = 0; i < arr.Length; i++)
                    arr[i] = queue.Pop();
            }

            return arr;
        }
    }
}
