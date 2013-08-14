using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath.Matrices
{
    /// <summary>
    /// The class represents the continuous submatrix of a numeric matrix.
    /// All matrix operations supported, but any changes made to the submatrix will be reflected 
    /// on the parent matrix object. The mechanism uses an encapsulated parent matrix 
    /// reference.
    /// 
    /// An object of this class cannot be instantiated directly.
    /// Call method getSubmatrixAt() for a particular Matrix object instead.
    /// </summary>
    public class Submatrix<T,C>: Matrix<T,C> where C: ICalc<T>, new()
    {
        private MatrixType mt;

        protected int offsetRow = 0;
        protected int offsetColumn = 0;

        /// <summary>
        /// Gets the row offset (in the parent matrix) for the current submatrix.
        /// </summary>
        public int RowOffset { get { return offsetRow; } }

        /// <summary>
        /// Gets the column offset (in the parent matrix) for the current submatrix.
        /// </summary>
        public int ColumnOffset { get { return offsetColumn; } }

        protected Matrix<T,C> parent;
        
        /// <summary>
        /// Gets the parent matrix object.
        /// </summary>
        public Matrix<T,C> Parent { get { return parent; } }

        // ---------------------
        // ------- ctors -------
        // ---------------------

        internal Submatrix(Matrix<T,C> matrix, int rowOffset, int columnOffset, int rows, int columns)
        {
            if (matrix is Matrix_SDA<T, C>) 
                mt = MatrixType.SDA;
            else 
                mt = MatrixType.DI;

            this.offsetRow = rowOffset;
            this.offsetColumn = columnOffset;

            this.rows = rows;
            this.columns = columns;

            this.parent = matrix;
        }

        // -----------------------------------------------------------------------------------

        protected internal override Numeric<T,C> getItemAt(int row, int column)
        {
            return parent.getItemAt(offsetRow + row, offsetColumn + column);
        }

        protected internal override void setItemAt(int row, int column, Numeric<T,C> value)
        {
            parent.setItemAt(offsetRow + row, offsetColumn + column, value);
        }

        // -----------------------------------------------------------------------------------

        public override Matrix<T,C> getSubMatrixCopyAt(int i, int j, int rows, int columns)
        {
            checkPositive(i, j);
            checkBounds(i + rows, j + columns);

            return parent.getSubMatrixCopyAt(offsetRow + i, offsetColumn + j, rows, columns);
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
            return parent.getSubMatrixCopyAt(offsetRow, offsetColumn, rows, columns);
        }

        protected override Matrix<T,C> multiply(Matrix<T,C> another)
        {
            if (this.ColumnCount != another.RowCount)
                throw new ArgumentException("The column count of the first matrix and the row count of the second matrix must match.");

            Matrix<T,C> temp = MatrixNumericHelper<T,C>.getMatrixOfSize(mt, this.rows, another.ColumnCount);
            MatrixNumericHelper<T,C>.multiplySimple(this, another, temp);

            return temp;
        }

        protected override Matrix<T,C> negate()
        {
            Matrix<T,C> temp = MatrixNumericHelper<T,C>.getMatrixOfSize(mt, rows, columns);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    temp[i, j] = -this.getItemAt(i, j);

            return temp;
        }

        protected override Matrix<T,C> sum(Matrix<T,C> another)
        {
            if (this.rows != another.RowCount || this.columns != another.ColumnCount)
                throw new ArgumentException("Matrices must be of the same size in order to sum.");

            Matrix<T,C> temp = MatrixNumericHelper<T,C>.getMatrixOfSize(mt, rows, columns);
            MatrixNumericHelper<T,C>.sum(this, another, temp);
            return temp;
        }

        protected override Matrix<T,C> substract(Matrix<T,C> another)
        {
            if (this.rows != another.RowCount || this.columns != another.ColumnCount)
                throw new ArgumentException("Matrices must be of the same size in order to substract.");

            Matrix<T,C> temp = MatrixNumericHelper<T,C>.getMatrixOfSize(mt, rows, columns);
            MatrixNumericHelper<T,C>.dif(this, another, temp);
            return temp;
        }
    }
}
