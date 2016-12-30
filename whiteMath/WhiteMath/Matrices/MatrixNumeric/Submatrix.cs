using System;

using WhiteMath.Calculators;

namespace WhiteMath.Matrices
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
        private MatrixType _matrixType;

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
            if (matrix is MatrixSDA<T, C>) 
                _matrixType = MatrixType.SDA;
            else 
                _matrixType = MatrixType.DI;

            this.offsetRow = rowOffset;
            this.offsetColumn = columnOffset;

            this.RowCount = rows;
            this.ColumnCount = columns;

            this.parent = matrix;
        }

        // -----------------------------------------------------------------------------------

        protected internal override Numeric<T,C> GetElementAt(int row, int column)
        {
            return parent.GetElementAt(offsetRow + row, offsetColumn + column);
        }

        protected internal override void SetItemAt(int row, int column, Numeric<T,C> value)
        {
            parent.SetItemAt(offsetRow + row, offsetColumn + column, value);
        }

        // -----------------------------------------------------------------------------------

        public override Matrix<T,C> getSubMatrixCopyAt(int i, int j, int rows, int columns)
        {
            CheckArePositive(i, j);
            CheckAreWithinBounds(i + rows, j + columns);

            return parent.getSubMatrixCopyAt(offsetRow + i, offsetColumn + j, rows, columns);
        }

        // -----------------------------
        // ----------- different -------
        // -----------------------------

        /// <summary>
        /// Returns the deep copy of the current submatrix as a stand-alone Matrix object.
        /// </summary>
        public override object Clone()
        {
            return parent.getSubMatrixCopyAt(offsetRow, offsetColumn, RowCount, ColumnCount);
        }

        protected override Matrix<T,C> Multiply(Matrix<T,C> another)
        {
            if (this.ColumnCount != another.RowCount)
                throw new ArgumentException("The column count of the first matrix and the row count of the second matrix must match.");

            Matrix<T,C> temp = MatrixNumericHelper<T,C>.GetMatrixOfSize(_matrixType, this.RowCount, another.ColumnCount);
            MatrixNumericHelper<T,C>.MultiplySimple(this, another, temp);

            return temp;
        }

        protected override Matrix<T,C> Negate()
        {
            Matrix<T,C> temp = MatrixNumericHelper<T,C>.GetMatrixOfSize(_matrixType, RowCount, ColumnCount);

            for (int i = 0; i < RowCount; i++)
                for (int j = 0; j < ColumnCount; j++)
                    temp[i, j] = -this.GetElementAt(i, j);

            return temp;
        }

        protected override Matrix<T,C> Add(Matrix<T,C> another)
        {
            if (this.RowCount != another.RowCount || this.ColumnCount != another.ColumnCount)
                throw new ArgumentException("Matrices must be of the same size in order to sum.");

            Matrix<T,C> temp = MatrixNumericHelper<T,C>.GetMatrixOfSize(_matrixType, RowCount, ColumnCount);
            MatrixNumericHelper<T,C>.sum(this, another, temp);
            return temp;
        }

        protected override Matrix<T,C> Subtract(Matrix<T,C> another)
        {
            if (this.RowCount != another.RowCount || this.ColumnCount != another.ColumnCount)
                throw new ArgumentException("Matrices must be of the same size in order to substract.");

            Matrix<T,C> temp = MatrixNumericHelper<T,C>.GetMatrixOfSize(_matrixType, RowCount, ColumnCount);
            MatrixNumericHelper<T,C>.dif(this, another, temp);
            return temp;
        }
    }
}
