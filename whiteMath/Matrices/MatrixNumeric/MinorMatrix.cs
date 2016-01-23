using System;

using whiteMath.Calculators;

namespace whiteMath.Matrices
{
    /// <summary>
    /// The class represents the minor matrix of a numeric matrix.
    /// All matrix operations supported, but any changes (i.e. element setting) made to the submatrix 
    /// will be reflected on the parent matrix object. The mechanism uses an encapsulated parent matrix 
    /// reference.
    /// 
    /// An object of this class cannot be instantiated directly.
    /// Call method minorMatrixAt() for a particular DoubleMatrix object instead.
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

        // ---------------------
        // ------- ctors -------
        // ---------------------

        internal MinorMatrix(Matrix<T,C> parent, int removedRow, int removedColumn)
        {
            this.Matrix_Type = MatrixNumericHelper<T, C>.getMatrixType(parent);

            this.RemovedRow     = removedRow;
            this.RemovedColumn  = removedColumn;

            this.rows       = parent.RowCount-1;
            this.columns    = parent.ColumnCount-1;

            this.Parent     = parent;
        }

        // -----------------------------------------------------------------------------------

        protected internal override Numeric<T,C> getItemAt(int row, int column)
        {
            int parentRow    = (row < RemovedRow ? row : row + 1);
            int parentColumn = (column < RemovedColumn ? column : column + 1);

            return Parent.getItemAt(parentRow, parentColumn);
        }

        protected internal override void setItemAt(int row, int column, Numeric<T,C> value)
        {
            int parentRow = (row < RemovedRow ? row : row + 1);
            int parentColumn = (column < RemovedColumn ? column : column + 1);

            Parent.setItemAt(parentRow, parentColumn, value);
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
            return this.getSubMatrixCopyAt(0, 0, rows, columns);
        }

        protected override Matrix<T,C> multiply(Matrix<T,C> another)
        {
            if (this.ColumnCount != another.RowCount)
                throw new ArgumentException("The column count of the first matrix and the row count of the second matrix must match.");

            Matrix<T,C> temp = MatrixNumericHelper<T,C>.getMatrixOfSize(Parent.Matrix_Type, this.rows, another.ColumnCount);
            MatrixNumericHelper<T,C>.multiplySimple(this, another, temp);

            return temp;
        }

        protected override Matrix<T,C> negate()
        {
            Matrix<T,C> temp = MatrixNumericHelper<T,C>.getMatrixOfSize(Parent.Matrix_Type, rows, columns);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    temp[i, j] = -this.getItemAt(i, j);

            return temp;
        }

        protected override Matrix<T,C> sum(Matrix<T,C> another)
        {
            if (this.rows != another.RowCount || this.columns != another.ColumnCount)
                throw new ArgumentException("Matrices must be of the same size in order to sum.");

            Matrix<T,C> temp = MatrixNumericHelper<T,C>.getMatrixOfSize(Parent.Matrix_Type, rows, columns);
            MatrixNumericHelper<T,C>.sum(this, another, temp);
            return temp;
        }

        protected override Matrix<T,C> substract(Matrix<T,C> another)
        {
            if (this.rows != another.RowCount || this.columns != another.ColumnCount)
                throw new ArgumentException("Matrices must be of the same size in order to substract.");

            Matrix<T,C> temp = MatrixNumericHelper<T,C>.getMatrixOfSize(Parent.Matrix_Type, rows, columns);
            MatrixNumericHelper<T,C>.dif(this, another, temp);
            return temp;
        }
    }
}
