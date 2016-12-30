using WhiteMath.Calculators;

namespace WhiteMath.Matrices
{
    /// <summary>
    /// Static class providing different methods to operate with matrices.
    /// Provides standard arithmetic methods with (A, B, res C) signature
    /// as an alternative to overloaded operators in the Matrix class itself - because in this case
    /// the user himself can control the overall number of matrices created, preserving memory.
    /// -----------------
    /// Is thread-safe.
    /// -----------------
    /// Please notice that all of the arithmetic methods DO NOT PERFORM THE OPERANDS' size checking!
    /// If you wish to perform one, please explicitly use the static boolean methods
    /// checkArithmeticSize(...) for most operations and checkMultiplySize(...) for matrix multiplication.
    /// </summary>
    public static class MatrixNumericHelper<T,C> where C: ICalc<T>, new()
    {
        private static C calc = new C();

        /// ---------------------------------------------
        ///             SERVICE METHODS
        /// ---------------------------------------------

        public static MatrixType getMatrixType(Matrix<T, C> matrix)
        {
            if (matrix is MatrixSDA<T, C>)
                return MatrixType.SDA;
            else
                return MatrixType.DI;
        }

        /// <summary>
        /// Gets a blank matrix of the specified size.
        /// The type of the matrix depends on the MatrixType object passed.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static Matrix<T, C> GetMatrixOfSize(MatrixType mt, int rows, int columns)
        {
            switch (mt)
            {
                case MatrixType.SDA: return new MatrixSDA<T, C>(rows, columns);
                default: return new MatrixSDA<T, C>(rows, columns);
            }
        }

        // ---------------------------------------------
        //             MATRIX ARITHMETIC
        // ---------------------------------------------

        /// <summary>
        /// Performs the matrix multiplication using the quick Strassen algorithm.
        /// Consumes more memory than simple iterational method, but much quicker
        /// on bigger dimensions (128+).
        /// 
        /// If the dimension of method parameters passed is N, then maximum additional object memory consumption is 
        /// <value>9N + [if N>64: (log2(N)-6)*f] + o(N)</value>
        /// Where f == (nearly) 4.5*(new Matrix of 2N*2N memory consumption).
        /// </summary>
        /// <param name="A">The matrix to multiply.</param>
        /// <param name="B">The matrix to multiply by.</param>
        /// <param name="result">The resulting matrix of [A.Rows x B.Columns] size.</param>
        public static void multiplyStrassen(Matrix<T,C> A, Matrix<T,C> B, Matrix<T,C> result)
        {
            int exp = 1;
            do exp *= 2; while (A.ColumnCount > exp || A.RowCount > exp || B.ColumnCount > exp || B.RowCount > exp);

            MatrixSDA<T,C> Anew = new MatrixSDA<T,C>(exp, exp);
            MatrixSDA<T,C> Bnew = new MatrixSDA<T,C>(exp, exp);
            MatrixSDA<T,C> ResNew = new MatrixSDA<T,C>(exp, exp);

            Anew.layMatrixAt(A, 0, 0);
            Bnew.layMatrixAt(B, 0, 0);

            strassenSkeleton(Anew, Bnew, ResNew, exp);

            result.layMatrixAt(ResNew.getSubMatrixCopyAt(0, 0, result.RowCount, result.ColumnCount), 0, 0);
        }

