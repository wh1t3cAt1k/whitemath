using WhiteStructs.Conditions;

namespace WhiteMath.General
{
    /// <summary>
    /// Creates a zero-based matrix wrapper for 
    /// matrices with arbitrary index base.
    /// </summary>
    public class ZeroBasedMatrixWrapper<T>
    {
        /// <summary>
        /// Gets the row minimum index boundary in the 
        /// parent matrix.
        /// </summary>
        public int ParentMinRowIndex 
        {
            get
            {
                return this.ParentMatrix.GetLowerBound(0);
            }
        }

        /// <summary>
        /// Gets the column minimum index boundary in the 
        /// parent matrix.
        /// </summary>
        public int ParentMinColumnIndex 
        { 
            get 
            {
                return this.ParentMatrix.GetLowerBound(1);
            }
        }

        /// <summary>
        /// Gets the number of rows in the matrix.
        /// </summary>
        public int RowCount
        {
            get
            {
                return this.ParentMatrix.GetLength(0);
            }
        }
        
        /// <summary>
        /// Gets the number of columns in the matrix.
        /// </summary>
        public int ColumnCount
        {
            get
            {
                return this.ParentMatrix.GetLength(1);
            }
        }

        /// <summary>
        /// Gets the parent arbitrarily-indexed matrix object.
        /// </summary>
        public T[,] ParentMatrix { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ZeroBasedMatrixWrapper"/>
        /// object from an arbitrarily-indexed parent matrix. 
        /// </summary>
        /// <param name="parentMatrix">The parent matrix object.</param>
        public ZeroBasedMatrixWrapper(T[,] parentMatrix)
        {
			Condition.ValidateNotNull(parentMatrix, nameof(parentMatrix));

            this.ParentMatrix = parentMatrix; 
        }

        /// <summary>
        /// Gets or sets the element at the 
        /// specified row/column coordinate pair.
        /// </summary>
        /// <param name="indexRow">A zero-based row index of the element to be got or set.</param>
        /// <param name="indexColumn">A zero-based column index of the element to be got or set.</param>
        /// <returns>The element located in the specified row and column.</returns>
        public T this[int indexRow, int indexColumn]
        {
            get
            {
                return 
                    this.ParentMatrix[this.ParentMinRowIndex + indexRow, this.ParentMinColumnIndex + indexColumn];
            }
            set
            {
                this.ParentMatrix[this.ParentMinRowIndex + indexRow, this.ParentMinColumnIndex + indexColumn] = value;
            }
        }

        /// <summary>
        /// Creates a new zero-based two-dimensional array
        /// from the current zero-based wrapper.
        /// </summary>
        /// <returns>
        /// A new zero-based two dimensional array populated
        /// with the same shallow-copied data.
        /// </returns>
        public T[,] ToTwoDimensionalArray()
        {
            T[,] result = new T[this.RowCount, this.ColumnCount];

            for (int indexRow = 0; indexRow < this.RowCount; ++indexRow)
            {
                for (int indexColumn = 0; indexColumn < this.ColumnCount; ++indexColumn)
                {
                    result[indexRow, indexColumn] = this[indexRow, indexColumn];
                }
            }

            return result;
        }
    }
}
