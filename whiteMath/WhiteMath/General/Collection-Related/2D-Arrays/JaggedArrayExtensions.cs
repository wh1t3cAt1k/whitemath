using System;
using System.Linq;

using WhiteStructs.Conditions;

namespace WhiteMath.General
{
    /// <summary>
    /// This class provides extension methods 
    /// for multi-dimensional jagged arrays.
    /// </summary>
    public static class JaggedArrayExtensions
    {
        /// <summary>
        /// Converts a rectangular two-dimensional jagged array
        /// to a two-dimensional array with the same dimensionality
        /// and data.
        /// 
        /// The source jagged array must contain no <c>null</c> rows
        /// and all the rows must have the same column count.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="jagged">The source array to be transformed. Remains untouched.</param>
        /// <exception cref="ArgumentException">
        /// Throws an <see cref="ArgumentException"/> if the source
        /// array is not rectangular (i.e. not all rows of the source
        /// array have the same column count).
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Throws an <see cref="ArgumentException"/> if the source
        /// array contains null values.
        /// </exception>
        /// <returns>
        /// A two-dimensional array with the same dimensionality and data
        /// as the source jagged array.
        /// </returns>
        public static T[,] To2DArray<T>(this T[][] jagged)
        {
			Condition.ValidateNotNull(jagged, nameof(jagged));
			Condition
				.Validate(jagged.All(x => x != null))
				.OrArgumentException("The source array must contain no null rows.");
			Condition
				.Validate(jagged.All(x => (x.Length == jagged.First().Length)))
				.OrArgumentException("The source array must be rectangular.");

            // We are now sure that every row (if any rows are present)
            // contains the same number of columns.
			//
            // Here we check if there are no rows at all, or if 
            // all rows have the column count of zero.
            // -
            if (jagged.Length == 0 || jagged[0].Length == 0)
            {
                return new T[0, 0];
            }

            int rowCount = jagged.Length;
            int columnCount = jagged[0].Length;

            T[,] result = new T[rowCount, columnCount];

            for (int i = 0; i < rowCount; ++i)
            {
                for (int j = 0; j < columnCount; ++j)
                {
                    result[i, j] = jagged[i][j];
                }
            }

            return result;
        }

        /// <summary>
        /// Removes all null rows from the source
        /// jagged array and returns the jagged array
        /// of possibly reduced row dimensionality. 
        /// The resulting object contains the same data 
        /// as the source array, excluding the null rows.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="jagged">The source array which possibly contains null rows.</param>
        /// <returns>
        /// A jagged array containing the same data 
        /// as the source array, excluding the null rows.
        /// </returns>
		public static T[][] GetArrayWithoutNullRows<T>(this T[][] jagged)
        {
			Condition.ValidateNotNull(jagged, nameof(jagged));
            
            return jagged
				.Where(x => x != null)
				.ToArray();
        }
    }
}
