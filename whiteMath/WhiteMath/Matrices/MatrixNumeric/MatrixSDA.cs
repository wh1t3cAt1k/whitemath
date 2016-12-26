using System;

using WhiteMath.Calculators;
using WhiteMath.General;

namespace WhiteMath.Matrices
{
    /// <summary>
    /// Matrix containing the single-dimensional array inside.
    /// Works fast.
    /// </summary>
    public class Matrix_SDA<T, C>: Matrix<T, C> where C: ICalc<T>, new()
    {
        internal Numeric<T, C>[] matrixArray;       // Single-dimensional array containing the matrix
        
        //--------------- CONSTRUCTORS

        /// <summary>
        /// Constructs a new single-dimensional-array-based Matrix object
        /// </summary>
        /// <param name="rows">Height of the matrix</param>
        /// <param name="columns">Width of the matrix</param>
        public Matrix_SDA(int rows, int columns)
        {
            if (rows <= 0 || columns <= 0) throw new ArgumentException("Witdh and height are both must be non-negative numbers.");
            
            this.matrixArray = new Numeric<T,C>[rows * columns];
            this.matrixArray.FillByAssign(Numeric<T, C>.Zero);              // ОБЯЗАТЕЛЬНО! Иначе будет default(T). Это может быть null!

            this.Matrix_Type = MatrixType.SDA;

            this.rows = rows;
            this.columns = columns;
        }

        /// <summary>
        /// Constructor "internal-only" version
        /// </summary>
        internal Matrix_SDA() { }

        // --------------- Overriding properties of abstract matrix

        /// <summary>
        /// Implementing <see>setItemAt()</see>
        /// WARNING! Does not perform any object copying, just reference copying.
        /// For class types, all changes made to the element passed will be reflected on the matrix element as well.
        /// Please explicitly use the number copy method if needed.
        /// </summary>
        protected internal override void setItemAt(int row, int column, Numeric<T,C> value)
        {
            matrixArray[row * this.columns + column] = value;
        }

        /// <summary>
        /// Implementing <see>getItemAt()</see>
        /// </summary>
        protected internal override Numeric<T, C> getItemAt(int row, int column)
        {
            return matrixArray[row * this.columns + column];
        }
    
        // ------------------ OPERATORS

        /// <summary>
        /// Negates the matrix so that all the elements change their sign to the opposite.
        /// </summary>
        /// <returns></returns>
        protected override Matrix<T,C> negate()
        {
            Matrix_SDA<T,C> temp = new Matrix_SDA<T,C>(rows, columns);
            temp.matrixArray = (Numeric<T,C>[])this.matrixArray.Clone();

            for (int i = 0; i < this.ElementCount; i++)
                temp.matrixArray[i] = -temp.matrixArray[i];

            return temp;
        }

        protected override Matrix<T,C> multiply(Matrix<T,C> another)
        {
            if (this.columns != another.RowCount)
                throw new ArgumentException("The column count of the first matrix and the row count of the second matrix must match.");

            Matrix_SDA<T,C> temp = new Matrix_SDA<T,C>(this.rows, another.ColumnCount);
            MatrixNumericHelper<T,C>.multiplySimple(this, another, temp);

            return temp;
        }

        protected override Matrix<T,C> substract(Matrix<T,C> another)
        {
            if (this.rows!=another.RowCount || this.columns!=another.ColumnCount)
                throw new ArgumentException("Matrices must be of the same size in order to substract.");

            // If the matrix is SDA, we can do it quick'n'lucky.
            if (this.GetType().IsInstanceOfType(another))
            {
                Matrix_SDA<T,C> temp = (Matrix_SDA<T,C>)another;
                Matrix_SDA<T,C> newMatrix = new Matrix_SDA<T,C>(this.rows, this.columns);

                for (int i = 0; i < this.ElementCount; i++)
                    newMatrix.matrixArray[i] = this.matrixArray[i] - temp.matrixArray[i];

                return newMatrix;
            }
            // Here comes the bad case
            else
            {
                Matrix_SDA<T,C> newMatrix = new Matrix_SDA<T,C>(this.rows, this.columns);

                for (int i = 0; i < this.ElementCount; i++)
                    newMatrix.matrixArray[i] = this.matrixArray[i] - another.getItemAt(i / columns, i % columns);

                return newMatrix;
            }
        }

        protected override Matrix<T,C> sum(Matrix<T,C> another)
        {
            if (this.rows != another.RowCount || this.columns != another.ColumnCount)
                throw new MatrixSizeException("Matrices must be of the same size in order to sum.");

            // If the matrix is SDA, we can do it quick'n'lucky.
            if (this.GetType().IsInstanceOfType(another))
            {
                Matrix_SDA<T,C> temp = (Matrix_SDA<T,C>)another;
                Matrix_SDA<T,C> newMatrix = new Matrix_SDA<T,C>(this.rows, this.columns);

                for (int i = 0; i < this.ElementCount; i++)
                    newMatrix.matrixArray[i] = this.matrixArray[i] + temp.matrixArray[i];

                return newMatrix;
            }
            // Here comes the bad case
            else
            {
                Matrix_SDA<T,C> newMatrix = new Matrix_SDA<T,C>(this.rows, this.columns);

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
            Matrix_SDA<T,C> temp = new Matrix_SDA<T,C>();

            temp.matrixArray = (Numeric<T,C>[])this.matrixArray.Clone();
            temp.rows = this.rows;
            temp.columns = this.columns;

            return temp;
        }
    }
}
