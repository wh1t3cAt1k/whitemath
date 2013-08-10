using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using whiteMath.Matrices;

namespace whiteMath.Matrices
{
    public static class SLAESolving
    {
        /// <summary>
        /// This algorithm uses the LU-Factorization of coefficient matrix in order
        /// to calculate the solution of the equation system.
        /// 
        /// As LU-Factorization is not guaranteed to exist for every 
        /// non-singular matrix, this method may sometimes fail.
        /// </summary>
        /// <param name="coefficients">A square matrix of unknown terms' coefficients.</param>
        /// <param name="freeTerm">A vector of free terms.</param>
        /// <param name="x">The vector containing the solution of the equation system.</param>
        public static void LU_FactorizationSolving<T,C>(Matrix<T,C> coefficients, Vector<T,C> freeTerm, out Vector<T,C> x) where C:ICalc<T>, new()
        {
            if (coefficients.RowCount != coefficients.ColumnCount)
                throw new ArgumentException("Only square matrices are supported.");

            int dim = coefficients.RowCount;

            Matrix_SDA<T, C> K = new Matrix_SDA<T, C>(dim, dim);
            Matrix_SDA<T, C> M = new Matrix_SDA<T, C>(dim, dim);

            // Заполняем единичную диагональ матрицы K.
            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                    K[i, j] = M[i, j] = Numeric<T,C>.Zero;

                M[i, i] = (Numeric<T,C>)1;
                K[i, 0] = coefficients[i, 0];
                M[0, i] = coefficients[0, i] / K[0, 0];
            }

            // вектор промежуточных решений
            Vector<T,C> y = new Vector<T,C>(dim);
            y[0] = freeTerm[0] / K[0, 0];

            Numeric<T,C> s; // вспомогательная

            for (int i = 1; i < dim; i++)
            {
                int j;

                for (j = 0; j < dim; j++)
                {
                    s = (Numeric<T,C>)0;

                    for (int z = 0; z <= j - 1; z++)
                        s += K[i, z] * M[z, j];

                    K[i, j] = coefficients[i,j] - s;
                }

                for (j = i; j < dim; j++)
                {
                    Numeric<T,C> s1 = (Numeric<T,C>)0;
                    Numeric<T,C> s2 = (Numeric<T,C>)0;

                    for (int z = 0; z <= i - 1; z++)
                    {
                        s1 += K[i, z] * M[z, j];
                        s2 += K[i, z] * y[z];
                    }

                    M[i, j] = (coefficients[i, j] - s1) / K[i, i];
                    y[i] = (freeTerm[i] - s2) / K[i, i];
                }
            }

            x = new Vector<T,C>(dim);
            x[dim - 1] = y[dim - 1];

            for (int i = dim - 2; i >= 0; i--)
            {
                s = (Numeric<T,C>)0;

                for (int z = i + 1; z < dim; z++)
                    s += M[i, z] * x[z];

                x[i] = y[i] - s;
            }

            return;
        }
    }
}
