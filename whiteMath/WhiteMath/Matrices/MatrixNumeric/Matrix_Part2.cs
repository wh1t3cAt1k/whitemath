using System;

using WhiteMath.Mathematics;
using WhiteMath.Calculators;
using WhiteMath.General;
using WhiteMath.Combinatorics;

namespace WhiteMath.Matrices
{
    public abstract partial class Matrix<T,C> where C: ICalc<T>, new()
    {
        // ------------------------
        // ---boolean properties---
        // ------------------------

        /// <summary>
        /// Tests if current matrix is a symmetric square matrix.
        /// If the matrix is not square, a MatrixSizeException will be thrown.
        /// </summary>
        public bool IsSymmetric
        {
            get 
            {
                this.checkSquare();

                bool flag = true;

                for (int i = 0; i < this.rows; i++)
                    for (int j = i; j < this.columns; j++)
                        flag &= (Numeric<T, C>.NumericComparer.Compare(this.getItemAt(i, j), this.getItemAt(j, i)) == 0);

                return flag;
            }
        }

        // ------------------------
        // ---- interfaces --------
        // ------------------------

        public static Matrix<T,C> operator +(Matrix<T,C> one, Numeric<T,C> two)
        {
            return one.addValue(two);
        }

        public static Matrix<T,C> operator -(Matrix<T,C> one, Numeric<T,C> two)
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
                this.checkSquare();     // check that this is a square matrix.
                
                Numeric<T, C> sum = Numeric<T, C>.Zero;

                for (int i = 0; i < this.rows; i++)
                    sum += this.getItemAt(i, i);

