using System;

using WhiteMath.Mathematics;
using WhiteMath.Calculators;
using WhiteMath.General;
using WhiteMath.Combinatorics;

namespace WhiteMath.Matrices
{
    public abstract partial class Matrix<T, C> 
		where C: ICalc<T>, new()
    {
        /// <summary>
        /// Tests if current matrix is a symmetric square matrix.
		/// If the matrix is not square, a <see cref="MatrixSiz"/> will be thrown.
        /// </summary>
        public bool IsSymmetric
        {
            get 
            {
                EnsureIsSquare();

				bool result = true;

				for (int rowIndex = 0; rowIndex < this.RowCount; ++rowIndex)
				{
					for (int columnIndex = rowIndex; columnIndex < this.ColumnCount; ++columnIndex)
					{
						result &= (Numeric<T, C>.NumericComparer.Compare(
							this.GetElementAt(rowIndex, columnIndex), 
							this.GetElementAt(columnIndex, rowIndex)) == 0);
					}
				}

                return result;
            }
        }

        // ------------------------
        // ---- interfaces --------
        // ------------------------

        public static Matrix<T, C> operator +(Matrix<T,C> one, Numeric<T,C> two)
        {
            return one.addValue(two);
        }

        public static Matrix<T, C> operator -(Matrix<T,C> one, Numeric<T,C> two)
        {
            return one.addValue(-two);
        }

        public static Matrix<T,C> operator *(Matrix<T,C> one, Numeric<T,C> two)
        {
            return one.multiplyByValue(two);
        }

        public static Matrix<T,C> operator /(Matrix<T,C> one, Numeric<T,C> two)
        {
            return one.divideByValue(two);
        }
        
        /// <summary>
        /// Adds a double value to all of the matrix elements.
        /// </summary>
        /// <param name="value">The integer value added to all of the matrix elements.</param>
        /// <returns>The result matrix.</returns>
        protected virtual Matrix<T,C> addValue(Numeric<T,C> value)
        {
            Matrix<T,C> temp = this.Clone() as Matrix<T,C>;
            MatrixNumericHelper<T,C>.addValue(temp, value, temp);

            return temp;
        }

        /// <summary>
        /// Multiplies all of the matrix elements by a double value.
        /// </summary>
        /// <param name="value">The double value.</param>
        /// <returns>The result matrix.</returns>
        protected virtual Matrix<T,C> multiplyByValue(Numeric<T,C> value)
        {
            Matrix<T, C> temp = this.Clone() as Matrix<T, C>;
            MatrixNumericHelper<T,C>.mulValue(temp, value, temp);
            
            return temp;
        }

        /// <summary>
        /// Divides all of the matrix elements by a double value.
        /// </summary>
        /// <param name="value">The double value.</param>
        /// <returns>The result matrix.</returns>
        protected virtual Matrix<T,C> divideByValue(Numeric<T,C> value)
        {
            Matrix<T,C> temp = this.Clone() as Matrix<T,C>;
            MatrixNumericHelper<T,C>.divValue(temp, value, temp);
            return temp;
        }

        // ------------------- TRACE ------------------------------------

        /// <summary>
        /// Returns the trace (the sum of diagonal elements) of current matrix if it is square.
        /// If it is not, a MatrixSizeException is thrown.
        /// </summary>
        public Numeric<T, C> Trace
        {
            get
            {
                this.EnsureIsSquare();     // check that this is a square matrix.
                
                Numeric<T, C> sum = Numeric<T, C>.Zero;

                for (int i = 0; i < this.RowCount; i++)
                    sum += this.GetElementAt(i, i);

                return sum;
            }
        }

        // ------------------- DETERMINANT ------------------------------

        /// <summary>
        /// Performs the matrix determinant calculation using the LUP-factorization
        /// of the matrix.
        /// </summary>
        /// <returns></returns>
		public Numeric<T, C> CalculateDeterminantLupFactorization()
        {
            int[] P;
            MatrixSDA<T, C> C;

            try
            {
				this.LupFactorize(out P, out C);
            }
            catch (MatrixSingularityException)
            {
                return Numeric<T, C>.Zero;
            }

            int k = P.InversionCount<int, CalcInt>();

            Numeric<T, C> mul = C.GetElementAt(0, 0);
            
            for (int i = 1; i < this.RowCount; i++)
                mul *= C.GetElementAt(i, i);
            
            return (k % 2 == 0 ? mul : -mul);
        }

        /// <summary>
        /// Counts the matrix determinant using the definition of matrix determinant.
        /// Creates all of the row index permutations: thus (WARNING!)
        /// the complexity of algorithm is O(n!) operations for square matrices of size 'n'.
        /// 
        /// Please do not use for matrices bigger than 7-10.
        /// </summary>
        /// <returns>The value of matrix determinant.</returns>
		public Numeric<T, C> CalculateDeterminantPermutations()
        {
            if (this.RowCount != this.ColumnCount)
                throw new MatrixSizeException("Only square matrices can have a determinant.");

			int rowCount = this.RowCount;

			if (rowCount == 0)
			{
				return Numeric<T, C>.Zero;
			}
			else if (rowCount == 1)
			{
				return this.GetElementAt(0, 0).Copy;
			}

            int[] indices = new int[rowCount];

			indices.FillByAssign(index => index);

            LexicographicPermutator<int> perm = new LexicographicPermutator<int>(indices);

			Numeric<T, C> result = Numeric<T, C>.Zero;
            Numeric<T, C> product;

            do
            {
				int inversionsCount = perm.InversionCount<int, CalcInt>();

				product = this.GetElementAt(perm[0], 0);

				for (int i = 1; i < rowCount && product != Numeric<T, C>.Zero; i++)
				{
					product *= this.GetElementAt(perm[i], i);
				}

				if (inversionsCount % 2 == 0)
				{
					result += product;
				}
				else
				{
					result -= product;
				}
            } 
			while (perm.CreateNextPermutation());

            return result;
        }

        /// <summary>
        /// Returns the dependent minor matrix which is made by hiding
        /// the row with index <paramref name="rowIndexToExclude"/> and the column with
        /// index <paramref name="columnIndexToExclude"/>.
        /// Any changes made to this minor matrix will be reflected on the current matrix.
        /// </summary>
        /// <param name="rowIndexToExclude">The row to be removed to form the minor matrix.</param>
        /// <param name="columnIndexToExclude">The column to be removed to form the minor matrix.</param>
        /// <returns>The dependent minor matrix by hiding one row and one column from the current matrix.</returns>
		public Matrix<T, C> GetMinorMatrix(int rowIndexToExclude, int columnIndexToExclude)
        {
            CheckArePositive(rowIndexToExclude, columnIndexToExclude);
            CheckAreWithinBounds(rowIndexToExclude + 1, columnIndexToExclude + 1);

            return new MinorMatrix<T, C>(this, rowIndexToExclude, columnIndexToExclude);
        }

        /// <summary>
        /// Returns the minor matrix copy (independent from the current),
        /// which is made by removing the row with index <paramref name="rowToRemove"/>
        /// and the column with index <paramref name="columnToRemove"/>.
        /// </summary>
        /// <param name="rowToRemove">The index of the row to remove.</param>
        /// <param name="columnToRemove">The index of the column to remove.</param>
        /// <returns>The minor matrix copy (independent from the current).</returns>
		public Matrix<T, C> GetMinorMatrixCopy(int rowToRemove, int columnToRemove)
        {
            CheckArePositive(rowToRemove, columnToRemove);
            CheckAreWithinBounds(rowToRemove + 1, columnToRemove + 1);
         
			Matrix<T, C> result = MatrixNumericHelper<T,C>.GetMatrixOfSize(
				this.Matrix_Type, 
				this.RowCount - 1, 
				this.ColumnCount - 1);

			for (int rowIndex = 0; rowIndex < RowCount; rowIndex++)
			{
				for (int columnIndex = 0; columnIndex < ColumnCount; columnIndex++)
				{
					int positionIndexRow = rowIndex < rowToRemove 
						? rowIndex 
						: rowIndex > rowToRemove 
						   ? rowIndex - 1 
						   : -1;
					
					int positionIndexColumn = columnIndex < columnToRemove 
						? columnIndex 
						: columnIndex > columnToRemove 
							? columnIndex - 1 
							: -1;

					if (positionIndexColumn >= 0 && positionIndexRow >= 0)
					{
						result.SetItemAt(positionIndexRow, positionIndexColumn, this.GetElementAt(rowIndex, columnIndex));
					}
				}
			}

            return result;
        }

        // ------------------- INVERSE MATRIX ---------------------------

        /// <summary>
        /// Calculates the inverse matrix using the LUP-factorization for calculating matrix determinants 
        /// and algebraic supplements of its elements.
        /// It takes O(n^5) operations to run.
        /// 
        /// Works only for square, non-singular matrices.
        /// If these requirements are not met, either a MatrixSizeException 
        /// or a MatrixSingularityException shall be thrown.
        /// </summary>
        public Matrix<T, C> CalculateInverseMatrixLupFactorization()
        {
            this.EnsureIsSquare();

            Numeric<T, C> determinant = this.CalculateDeterminantLupFactorization();

            if(determinant == Numeric<T,C>.Zero)
                throw new MatrixSingularityException("cannot calculate the inverse matrix.");

            // to do: make matrix type

            Matrix<T, C> inverse = new MatrixSDA<T, C>(this.RowCount, this.ColumnCount);
 
            for(int i = 0; i< RowCount; i++)
                for (int j = 0; j < ColumnCount; j++)
                {
                    Numeric<T, C> minor = this.GetMinorMatrix(j, i).CalculateDeterminantLupFactorization();

                    if (((i + j) & 1) == 1)
                        minor = -minor;

                    inverse.SetItemAt(i, j, minor / determinant);
                }

            return inverse;
        }

        // ------------------- FACTORIZATION ----------------------------

        /// <summary>
        /// Preforms the LUP-factorization of a matrix (let it be A)
        /// in the form of:
        /// 
        /// P*A = L*U.
        /// 
        /// The P is an identity matrix with a plenty of row inversions.
        /// For the economy of space it is provided as a single-dimensional array of integers:
        /// 
        /// (0, 1, 2, 3, ..., n).
        /// 
        /// Element indices of this array stand for matrix rows, and elements value
        /// mean the position of '1' in a row.
        /// 
        /// Requirements: works for any square and nonsingular matrix (having a non-zero determinant).
        /// If these requirements aren't met, either a MatrixSingularException or a MatrixSizeException
        /// would be thrown.
        /// </summary>
        /// <param name="C">The matrix C containing L + U - E. It is clear that both L and U can be easily extracted from this matrix.</param>
        /// <param name="P">The identity matrix with a plenty of row inversions in the form of array.</param>
        public void LupFactorize(out int[] P, out MatrixSDA<T, C> C)
        {
            if (this.RowCount != this.ColumnCount)
                throw new MatrixSizeException("The matrix is not square an thus cannot be factorized.");

            int n = this.RowCount;  // размер матрицы

            C = new MatrixSDA<T, C>(n, n);

            for(int i=0; i<n; i++)
                for(int j=0; j<n; j++)
                    C.SetItemAt(i, j, this.GetElementAt(i, j));

            P = new int[n];
            P.FillByAssign(delegate(int i) { return i; });

            // ----- пошел ----------

            Numeric<T,C> pivot;
            Numeric<T,C> abs;

            int pivotIndex;

            for (int i = 0; i < n; i++)
            {
                pivot = Numeric<T, C>.Zero;
                pivotIndex = -1;

                for (int row = i; row < n; row++)
                {
                    abs = Mathematics<T, C>.Abs(this.GetElementAt(row, i));

                    if (abs > pivot)
                    {
                        pivot = abs;
                        pivotIndex = row;
                    }
                }

                if (pivot == Numeric<T, C>.Zero)
                    throw new MatrixSingularityException("The matrix is singular. It cannot be factorized.");

                if (pivotIndex != i)
                {
                    P.SwapElements(pivotIndex, i);
                    C.SwapRows(pivotIndex, i);
                }

                try
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        C.SetItemAt(j, i, C.GetElementAt(j, i) / C.GetElementAt(i, i));

                        if (Numeric<T, C>.IsInfinity(C.GetElementAt(j, i)) || Numeric<T, C>.IsNaN(C.GetElementAt(j, i)))
                            throw new DivideByZeroException();

                        for (int k = i + 1; k < n; k++)
                            C.SetItemAt(j, k, C.GetElementAt(j, k) - C.GetElementAt(j, i) * C.GetElementAt(i, k));
                    }
                }
                catch (DivideByZeroException)
                {
                    throw new MatrixSingularityException("The matrix is singular. It cannot be factorized.");
                }
            }

