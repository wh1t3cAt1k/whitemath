using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace whiteMath.General
{
    /// <summary>
    /// Provides extension methods for two-dimensional arrays.
    /// </summary>
    [ContractVerification(true)]
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
            Contract.Requires<ArgumentNullException>(matrix != null, "matrix");

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
            Contract.Requires<ArgumentNullException>(matrix != null, "matrix");
            Contract.Requires<ArgumentException>(
                columnHeaders == null || matrix.GetLength(1) == columnHeaders.Count, 
                "The column headers list length must match the matrix' column count.");

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
            Contract.Requires<ArgumentNullException>(matrix != null, "matrix");
            Contract.Ensures(Contract.Result<T[][]>() != null);

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
            Contract.Requires<ArgumentNullException>(matrix != null, "matrix");
            Contract.Requires<ArgumentOutOfRangeException>(rowIndex >= 0 && rowIndex < matrix.GetLength(0), "The row index is out of range.");

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
        public static void SetRowAt<T>(this T[,] matrix, int rowIndex, IList<T> list)
        {
            Contract.Requires<ArgumentNullException>(matrix != null, "matrix");
            Contract.Requires<ArgumentNullException>(list != null, "list");
            Contract.Requires<ArgumentOutOfRangeException>(rowIndex >= 0 && rowIndex < matrix.GetLength(0), "The row index is out of range.");

            TwoDimensionalArrayRow<T> matrixRow = matrix.GetRowWrapper(rowIndex);

            ServiceMethods.Copy(list, matrixRow);
        }
    }
}