        /// <summary>
        /// Private recursive Strassen multiplying method skeleton called implicitly by the wrapper method.
        /// </summary>
        /// <param name="A">The matrix to multiply</param>
        /// <param name="B">The matrix to multiply by</param>
        /// <param name="result">The resulting matrix</param>
        /// <param name="curDim">Current matrix dimension</param>
        private static void strassenSkeleton(Matrix<T,C> A, Matrix<T,C> B, Matrix<T,C> result, int curDim)
        {
            if (curDim <= 64)
            {
                MultiplySimple(A, B, result);
                return;
            }

            int size = curDim / 2;

            Matrix<T,C> A11 = A.getSubMatrixAt(0, 0, size, size);
            Matrix<T,C> A12 = A.getSubMatrixAt(0, size, size, size);
            Matrix<T,C> A21 = A.getSubMatrixAt(size, 0, size, size);
            Matrix<T,C> A22 = A.getSubMatrixAt(size, size, size, size);

            Matrix<T,C> B11 = B.getSubMatrixAt(0, 0, size, size);
            Matrix<T,C> B12 = B.getSubMatrixAt(0, size, size, size);
            Matrix<T,C> B21 = B.getSubMatrixAt(size, 0, size, size);
            Matrix<T,C> B22 = B.getSubMatrixAt(size, size, size, size);

            Matrix<T,C> P1 = new MatrixSDA<T,C>(size, size);
            Matrix<T,C> P2 = new MatrixSDA<T,C>(size, size);
            Matrix<T,C> P3 = new MatrixSDA<T,C>(size, size);
            Matrix<T,C> P4 = new MatrixSDA<T,C>(size, size);
            Matrix<T,C> P5 = new MatrixSDA<T,C>(size, size);
            Matrix<T,C> P6 = new MatrixSDA<T,C>(size, size);
            Matrix<T,C> P7 = new MatrixSDA<T,C>(size, size);
            
            Matrix<T,C> temp1 = new MatrixSDA<T,C>(size, size);
            Matrix<T,C> temp2 = new MatrixSDA<T,C>(size, size);
            
            sum(A11, A22, temp1);
            sum(B11, B22, temp2);

            strassenSkeleton(temp1, temp2, P1, size);
            
            sum(A21, A22, temp1); 
            
            strassenSkeleton(temp1, B11, P2, size);

            dif(B12, B22, temp1);

            strassenSkeleton(A11, temp1, P3, size);

            dif(B21, B11, temp1);

            strassenSkeleton(A22, temp1, P4, size);

            sum(A11, A12, temp1);

            strassenSkeleton(temp1, B22, P5, size);

            dif(A21, A11, temp1);
            sum(B11, B12, temp2);
            
            strassenSkeleton(temp1, temp2, P6, size);

            dif(A12, A22, temp1);
            sum(B21, B22, temp2);
            
            strassenSkeleton(temp1, temp2, P7, size);

            sum(P1, P4, temp1);
            dif(temp1, P5, temp1);
            sum(temp1, P7, temp1);

            result.layMatrixAt(temp1, 0, 0);

            sum(P3, P5, temp1);

            result.layMatrixAt(temp1, 0, size);

            sum(P2, P4, temp1);

            result.layMatrixAt(temp1, size, 0);

            dif(P1, P2, temp1);
            sum(temp1, P3, temp1);
            sum(temp1, P6, temp1);

            result.layMatrixAt(temp1, size, size);
        }

        /// <summary>
        /// Fills the result matrix with (A[i,j] + B[i,j]) elements.
        /// </summary>
        /// <param name="A">The first matrix</param>
        /// <param name="B">The second matrix</param>
        /// <param name="result">The result matrix</param>
        public static void sum(Matrix<T,C> A, Matrix<T,C> B, Matrix<T,C> result)
        {
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.ColumnCount; j++)
                    result.SetItemAt(i, j, A.GetElementAt(i, j) + B.GetElementAt(i, j));
        }

