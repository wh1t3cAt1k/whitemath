using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath.General
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

        // ----------------------------------

        private bool less(T one, T two)
        {
            return Comparer.Compare(one, two) < 0;
        }

        // ----------------------------------

        /// <summary>
        /// Сливает два биномиальных дерева и возвращает ссылку на нового родителя.
        /// </summary>
        private TreeNode<T> mergeSubtrees(TreeNode<T> p, TreeNode<T> q)
        {
            if (less(p.Value, q.Value))
            {
                q.AddChild(p);
                return q;
            }
            else
            {
                p.AddChild(q);
                return p;
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
        public int ChildrenCount { get { return this.children.Count; } }

        public bool HasParent { get { return this.Parent != null; } }
        public bool HasChildren { get { return this.children.Count > 0; } }

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
        private void descendantAdded()
        {
            ++this.DescendantsCount;

			if (Parent != null)
			{
				Parent.descendantAdded();
			}
        }

        /// <summary>
        /// Event that is called upon the parent when a descendant is deleted.
        /// </summary>
        private void descendantRemoved()
        {
            --this.DescendantsCount;

			if (Parent != null)
			{
				Parent.descendantRemoved();
			}
        }

        /// <summary>
		/// Event that is called upon the parent when a child's
		/// order is changed.
        /// </summary>
        /// <param name="oldOrder">Old descendant order.</param>
        /// <param name="newOrder">New descendant order.</param>
        private void childOrderChanged(int oldOrder, int newOrder)
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
					Parent.childOrderChanged(thisOldHeight, this.Height);
				}
            }
            else if (oldOrder + 1 == this.Height)
            {
                int thisOldHeight = this.Height;

				this.Height = children.Max(treeNode => treeNode.Height) + 1;

				if (thisOldHeight < this.Height && Parent != null)
				{
					Parent.childOrderChanged(thisOldHeight, this.Height);
				}
            }
        }

        private List<TreeNodeSmart<T>> children = null;

        /// <summary>
        /// Gets the parent node for the current node.
        /// If no parent node is present, null value is returned.
        /// </summary>
        public TreeNodeSmart<T> Parent          { get; private set; }
               ITreeNode<T> ITreeNode<T>.Parent { get { return this.Parent; } }
        
        /// <summary>
        /// Gets the value stored in the node.
        /// </summary>
        public T Value { get; private set; }

        public void AddChild(TreeNodeSmart<T> child)
        {
            AddChild(child, children.Count);
        }

        public void AddChild(TreeNodeSmart<T> child, int index)
        {
            this.DescendantsCount++;

            children.Insert(index, child);
            child.Parent = this;

            // Height
			// -
            if (child.Height + 1 > this.Height)
            {
                int old = this.Height;
                this.Height = child.Height + 1;

                if (this.Parent != null)
                    Parent.childOrderChanged(old, this.Height);
            }

            if (this.Parent != null)
                Parent.descendantAdded();
        }

        ITreeNode<T> ITreeNode<T>.GetChildAt(int index)
        {
            return this.GetChildAt(index);
        }

        public TreeNodeSmart<T> GetChildAt(int index)
        {
            return children[index];
        }

        public virtual void RemoveChildAt(int index)
        {
            TreeNodeSmart<T> child = children[index];

            this.DescendantsCount--;

            children.RemoveAt(index);

            // Height
			// -
            if (child.Height + 1 == this.Height)
            {
                int oldOrder = this.Height;

                if (children.Count > 0)
                    this.Height = children.Max(delegate(TreeNodeSmart<T> obj) { return obj.Height; }) + 1;
                else
                    this.Height = 0;

                if (oldOrder != this.Height && Parent != null)
                    Parent.childOrderChanged(oldOrder, this.Height);
            }

            if (Parent != null)
                Parent.descendantRemoved();
        }

        public TreeNodeSmart(T value)
        {
            this.Value = value;
            this.Height = 0;

            this.children = new List<TreeNodeSmart<T>>();
        }

        // ----------- root finding ----------

        /// <summary>
        /// Returns the value determining if the current node is a root node for 
        /// some tree. It is true if the node does not have a parent.
        /// </summary>
        public bool IsRoot
        {
            get { return this.Parent == null; }
        }

        /// <summary>
        /// Gets the tree root for the current node, i.e. the farthest
		/// ascendant that has no parent node.
        /// </summary>
        public TreeNodeSmart<T> TreeRoot
        {
            get
            {
				if (this.IsRoot)
				{
					return this;
				}
				else
				{
					return this.Parent.TreeRoot;
				}
            }
        }

        // -----------------------------------
        // ----------- ToString --------------
        // -----------------------------------

        public override string ToString()
        {
            return Value.ToString();
        }

        // -----------------------------------
        // ----------- Conversion ------------
        // -----------------------------------

        public static implicit operator TreeNodeSmart<T>(T value)
        {
            return new TreeNodeSmart<T>(value);
        }

        // -----------------------------------
        // ----------- Swapping --------------
        // -----------------------------------

        /// <summary>
        /// Swaps the node with its child of index <paramref name="i"/>.
        /// </summary>
        /// <param name="i">The number of child index to swap with.</param>
        public void SwapWithChild(int i)
        {
            if (i < 0 || i >= this.children.Count)
                throw new ArgumentException("There is no child with such index.");

            TreeNodeSmart<T> child = this.children[i];

            // Tell each of this child's children that we are their new parent.
			// 
            foreach (TreeNodeSmart<T> childChild in child.children)
                childChild.Parent = this;

            // Tell each of our own children that this child is your new parent.
			//
            for (int j = 0; j < children.Count; j++)
                if (i != j)
                    this.children[j].Parent = child;

            // Make ourselves our child.
			// -
            this.children[i] = this;

            // Exchange children
			// -
            List<TreeNodeSmart<T>> childChildren = child.children;

            child.children = this.children;
            this.children = childChildren;

            // Exchange parents
			// -
            if (this.Parent != null)
            {
                this.Parent.children.Remove(this);
                this.Parent.children.Add(child);
            }

            child.Parent = this.Parent;
            this.Parent = child;

            // Exchange heights
			// -
            int tmp = child.Height;
            
            child.Height = this.Height;
            this.Height = tmp;

            // Exchange descendants counts
			// -
            tmp = child.DescendantsCount;

            child.DescendantsCount = this.DescendantsCount;
            this.DescendantsCount = tmp;

            return;
        }
    }
}
