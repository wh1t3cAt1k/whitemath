#if OLD_VERSION

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath.Matrices
{
    /// <summary>
    /// Flexible matrix class allowing to:
    /// 
    /// 1. Choose the multiplying method
    /// 2. Choose the determinant calculating method
    /// 3. ????
    /// 4. PROFIT!
    /// </summary>
    public abstract class DoubleMatrix: Matrix<double>
    {
        protected int rows;                   // Matrix row count
        protected int columns;                // Matrix column count

        /// <summary>
        /// Returns the column count for current matrix
        /// </summary>
        public override int ColumnCount
        {
            get { return this.columns; }
        }

        /// <summary>
        /// Returns the row count for current matrix
        /// </summary>
        public override int RowCount
        {
            get { return this.rows; }
        }

        // ----------------------------------------------------------

        public static DoubleMatrix operator +(DoubleMatrix one, double two)
        {
            return one.addValue(two);
        }

        public static DoubleMatrix operator -(DoubleMatrix one, double two)
        {
            return one.addValue(-two);
        }

        public static DoubleMatrix operator *(DoubleMatrix one, double two)
        {
            return one.multiplyByValue(two);
        }

        public static DoubleMatrix operator /(DoubleMatrix one, double two)
        {
            return one.divideByValue(two);
        }
        
        /// <summary>
        /// Adds a double value to all of the matrix elements.
        /// </summary>
        /// <param name="value">The integer value added to all of the matrix elements.</param>
        /// <returns>The result matrix.</returns>
        protected virtual DoubleMatrix addValue(double value)
        {
            DoubleMatrix temp = (DoubleMatrix)this.Clone();
            MatrixHelper.addValue(temp, value, temp);
            return temp;
        }

        /// <summary>
        /// Multiplies all of the matrix elements by a double value.
        /// </summary>
        /// <param name="value">The double value.</param>
        /// <returns>The result matrix.</returns>
        protected virtual DoubleMatrix multiplyByValue(double value)
        {
            DoubleMatrix temp = (DoubleMatrix)this.Clone();
            MatrixHelper.mulValue(temp, value, temp);
            return temp;
        }

        /// <summary>
        /// Divides all of the matrix elements by a double value.
        /// </summary>
        /// <param name="value">The double value.</param>
        /// <returns>The result matrix.</returns>
        protected virtual DoubleMatrix divideByValue(double value)
        {
            DoubleMatrix temp = (DoubleMatrix)this.Clone();
            MatrixHelper.divValue(temp, value, temp);
            return temp;
        }
    }
}

#endif