using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath.General
{
    /// <summary>
    /// Represents a simple, dynamic-size binary heap which can be used
    /// as a priority queue for different purposes.
    /// 
    /// Element removal is O(log(n)), element insertion is
    /// O(N) if the current queue capacity is insufficient for the insertion,
    /// O(log(n)) otherwise.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the heap.</typeparam>
    public sealed class BinaryHeap<T>: IPriorityQueue<T>
    {
        private List<T> tree;

        public IComparer<T> Comparer { get; private set; }
        
        public ITreeNode<T> Root 
        { 
            get 
            { 
                if (this.IsEmpty) 
                    return null; 
                
                else 
                    return new BinaryHeapNode(this, 0); 
            } 
        }

        // -----------------------------
        // -------- ctors --------------
        // -----------------------------

        /// <summary>
        /// Creates an instance of a new empty binary heap
        /// using the default comparer for <typeparamref name="T"/> type.
        /// </summary>
        public BinaryHeap():
            this(new List<T>(), Comparer<T>.Default)
        { }

        /// <summary>
        /// Creates an instance of a new empty binary heap
        /// using the IComparer object passed to compare the values of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="comparer">The comparer for the <typeparamref name="T"/> type.</param>
        public BinaryHeap(IComparer<T> comparer):
            this(new List<T>(), comparer)
        { }

        /// <summary>
        /// Creates an instance of a new binary heap
        /// from the list containing <typeparamref name="T"/> values and using
        /// (if available) the default comparer for <typeparamref name="T"/> type.
        /// 
        /// If there are N elements in the list, the time of initializing is O(N).
        /// </summary>
        /// <param name="list">The list containing the values to insert into the heap.</param>
        public BinaryHeap(IList<T> list):
            this(list, Comparer<T>.Default)
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
            this.tree = list.ToList();
            this.Comparer = comparer;

            heapify();
        }

        // --- Interface implementation

        /// <summary>
        /// Returns the total element count in the heap.
        /// </summary>
        public int Count { get { return tree.Count; } }
        
        /// <summary>
        /// Clears the heap so that it does not contain any elements anymore.
        /// </summary>
        public void Clear()
        {
            tree.Clear();
        }

        /// <summary>
        /// Returns the value determining whether the heap is empty.
        /// </summary>
        public bool IsEmpty { get { return Count == 0; } }

        /// <summary>
        /// Returns the value of the maximal (according to the comparer used)
        /// element in the heap without actually removing it.
        /// </summary>
        /// <returns>The value of the maximal element in the heap without removing it.</returns>
        public T PeekMax()
        {
            if (this.IsEmpty)
                throw new InvalidOperationException("The heap is currently empty.");

            return tree[0];
        }
        
        /// <summary>
        /// Removes the maximal (according to the comparer used)
        /// element in the heap and returns its value.
        /// </summary>
        /// <returns>The value of the removed maximal element.</returns>
        public T Pop()
        {
            if (this.IsEmpty)
                throw new InvalidOperationException("The heap is currently empty.");

            T value = tree[0];

            tree.Swap(0, tree.Count - 1);
            tree.RemoveAt(tree.Count - 1);
            
            heapifyDown(0);

            return value;
        }

        /// <summary>
        /// Inserts an element into the queue.
        /// </summary>
        /// <param name="value">The value to be inserted.</param>
        public void Insert(T value)
        {
            tree.Add(value);
            heapifyUp(tree.Count - 1);
        }
			
        // --- Operations
        
        /// <summary>
        /// Кучифицирует все дерево за логарифмическое время.
        /// </summary>
        private void heapify()
        {
            for (int i = (tree.Count - 2) / 2; i >= 0; i--)
                heapifyDown(i);
        }

        /// <summary>
        /// Кучифицирует элемент с определенным индексом вверх.
        /// Сравнивает с родителем - и, если дендрить, так дендрить.
        /// </summary>
        private void heapifyUp(int i)
        {
            // Индекс элемента и его родителя.

            int index = i;
            int parentIndex = parent(i);

            // Пока находимся внутри границ и элемент больше своего родителя, меняем его
            // местами с родителем.

            while (isInHeap(parentIndex) && Comparer.Compare(tree[index], tree[parentIndex]) > 0)
            {
                tree.Swap(index, parentIndex);

                index = parentIndex;
                parentIndex = parent(index);
            }

            return;
        }

        /// <summary>
        /// Кучифицирует элемент с определенным индексом вниз.
        /// Сравнивает с детьми - и меняет местами c большим из детей.
        /// Дальше рекуррентно.
        /// </summary>
        private void heapifyDown(int i)
        {
            int index = i;
            
            while (true)
            {
                int lc = leftChild(index);
                int rc = rightChild(index);

                if (isInHeap(lc) && Comparer.Compare(tree[lc], tree[index]) > 0)
                {
                    if (isInHeap(rc) && Comparer.Compare(tree[rc], tree[lc]) > 0)
                    {
                        tree.Swap(index, rc);
                        index = rc;
                    }
                    else
                    {
                        tree.Swap(index, lc);
                        index = lc;
                    }
                }
                else if (isInHeap(rc) && Comparer.Compare(tree[rc], tree[index]) > 0)
                {
                    tree.Swap(index, rc);
                    index = rc;
                }
                else
                    break;
            }

            return;
        }
			
        // --- service methods

        /// <summary>
        /// Tests whether there is an element with such index.
        /// </summary>
        private bool isInHeap(int i)
        {
            return (i >= 0 && i < tree.Count);
        }

        /// <summary>
        /// Returns the index of i-th element's parent.
        /// </summary>
        private static int parent(int i)
        {
            return (i - 1) / 2;
        }

        /// <summary>
        /// Returns the index of the left child of i-th element. 
        /// </summary>
        private static int leftChild(int i)
        {
            return 2 * i + 1;
        }

        /// <summary>
        /// Returns the index of the right child of i-th element.
        /// </summary>
        private static int rightChild(int i)
        {
            return 2 * i + 2;
        }

        // --- Support the tree-like structure

        private class BinaryHeapNode : ITreeNode<T>
        {
            BinaryHeap<T> heap;
            int i;

            public BinaryHeapNode(BinaryHeap<T> heap, int i)
            {
                this.heap = heap;
                this.i = i;
            }

            public override string ToString()
            {
                return Value.ToString();
            }

            public bool HasParent { get { return heap.isInHeap(parent(i)); } }

            public ITreeNode<T> Parent
            {
                get
                {
                    if(!heap.isInHeap(parent(i)))
                        return null;

                    return new BinaryHeapNode(heap, parent(i));
                }
            }

            public int ChildrenCount
            {
                get
                {
                    int count = 0;

                    if (heap.isInHeap(leftChild(i)))
                        count++;

                    if (heap.isInHeap(rightChild(i)))
                        count++;

                    return count;
                }
            }

            public ITreeNode<T> GetChildAt(int i)
            {
                if (i < 0 || i > 1)
                    return null;

                if (i == 0)
                {
                    if (!heap.isInHeap(leftChild(this.i)))
                        return null;

                    return new BinaryHeapNode(heap, leftChild(this.i));
                }
                else
                {
                    if(!heap.isInHeap(rightChild(this.i)))
                        return null;

                    return new BinaryHeapNode(heap, rightChild(this.i));
                }
            }

            public bool HasChildren { get { return ChildrenCount > 0; } }

            public T Value { get { return heap.tree[i]; } }
        }
    }
}
