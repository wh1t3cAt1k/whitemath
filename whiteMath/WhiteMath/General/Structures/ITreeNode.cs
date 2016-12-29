using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteMath.General
{
    /// <summary>
    /// The generic interface for tree structure nodes.
    /// </summary>
    /// <typeparam name="T">The type of node values.</typeparam>
    public interface ITreeNode<T>
    {
        /// <summary>
        /// Returns the value determining whether the node has any children.
        /// </summary>
        bool HasChildren { get; }

        /// <summary>
        /// Returns the value determining whether the node has a parent.
        /// </summary>
        bool HasParent { get; }

        /// <summary>
        /// Returns the value determining how many children the node has.
        /// </summary>
        int ChildrenCount { get; }

        /// <summary>
        /// Gets the child node with index i.
        /// </summary>
        /// <param name="i">The index of the child node.</param>
        /// <returns>The child node with index i.</returns>
        ITreeNode<T> GetChildAt(int i);

        /// <summary>
        /// Gets the parent node of the current node.
        /// If the node is the root node (i.e. has no parent),
        /// should return null.
        /// </summary>
        ITreeNode<T> Parent { get; }

        /// <summary>
        /// Gest the value stored in the node.
        /// </summary>
        T Value { get; }
    }
}
