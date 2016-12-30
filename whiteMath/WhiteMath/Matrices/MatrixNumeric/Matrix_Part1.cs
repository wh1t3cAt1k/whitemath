using System;

using WhiteMath.Calculators;

namespace WhiteMath.Matrices
{
    /// <summary>
    /// Enumeration containing main matrix types, used by MatrixHelper class.
    /// 
    /// 1. Matrix based on a single-dimensional array. Quick element indexation, lower memory consumption.
    /// 2. Matrix based on double indexing. Stupid.
    /// </summary>
    public enum MatrixType
    {
        SDA, TDA, DI, Unknown
    }

    /// <summary>
    /// The generic class for matrices.
    /// </summary>
    /// <typeparam name="T">The type of matrix elements</typeparam>
    public abstract partial class Matrix<T, C> : 
        ICloneable, IMutableMatrix, IMutableMatrix<T>, IMutableMatrix<Numeric<T,C>> 
		where C: ICalc<T>, new()
    {
        public static C calc = new C();

        public MatrixType Matrix_Type { get; protected set; }

        /// <summary>
        /// Returns the column count for current matrix
        /// </summary>
        public int ColumnCount
        {
			get;
			protected set;
        }

        /// <summary>
        /// Returns the row count for current matrix
        /// </summary>
        public int RowCount
        {
			get;
			protected set;
        }

        /// <summary>
        /// Returns the total element count for current matrix
        /// </summary>
        public long ElementCount => RowCount * ColumnCount;

        /// ----------------------------------
        /// ------------- INDEXERS -----------
        /// ----------------------------------

        /// <summary>
        /// Overloaded. Gets or sets the double value using the pair of indexes.
        /// Performs the bounds checking.
        /// 
        /// Does NOT perform the element copying.
        /// All changes made to the reference type element got or set will be reflected on the matrix.
        /// Use <see cref="MatrixCopyAdapter&lt;T,C&gt;"/> if such behaviour is unwanted. 
        /// </summary>
        /// <param name="row">The zero-based row index.</param>
        /// <param name="column">The zero-based column index.</param>
        /// <returns>The double value at specified point.</returns>
        public Numeric<T,C> this[int row, int column]
        {
            get
            {
                CheckArePositive(row, column);
                CheckAreWithinBounds(row + 1, column + 1);

                return GetElementAt(row, column);
            }
            set
            {
                CheckArePositive(row, column);
                CheckAreWithinBounds(row + 1, column + 1);

                SetItemAt(row, column, value);
            }
        }

        /// <summary>
        /// Overloaded. Gets or sets the numeric value using the pair of indexes.
        /// Performs the bounds checking.
        /// 
        /// Does NOT perform the element copying.
        /// All changes made to the reference type element got or set will be reflected on the matrix.
        /// Use <see cref="MatrixElementCopyAdapter"/> if such behaviour is unwanted.
        /// </summary>
        /// <param name="indexPair">The IndexPair object containing a pair of indexes.</param>
        /// <returns>The double value at specified point.</returns>
        public Numeric<T,C> this[IndexPair indexPair]
        {
            get { return this[indexPair.row, indexPair.column]; }
            set { this[indexPair.row, indexPair.column] = value; }
        }

        // -----------------------------------
        // ---------- INDEXERS-RELATED -------
        // -----------------------------------

        /// <summary>
        /// Returns the value at specified matrix index.
        /// 
        /// Contract: 
        /// 
        /// 1. should NOT perform any speed-lowering index-bound checking, because all checking has
        /// been already performed at DoubleMatrix level in this[,] indexer.
        /// 
        /// 2. should NOT perform any element copying for class types.
        /// </summary>
        /// <param name="row">The target row of the matrix.</param>
        /// <param name="column">The target column of the matrix.</param>
        /// <returns>Value at the specified index pair.</returns>
        protected internal abstract Numeric<T,C> GetElementAt(int row, int column);

        /// <summary>
        /// Sets the value at the specified index pair.
        /// 
        /// Contract: 
        /// 
        /// 1. should NOT perform any speed-lowering index-bound checking, because all checking has
        /// been already performed at DoubleMatrix level in this[,] indexer.
        /// 
        /// 2. should NOT perform any element copying for class types.
        /// </summary>
        /// <param name="row">The ROW!</param>
        /// <param name="column">The COLUMN!</param>
        /// <param name="value">The VALUE TO SET!</param>
        protected internal abstract void SetItemAt(int row, int column, Numeric<T,C> value);

        // ----------------------------------
        // --------TRANSPOSE ----------------
        // ----------------------------------

        /// <summary>
        /// Transposes the current matrix in-place.
        /// </summary>
		/// <remarks>
		/// Works only for square matrices, otherwise, a 
		/// <see cref="MatrixSizeException"/> will be thrown.
		/// </remarks>
		public void TransposeInPlace()
        {
			if (this.RowCount != this.ColumnCount)
			{
				throw new MatrixSizeException("The matrix is not square.");
			}

			for (int i = 0; i < this.RowCount; i++)
			{
				for (int j = i; j < this.ColumnCount; j++)
				{
					Numeric<T, C> tmp = this.GetElementAt(i, j);

					this.SetItemAt(i, j, this.GetElementAt(j, i));
					this.SetItemAt(j, i, tmp);
				}
			}
        }

        /// <summary>
        /// Returns the matrix that will be equal to the current matrix after the transposition.
        /// Performs memory allocation for the new matrix, so any changes made to it will not be 
		/// reflected on the current matrix.
        /// </summary>
        /// <returns>Returns the matrix that will be equal to the current matrix after the transposition.</returns>
        public Matrix<T, C> GetTransposedMatrix()
        {
            Matrix<T, C> transposedMatrix = MatrixNumericHelper<T,C>.GetMatrixOfSize(this.Matrix_Type, this.ColumnCount, this.RowCount);

            for (int i = 0; i < this.RowCount; i++)
                for (int j = 0; j < this.ColumnCount; j++)
                    transposedMatrix.SetItemAt(j, i, this.GetElementAt(i, j));

            return transposedMatrix;
        }

        // ----------------------------------
        // --------ROW AND COLUMN SWAPPING---
        // ----------------------------------

        /// <summary>
        /// Virtual method. Swaps two rows with specified indices in the matrix.
        /// <param name="rowIndex1">The zero-based index of the first row to be swapped with the second.</param>
        /// <param name="rowIndex2">The zero-based index of the second row to be swapped with the first.</param>
        /// </summary>
        public virtual void swapRows(int rowIndex1, int rowIndex2)
        {
            CheckAreWithinBounds(rowIndex1, 0);
            CheckAreWithinBounds(rowIndex2, 0);

            Numeric<T,C> temp;

            for (int j = 0; j < ColumnCount; j++)
            {
                temp = this.GetElementAt(rowIndex1, j);
               
                this.SetItemAt(rowIndex1, j, this.GetElementAt(rowIndex2, j));
                this.SetItemAt(rowIndex2, j, temp);
            }
        }

        /// <summary>
        /// Virtual method. Swaps two columns with specified indices in the matrix.
        /// </summary>
        /// <param name="columnIndex1">The zero-based index of the first column to be swapped with the second.</param>
        /// <param name="columnIndex2">The zero-based index of the second column to be swapped with the first.</param>
        public virtual void swapColumns(int columnIndex1, int columnIndex2)
        {
            CheckAreWithinBounds(0, columnIndex1);
            CheckAreWithinBounds(0, columnIndex2);

            Numeric<T, C> temp;

            for (int i = 0; i < RowCount; i++)
            {
                temp = this.GetElementAt(i, columnIndex1);

                this.SetItemAt(i, columnIndex1, this.GetElementAt(i, columnIndex2));
                this.SetItemAt(i, columnIndex2, temp);
            }
        }

        /// -------------------------------
        /// --------ARITHMETIC OPERATORS---
        /// -------------------------------

        public static Matrix<T, C> operator *(Matrix<T, C> one, Matrix<T, C> two)
        {
            return one.Multiply(two);
        }

        public static Matrix<T,C> operator +(Matrix<T,C> one, Matrix<T,C> two)
        {
            return one.Add(two);
        }

        public static Matrix<T, C> operator -(Matrix<T, C> one, Matrix<T, C> two)
        {
            return one.Add(two.Negate());
        }

        public static Matrix<T,C> operator -(Matrix<T,C> one)
        {
            return one.Negate();
        }

        public static bool operator ==(Matrix<T, C> one, Matrix<T, C> two)
        {
            return one.Equals(two);
        }

        public static bool operator !=(Matrix<T, C> one, Matrix<T, C> two)
        {
            return !(one.Equals(two));
        }

        // --------------------------------
        // --------SUBMATRICES AND MINORS--
        // --------------------------------

        /// <summary>
        /// Inserts the submatrix at the specified point. The current matrix WILL NOT BE STRETCHED
        /// if the submatrix is too large. In this case, exception will be thrown.
        /// </summary>
        /// <param name="subMatrix">The submatrix object.</param>
        /// <param name="i">A zero-based row index</param>
        /// <param name="j">A zero-based column index</param>
        public void layMatrixAt(Matrix<T, C> subMatrix, int i, int j)
        {
            CheckArePositive(i, j);
            CheckAreWithinBounds(i + subMatrix.RowCount, j + subMatrix.ColumnCount);

            for (int k = 0; k < subMatrix.RowCount; k++)
                for (int m = 0; m < subMatrix.ColumnCount; m++)
                    this.SetItemAt(i + k, j + m, calc.GetCopy(subMatrix.GetElementAt(k, m)));
        }
        
        /// <summary>
        /// Overloaded. Gets the stand-alone submatrix copy at the specified point and with specified size.
        /// </summary>
        /// <param name="i">Row index of the upper-left corner element.</param>
        /// <param name="j">Columnt index of the upper-left corner element.</param>
        /// <param name="rows">Row count of the submatrix.</param>
        /// <param name="columns">Column count of the submatrix.</param>
        /// <returns>The submatrix of specified size.</returns>
        public virtual Matrix<T,C> getSubMatrixCopyAt(int i, int j, int rows, int columns)
        {
            CheckArePositive(rows, columns);
            CheckAreWithinBounds(i + rows, j + columns);

            Numeric<T,C>[] ma = new Numeric<T,C>[rows * columns];
            
            for (int k = i; k < i + rows; k++)
                for (int m = j; m < j + rows; m++)
                    ma[k * rows + m] = calc.GetCopy(this.GetElementAt(k, m));

            MatrixSDA<T,C> temp = new MatrixSDA<T,C>();
                
                temp._elements = ma;
                temp.RowCount = rows;
                temp.ColumnCount = columns;

            return temp;
        }
        /// <summary>
        /// Overloaded. Gets the stand-alone submatrix copy at the specified point.
        /// </summary>
        /// <param name="i">Row index of the upper-left corner element</param>
        /// <param name="j">Column index of the upper-left corner element</param>
        /// <returns>The submatrix of size [RowCount-i; ColumnCount-j]</returns>
        public Matrix<T, C> getSubMatrixCopyAt(int i, int j)
            { return this.getSubMatrixCopyAt(i, j, RowCount - i, ColumnCount - j); }

        /// <summary>
        /// Overloaded. Gets the binded submatrix for the current matrix.
        /// Changes to the submatrix WILL affect the current.
        /// </summary>
        /// <param name="i">Row index of the upper-left corner element</param>
        /// <param name="j">Column index of the upper-left corner element</param>
        /// <param name="rows">Row count of the submatrix</param>
        /// <param name="columns">Column count of the submatrix</param>
        /// <returns>A binded submatrix of specified size</returns>
        public Matrix<T, C> getSubMatrixAt(int i, int j, int rows, int columns)
        {
            CheckArePositive(i, j);
            CheckAreWithinBounds(i + rows, j + columns);

            return new Submatrix<T, C>(this, i, j, rows, columns);
        }

        /// <summary>
        /// Overloaded. Gets the binded submatrix for the current matrix.
        /// Changes to the submatrix WILL affect the current.
        /// </summary>
        /// <param name="i">Row index of the upper-left corner element</param>
        /// <param name="j">Column index of the upper-left corner element</param>
        /// <returns>A binded submatrix of size [RowCount-i; ColumnCount-j]</returns>
        public Matrix<T, C> GetSubMatrixAt(int i, int j)
            { return this.getSubMatrixAt(i, j, RowCount - i, ColumnCount - j); }

        // -------------------------------------------------------------
        // ----------------------- Matrix arithmetic -------------------
        // -------------------------------------------------------------

        protected abstract Matrix<T,C> Multiply(Matrix<T,C> another);
        protected abstract Matrix<T,C> Subtract(Matrix<T,C> another);
        protected abstract Matrix<T,C> Add(Matrix<T,C> another);
        protected abstract Matrix<T,C> Negate();

        /// <summary>
        /// Converts the current matrix to a two-dimensional array.
        /// 
        /// Does not use the element copying, so when <typeparamref name="T"/> is a reference type,
        /// all changes made to the array OBJECTS (not to the array itself) will affect the matrix 
        /// elements and vice versa.
        /// 
        /// Use <see cref="ElementCopyAdapter"/> if such behaviour is not expected.
        /// </summary>
        /// <returns></returns>
        public T[,] convertToArray()
        {
            T[,] array = new T[RowCount, ColumnCount];

            for (int i = 0; i < RowCount; i++)
                for (int j = 0; j < ColumnCount; j++)
                    array[i, j] = this[i, j];

            return array;
        }

        /// <summary>
        /// Fills the current matrix with the elements of matching type stored in a 2D array.
        /// The array size and the current matrix' size should match.
        ///
        /// Does not use the element copying, so when <typeparamref name="T"/> is a reference type,
        /// all changes made to the array OBJECTS (not to the array itself) will affect the matrix 
        /// elements and vice versa.
        /// 
        /// Use <see cref="ElementCopyAdapter"/> if such behaviour is not expected.
        /// </summary>
        /// <param name="matrix"></param>
        public void convertFromArray(T[,] matrix)
        {
            if (matrix.GetLength(0) != this.RowCount || matrix.GetLength(1) != this.ColumnCount)
                throw new ArgumentException("The size of the array doesn't match with current matrix' size.");

            for (int i = 0; i < this.RowCount; i++)
                for (int j = 0; j < this.ColumnCount; j++)
                    this.SetItemAt(i, j, matrix[i, j]);
        }

        // ----------------------------------------------------------------
        // ----------------------------- Inherited from interfaces---------
        // ----------------------------------------------------------------

        /// <summary>
        /// Creates a deep copy of current object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public abstract object Clone();

        /// <summary>
        /// Checks whether all of the elements in the current matrix
        /// are equal to the respective elements in the other matrix.
        /// 
        /// The other matrix may be either the IMatrix object containing
        /// <typeparamref name="T"/> elements or Numeric IMatrix object. (e.g. Matrix(T,C)).
        /// </summary>
		public override bool Equals(object obj)
        {
			if (!(obj is IMatrix<Numeric<T, C>>) && !(obj is IMatrix<T>))
			{
				return false;
			}

            IMatrix reference = obj as IMatrix;

			if (reference.RowCount != this.RowCount || reference.ColumnCount != this.ColumnCount)
			{
				return false;
			}

            if (obj is IMatrix<T>)
            {
				IMatrix<T> anotherAsMatrix = obj as IMatrix<T>;

				for (int rowIndex = 0; rowIndex < this.RowCount; ++rowIndex)
				{
					for (int columnIndex = 0; columnIndex < this.ColumnCount; ++columnIndex)
					{
						if (this.GetElementAt(rowIndex, columnIndex) 
						    != anotherAsMatrix.GetElementAt(rowIndex, columnIndex))
						{
							return false;
						}
					}
				}
            }
            else
            {
				IMatrix<Numeric<T,C>> anotherAsNumericMatrix = obj as IMatrix<Numeric<T,C>>;

				for (int rowIndex = 0; rowIndex < this.RowCount; ++rowIndex)
				{
					for (int columnIndex = 0; columnIndex < this.ColumnCount; ++columnIndex)
					{
						if (this.GetElementAt(rowIndex, columnIndex) 
					    	!= anotherAsNumericMatrix.GetElementAt(rowIndex, columnIndex))
						{
							return false;
						}
					}
				}
            }

            return true;
        }
		   
        void IMutableMatrix.SetElementAt(int row, int column, object value)
        {
            this[row, column] = (Numeric<T, C>)value;
        }

        void IMutableMatrix<T>.SetElementAt(int row, int column, T value)
        {
            this[row, column] = value;
        }
        
        void IMutableMatrix<Numeric<T, C>>.SetElementAt(int row, int column, Numeric<T,C> value) 
        { 
            this[row, column] = value; 
        }

        object IMatrix.GetElementAt(int row, int column) 
        { 
            return this[row, column]; 
        }

        T IMatrix<T>.GetElementAt(int row, int column)
        {
            return this[row, column];
        }

        Numeric<T, C> IMatrix<Numeric<T, C>>.GetElementAt(int row, int column)
        {
            return this[row, column];
        }

        public T[] UnwindIntoArray(IWinder winder)
        {
			T[] result = new T[this.ElementCount];

            winder.reset();

			for (int elementIndex = 0; elementIndex < this.ElementCount; ++elementIndex)
			{
				result[elementIndex] = this[winder.GetNextIndexPair()];
			}

            return result;
        }

        /// <summary>
        /// Winds a flat array onto the current matrix using the provided 
		/// <see cref="IWinder"/> object. The <see cref="IWinder"/> object 
		/// is automatically reset before winding.
        /// The winder and the current matrix should have the same dimension 
		/// (i.e. row count and column count).
        /// </summary>
		/// <param name="winder">
		/// An <see cref="IWinder"/> object. 
		/// Row and column count should match with current matrix.
		/// </param>
        /// <param name="flatMatrix">A single-dimension matrix. Element count should match with current matrix.</param>
		public void WindFromArray(IWinder winder, T[] flatMatrix)
        {
			if (flatMatrix.Length != this.ElementCount)
			{
				throw new ArgumentException("The element count of the current matrix and the element count of the flat matrix must match.");
			}

            winder.reset();
            
			for (int elementIndex = 0; elementIndex < this.ElementCount; ++elementIndex)
            {
                this[winder.GetNextIndexPair()] = flatMatrix[elementIndex];
            }
        }

        // ------------------------------------------
        // ------------- COPY ADAPTERS --------------
        // ------------------------------------------

		public ElementCopyAdapter GetCopyAdapter()
        {
            return new ElementCopyAdapter(this);
        }

        // --------- Service methods

        /// <summary>
        /// Checks if the matrix is square.
        /// Throws MatrixSizeException if this requirement is not met. 
        /// </summary>
        protected void EnsureIsSquare()
        {
			if (this.RowCount != this.ColumnCount)
			{
				throw new MatrixSizeException("The matrix is not square.");
			}
        }

        /// <summary>
        /// Checks if the arguments do not exceed the number of total rows and columns respectively.
        /// Throws MatrixSizeException otherwise.
        /// </summary>
        /// <param name="exceedsRows">Argument suspicious to exceed the number of rows.</param>
        /// <param name="exceedsColumns">Argument suspicious to exceed the number of columns.</param>
        protected void CheckAreWithinBounds(int exceedsRows, int exceedsColumns)
        {
            if (exceedsRows > this.RowCount) throw new MatrixSizeException("Invalid row count: out of the matrix bounds.");
            if (exceedsColumns > this.ColumnCount) throw new MatrixSizeException("Invalid column count: out of the matrix bounds.");
        }

        /// <summary>
        /// Checks if the arguments are zero or positive.
        /// </summary>
        protected void CheckArePositive(params int[] arguments)
        {
            foreach (int a in arguments) if (a < 0) throw new ArgumentException("Negative argument specified.");
        }
    }
}

