﻿using System;
using System.Collections.Generic;

using WhiteMath.Calculators;
using WhiteMath.General;
using WhiteMath.Matrices;
using WhiteMath.Matrices.Winders;
using WhiteMath.Vectors;

namespace WhiteMath.Statistics
{
    public static class CLNRM
    {
        public struct LeastSquaresResult<T, C> 
			where C : ICalc<T>, new()
        {
			Vector<T, C> LeastSquaresCoefficients;

            T RSS;
            T ESS;
            T TSS;
        }

        /// <summary>
        /// TODO: write documentation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <returns></returns>
        public static Vector<T, C> EstimateLeastSquaresCoefficients<T, C>(IList<T> xValues, IList<T> yValues) 
			where C: ICalc<T>, new()
        {
            return EstimateLeastSquaresCoefficients<T, C>(PointExtensions.ConvertToListOfPairs(xValues, yValues));
        }

        /// <summary>
        /// TODO: write documentation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Vector<T, C> EstimateLeastSquaresCoefficients<T, C>(IList<Point<T>> points) where C: ICalc<T>, new()
        {
            return EstimateLeastSquaresCoefficients(points.ConvertToMatrixRows<T,C>());
        }

        /// <summary>
        /// Estimates the least squares coefficients for the regression formula: Y = a + bX_1 + cX_2 + ... + dX_k 
        /// </summary>
        /// <typeparam name="T">The type of observation values and the regression coefficients.</typeparam>
        /// <typeparam name="C">The calculator for the observations' value type.</typeparam>
        /// <param name="observationRows">The matrix of observations having rows in the following format: X_11 | X_12 | ... | X_1K | Y_1</param>
        /// <returns>The vector of estimated least squares regression coefficients. Its first term is a free term, and all the following terms correspond to X_1, X_2 etc. up to X_k</returns>
        public static Vector<T, C> EstimateLeastSquaresCoefficients<T, C>(Matrix<T, C> observationRows) 
			where C: ICalc<T>, new()
        {
            Matrix<T, C> xMatrix = new MatrixSDA<T, C>(observationRows.RowCount, observationRows.ColumnCount);
            Matrix<T, C> yVector = observationRows.GetSubMatrixAt(0, observationRows.ColumnCount - 1);

			for (int rowIndex = 0; rowIndex < xMatrix.RowCount; ++rowIndex)
            {
                xMatrix[rowIndex, 0] = Numeric<T, C>._1;

				for (int columnIndex = 1; columnIndex < xMatrix.ColumnCount; ++columnIndex)
				{
					xMatrix[rowIndex, columnIndex] = observationRows[rowIndex, columnIndex - 1];
				}
            }

            // TODO: transposition should NOT create a new matrix.
            // write a new matrix class

            Matrix<T, C> xTransposed = xMatrix.GetTransposedMatrix();
            Matrix<T, C> result = (xTransposed * xMatrix).CalculateInverseMatrixLupFactorization() * xTransposed * yVector;
            
            IWinder winder = result.GetRowByRowWinder();
            return result.UnwindIntoArray(winder);
        }

        /// <summary>
        /// TODO: write the documentation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="estimatedY"></param>
        /// <param name="realY"></param>
        /// <returns></returns>
        public static Numeric<T,C> LeastSquares_RSS<T, C>(IList<T> estimatedY, IList<T> realY) where C: ICalc<T>, new()
        {
            Summator<T> summator = new Summator<T>(Numeric<T, C>.Zero, Numeric<T, C>.Calculator.Add);

            Func<int, T> memberFormula = delegate (int i)
            {
                Numeric<T,C> simpleDif = Numeric<T,C>.Calculator.Subtract(estimatedY[i], realY[i]);
                return simpleDif * simpleDif;
            };

            return summator.SumSmallerToLarger(memberFormula, 0, estimatedY.Count - 1, Numeric<T, C>.UnderlyingTypeComparer);
        }

        /// <summary>
        /// TODO: write the documentation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="estimatedY"></param>
        /// <param name="averageY"></param>
        /// <returns></returns>
        public static Numeric<T, C> LeastSquares_ESS<T, C>(IList<T> estimatedY, T averageY) where C: ICalc<T>, new()
        {
            Summator<T> summator = new Summator<T>(Numeric<T, C>.Zero, Numeric<T, C>.Calculator.Add);

            Func<int, T> memberFormula = delegate(int i)
            {
                Numeric<T, C> simpleDif = Numeric<T, C>.Calculator.Subtract(estimatedY[i], averageY);
                return simpleDif * simpleDif;
            };

            return summator.SumSmallerToLarger(memberFormula, 0, estimatedY.Count - 1, Numeric<T, C>.UnderlyingTypeComparer);
        }

        /// <summary>
        /// TODO: write the documentation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="realY"></param>
        /// <param name="averageY"></param>
        /// <returns></returns>
        public static Numeric<T, C> LeastSquares_TSS<T, C>(IList<T> realY, T averageY) where C : ICalc<T>, new()
        {
            if (averageY == null)
                averageY = realY.SampleAverage<T, C>();

            Summator<T> summator = new Summator<T>(Numeric<T, C>.Zero, Numeric<T, C>.Calculator.Add);

            Func<int, T> memberFormula = delegate(int i)
            {
                Numeric<T, C> simpleDif = Numeric<T, C>.Calculator.Subtract(realY[i], averageY);
                return simpleDif * simpleDif;
            };

            return summator.SumSmallerToLarger(memberFormula, 0, realY.Count - 1, Numeric<T, C>.UnderlyingTypeComparer);
        }
    }
}