        /// <summary>
        /// Fills the result matrix with (A[i,j] - B[i,j]) elements.
        /// </summary>
        /// <param name="A">The first matrix</param>
        /// <param name="B">The second matrix</param>
        /// <param name="result">The result matrix</param>
        public static void dif(Matrix<T,C> A, Matrix<T,C> B, Matrix<T,C> result)
        {
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.ColumnCount; j++)
                    result.SetItemAt(i, j, A.GetElementAt(i, j) - B.GetElementAt(i, j));
        }

        /// <summary>
        /// Negates the source matrix and writes the result to another matrix.
        /// The result matrix can match with the source, though.
        /// </summary>
        /// <param name="A">The source matrix</param>
        /// <param name="result">The matrix that should contain the result</param>
        public static void negate(Matrix<T,C> A, Matrix<T,C> result)
        {
            for(int i=0; i<A.RowCount; i++)
                for(int j=0; j<A.RowCount; j++)
                    result.SetItemAt(i, j, -A.GetElementAt(i,j));
        }

        /// <summary>
        /// Adds a <see cref="Numeric&lt;T,C&gt;"/> value to all of the matrix elements 
        /// and writes the result to another matrix.
        /// The result matrix can point to the source.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="value"></param>
        /// <param name="result"></param>
        public static void addValue(Matrix<T,C> A, Numeric<T,C> value, Matrix<T,C> result)
        {
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.ColumnCount; j++)
                    result.SetItemAt(i, j, A.GetElementAt(i, j) + value);
        }

        /// <summary>
        /// Substracts a Numeric&lt;T,C&gt; value from all of the matrix elements and writes the result to another matrix.
        /// The result matrix, though, can match with the source.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="value"></param>
        /// <param name="result"></param>
        public static void substractValue(Matrix<T,C> A, Numeric<T,C> value, Matrix<T,C> result)
        {
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.ColumnCount; j++)
                    result.SetItemAt(i, j, A.GetElementAt(i, j) - value);
        }

        /// <summary>
        /// Divides all of the matrix elements by a Numeric&lt;T,C&gt; value and writes the result to another matrix.
        /// The reference to the result matrix CAN point to the source.
        /// </summary>
        /// <param name="A">The matrix whose elements are to be divided by a constant value.</param>
        /// <param name="value"></param>
        /// <param name="result"></param>
        public static void divValue(Matrix<T,C> A, Numeric<T,C> value, Matrix<T,C> result)
        {
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.ColumnCount; j++)
                    result.SetItemAt(i, j, A.GetElementAt(i, j) / value);
        }

        /// <summary>
        /// Multiplies all of the matrix elements by a Numeric&lt;T,C&gt; value and writes the result to another matrix.
        /// The reference to the result matrix CAN point to the source.
        /// </summary>
        /// <param name="A">The matrix whose elements are to be multiplied by a constant value.</param>
        /// <param name="value"></param>
        /// <param name="result"></param>
        public static void mulValue(Matrix<T,C> A, Numeric<T,C> value, Matrix<T,C> result)
        {
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.ColumnCount; j++)
                    result.SetItemAt(i, j, A.GetElementAt(i, j) * value);
        }

        // -----------------

        /// <summary>
        /// For each A[i,j] counts A[i,j]*B[i,j] and writes the result to another matrix.
        /// The source matrix can match with result parameter.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="result"></param>
        public static void eachElementMul(Matrix<T,C> A, Matrix<T,C> B, Matrix<T,C> result)
        {
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.ColumnCount; j++)
                    result.SetItemAt(i, j, A.GetElementAt(i, j) * B.GetElementAt(i,j));
        }

        /// <summary>
        /// For each A[i,j] counts A[i,j]/B[i,j] and writes the result to another matrix.
        /// The source matrix can match with result parameter.
        /// Caution! Watch out for zeros in the B[i,j].
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="result"></param>
        public static void eachElementDiv(Matrix<T,C> A, Matrix<T,C> B, Matrix<T,C> result)
        {
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.ColumnCount; j++)
                    result.SetItemAt(i, j, A.GetElementAt(i, j) / B.GetElementAt(i, j));
        }

        /// <summary>
        /// Standard multiplying method with O(n^3).
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="result"></param>
        public static void MultiplySimple(Matrix<T,C> A, Matrix<T,C> B, Matrix<T,C> result)
        {
            Numeric<T,C> sum;

            for(int i=0; i<A.RowCount; i++)
                for (int j = 0; j < B.ColumnCount; j++)
                {
                    sum = Numeric<T,C>.Zero;
                    
                    for (int z = 0; z < A.ColumnCount; z++)
                        sum += A[i,z] * B[z,j];

                    result[i,j] = sum;
                }
        }

        /// <summary>
        /// Checks if matrices passed to common arithmetic methods (sum and substraction between matrices etc., 
        /// arithmetic operations between matrices and <see cref="Numeric&lt;T, C&gt;"/> values etc.)
        /// 
        /// The principle is that (dim(A) = dim(B) = dim(res)) should be true, where dim is imaginary function
        /// returning the [row x columns] dimension object.
        /// </summary>
        /// <param name="A">The first matrix.</param>
        /// <param name="B">The second matrix.</param>
        /// <param name="res">The result matrix.</param>
        /// <returns>True if sizes are valid, false otherwise.</returns>
        public static bool checkArithmeticSize(IMatrix A, IMatrix B, IMatrix res)
        {
            return (A.RowCount == B.RowCount && A.RowCount == res.RowCount && A.ColumnCount == B.ColumnCount && A.ColumnCount == res.ColumnCount);
        }

        /// <summary>
        /// Checks if matrices passed to the multiplying method all have valid size.
        /// </summary>
        /// <param name="A">The first matrix.</param>
        /// <param name="B">The second matrix.</param>
        /// <param name="res">The result matrix of (A.Rows x B.Columns) size.</param>
        /// <returns>True if sizes are valid, false otherwise.</returns>
        public static bool checkMultiplySize(IMatrix A, IMatrix B, IMatrix res)
        {
            return (A.ColumnCount == B.RowCount && A.RowCount == res.RowCount && B.ColumnCount == res.ColumnCount);
        }
    }
}
