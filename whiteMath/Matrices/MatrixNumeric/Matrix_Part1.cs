using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath.Matrices
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
        
        // Interfaces implemented

        ICloneable, 
        IMutableMatrix,
        IMutableMatrix<T>, 
        IMutableMatrix<Numeric<T,C>> 

        where C:ICalc<T>, new()

    {
        public static C calc = new C();

        /// -----------------------------------
        /// ----- MatrixType-concerned things--
        /// -----------------------------------

        public MatrixType Matrix_Type { get; protected set; }

        /// -------------------------------
        /// ----- Length-concerned things--
        /// -------------------------------

        protected int rows;                   // Matrix row count
        protected int columns;                // Matrix column count

        /// <summary>
        /// Returns the column count for current matrix
        /// </summary>
        public int ColumnCount
        {
            get { return this.columns; }
        }

        /// <summary>
        /// Returns the row count for current matrix
        /// </summary>
        public int RowCount
        {
            get { return this.rows; }
        }

        /// <summary>
        /// Returns the total element count for current matrix
        /// </summary>
        public long ElementCount { get { return RowCount * ColumnCount; } }

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
                checkPositive(row, column);
                checkBounds(row + 1, column + 1);

                return this.getItemAt(row, column);
            }
            set
            {
                checkPositive(row, column);
                checkBounds(row + 1, column + 1);

                this.setItemAt(row, column, value);
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

        /// -----------------------------------
        /// ---------- INDEXERS CONCERNED -----
        /// -----------------------------------

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
        protected internal abstract Numeric<T,C> getItemAt(int row, int column);

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
        protected internal abstract void setItemAt(int row, int column, Numeric<T,C> value);

        /// ----------------------------------
        /// --------TRANSPOSE ----------------
        /// ----------------------------------

        /// <summary>
        /// Transposes the current matrix.
        /// Works only for square matrices, otherwise, a MatrixSizeException will be thrown.
        /// </summary>
        public void transpose()
        {
            if (this.rows != this.columns)
                throw new MatrixSizeException("The matrix is not square.");

            for (int i = 0; i < this.rows; i++)
                for (int j = i; j < this.columns; j++)
                {
                    Numeric<T,C> tmp = this.getItemAt(i, j);

                    this.setItemAt(i, j, this.getItemAt(j, i));
                    this.setItemAt(j, i, tmp);
                }
        }

        /// <summary>
        /// Returns the matrix that will be equal to the current matrix after the transposition.
        /// Performs memory allocation for the new matrix, so any changes made to it will not be reflected on the current matrix.
        /// </summary>
        /// <returns>Returns the matrix that will be equal to the current matrix after the transposition.</returns>
        public Matrix<T, C> transposedMatrixCopy()
        {
            Matrix<T, C> transposedMatrix = MatrixNumericHelper<T,C>.getMatrixOfSize(this.Matrix_Type, this.columns, this.rows);

            for (int i = 0; i < this.rows; i++)
                for (int j = 0; j < this.columns; j++)
                    transposedMatrix.setItemAt(j, i, this.getItemAt(i, j));

            return transposedMatrix;
        }

        /// ----------------------------------
        /// --------ROW AND COLUMN SWAPPING---
        /// ----------------------------------

        /// <summary>
        /// Virtual method. Swaps two rows with specified indices in the matrix.
        /// <param name="rowIndex1">The zero-based index of the first row to be swapped with the second.</param>
        /// <param name="rowIndex2">The zero-based index of the second row to be swapped with the first.</param>
        /// </summary>
        public virtual void swapRows(int rowIndex1, int rowIndex2)
        {
            checkBounds(rowIndex1, 0);
            checkBounds(rowIndex2, 0);

            Numeric<T,C> temp;

            for (int j = 0; j < columns; j++)
            {
                temp = this.getItemAt(rowIndex1, j);
               
                this.setItemAt(rowIndex1, j, this.getItemAt(rowIndex2, j));
                this.setItemAt(rowIndex2, j, temp);
            }
        }

        /// <summary>
        /// Virtual method. Swaps two columns with specified indices in the matrix.
        /// </summary>
        /// <param name="columnIndex1">The zero-based index of the first column to be swapped with the second.</param>
        /// <param name="columnIndex2">The zero-based index of the second column to be swapped with the first.</param>
        public virtual void swapColumns(int columnIndex1, int columnIndex2)
        {
            checkBounds(0, columnIndex1);
            checkBounds(0, columnIndex2);

            Numeric<T, C> temp;

            for (int i = 0; i < rows; i++)
            {
                temp = this.getItemAt(i, columnIndex1);

                this.setItemAt(i, columnIndex1, this.getItemAt(i, columnIndex2));
                this.setItemAt(i, columnIndex2, temp);
            }
        }

        /// -------------------------------
        /// --------ARITHMETIC OPERATORS---
        /// -------------------------------

        public static Matrix<T, C> operator *(Matrix<T, C> one, Matrix<T, C> two)
        {
            return one.multiply(two);
        }

        public static Matrix<T,C> operator +(Matrix<T,C> one, Matrix<T,C> two)
        {
            return one.sum(two);
        }

        public static Matrix<T, C> operator -(Matrix<T, C> one, Matrix<T, C> two)
        {
            return one.sum(two.negate());
        }

        public static Matrix<T,C> operator -(Matrix<T,C> one)
        {
            return one.negate();
        }

        public static bool operator ==(Matrix<T, C> one, Matrix<T, C> two)
        {
            return one.Equals(two);
        }

        public static bool operator !=(Matrix<T, C> one, Matrix<T, C> two)
        {
            return !(one.Equals(two));
        }

        /// --------------------------------
        /// --------SUBMATRICES AND MINORS--
        /// --------------------------------

        /// <summary>
        /// Inserts the submatrix at the specified point. The current matrix WILL NOT BE STRETCHED
        /// if the submatrix is too large. In this case, exception will be thrown.
        /// </summary>
        /// <param name="subMatrix">The submatrix object.</param>
        /// <param name="i">A zero-based row index</param>
        /// <param name="j">A zero-based column index</param>
        public void layMatrixAt(Matrix<T, C> subMatrix, int i, int j)
        {
            checkPositive(i, j);
            checkBounds(i + subMatrix.RowCount, j + subMatrix.ColumnCount);

            for (int k = 0; k < subMatrix.RowCount; k++)
                for (int m = 0; m < subMatrix.ColumnCount; m++)
                    this.setItemAt(i + k, j + m, calc.getCopy(subMatrix.getItemAt(k, m)));
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
            checkPositive(rows, columns);
            checkBounds(i + rows, j + columns);

            Numeric<T,C>[] ma = new Numeric<T,C>[rows * columns];
            
            for (int k = i; k < i + rows; k++)
                for (int m = j; m < j + rows; m++)
                    ma[k * rows + m] = calc.getCopy(this.getItemAt(k, m));

            Matrix_SDA<T,C> temp = new Matrix_SDA<T,C>();
                
                temp.matrixArray = ma;
                temp.rows = rows;
                temp.columns = columns;

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
        /// <returns>The binded submatrix of specified size</returns>
        public Matrix<T, C> getSubMatrixAt(int i, int j, int rows, int columns)
        {
            checkPositive(i, j);
            checkBounds(i + rows, j + columns);

            return new Submatrix<T, C>(this, i, j, rows, columns);
        }

        /// <summary>
        /// Overloaded. Gets the binded submatrix for the current matrix.
        /// Changes to the submatrix WILL affect the current.
        /// </summary>
        /// <param name="i">Row index of the upper-left corner element</param>
        /// <param name="j">Column index of the upper-left corner element</param>
        /// <param name="rows">Row count of the submatrix</param>
        /// <param name="columns">Column count of the submatrix</param>
        /// <returns>The binded submatrix of size [RowCount-i; ColumnCount-j]</returns>
        public Matrix<T, C> getSubMatrixAt(int i, int j)
            { return this.getSubMatrixAt(i, j, RowCount - i, ColumnCount - j); }

        // -------------------------------------------------------------
        // ----------------------- Matrix arithmetic -------------------
        // -------------------------------------------------------------

        protected abstract Matrix<T,C> multiply(Matrix<T,C> another);
        protected abstract Matrix<T,C> substract(Matrix<T,C> another);
        protected abstract Matrix<T,C> sum(Matrix<T,C> another);
        protected abstract Matrix<T,C> negate();

        /// <summary>
        /// Converts the current matrix to a two-dimensional array.
        /// 
        /// Does not use the element copying, so when <typeparamref name="T"/> is a reference type,
        /// all changes made to the array OBJECTS (not to the array itself) will affect the matrix 
        /// elements and vice versa.
        /// 
        /// Use <see cref="MatrixElementCopyAdapter"/> if such behaviour is not expected.
        /// </summary>
        /// <returns></returns>
        public T[,] convertToArray()
        {
            T[,] array = new T[rows, columns];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
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
        /// Use <see cref="MatrixElementCopyAdapter"/> if such behaviour is not expected.
        /// </summary>
        /// <param name="matrix"></param>
        public void convertFromArray(T[,] matrix)
        {
            if (matrix.GetLength(0) != this.RowCount || matrix.GetLength(1) != this.ColumnCount)
                throw new ArgumentException("The size of the array doesn't match with current matrix' size.");

            for (int i = 0; i < this.RowCount; i++)
                for (int j = 0; j < this.ColumnCount; j++)
                    this.setItemAt(i, j, matrix[i, j]);
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
                return false;

            IMatrix reference = obj as IMatrix;

            if (reference.RowCount != this.RowCount || reference.ColumnCount != this.ColumnCount)
                return false;

            if (obj is IMatrix<T>)
            {
                IMatrix<T> alpha = obj as IMatrix<T>;

                for (int i = 0; i < this.rows; i++)
                    for (int j = 0; j < this.columns; j++)
                        if (this.getItemAt(i, j) != alpha.GetElementValue(i,j))
                            return false;
            }
            else
            {
                IMatrix<Numeric<T,C>> alpha = obj as IMatrix<Numeric<T,C>>;

                for (int i = 0; i < this.rows; i++)
                    for (int j = 0; j < this.columns; j++)
                        if (this.getItemAt(i, j) != alpha.GetElementValue(i, j))
                            return false;
            }

            return true;
        }

        // --------------------------------------
        // -------- Interface implementations ---
        // --------------------------------------
   
        void IMutableMatrix.SetElementValue(int row, int column, object value)
        {
            this[row, column] = (Numeric<T, C>)value;
        }

        void IMutableMatrix<T>.SetElementValue(int row, int column, T value)
        {
            this[row, column] = value;
        }
        
        void IMutableMatrix<Numeric<T, C>>.SetElementValue(int row, int column, Numeric<T,C> value) 
        { 
            this[row, column] = value; 
        }

        object IMatrix.GetElementValue(int row, int column) 
        { 
            return this[row, column]; 
        }

        T IMatrix<T>.GetElementValue(int row, int column)
        {
            return this[row, column];
        }

        Numeric<T, C> IMatrix<Numeric<T, C>>.GetElementValue(int row, int column)
        {
            return this[row, column];
        }

        // ------------------------------------------------------------------------------
        // -----------------------------WINDING capabilities-----------------------------

        public T[] unwindToArray(IWinder winder)
        {
            T[] temp = new T[this.ElementCount];

            winder.reset();

            for (int i = 0; i < this.ElementCount; i++)
                temp[i] = this[winder.getNextIndexPair()];

            return temp;
        }

        /// <summary>
        /// Winds a flat array onto current using the IWinder object.
        /// The IWinder object is automatically reset before winding.
        /// IWinder object and current matrix should be have the same dimension (i.e. row count and column count).
        /// </summary>
        /// <param name="winder">An IWinder object. Row and column count should match with current matrix.</param>
        /// <param name="flatMatrix">A single-dimension matrix. Element count should match with current matrix.</param>
        public void windFromArray(IWinder winder, T[] flatMatrix)
        {
            if(flatMatrix.Length!=this.ElementCount)
                throw new ArgumentException("The element count of the current matrix and the element count of the flat matrix must match.");

            winder.reset();
            for (int i = 0; i < this.ElementCount; i++)
            {
                this[winder.getNextIndexPair()] = flatMatrix[i];
            }
        }

        // ------------------------------------------
        // ------------- COPY ADAPTERS --------------
        // ------------------------------------------

        public ElementCopyAdapter getCopyAdapter()
        {
            return new ElementCopyAdapter(this);
        }

        // --------- Service methods

        /// <summary>
        /// Checks if the matrix is square.
        /// Throws MatrixSizeException if this requirement is not met. 
        /// </summary>
        protected void checkSquare()
        {
            if (this.rows != this.columns)
                throw new MatrixSizeException("The matrix is not square.");
        }

        /// <summary>
        /// Checks if the arguments do not exceed the number of total rows and columns respectively.
        /// Throws MatrixSizeException otherwise.
        /// </summary>
        /// <param name="exceedsRows">Argument suspicious to exceed the number of rows.</param>
        /// <param name="exceedsColumns">Argument suspicious to exceed the number of columns.</param>
        protected void checkBounds(int exceedsRows, int exceedsColumns)
        {
            if (exceedsRows > this.RowCount) throw new MatrixSizeException("Invalid row count: out of the matrix bounds.");
            if (exceedsColumns > this.ColumnCount) throw new MatrixSizeException("Invalid column count: out of the matrix bounds.");
        }

        /// <summary>
        /// Checks if the arguments are zero or positive.
        /// </summary>
        protected void checkPositive(params int[] arguments)
        {
            foreach (int a in arguments) if (a < 0) throw new ArgumentException("Negative argument specified.");
        }
    }
}

