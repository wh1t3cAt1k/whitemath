using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath.General
{
    public class TreeNode<T>: ITreeNode<T>
    {
        public bool HasChildren { get { return this.children.Count > 0; } }
        public bool HasParent { get { return this.Parent != null; } }

        public int ChildrenCount { get { return this.children.Count; } }

        /// <summary>
        /// Gets the value stored in the node.
        /// </summary>
        public T Value { get; private set; }

        private List<TreeNode<T>> children = null;

        /// <summary>
        /// Gets the parent of the node if it exists.
        /// </summary>
        public TreeNode<T> Parent { get; private set; }

        // --------------------------
        // ------ child adding ------
        // --------------------------

        public void AddChild(TreeNode<T> child)
        {
            children.Add(child);
            children.Last().Parent = this;
        }

        public void AddChild(TreeNode<T> child, int index)
        {
            children.Insert(index, child);
            children[index].Parent = this;
        }

        // --------------------------
        // ------ child getting -----
        // --------------------------

        ITreeNode<T> ITreeNode<T>.GetChildAt(int index)
        {
            return this.GetChildAt(index);
        }

        ITreeNode<T> ITreeNode<T>.Parent
        {
            get { return this.Parent; }
        }

        public TreeNode<T> GetChildAt(int index)
        {
            return children[index];
        }

        public void RemoveChildAt(int index)
        {
            children[index].Parent = null;
            children.RemoveAt(index);
        }

        public TreeNode(T value)
        {
            this.Value = value;            
            this.children = new List<TreeNode<T>>();
        }

        // -----------------------------------
        // ----------- ToString --------------
        // -----------------------------------

        public override string ToString()
        {
            return Value.ToString();
        }

        // -----------------------------------
        // ----------- conversion ------------
        // -----------------------------------

        public static implicit operator TreeNode<T>(T value)
        {
            return new TreeNode<T>(value);
        }

        // -----------------------------------
        // ----------- swapping --------------
        // -----------------------------------

        /// <summary>
        /// Swaps the node with its child of index <paramref name="i"/>.
        /// </summary>
        /// <param name="i">The number of child index to swap with.</param>
        public void SwapWithChild(int i)
        {
            if (i < 0 || i >= this.children.Count)
                throw new ArgumentException("There is no child with such index.");

            TreeNode<T> child = this.children[i];

            // Каждому из детей ребенка говорим, что теперь я - ваш родитель.

            foreach (TreeNode<T> childChild in child.children)
                childChild.Parent = this;

            // Каждому из собственных детей говорим, что теперь ребенок - ваш родитель.

            for (int j = 0; j < children.Count; j++)
                if (i != j)
                    this.children[j].Parent = child;

            // Делаем себя своим ребенком вместо child.

            this.children[i] = this;

            // Меняемся детьми

            List<TreeNode<T>> childChildren = child.children;

            child.children = this.children;
            this.children = childChildren;

            // Родителями

            if (this.Parent != null)
            {
                this.Parent.children.Remove(this);
                this.Parent.children.Add(child);
            }

            child.Parent = this.Parent;
            this.Parent = child;


            return;
        }

    }
}
