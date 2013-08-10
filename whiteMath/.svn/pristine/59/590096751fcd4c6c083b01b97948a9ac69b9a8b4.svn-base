#if OLD_VERSION

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath.Matrices
{
    /// <summary>
    /// Matrix containing the single-dimensional array inside.
    /// Is quickest to work, but definitely not-so-understandable.
    /// </summary>
    public class Matrix_SDA: DoubleMatrix
    {
        private double[] matrixArray;       // Single-dimensional array containing the matrix
        
        //--------------- CONSTRUCTORS

        /// <summary>
        /// Constructs a new single-dimensional-array-based Matrix object
        /// </summary>
        /// <param name="rows">Height of the matrix</param>
        /// <param name="columns">Width of the matrix</param>
        public Matrix_SDA(int rows, int columns)
        {
            if (rows <= 0 || columns <= 0) throw new ArgumentException("Witdh and height are both must be non-negative numbers.");
            this.matrixArray = new double[rows * columns];
            this.rows = rows;
            this.columns = columns;
        }

        /// <summary>
        /// Constructor "private-only" version
        /// </summary>
        private Matrix_SDA() { }

        // --------------- Overriding properties of abstract matrix

        /// <summary>
        /// Implementing <see>DoubleMatrix.setItemAt()</see>
        /// </summary>
        protected internal override void setItemAt(int row, int column, double value)
        {
            matrixArray[row * this.columns + column] = value;
        }

        /// <summary>
        /// Implementing <see>DoubleMatrix.getItemAt()</see>
        /// </summary>
        protected internal override double getItemAt(int row, int column)
        {
            return matrixArray[row * this.columns + column];
        }

        // ------------------ Submatrix operation

        public override Matrix<double> getSubMatrixAt(int i, int j, int rows, int columns)
        {
            checkPositive(i, j);
            checkBounds(i + rows, j + columns);

            return new DoubleSubmatrix(this, i, j, rows, columns);
        }

        public override void layMatrixAt(Matrix<double> subMatrix, int i, int j)
        {
            checkPositive(i,j);
            checkBounds(i + subMatrix.RowCount, j + subMatrix.ColumnCount);

            for(int k=0; k<subMatrix.RowCount; k++)
                for(int m=0; m<subMatrix.ColumnCount; m++)
                    this.setItemAt(i+k, j+m, subMatrix.getItemAt(k,m));
        }

        /// <summary>
        /// Overloaded. Gets the submatrix at the specified point and with specified size.
        /// </summary>
        /// <param name="i">Row index of the upper-left corner element</param>
        /// <param name="j">Columnt index of the upper-left corner element</param>
        /// <param name="rows">Row count of the submatrix</param>
        /// <param name="columns">Column count of the submatrix</param>
        /// <returns>The submatrix of specified size</returns>
        public override Matrix<double> getSubMatrixCopyAt(int i, int j, int rows, int columns)
        {
            checkPositive(rows, columns);
            checkBounds(i + rows, j + columns);

            double[] ma = new double[rows * columns];
            int z=0;

            for (int k = i; k < i + rows; k++)
            {
                Array.Copy(this.matrixArray, this.columns*k + j, ma, z++*columns, columns);
            }

            Matrix_SDA temp = new Matrix_SDA();
            temp.matrixArray = ma;
            temp.rows = rows;
            temp.columns = columns;

            return temp;
        }

        // TODO: setsubmatrix at...

        // ------------------ OPERATORS

        public static implicit operator Matrix_SDA(double[,] doubleMatrix)
        {
            Matrix_SDA temp = new Matrix_SDA();

            temp.rows = doubleMatrix.GetLength(0);
            temp.columns = doubleMatrix.GetLength(1);
            temp.matrixArray = new double[temp.rows * temp.columns];

            int z=0;
            for (int i = 0; i < temp.rows; i++)
                for (int j = 0; j < temp.columns; j++)
                    temp.matrixArray[z++] = doubleMatrix[i, j];

            return temp;
        }

        public static implicit operator double[,](Matrix_SDA matrix)
        {
            double[,] temp = new double[matrix.rows, matrix.columns];

            int z=0;
            for (int i = 0; i < matrix.rows; i++)
                for (int j = 0; j < matrix.columns; j++)
                    temp[i, j] = matrix.matrixArray[z++];

            return temp;
        }

        public override double[,] convertToArray()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Negates the matrix so that all the elements change their sign to the opposite.
        /// </summary>
        /// <returns></returns>
        protected override Matrix<double> negate()
        {
            Matrix_SDA temp = new Matrix_SDA(rows, columns);
            temp.matrixArray = (double[])this.matrixArray.Clone();

            for (int i = 0; i < this.ElementCount; i++)
                temp.matrixArray[i] *= -1;

            return temp;
        }

        protected override Matrix<double> multiply(Matrix<double> another)
        {
            if (this.columns != another.RowCount)
                throw new ArgumentException("The column count of the first matrix and the row count of the second matrix must match.");

            Matrix_SDA temp = new Matrix_SDA(this.rows, another.ColumnCount);
            MatrixHelper.multiplySimple(this, another, temp);
            return temp;
        }

        protected override Matrix<double> substract(Matrix<double> another)
        {
            if (this.rows!=another.RowCount || this.columns!=another.ColumnCount)
                throw new ArgumentException("Matrices must be of the same size in order to substract.");

            // If the matrix is SDA, we can do it quick'n'lucky.
            if (this.GetType().IsInstanceOfType(another))
            {
                Matrix_SDA temp = (Matrix_SDA)another;
                Matrix_SDA newMatrix = new Matrix_SDA(this.rows, this.columns);

                for (int i = 0; i < this.ElementCount; i++)
                    newMatrix.matrixArray[i] = this.matrixArray[i] - temp.matrixArray[i];

                return newMatrix;
            }
            // Here comes the bad case
            else
            {
                Matrix_SDA newMatrix = new Matrix_SDA(this.rows, this.columns);

                for (int i = 0; i < this.ElementCount; i++)
                    newMatrix.matrixArray[i] = this.matrixArray[i] - another.getItemAt(i / columns, i % columns);

                return newMatrix;
            }
        }

        protected override Matrix<double> sum(Matrix<double> another)
        {
            if (this.rows != another.RowCount || this.columns != another.ColumnCount)
                throw new ArgumentException("Matrices must be of the same size in order to sum.");

            // If the matrix is SDA, we can do it quick'n'lucky.
            if (this.GetType().IsInstanceOfType(another))
            {
                Matrix_SDA temp = (Matrix_SDA)another;
                Matrix_SDA newMatrix = new Matrix_SDA(this.rows, this.columns);

                for (int i = 0; i < this.ElementCount; i++)
                    newMatrix.matrixArray[i] = this.matrixArray[i] + temp.matrixArray[i];

                return newMatrix;
            }
            // Here comes the bad case
            else
            {
                Matrix_SDA newMatrix = new Matrix_SDA(this.rows, this.columns);

                for (int i = 0; i < this.ElementCount; i++)
                    newMatrix.matrixArray[i] = this.matrixArray[i] + another.getItemAt(i / columns, i % columns);

                return newMatrix;
            }
        }

        /// <summary>
        /// Provides a deep clone of the current matrix.
        /// </summary>
        /// <returns>The cloned matrix.</returns>
        public override object Clone()
        {
            Matrix_SDA temp = new Matrix_SDA();
            temp.matrixArray = (double[])this.matrixArray.Clone();
            temp.rows = this.rows;
            temp.columns = this.columns;

            return temp;
        }
    }
}

#endif