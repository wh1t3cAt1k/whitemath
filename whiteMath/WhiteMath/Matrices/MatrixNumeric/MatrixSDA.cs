using System;

using WhiteMath.Calculators;
using WhiteMath.General;

namespace WhiteMath.Matrices
{
    /// <summary>
    /// Matrix containing the single-dimensional array inside.
    /// Works fast.
    /// </summary>
    public class MatrixSDA<T, C>: Matrix<T, C> where C: ICalc<T>, new()
    {
        internal Numeric<T, C>[] _elements;
        
        //--------------- CONSTRUCTORS

        /// <summary>
        /// Constructs a new single-dimensional-array-based Matrix object
        /// </summary>
        /// <param name="rows">Height of the matrix</param>
        /// <param name="columns">Width of the matrix</param>
        public MatrixSDA(int rows, int columns)
        {
            if (rows <= 0 || columns <= 0) throw new ArgumentException("Witdh and height must both be non-negative numbers.");
            
            this._elements = new Numeric<T,C>[rows * columns];
            this._elements.FillByAssign(Numeric<T, C>.Zero);

            this.Matrix_Type = MatrixType.SDA;

            this.RowCount = rows;
            this.ColumnCount = columns;
        }

        /// <summary>
        /// Constructor "internal-only" version
        /// </summary>
        internal MatrixSDA() { }

        /// <summary>
        /// Implementing <see>setItemAt()</see>
        /// WARNING! Does not perform any object copying, just reference copying.
        /// For class types, all changes made to the element passed will be reflected on the matrix element as well.
        /// Please explicitly use the number copy method if needed.
        /// </summary>
        protected internal override void SetItemAt(int row, int column, Numeric<T,C> value)
        {
            _elements[row * this.ColumnCount + column] = value;
        }

        /// <summary>
        /// Implementing <see>getItemAt()</see>
        /// </summary>
        protected internal override Numeric<T, C> GetElementAt(int row, int column)
        {
            return _elements[row * this.ColumnCount + column];
        }
    
        // ------------------ OPERATORS

        /// <summary>
        /// Negates the matrix so that all the elements change their sign to the opposite.
        /// </summary>
        /// <returns></returns>
        protected override Matrix<T,C> Negate()
        {
			MatrixSDA<T,C> result = new MatrixSDA<T,C>(RowCount, ColumnCount);

            result._elements = (Numeric<T,C>[])this._elements.Clone();

			for (int elementIndex = 0; elementIndex < this.ElementCount; elementIndex++)
			{
				result._elements[elementIndex] = -result._elements[elementIndex];
			}

            return result;
        }

        protected override Matrix<T,C> Multiply(Matrix<T,C> another)
        {
			if (this.ColumnCount != another.RowCount)
			{
				throw new ArgumentException("The column count of the first matrix and the row count of the second matrix must match.");
			}

			MatrixSDA<T,C> result = new MatrixSDA<T,C>(this.RowCount, another.ColumnCount);
            MatrixNumericHelper<T,C>.MultiplySimple(this, another, result);

            return result;
        }

        protected override Matrix<T,C> Subtract(Matrix<T,C> another)
        {
			if (this.RowCount != another.RowCount || this.ColumnCount != another.ColumnCount)
			{
				throw new ArgumentException("Matrices must be of the same size in order to substract.");
			}

			if (another is MatrixSDA<T, C>)
            {
				MatrixSDA<T,C> anotherAsMatrixSDA = another as MatrixSDA<T, C>;
                MatrixSDA<T,C> newMatrix = new MatrixSDA<T,C>(this.RowCount, this.ColumnCount);

				for (int elementIndex = 0; elementIndex < this.ElementCount; ++elementIndex)
				{
					newMatrix._elements[elementIndex] = 
						this._elements[elementIndex] 
						- anotherAsMatrixSDA._elements[elementIndex];
				}

                return newMatrix;
            }
            else
            {
                MatrixSDA<T,C> newMatrix = new MatrixSDA<T,C>(this.RowCount, this.ColumnCount);

				for (int elementIndex = 0; elementIndex < this.ElementCount; ++elementIndex)
				{
					newMatrix._elements[elementIndex] = 
						this._elements[elementIndex] 
						- another.GetElementAt(elementIndex / ColumnCount, elementIndex % ColumnCount);
				}

                return newMatrix;
            }
        }

        protected override Matrix<T,C> Add(Matrix<T,C> another)
        {
			if (this.RowCount != another.RowCount || this.ColumnCount != another.ColumnCount)
			{
				throw new MatrixSizeException("Matrices must be of the same size in order to add together.");
			}

            if (this.GetType().IsInstanceOfType(another))
            {
                MatrixSDA<T,C> temp = (MatrixSDA<T,C>)another;
                MatrixSDA<T,C> newMatrix = new MatrixSDA<T,C>(this.RowCount, this.ColumnCount);

				for (int elementIndex = 0; elementIndex < this.ElementCount; ++elementIndex)
				{
					newMatrix._elements[elementIndex] = this._elements[elementIndex] + temp._elements[elementIndex];
				}

                return newMatrix;
            }
            else
            {
                MatrixSDA<T,C> newMatrix = new MatrixSDA<T,C>(this.RowCount, this.ColumnCount);

				for (int elementIndex = 0; elementIndex < this.ElementCount; ++elementIndex)
				{
					newMatrix._elements[elementIndex] = 
						this._elements[elementIndex] 
						+ another.GetElementAt(elementIndex / ColumnCount, elementIndex % ColumnCount);
				}

                return newMatrix;
            }
        }

        /// <summary>
        /// Provides a deep copy of the current matrix.
        /// </summary>
        /// <returns>The cloned matrix.</returns>
        public override object Clone()
        {
            MatrixSDA<T,C> temp = new MatrixSDA<T,C>();

            temp._elements = (Numeric<T,C>[])this._elements.Clone();
            temp.RowCount = this.RowCount;
            temp.ColumnCount = this.ColumnCount;

            return temp;
        }
    }
}