            return;
        }

        /// <summary>
        /// Preforms the LUP-factorization of a matrix (let it be A)
        /// in the form of:
        /// 
        /// P*A = L*U.
        /// 
        /// The P is an identity matrix with a plenty of row inversions.
        /// For the economy of space it is provided as a single-dimensional array of integers:
        /// 
        /// (0, 1, 2, 3, ..., n).
        /// 
        /// Element indices of this array stand for matrix rows, and elements value
        /// mean the position of '1' in a row.
        /// 
        /// Requirements: works for any square and nonsingular matrix (having a non-zero determinant).
        /// If these requirements aren't met, either MatrixSingularException of MatrixSizeException
        /// would be thrown.
        /// </summary>
        /// <param name="L">The lower-triangular matrix with '1' on the main diagonal.</param>
        /// <param name="U">The upper-triangular matrix.</param>
        /// <param name="P">The identity matrix with a plenty of row inversions in the form of array.</param>
        public void LUP_Factorization(out int[] P, out MatrixSDA<T, C> L, out MatrixSDA<T, C> U)
        {
            MatrixSDA<T, C> C;

            this.LupFactorize(out P, out C);

            int n = this.RowCount;

            L = new MatrixSDA<T, C>(n, n);
            U = new MatrixSDA<T, C>(n, n);

            Numeric<T, C> one = (Numeric<T, C>)1;

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    if (i < j)
                        U.SetItemAt(i, j, C.GetElementAt(i, j));
                    else if (i > j)
                        L.SetItemAt(i, j, C.GetElementAt(i, j));
                    else
                    {
                        L.SetItemAt(i, j, one);                         // здесь единицы на диагонали
                        U.SetItemAt(i, j, C.GetElementAt(i, j));           // E возместить не надо.
                    }
                }

            return;
        }

        /// <summary>
        /// Preforms the LUP-factorization of a matrix (let it be A)
        /// in the form of:
        /// 
        /// P*A = L*U.
        /// 
        /// The P is an identity matrix with a plenty of row inversions.
        /// 
        /// Requirements: works for any square and nonsingular matrix (having a non-zero determinant).
        /// If these requirements aren't met, either MatrixSingularException of MatrixSizeException
        /// would be thrown.
        /// </summary>
        /// <param name="L">The lower-triangular matrix with '1' on the main diagonal.</param>
        /// <param name="U">The upper-triangular matrix.</param>
        /// <param name="P">The identity matrix with a plenty of row inversions.</param>
        public void LUP_Factorization(out MatrixSDA<T, C> P, out MatrixSDA<T, C> L, out MatrixSDA<T, C> U)
        {
            int[] arr;
            int n = this.RowCount;

            Numeric<T,C> one = (Numeric<T,C>) 1;

            LUP_Factorization(out arr, out L, out U);

            P = new MatrixSDA<T, C>(n, n);

            for (int i = 0; i < n; i++)
                P.SetItemAt(i, arr[i], one);

            return;
        }
    }
}
