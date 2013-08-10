using System;

namespace whiteMath.Matrices
{
    /// <summary>
    /// A struct representing a pair of matrix indexes.
    /// Provides alternative way of indexing in Matrix objects;
    /// Also is used in Winder-classes for compact next-IndexPair return of getNextIndexPair();
    /// <see>Winder.getNextIndexPair()</see>
    /// </summary>
    public struct IndexPair
    {
        public int row;
        public int column;

        /// <summary>
        /// Constructs a new IndexPair object using a pair of matrix indices.
        /// </summary>
        /// <param name="row">A row index</param>
        /// <param name="column">A column index</param>
        public IndexPair(int row, int column)
        {
            this.row = row;
            this.column = column;
        }

        public override int GetHashCode()
        {
            return row + column / 2;
        }

        public override string ToString()
        {
            return String.Format("IndexPair. Row {0}, column {1}. Hashcode: {2}", row, column, GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!this.GetType().IsInstanceOfType(obj)) return false;
            else return (this.row == ((IndexPair)(obj)).row && this.column == ((IndexPair)(obj)).column);
        }
    }
}