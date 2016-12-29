using System.Collections.Generic;

namespace WhiteMath.General.Structures
{
	/// <summary>
	/// Provides methods which work for any kind of tree nodes implementing the ITreeNode(T) interface.
	/// Specialized versions provided inside the classes are expected to be more effective.
	/// </summary>
	public static class TreeNodeExtensions
	{
		/// <summary>
		/// Gets the total amount of node's descendants excluding itself.
		/// </summary>
		/// <remarks>
		/// The time complexity is <c>O(n)</c>, where <c>n</c> is the number of
		/// nodes in the tree.
		/// </remarks>
		/// <typeparam name="T">The type of value stored in the node.</typeparam>
		/// <param name="node">The calling node object.</param>
		/// <returns>The total amount of node's descendants excluding itself.</returns>
		public static int DescendantsCount<T>(this ITreeNode<T> node)
		{
			int sum = 1;

			for (int i = 0; i < node.ChildrenCount; ++i)
			{
				sum += node.GetChildAt(i).DescendantsCount();
			}

			return sum - 1;
		}

		/// <summary>
		/// Follows the chain of parent references to return the root.
		/// The latter may be equal to the calling node object if the are no parents.
		/// </summary>
		/// <typeparam name="T">The type of node values.</typeparam>
		/// <param name="node">The calling node object.</param>
		/// <returns>The root node of the tree.</returns>
		public static ITreeNode<T> GetTreeRoot<T>(this ITreeNode<T> node)
		{
			ITreeNode<T> parent = node;

			while (parent.HasParent)
			{
				parent = parent.Parent;
			}

			return parent;
		}

		/// <summary>
		/// Returns the overall tree height in which
		/// the node is located.
		/// </summary>
		/// <remarks>
		/// The time complexity is <c>O(n)</c>, where <c>n</c> is the number
		/// of nodes in the tree.
		/// </remarks>
		/// <typeparam name="T">The type of node values.</typeparam>
		/// <param name="node">The calling node object.</param>
		/// <returns>The overall tree height in which the node is located.</returns>
		public static int TreeHeight<T>(this ITreeNode<T> node)
		{
			return node.GetTreeRoot().GetHeight();
		}

		/// <summary>
		/// Returns the length of the longest chain from the root to a leaf, which
		/// passes through the current node.
		/// </summary>
		/// <typeparam name="T">The type of node values.</typeparam>
		/// <param name="node">The calling node object.</param>
		/// <returns>The length of the longest chain from the root to a leaf which
		/// passes through the current node.</returns>
		public static int LongestChainLength<T>(this ITreeNode<T> node)
		{
			return node.GetHeight() + node.LengthToTheRoot();
		}

		/// <summary>
		/// Returns the length of the path from a tree node to the root.
		/// If the node is a root itself, the path length is zero.
		/// </summary>
		/// <typeparam name="T">The type of node value.</typeparam>
		/// <param name="node">The calling node object.</param>
		/// <returns>The length of the path from the node to the tree root. If the node is a root, the path length is zero.</returns>
		public static int LengthToTheRoot<T>(this ITreeNode<T> node)
		{
			int sum = 0;

			ITreeNode<T> parent = node;

			if (parent.HasParent)
			{
				sum++;
				parent = parent.Parent;
			}

			return sum;
		}

		/// <summary>
		/// Gets the height of the node, that is, the length of the path to its
		/// farthermost descendant.
		/// </summary>
		/// <typeparam name="T">The type of value stored in the node.</typeparam>
		/// <param name="node">The calling node object.</param>
		/// <returns>The total amount of node's descendants excluding himself.</returns>
		public static int GetHeight<T>(this ITreeNode<T> node)
		{
			int sum = 0;

			if (node.ChildrenCount > 0)
			{
				sum++;

				int max = node.GetChildAt(0).GetHeight();

				for (int i = 1; i < node.ChildrenCount; i++)
				{
					int height = node.GetChildAt(i).GetHeight();

					if (height > max)
						max = height;
				}

				sum += max;
			}

			return sum;
		}

		/// <summary>
		/// Performs the infix traversing of the tree node and returns
		/// the list filled up with nodes in the traversion order.
		/// 
		/// The <paramref name="prevIndex"/> parameted is the index of child node
		/// after which the currently processed node should be traversed.
		/// 
		/// If <paramref name="prevIndex"/> is negative, the current node will be traverse first.
		/// Else, if it is more than or equals to the current node's children amount,
		/// the current node will be traversed last.
		/// 
		/// Works only for any kind of trees.
		/// </summary>
		/// <param name="prevIndex"></param>
		/// <returns>List that would be filled up with nodes during the traversing.</returns>
		public static List<ITreeNode<T>> TraverseInfix<T>(this ITreeNode<T> node, int prevIndex)
		{
			List<ITreeNode<T>> list = new List<ITreeNode<T>>();

			node.TraverseInfix(list, prevIndex);

			return list;
		}

		private static void TraverseInfix<T>(this ITreeNode<T> node, List<ITreeNode<T>> list, int index)
		{
			bool traversed = false;

			for (int i = 0; i < node.ChildrenCount; i++)
			{
				if (!traversed && i > index)
				{
					list.Add(node);
					traversed = true;
				}

				node.GetChildAt(i).TraverseInfix(list, index);
			}

			if (!traversed)
				list.Add(node);
		}

		/// <summary>
		/// Performs the prefix traversing of the tree node and fills up
		/// the list passed with nodes in the traversion order.
		/// 
		/// The node is traversed first, then all of its children, beginning with the leftmost,
		/// are recursively traversed.
		/// 
		/// Works for any kind of trees.
		/// </summary>
		/// <returns>List that would be filled up with nodes during the traversing.</returns>
		public static List<ITreeNode<T>> TraversePrefix<T>(this ITreeNode<T> node)
		{
			List<ITreeNode<T>> list = new List<ITreeNode<T>>();

			node.TraversePrefix(list);

			return list;
		}

		private static void TraversePrefix<T>(this ITreeNode<T> node, List<ITreeNode<T>> list)
		{
			list.Add(node);

			for (int i = 0; i < node.ChildrenCount; i++)
				node.GetChildAt(i).TraversePrefix(list);
		}

		/// <summary>
		/// Performs the postfix traversing of the tree node and returns the list 
		/// filled up with nodes in the traversion order.
		/// 
		/// The node is traversed last, first all of its children, beginning with the leftmost,
		/// are recursively traversed.
		/// 
		/// Works for any kind of trees.
		/// </summary>
		/// <returns>List that would be filled up with nodes during the traversing.</returns>
		public static List<ITreeNode<T>> TraversePostfix<T>(this ITreeNode<T> node)
		{
			List<ITreeNode<T>> list = new List<ITreeNode<T>>();

			node.TraversePostfix(list);

			return list;
		}

		private static void TraversePostfix<T>(this ITreeNode<T> node, List<ITreeNode<T>> list)
		{
			for (int i = 0; i < node.ChildrenCount; ++i)
			{
				node.GetChildAt(i).TraversePostfix(list);
			}

			list.Add(node);
		}
	}
}
