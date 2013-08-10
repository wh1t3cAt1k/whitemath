#if OLD_VERSION

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath.Matrices
{
    /// <summary>
    /// The class represents the submatrix of a double matrix.
    /// All matrix operations supported, but the changes made to the submatrix will be reflected 
    /// on the parent matrix object. The mechanism uses an encapsulated parent matrix 
    /// reference.
    /// 
    /// An object of this class cannot be instantiated directly.
    /// Call method getSubmatrixAt() for a particular DoubleMatrix object instead.
    /// </summary>
    public class DoubleSubmatrix: DoubleMatrix
    {
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

        protected Matrix<double> parent;
        
        /// <summary>
        /// Gets the parent matrix object.
        /// </summary>
        public Matrix<double> Parent { get { return parent; } }

        private MatrixType mt;

        internal DoubleSubmatrix(Matrix<double> matrix, int rowOffset, int columnOffset, int rows, int columns)
        {
            if (typeof(Matrix_SDA).IsInstanceOfType(matrix)) mt = MatrixType.SDA; // дописать
            else mt = MatrixType.SDA;

            this.offsetRow = rowOffset;
            this.offsetColumn = columnOffset;

            this.rows = rows;
            this.columns = columns;

            this.parent = matrix;
        }

        protected internal override double getItemAt(int row, int column)
        {
            return parent.getItemAt(offsetRow + row, offsetColumn + column);
        }

        protected internal override void setItemAt(int row, int column, double value)
        {
            parent.setItemAt(offsetRow + row, offsetColumn + column, value);
        }

        public override Matrix<double> getSubMatrixCopyAt(int i, int j, int rows, int columns)
        {
            checkPositive(i, j);
            checkBounds(i + rows, j + columns);

            return parent.getSubMatrixCopyAt(offsetRow + i, offsetColumn + j, rows, columns);
        }
        
        /// <summary>
        /// Lays a matrix over the current, reflecting the changes on the parent Matrix object.
        /// </summary>
        /// <param name="subMatrix">The matrix to lay over the current.</param>
        /// <param name="i">The row coordinate in the current submatrix.</param>
        /// <param name="j">The column coordinate in the current submatrix.</param>
        public override void layMatrixAt(Matrix<double> subMatrix, int i, int j)
        {
            this.checkPositive(i, j);
            this.checkBounds(subMatrix.RowCount + i, subMatrix.ColumnCount + j);

            parent.layMatrixAt(subMatrix, offsetRow + i, offsetColumn + j);
        }

        /// <summary>
        /// Converts the current submatrix to a double[,] array. 
        /// </summary>
        /// <returns>A two-dimensional array containing all the elements of the current submatrix.</returns>
        public override double[,] convertToArray()
        {
            double[,] arr = new double[rows, columns];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    arr[i, j] = parent.getItemAt(offsetRow + i, offsetColumn + j);

            return arr;
        }

        /// <summary>
        /// Returns the COPY of the current submatrix as a stand-alone Matrix object.
        /// The changes made to it will never reflect the parent matrix.
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return parent.getSubMatrixCopyAt(offsetRow, offsetColumn, rows, columns);
        }

        public override Matrix<double> getSubMatrixAt(int i, int j, int rows, int columns)
        {
            checkPositive(rows, columns);
            checkBounds(i + rows, j + columns);

            return new DoubleSubmatrix(this, i, j, rows, columns);
        }

        private Matrix<double> getMatrixOfSize(int rows, int columns)
        {
            switch (mt)
            {
                case MatrixType.SDA: return new Matrix_SDA(rows, columns);
                default: return new Matrix_SDA(rows, columns);
            }
        }

        protected override Matrix<double> multiply(Matrix<double> another)
        {
            if (this.ColumnCount != another.RowCount)
                throw new ArgumentException("The column count of the first matrix and the row count of the second matrix must match.");

            Matrix<double> temp = getMatrixOfSize(this.rows, another.ColumnCount);
            MatrixHelper.multiplySimple(this, another, temp);

            return temp;
        }

        protected override Matrix<double> negate()
        {
            Matrix<double> temp = getMatrixOfSize(rows, columns);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    temp[i, j] = -this.getItemAt(i, j);

            return temp;
        }

        protected override Matrix<double> sum(Matrix<double> another)
        {
            if (this.rows != another.RowCount || this.columns != another.ColumnCount)
                throw new ArgumentException("Matrices must be of the same size in order to sum.");

            Matrix<double> temp = getMatrixOfSize(rows, columns);
            MatrixHelper.sum(this, another, temp);
            return temp;
        }

        protected override Matrix<double> substract(Matrix<double> another)
        {
            if (this.rows != another.RowCount || this.columns != another.ColumnCount)
                throw new ArgumentException("Matrices must be of the same size in order to substract.");

            Matrix<double> temp = getMatrixOfSize(rows, columns);
            MatrixHelper.dif(this, another, temp);
            return temp;
        }
    }
}

#endif