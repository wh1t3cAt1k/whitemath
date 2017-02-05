using System;
using System.Collections.Generic;
using System.Data;

using WhiteStructs.Conditions;

namespace WhiteMath.General
{
    /// <summary>
    /// Provides extension methods for two-dimensional arrays.
    /// </summary>
    public static class TwoDimensionalArrayExtensions
    {
        /// <summary>
        /// Creates a two-dimensional array with the same data as in the source array,
        /// but with zero-based indices for both rows and columns. 
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="matrix">An arbitrarily-indexed source two-dimensional array.</param>
        /// <returns>
        /// A two-dimensional array with the same data as in the source array,
        /// but with zero-based indices for both rows and columns.
        /// </returns>
        public static T[,] ToZeroBasedTwoDimensionalArray<T>(this T[,] matrix)
        {
			Condition.ValidateNotNull(matrix, nameof(matrix));

            // If the object is already zero-indexed, just return its copy.
            // -
            if (matrix.GetLowerBound(0) == 0 && matrix.GetLowerBound(1) == 0)
            {
                return matrix.Clone() as T[,];
            }
            else
            {
                return (new ZeroBasedMatrixWrapper<T>(matrix)).ToTwoDimensionalArray();
            }
        }

        /// <summary>
        /// Creates a <see cref="DataTable"/> populated with 
        /// data from a two-dimensional array with
        /// optionally specified column headers list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="matrix">The source array to be converted to a <see cref="DataTable"/>.</param>
        /// <param name="columnHeaders">
        /// An optional list containing the column headers.
        /// If not <c>null</c>, the size of the list
        /// MUST match the amount of columns in the <paramref name="matrix"/>.
        /// </param>
        /// <returns>
        /// A <see cref="DataTable"/> populated with the data from the source
        /// two-dimensional array.
        /// </returns>
        public static DataTable ToDataTable<T>(this T[,] matrix, IList<string> columnHeaders = null)
        {
			Condition.ValidateNotNull(matrix, nameof(matrix));
			Condition
				.Validate(columnHeaders == null || matrix.GetLength(1) == columnHeaders.Count)
				.OrArgumentException("The column headers list length must match the matrix' column count.");

            int rowCount = matrix.GetLength(0);
            int columnCount = matrix.GetLength(1);

            DataTable result = new DataTable();

            for (int indexColumn = 0; indexColumn < columnCount; ++indexColumn)
            {
                if (columnHeaders == null)
                {
                    result.Columns.Add();
                }
                else
                {
                    result.Columns.Add(columnHeaders[indexColumn]);
                }
            }

            for (int indexRow = 0; indexRow < rowCount; ++indexRow)
            {
                DataRow currentRow = result.NewRow();

                for (int indexColumn = 0; indexColumn < columnCount; ++indexColumn)
                {
                    currentRow[indexColumn] = matrix[indexRow, indexColumn];
                }

                result.Rows.Add(currentRow);
            }

            return result;
        }

