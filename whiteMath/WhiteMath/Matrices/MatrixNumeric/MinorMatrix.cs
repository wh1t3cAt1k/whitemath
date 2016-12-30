using System;

using WhiteMath.Calculators;

namespace WhiteMath.Matrices
{
    /// <summary>
    /// The class represents the minor matrix of a numeric matrix.
    /// All matrix operations supported, but any changes (i.e. element setting) made to the submatrix 
    /// will be reflected on the parent matrix object. The mechanism uses an encapsulated parent matrix 
    /// reference.
    /// 
    /// An object of this class cannot be instantiated directly.
    /// Call method minorMatrixAt() for a particular matrix object instead.
    /// </summary>
    public class MinorMatrix<T,C>: Matrix<T,C> where C: ICalc<T>, new()
    {
        /// <summary>
        /// Gets the row intex of the parent matrix, which has been removed 
		/// to get the current minor matrix.
        /// </summary>
        public int RemovedRow { get; private set; }

        /// <summary>
        /// Gets the column index of the parent matrix, which has been removed 
		/// to get current minor matrix.
        /// </summary>
        public int RemovedColumn { get; private set; }

        /// <summary>
        /// Gets the parent matrix object.
        /// </summary>
        public Matrix<T,C> Parent { get; protected set; }

        internal MinorMatrix(Matrix<T,C> parent, int removedRow, int removedColumn)
        {
            this.Matrix_Type = MatrixNumericHelper<T, C>.getMatrixType(parent);

            this.RemovedRow     = removedRow;
            this.RemovedColumn  = removedColumn;

            this.RowCount       = parent.RowCount-1;
            this.ColumnCount    = parent.ColumnCount-1;

            this.Parent     = parent;
        }

        protected internal override Numeric<T,C> GetElementAt(int row, int column)
        {
            int parentRow    = (row < RemovedRow ? row : row + 1);
            int parentColumn = (column < RemovedColumn ? column : column + 1);

            return Parent.GetElementAt(parentRow, parentColumn);
        }

        protected internal override void SetItemAt(int row, int column, Numeric<T,C> value)
        {
            int parentRow = (row < RemovedRow ? row : row + 1);
            int parentColumn = (column < RemovedColumn ? column : column + 1);

            Parent.SetItemAt(parentRow, parentColumn, value);
        }

        // -----------------------------
        // ----------- different -------
        // -----------------------------

        /// <summary>
        /// Returns the COPY of the current submatrix as a stand-alone Matrix object.
        /// The changes made to it will never influence the parent matrix.
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return this.getSubMatrixCopyAt(0, 0, RowCount, ColumnCount);
        }

        protected override Matrix<T,C> Multiply(Matrix<T,C> another)
        {
			if (this.ColumnCount != another.RowCount)
			{
				throw new ArgumentException("The column count of the first matrix and the row count of the second matrix must match.");
			}

			Matrix<T,C> result = MatrixNumericHelper<T,C>.GetMatrixOfSize(Parent.Matrix_Type, this.RowCount, another.ColumnCount);
            MatrixNumericHelper<T,C>.MultiplySimple(this, another, result);

            return result;
        }

        protected override Matrix<T,C> Negate()
        {
            Matrix<T,C> temp = MatrixNumericHelper<T,C>.GetMatrixOfSize(Parent.Matrix_Type, RowCount, ColumnCount);

            for (int i = 0; i < RowCount; i++)
                for (int j = 0; j < ColumnCount; j++)
                    temp[i, j] = -this.GetElementAt(i, j);

            return temp;
        }

        protected override Matrix<T,C> Add(Matrix<T,C> another)
        {
            if (this.RowCount != another.RowCount || this.ColumnCount != another.ColumnCount)
                throw new ArgumentException("Matrices must be of the same size in order to sum.");

            Matrix<T,C> temp = MatrixNumericHelper<T,C>.GetMatrixOfSize(Parent.Matrix_Type, RowCount, ColumnCount);
            MatrixNumericHelper<T,C>.sum(this, another, temp);
            return temp;
        }

        protected override Matrix<T,C> Subtract(Matrix<T,C> another)
        {
            if (this.RowCount != another.RowCount || this.ColumnCount != another.ColumnCount)
                throw new ArgumentException("Matrices must be of the same size in order to substract.");

            Matrix<T,C> temp = MatrixNumericHelper<T,C>.GetMatrixOfSize(Parent.Matrix_Type, RowCount, ColumnCount);
            MatrixNumericHelper<T,C>.dif(this, another, temp);
            return temp;
        }
    }
}