                return sum;
            }
        }

        // ------------------- DETERMINANT ------------------------------

        /// <summary>
        /// Performs the matrix determinant calculation using the LUP-factorization
        /// of the matrix.
        /// </summary>
        /// <returns></returns>
        public Numeric<T, C> Determinant_LUP_Factorization()
        {
            int[] P;
            Matrix_SDA<T,C> C;

            // -------------------------------------------------------------
            // LUP-factorize the matrix. If cannot, the determinant is zero.

            try
            {
                this.LUP_Factorization(out P, out C);
            }
            catch (MatrixSingularityException)
            {
                return Numeric<T, C>.Zero;
            }

            int k = P.InversionCount<int, CalcInt>();

            Numeric<T, C> mul = C.getItemAt(0, 0);
            
            for (int i = 1; i < this.rows; i++)
                mul *= C.getItemAt(i, i);
            
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
        public Numeric<T, C> Determinant_Permutations()
        {
            if (this.rows != this.columns)
                throw new MatrixSizeException("Only square matrices can have a determinant.");

            int n = this.rows;
 
            if      (n==0)    return Numeric<T,C>.Zero;
            else if (n==1)    return this.getItemAt(0,0).Copy;
            
            int[] indices = new int[n];

            indices.FillByAssign(delegate(int i) { return i; });

            LexicographicPermutator<int> perm = new LexicographicPermutator<int>(indices);

            Numeric<T, C> sum = Numeric<T, C>.Zero;
            Numeric<T, C> multiplication;

            do
            {
                int inversions = perm.InversionCount<int, CalcInt>();

                multiplication = this.getItemAt(perm[0], 0);

                for (int i = 1; i < n && multiplication != Numeric<T,C>.Zero; i++)
                    multiplication *= this.getItemAt(perm[i], i);

                if (inversions % 2 == 0)
                    sum += multiplication;
                else
                    sum -= multiplication;

            } while (perm.CreateNextPermutation());

            return sum;
        }

        // ------------------- MATRIX MINOR -----------------------------

        /// <summary>
        /// Returns the dependent minor matrix which is made by hiding
        /// the row with index <paramref name="rowToRemove"/> and the column with
        /// index <paramref name="columnToRemove"/>.
        /// 
        /// Any changes made to this minor matrix will be reflected on the current matrix.
        /// </summary>
        /// <param name="rowToRemove">The row to be removed to form the minor matrix.</param>
        /// <param name="columnToRemove">The column to be removed to form the minor matrix.</param>
        /// <returns>The dependent minor matrix by hiding one row and one column from the current matrix.</returns>
        public Matrix<T, C> getMinorMatrix(int rowToRemove, int columnToRemove)
        {
            checkPositive(rowToRemove, columnToRemove);
            checkBounds(rowToRemove + 1, columnToRemove + 1);

            return new MinorMatrix<T, C>(this, rowToRemove, columnToRemove);
        }

        /// <summary>
        /// Returns the minor matrix copy (independent from the current),
        /// which is made by removing the row with index <paramref name="rowToRemove"/>
        /// and the column with index <paramref name="columnToRemove"/>.
        /// </summary>
        /// <param name="rowToRemove">The index of the row to remove.</param>
        /// <param name="columnToRemove">The index of the column to remove.</param>
        /// <returns>The minor matrix copy (independent from the current).</returns>
        public Matrix<T, C> getMinorMatrixCopy(int rowToRemove, int columnToRemove)
        {
            checkPositive(rowToRemove, columnToRemove);
            checkBounds(rowToRemove + 1, columnToRemove + 1);
         
            Matrix<T, C> tmp = MatrixNumericHelper<T,C>.getMatrixOfSize(this.Matrix_Type, this.rows-1, this.columns-1);
            
            for(int i=0; i<rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    int positionIndexRow    = (i < rowToRemove ? i : (i > rowToRemove ? i - 1 : -1));
                    int positionIndexColumn = (j < columnToRemove ? j : (j > columnToRemove ? j - 1 : -1));

                    if (positionIndexColumn >= 0 && positionIndexRow >= 0)
                        tmp.setItemAt(positionIndexRow, positionIndexColumn, this.getItemAt(i, j));
                }

            return tmp;
        }

        // ------------------- INVERSE MATRIX ---------------------------

        // TO DO: make without minor matrix copy, but with minor matrix itself. Economy of the memory.

        /// <summary>
        /// Calculates the inverse matrix using the LUP-factorization for calculating matrix determinants 
        /// and algebraic supplements of its elements.
        /// It takes O(n^5) operations to run.
        /// 
        /// Works only for square, non-singular matrices.
        /// If these requirements are not met, either a MatrixSizeException 
        /// or a MatrixSingularityException shall be thrown.
        /// </summary>
        public Matrix<T, C> inverseMatrix_LUP_Factorization()
        {
            this.checkSquare();

            Numeric<T, C> determinant = this.Determinant_LUP_Factorization();

            if(determinant == Numeric<T,C>.Zero)
                throw new MatrixSingularityException("cannot calculate the inverse matrix.");

            // to do: make matrix type

            Matrix<T, C> inverse = new Matrix_SDA<T, C>(this.rows, this.columns);
 
            for(int i = 0; i< rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    Numeric<T, C> minor = this.getMinorMatrix(j, i).Determinant_LUP_Factorization();

                    if (((i + j) & 1) == 1)
                        minor = -minor;

                    inverse.setItemAt(i, j, minor / determinant);
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
        public void LUP_Factorization(out int[] P, out Matrix_SDA<T, C> C)
        {
            if (this.rows != this.columns)
                throw new MatrixSizeException("The matrix is not square an thus cannot be factorized.");

            int n = this.rows;  // размер матрицы

            C = new Matrix_SDA<T, C>(n, n);

            for(int i=0; i<n; i++)
                for(int j=0; j<n; j++)
                    C.setItemAt(i, j, this.getItemAt(i, j));

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
                    abs = Mathematics<T, C>.Abs(this.getItemAt(row, i));

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
                    C.swapRows(pivotIndex, i);
                }

                try
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        C.setItemAt(j, i, C.getItemAt(j, i) / C.getItemAt(i, i));

                        if (Numeric<T, C>.IsInfinity(C.getItemAt(j, i)) || Numeric<T, C>.IsNaN(C.getItemAt(j, i)))
                            throw new DivideByZeroException();

                        for (int k = i + 1; k < n; k++)
                            C.setItemAt(j, k, C.getItemAt(j, k) - C.getItemAt(j, i) * C.getItemAt(i, k));
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
        public void LUP_Factorization(out int[] P, out Matrix_SDA<T, C> L, out Matrix_SDA<T, C> U)
        {
            Matrix_SDA<T, C> C;

            this.LUP_Factorization(out P, out C);

            int n = this.rows;

            L = new Matrix_SDA<T, C>(n, n);
            U = new Matrix_SDA<T, C>(n, n);

            Numeric<T, C> one = (Numeric<T, C>)1;

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    if (i < j)
                        U.setItemAt(i, j, C.getItemAt(i, j));
                    else if (i > j)
                        L.setItemAt(i, j, C.getItemAt(i, j));
                    else
                    {
                        L.setItemAt(i, j, one);                         // здесь единицы на диагонали
                        U.setItemAt(i, j, C.getItemAt(i, j));           // E возместить не надо.
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
        public void LUP_Factorization(out Matrix_SDA<T, C> P, out Matrix_SDA<T, C> L, out Matrix_SDA<T, C> U)
        {
            int[] arr;
            int n = this.rows;

            Numeric<T,C> one = (Numeric<T,C>) 1;

            LUP_Factorization(out arr, out L, out U);

            P = new Matrix_SDA<T, C>(n, n);

            for (int i = 0; i < n; i++)
                P.setItemAt(i, arr[i], one);

            return;
        }
    }
}