        /// <summary>
        /// Converts a two-dimensional matrix into 
        /// a two-dimensional jagged array with the same
        /// dimensionality and data.
        /// </summary>
        /// <typeparam name="T">The types of element in the matrix.</typeparam>
        /// <param name="matrix">The source object to be converted to a jagged array. Remains untouched.</param>
        /// <returns>
        /// A two-dimensional jagged array with the same
        /// dimensionality and data as the source object.
        /// </returns>
        public static T[][] ToJaggedArray<T>(this T[,] matrix)
        {
			Condition.ValidateNotNull(matrix, nameof(matrix));
            
            int resultRowCount = matrix.GetLength(0);
            int resultColumnCount = matrix.GetLength(1);

            T[][] result = new T[resultRowCount][];

            for (int i = 0; i < resultRowCount; ++i)
            {
                result[i] = new T[resultColumnCount];

                for (int j = 0; j < resultColumnCount; ++j)
                {
                    result[i][j] = matrix[i, j];
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a one-dimensional wrapper for the specified 
        /// row of a two-dimensional array.
        /// </summary>
        /// <param name="matrix">The source two-dimensional array to be wrapped.</param>
        /// <param name="rowIndex">The row index of the source array to be fixed.</param>
        /// <returns>A one-dimensional wrapper for the row at the specified row index.</returns>
        public static TwoDimensionalArrayRow<T> GetRowWrapper<T>(this T[,] matrix, int rowIndex)
        {
			Condition.ValidateNotNull(matrix, nameof(matrix));
			Condition
				.Validate(rowIndex >= 0 && rowIndex < matrix.GetLength(0))
				.OrIndexOutOfRangeException("The row index is out of range.");

            return new TwoDimensionalArrayRow<T>(matrix, rowIndex);
        }

        /// <summary>
        /// Replaces the values in the two-dimensional arary row at the specified
        /// row index by the values in the specified list.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collections.</typeparam>
        /// <param name="matrix">The two-dimensional array to be modified.</param>
        /// <param name="rowIndex">The row index of the two-dimensional array.</param>
        /// <param name="list">The list containing the values to be placed in the specified row.</param>
        public static void SetRowAt<T>(this T[,] matrix, int rowIndex, IReadOnlyList<T> list)
        {
			Condition.ValidateNotNull(matrix, nameof(matrix));
			Condition.ValidateNotNull(list, nameof(list));
			Condition
				.Validate(rowIndex >= 0 && rowIndex < matrix.GetLength(0))
				.OrIndexOutOfRangeException("The row index is out of range.");
			
            TwoDimensionalArrayRow<T> matrixRow = matrix.GetRowWrapper(rowIndex);

            ServiceMethods.Copy(list, matrixRow);
        }

        // -------------------------------------
        // ---- GETTING SUBARRAYS --------------
        // -------------------------------------

        /// <summary>
        /// Computes a subarray of a specified 2D array,
        /// starting at the specified position (row and column index)
        /// and spanning the specified number of rows and columns.
        /// The result 2D array does not depend on the original array in 
        /// terms of getting / setting its elements (the elements themselves
        /// are shallow copies though).
        /// </summary>
        /// <typeparam name="T">The type of elements in the source array.</typeparam>
        /// <param name="matrix">A 2D array whose submatrix is to be calculated.</param>
        /// <param name="atRowIndex">The row index in the <paramref name="matrix"/> where the subarray should begin.</param>
        /// <param name="atColumnIndex">The column index in the <paramref name="matrix"/> where the subarray should begin.</param>
        /// <param name="rowCount">Total number of rows in the resulting array.</param>
        /// <param name="columnCount">Total number of columns in the resulting array.</param>
        /// <returns>
        /// A 2D array containing elements of <paramref name="matrix"/> 
        /// starting from row <paramref name="atRowIndex"/> and column <paramref name="atColumnIndex"/>
        /// and spanning <paramref name="rowCount"/> rows and <paramref name="columnCount"/> columns.
        /// </returns>
        public static T[,] GetSubArray<T>(
            this T[,] matrix,
            int atRowIndex,
            int atColumnIndex,
            int rowCount,
            int columnCount)
        {
			Condition.ValidateNotNull(matrix, nameof(matrix));
			Condition
				.Validate(atRowIndex >= 0 && atRowIndex < matrix.GetLength(0))
				.OrIndexOutOfRangeException("The row index of the source 2D array is out of range.");            
            Condition
				.Validate(atColumnIndex >= 0 && atColumnIndex < matrix.GetLength(1))
				.OrIndexOutOfRangeException("The column index of the source 2D array is out of range.");
			Condition.ValidateNonNegative(rowCount, "The row count of the result subarray should not be negative.");
			Condition.ValidateNonNegative(columnCount, "The column count of the result subarray should not be negative.");
			Condition
				.Validate(atRowIndex + rowCount <= matrix.GetLength(0))
				.OrArgumentOutOfRangeException("The resulting subarray would exceed the row range of the source 2D array.");
			Condition
				.Validate(atColumnIndex + columnCount <= matrix.GetLength(1))
				.OrArgumentOutOfRangeException("The resulting subarray would exceed the column range of the source 2D array.");

            if (rowCount == 0 || columnCount == 0)
            {
                return new T[0, 0];
            }

            T[,] result = new T[rowCount, columnCount];

            for (int rowIndex = 0; rowIndex < rowCount; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < columnCount; ++columnIndex)
                {
                    result[rowIndex, columnIndex] = matrix[atRowIndex + rowIndex, atColumnIndex + columnIndex];
                }
            }

            return result;
        }

        // -------------------------------------
        // ---- REPLACING PARTS OF THE ARRAY ---
        // -------------------------------------

        /// <summary>
        /// Replaces the element in the source 2D array
        /// with elements from the second 2D array,
        /// starting at the specified row and column in the source array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="matrix">The source matrix to be modified.</param>
        /// <param name="patch">A second matrix whose elements will replace part of the original matrix.</param>
        /// <param name="atRowIndex">
        /// The row index in the <paramref name="matrix"/> where the 
        /// elements of <paramref name="patch"/> will be placed.
        /// </param>
        /// <param name="atColumnIndex">
        /// The column index in the <paramref name="matrix"/> where the elements 
        /// of <paramref name="patch"/> will be placed.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="patch"/> cannot be completely placed 
        /// within <paramref name="matrix"/> considering the specified indices of placement.
        /// </exception>
        public static void PlaceMatrixAt<T>(this T[,] matrix, int atRowIndex, int atColumnIndex, T[,] patch)
        {
			Condition.ValidateNotNull(matrix, nameof(matrix));
			Condition.ValidateNotNull(patch, nameof(patch));
			Condition
				.Validate(atRowIndex >= 0 && atRowIndex < matrix.GetLength(0))
				.OrIndexOutOfRangeException("The row index of the source 2D array is out of range.");            
			Condition
				.Validate(atColumnIndex >= 0 && atColumnIndex < matrix.GetLength(1))
				.OrIndexOutOfRangeException("The column index of the source 2D array is out of range.");
			Condition
				.Validate(atRowIndex + patch.GetLength(0) <= matrix.GetLength(0))
				.OrIndexOutOfRangeException("The patch 2D array does not fit within the rows of the source 2D array.");
			Condition
				.Validate(atColumnIndex + patch.GetLength(1) <= matrix.GetLength(1))
				.OrIndexOutOfRangeException("The patch 2D array does not fit within the columns of the source 2D array.");
			                   
			int patchRowCount = patch.GetLength(0);
            int patchColumnCount = patch.GetLength(1);

            for (int rowIndex = 0; rowIndex < patchRowCount; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < patchColumnCount; ++columnIndex)
                {
                    matrix[atRowIndex + rowIndex, atColumnIndex + columnIndex] = patch[rowIndex, columnIndex];
                }
            }
        }

        // -------------------------------------
        // --------- TRANSPOSE -----------------
        // -------------------------------------

        /// <summary>
        /// Returns the transposed array from a two-dimensional array
        /// i.e. such that <c>result[i, j] = source[j, i]</c>. If the source
        /// array was of size M x N, then the resulting array will have the 
        /// size N x M.
        /// </summary>
        /// <typeparam name="T">The type of elements in the arrays.</typeparam>
        /// <param name="matrix">The two-dimensional array to be transposed.</param>
        /// <returns>The transposed two-dimensional array.</returns>
        public static T[,] GetTransposedArray<T>(this T[,] matrix)
        {
			Condition.ValidateNotNull(matrix, nameof(matrix));

            int rowCount = matrix.GetLength(0);
            int columnCount = matrix.GetLength(1);

            T[,] result = new T[columnCount, rowCount];

            for (int rowIndex = 0; rowIndex < rowCount; ++rowIndex)
            {
                for (int columnIndex = 0; columnIndex < columnCount; ++columnIndex)
                {
                    result[columnIndex, rowIndex] = matrix[rowIndex, columnIndex];
                }
            }

            return result;
        }

        // -------------------------------------
        // ---- ATTACHING MATRICES -------------
        // -------------------------------------

        /// <summary>
        /// Attaches a two-dimensional "patch" array with the same row
        /// count to the left of the source 2D array. In the result
        /// 2D array, the columns of the original matrix will
        /// be shifted to the right depending on the column count of the patch. 
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="matrix">
        /// A two-dimensional array to which the <paramref name="leftPatch"/> will be attached.
        /// Remains unmodified.
        /// </param>
        /// <param name="leftPatch">
        /// A matrix that will be attached to the left of the original 2D array.
        /// </param>
        /// <returns>
        /// A new two-dimensional array of combined column count and the same row count,
        /// where the beginning columns contain elements of <paramref name="leftPatch"/>,
        /// and the ending columns contain elements of <paramref name="matrix"/>.
        /// </returns>
        public static T[,] WithAttachedToTheLeft<T>(this T[,] matrix, T[,] leftPatch) 
        {
			Condition.ValidateNotNull(matrix, nameof(matrix));
			Condition.ValidateNotNull(leftPatch, nameof(leftPatch));
			Condition
				.Validate(matrix.GetLength(0) == leftPatch.GetLength(0))
				.OrArgumentException("The 2D arrays should have the same row count in order to be attached to each other.");

			/*
            Contract.Ensures(Contract.Result<T[,]>().GetLength(0) == matrix.GetLength(0));
            Contract.Ensures(Contract.Result<T[,]>().GetLength(1) == matrix.GetLength(1) + leftPatch.GetLength(1));
			*/

            int rowCount = matrix.GetLength(0);
            int matrixColumnCount = matrix.GetLength(1);
            int patchColumnCount = leftPatch.GetLength(1);
            int resultColumnCount = matrixColumnCount + patchColumnCount;

            T[,] result = new T[rowCount, resultColumnCount];

            result.PlaceMatrixAt(0, 0, leftPatch);
            result.PlaceMatrixAt(0, patchColumnCount, matrix);

            return result;
        }

        /// <summary>
        /// Attaches a two-dimensional "patch" array with the same row
        /// count to the right of the source 2D array. In the result
        /// 2D array, the original 2D array will be located in the leftmost part,
        /// and the "patch" in the rightmost. 
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="matrix">
        /// A two-dimensional array to which the <paramref name="rightPatch"/> will be attached.
        /// Remains unmodified.
        /// </param>
        /// <param name="rightPatch">
        /// A matrix that will be attached to the right of the original 2D array.
        /// </param>
        /// <returns>
        /// A new two-dimensional array of combined column count and the same row count,
        /// where the beginning columns contain elements of <paramref name="matrix"/>,
        /// and the ending columns contain elements of <paramref name="rightPatch"/>.
        /// </returns>
        public static T[,] WithAttachedToTheRight<T>(this T[,] matrix, T[,] rightPatch)
        {
			Condition.ValidateNotNull(matrix, nameof(matrix));
			Condition.ValidateNotNull(rightPatch, nameof(rightPatch));
			Condition
				.Validate(matrix.GetLength(0) == rightPatch.GetLength(0))
				.OrArgumentException("The 2D arrays should have the same row count in order to be attached to each other.");

			/*
            Contract.Ensures(Contract.Result<T[,]>().GetLength(0) == matrix.GetLength(0));
            Contract.Ensures(Contract.Result<T[,]>().GetLength(1) == matrix.GetLength(1) + rightPatch.GetLength(1));
			*/

            int rowCount = matrix.GetLength(0);
            int matrixColumnCount = matrix.GetLength(1);
            int patchColumnCount = rightPatch.GetLength(1);
            int resultColumnCount = matrixColumnCount + patchColumnCount;

            T[,] result = new T[rowCount, resultColumnCount];

            result.PlaceMatrixAt(0, 0, matrix);
            result.PlaceMatrixAt(0, matrixColumnCount, rightPatch);

            return result;
        }
    }
}
