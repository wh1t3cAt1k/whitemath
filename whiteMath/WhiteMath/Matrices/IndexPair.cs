using System;

namespace WhiteMath.Matrices
{
    /// <summary>
    /// A struct representing a pair of matrix indexes.
    /// Provides an alternative way of indexing in Matrix objects;
    /// Also used in Winder-classes for compact next-IndexPair return of getNextIndexPair();
    /// <see>Winder.getNextIndexPair()</see>
    /// </summary>
    public class IndexPair : Tuple<int, int>
    {
        public int Row => Item1;
        public int Column => Item2;

        /// <summary>
        /// Constructs a new IndexPair object using a pair of matrix indices.
        /// </summary>
        /// <param name="row">A row index</param>
        /// <param name="column">A column index</param>
        public IndexPair(int row, int column)
			: base(row, column)
		{ }

        public override string ToString()
        {
            return string.Format("IndexPair. Row {0}, column {1}. Hashcode: {2}", Row, Column, GetHashCode());
        }
    }
}