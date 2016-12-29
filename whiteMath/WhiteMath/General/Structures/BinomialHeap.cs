using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteMath.General
{
    public class BinomialHeap<T>
    {
        /// <summary>
        /// Returns the comparer for elements in the heap.
        /// </summary>
        public IComparer<T> Comparer { get; private set; }
        
        /// <summary>
        /// Returns the overall element count in the heap.
        /// Takes O(N) time to perform.
        /// </summary>
        public int Count { get; private set; }

        private List<TreeNode<T>> roots;

		private bool IsLessThan(T one, T two)
        {
            return Comparer.Compare(one, two) < 0;
        }

        /// <summary>
        /// Сливает два биномиальных дерева и возвращает ссылку на нового родителя.
        /// </summary>
		private TreeNode<T> MergeSubtrees(TreeNode<T> firstSubtree, TreeNode<T> secondSubtree)
        {
            if (IsLessThan(firstSubtree.Value, secondSubtree.Value))
            {
                secondSubtree.AddChild(firstSubtree);
                return secondSubtree;
            }
            else
            {
                firstSubtree.AddChild(secondSubtree);
                return firstSubtree;
            }
        }
        
        /*
        private BinaryHeap<T> mergeHeaps(BinaryHeap<T> one, BinaryHeap<T> two)
        {

        }*/
    }

    // -----------------------

    public class TreeNodeSmart<T>: ITreeNode<T>
    {
        public int Height { get; private set; }
        public int ChildrenCount { get { return this._children.Count; } }

        public bool HasParent { get { return this.Parent != null; } }
        public bool HasChildren { get { return this._children.Count > 0; } }

        /// <summary>
        /// Gets the amount of total descendants of the current root, excluding the current.
        /// The time of getting is O(1).
        /// </summary>
        public int DescendantsCount
        {
            get;
            private set;
        }

        // ----------------------------------
        // ----------- EVENTS ---------------
        // ----------------------------------

        /// <summary>
        /// Event that is called upon the parent when a descendant is added.
        /// </summary>
		private void DescendantAdded()
        {
            ++DescendantsCount;

			if (Parent != null)
			{
				Parent.DescendantAdded();
			}
        }

        /// <summary>
        /// Event that is called upon the parent when a descendant is deleted.
        /// </summary>
		private void DescendantRemoved()
        {
            --this.DescendantsCount;

			if (Parent != null)
			{
				Parent.DescendantRemoved();
			}
        }

        /// <summary>
		/// Event that is called upon the parent when a child's
		/// order is changed.
        /// </summary>
        /// <param name="oldOrder">Old descendant order.</param>
        /// <param name="newOrder">New descendant order.</param>
		private void ChildOrderChanged(int oldOrder, int newOrder)
        {
			// If the old order + 1 is equal to the current order,
			// perhaps there is no such order anymore and we need
			// to find new.
			// -
            if (newOrder + 1 > this.Height)
            {
                int thisOldHeight = this.Height;

                this.Height = newOrder + 1;

				if (Parent != null)
				{
					Parent.ChildOrderChanged(thisOldHeight, this.Height);
				}
            }
            else if (oldOrder + 1 == this.Height)
            {
                int thisOldHeight = this.Height;

				this.Height = _children.Max(treeNode => treeNode.Height) + 1;

				if (thisOldHeight < this.Height && Parent != null)
				{
					Parent.ChildOrderChanged(thisOldHeight, this.Height);
				}
            }
        }

		private List<TreeNodeSmart<T>> _children = null;

        /// <summary>
        /// Gets the parent node for the current node.
        /// If no parent node is present, a <c>null</c> value is returned.
        /// </summary>
        public TreeNodeSmart<T> Parent 
		{ 
			get; 
			private set; 
		}

		ITreeNode<T> ITreeNode<T>.Parent => this.Parent;
        
        /// <summary>
        /// Gets the value stored in the node.
        /// </summary>
        public T Value 
		{
			get; 
			private set; 
		}

        public void AddChild(TreeNodeSmart<T> child)
        {
            AddChild(child, _children.Count);
        }

        public void AddChild(TreeNodeSmart<T> child, int index)
        {
            ++DescendantsCount;

            _children.Insert(index, child);
            child.Parent = this;

            if (child.Height + 1 > this.Height)
            {
                int old = this.Height;
                this.Height = child.Height + 1;

                if (this.Parent != null)
                    Parent.ChildOrderChanged(old, this.Height);
            }

			if (this.Parent != null)
			{
				Parent.DescendantAdded();
			}
        }

        ITreeNode<T> ITreeNode<T>.GetChildAt(int index)
        {
            return this.GetChildAt(index);
        }

        public TreeNodeSmart<T> GetChildAt(int index)
        {
            return _children[index];
        }

        public virtual void RemoveChildAt(int index)
        {
            TreeNodeSmart<T> child = _children[index];

            --DescendantsCount;

            _children.RemoveAt(index);

            if (child.Height + 1 == Height)
            {
                int oldOrder = Height;

				if (_children.Count > 0)
				{
					Height = _children.Max(node => node.Height) + 1;
				}
				else
				{
					Height = 0;
				}

				if (oldOrder != Height && Parent != null)
				{
					Parent.ChildOrderChanged(oldOrder, Height);
				}
            }

			if (Parent != null)
			{
				Parent.DescendantRemoved();
			}
        }

        public TreeNodeSmart(T value)
        {
            Value = value;
            Height = 0;

            _children = new List<TreeNodeSmart<T>>();
        }

		// ----------- root finding ----------

		/// <summary>
		/// Returns the value determining if the current node is a root node for 
		/// some tree. It is true if the node does not have a parent.
		/// </summary>
		public bool IsRoot => Parent == null;

		/// <summary>
		/// Gets the tree root for the current node, i.e. the farthest
		/// ascendant that has no parent node.
		/// </summary>
		public TreeNodeSmart<T> TreeRoot 
			=> IsRoot ? this : Parent.TreeRoot;

		public override string ToString()
			=> Value.ToString();

        public static implicit operator TreeNodeSmart<T>(T value)
			=> new TreeNodeSmart<T>(value);

		/// <summary>
        /// Swaps the node with its child of index <paramref name="childIndex"/>.
        /// </summary>
        /// <param name="childIndex">The number of child index to swap with.</param>
		public void SwapWithChild(int childIndex)
        {
			if (childIndex < 0 || childIndex >= _children.Count)
			{
				throw new ArgumentException("There is no child with such index.");
			}

            TreeNodeSmart<T> child = _children[childIndex];

			// Tell each of this child's children that we are their new parent.
			// 
			foreach (TreeNodeSmart<T> childChild in child._children)
			{
				childChild.Parent = this;
			}

			// Tell each of our own children that this child is your new parent.
			//
			for (int j = 0; j < _children.Count; ++j)
			{
				if (childIndex != j)
				{
					_children[j].Parent = child;
				}
			}

            // Make ourselves our child.
			// -
            this._children[childIndex] = this;

            // Exchange children.
			// -
            List<TreeNodeSmart<T>> childChildren = child._children;

            child._children = _children;
            _children = childChildren;

            // Exchange parents.
			// -
            if (Parent != null)
            {
                this.Parent._children.Remove(this);
                this.Parent._children.Add(child);
            }

            child.Parent = this.Parent;
            this.Parent = child;

            // Exchange heights.
			// -
			int tempHeight = child.Height;
            
            child.Height = this.Height;
            this.Height = tempHeight;

            // Exchange descendants counts.
			// -
            tempHeight = child.DescendantsCount;

            child.DescendantsCount = this.DescendantsCount;
            this.DescendantsCount = tempHeight;
        }
    }
}
