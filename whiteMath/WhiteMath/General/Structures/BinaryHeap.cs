using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteMath.General
{
    /// <summary>
    /// Represents a simple, dynamic-size binary heap which can be used
    /// as a priority queue for different purposes.
    /// </summary>
	/// <remarks>
	/// Suppose <c>n</c> is the number of elements in the queue. The time complexity 
	/// of element removal is <c>O(log(n))</c>. Element insertion will take
	/// <c>O(n)</c> if the current queue capacity is insufficient for the insertion,
	/// and <c>O(log(n))</c> otherwise.
	/// </remarks>
    /// <typeparam name="T">The type of elements stored in the heap.</typeparam>
    public sealed class BinaryHeap<T>: IPriorityQueue<T>
    {
		private List<T> _treeNodes;

        public IComparer<T> Comparer { get; private set; }
        
        public ITreeNode<T> Root 
        { 
            get 
            {
				if (IsEmpty)
				{
					return null;
				}

				return new BinaryHeapNode(this, 0); 
            } 
        }

        /// <summary>
        /// Creates an instance of a new empty binary heap
        /// using the default comparer for <typeparamref name="T"/> type.
        /// </summary>
        public BinaryHeap()
			: this(new List<T>(), Comparer<T>.Default)
        { }

        /// <summary>
        /// Creates an instance of a new empty binary heap
        /// using the IComparer object passed to compare the values of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="comparer">The comparer for the <typeparamref name="T"/> type.</param>
        public BinaryHeap(IComparer<T> comparer)
			: this(new List<T>(), comparer)
        { }

        /// <summary>
        /// Creates an instance of a new binary heap
        /// from the list containing <typeparamref name="T"/> values and using
        /// (if available) the default comparer for <typeparamref name="T"/> type.
        /// 
        /// If there are N elements in the list, the time of initializing is O(N).
        /// </summary>
        /// <param name="list">The list containing the values to insert into the heap.</param>
        public BinaryHeap(IList<T> list)
			: this(list, Comparer<T>.Default)
        { }

        /// <summary>
        /// Creates an instance of a new binary heap
        /// from the list containing <typeparamref name="T"/> values and using
        /// the IComparer object to compare the values of type <typeparamref name="T"/>.
        /// 
        /// If there are N elements in the list, the time of initializing is O(N).
        /// </summary>
        /// <param name="list">The list containing the values to insert into the heap.</param>
        /// <param name="comparer">The comparer for the <typeparamref name="T"/> type.</param>
        public BinaryHeap(IList<T> list, IComparer<T> comparer)
        {
            this._treeNodes = list.ToList();
            this.Comparer = comparer;

            Heapify();
        }

		// --- Interface implementation

		/// <summary>
		/// Returns the total element count in the heap.
		/// </summary>
		public int Count => _treeNodes.Count;

		/// <summary>
		/// Clears the heap so that it does not contain any elements anymore.
		/// </summary>
		public void Clear() => _treeNodes.Clear();

		/// <summary>
		/// Returns the value determining whether the heap is empty.
		/// </summary>
		public bool IsEmpty => Count == 0;

        /// <summary>
        /// Returns the value of the maximal (according to the comparer used)
        /// element in the heap without actually removing it.
        /// </summary>
        /// <returns>The value of the maximal element in the heap without removing it.</returns>
        public T PeekMax()
        {
			if (IsEmpty)
			{
				throw new InvalidOperationException("The heap is currently empty.");
			}

            return _treeNodes[0];
        }
        
        /// <summary>
        /// Removes the maximal (according to the comparer used)
        /// element in the heap and returns its value.
        /// </summary>
        /// <returns>The value of the removed maximal element.</returns>
        public T Pop()
        {
			if (IsEmpty)
			{
				throw new InvalidOperationException("The heap is currently empty.");
			}

            T value = _treeNodes[0];

            _treeNodes.SwapElements(0, _treeNodes.Count - 1);
            _treeNodes.RemoveAt(_treeNodes.Count - 1);
            
            HeapifyDownwards(0);

            return value;
        }

        /// <summary>
        /// Inserts an element into the queue.
        /// </summary>
        /// <param name="value">The value to be inserted.</param>
        public void Insert(T value)
        {
            _treeNodes.Add(value);
            HeapifyUpwards(_treeNodes.Count - 1);
        }

        /// <summary>
        /// Heapifies the whole tree in logarithmic time.
        /// </summary>
		private void Heapify()
        {
			for (int i = (_treeNodes.Count - 2) / 2; i >= 0; i--)
			{
				HeapifyDownwards(i);
			}
        }

        /// <summary>
		/// Heapifies upwards the element with the specified index.
        /// </summary>
		private void HeapifyUpwards(int index)
        {
			int elementIndex = index;
            int parentIndex = GetParentIndex(index);

            // Пока находимся внутри границ и элемент больше своего родителя, меняем его
            // местами с родителем.

            while (IsInHeap(parentIndex) && Comparer.Compare(_treeNodes[elementIndex], _treeNodes[parentIndex]) > 0)
            {
                _treeNodes.SwapElements(elementIndex, parentIndex);

                elementIndex = parentIndex;
                parentIndex = GetParentIndex(elementIndex);
            }

            return;
        }

        /// <summary>
		/// Heapifies downwards the element with the specified index.
        /// </summary>
		/// <remarks>
		/// Compares with children - and swaps the node with the biggest
		/// of the children. Continues recurrently.
		/// </remarks>
		private void HeapifyDownwards(int index)
        {
			int currentIndex = index;
            
            while (true)
            {
				int leftChildIndex = GetLeftChildIndex(currentIndex);
				int rightChildIndex = GetRightChildIndex(currentIndex);

				if (IsInHeap(leftChildIndex) && Comparer.Compare(_treeNodes[leftChildIndex], _treeNodes[currentIndex]) > 0)
				{
					if (IsInHeap(rightChildIndex) && Comparer.Compare(_treeNodes[rightChildIndex], _treeNodes[leftChildIndex]) > 0)
					{
						_treeNodes.SwapElements(currentIndex, rightChildIndex);
						currentIndex = rightChildIndex;
					}
					else
					{
						_treeNodes.SwapElements(currentIndex, leftChildIndex);
						currentIndex = leftChildIndex;
					}
				}
				else if (IsInHeap(rightChildIndex) && Comparer.Compare(_treeNodes[rightChildIndex], _treeNodes[currentIndex]) > 0)
				{
					_treeNodes.SwapElements(currentIndex, rightChildIndex);
					currentIndex = rightChildIndex;
				}
				else
				{
					break;
				}
            }

            return;
        }

        /// <summary>
        /// Tests whether there is an element with such index.
        /// </summary>
		private bool IsInHeap(int index)
        {
            return (index >= 0 && index < _treeNodes.Count);
        }

        /// <summary>
        /// Returns the index of i-th element's parent.
        /// </summary>
		private static int GetParentIndex(int index)
        {
            return (index - 1) / 2;
        }

        /// <summary>
        /// Returns the index of the left child of i-th element. 
        /// </summary>
		private static int GetLeftChildIndex(int index)
        {
            return 2 * index + 1;
        }

        /// <summary>
        /// Returns the index of the right child of i-th element.
        /// </summary>
		private static int GetRightChildIndex(int index)
        {
            return 2 * index + 2;
        }

        private class BinaryHeapNode : ITreeNode<T>
        {
			BinaryHeap<T> _heap;
			int _nodeIndex;

			public BinaryHeapNode(BinaryHeap<T> heap, int nodeIndex)
            {
                this._heap = heap;
                this._nodeIndex = nodeIndex;
            }

            public override string ToString()
            {
                return Value.ToString();
            }

            public bool HasParent => _heap.IsInHeap(GetParentIndex(_nodeIndex));

            public ITreeNode<T> Parent
            {
                get
                {
					if (!_heap.IsInHeap(GetParentIndex(_nodeIndex)))
					{
						return null;
					}

                    return new BinaryHeapNode(_heap, GetParentIndex(_nodeIndex));
                }
            }

            public int ChildrenCount
            {
                get
                {
                    int count = 0;

                    if (_heap.IsInHeap(GetLeftChildIndex(_nodeIndex)))
                        count++;

                    if (_heap.IsInHeap(GetRightChildIndex(_nodeIndex)))
                        count++;

                    return count;
                }
            }

			public ITreeNode<T> GetChildAt(int index)
            {
				if (index < 0 || index > 1)
				{
					return null;
				}

                if (index == 0)
                {
					if (!_heap.IsInHeap(GetLeftChildIndex(this._nodeIndex)))
					{
						return null;
					}

                    return new BinaryHeapNode(_heap, GetLeftChildIndex(this._nodeIndex));
                }
                else
                {
					if (!_heap.IsInHeap(GetRightChildIndex(this._nodeIndex)))
					{
						return null;
					}

                    return new BinaryHeapNode(_heap, GetRightChildIndex(this._nodeIndex));
                }
            }

			public bool HasChildren => ChildrenCount > 0;

            public T Value => _heap._treeNodes[_nodeIndex];
        }
    }
}
