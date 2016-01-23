using System;
using System.Collections.Generic;

using whiteMath.Calculators;
using whiteMath.Matrices;

namespace whiteMath.General
{
    public static class PointExtensions
    {
        /// <summary>
        /// Converts a list of points to a point which contains two single-dimensional arrays - an array of X's and an array of Y's.
        /// </summary>
        /// <typeparam name="T">The type of point coordinates.</typeparam>
        /// <param name="list">The list object containing points of type <typeparamref name="T"/>.</param>
        /// <returns>The point object whose X coordinate is a list of X's and Y coordinate is a list of Y's.</returns>
        public static Point<T[]> convertToPairOfLists<T>(this IList<Point<T>> list)
        {
            list.Assert_NotNull("The list of points should not be null.");

            T[] arrX = new T[list.Count];
            T[] arrY = new T[list.Count];

            arrX.FillByAssign(delegate(int i) { return list[i].X; });
            arrY.FillByAssign(delegate(int i) { return list[i].Y; });

            return new Point<T[]>(arrX, arrY);
        }

        /// <summary>
        /// Makes an array of point objects from two arrays containing X coordinates and Y coordinates respectively.
        /// Make sure that no array is null and that they have the same length. Otherwise,
        /// a NullReferenceException or an ArgumentException will be thrown.
        /// </summary>
        /// <typeparam name="T">The type of point elements.</typeparam>
        /// <param name="xValues">The array containing X coordinates.</param>
        /// <param name="yValues">The array containing Y coordinates.</param>
        /// <returns>An array of points with X coordinate storing the respective value of <paramref name="xValues"/> and Y coordinate storing the respective value of <paramref name="yValues"/>.</returns>
        public static Point<T>[] convertToListOfPairs<T>(IList<T> xValues, IList<T> yValues)
        {
            xValues.Assert_NotNull("The X coordinates array should not be null.");
            yValues.Assert_NotNull("The Y coordinates array should not be null.");

            (xValues.Count == yValues.Count).Assert(new ArgumentException("The lengths of coordinates' lists should be equal to each other."));

            Point<T>[] pointsArray = new Point<T>[xValues.Count];

            for (int i = 0; i < pointsArray.Length; i++)
                pointsArray[i] = new Point<T>(xValues[i], yValues[i]);

            return pointsArray;
        }

        /// <summary>
        /// Converts a list of points to a matrix of size (Nx2) which rows are exactly the point coordinates.
        /// </summary>
        /// <typeparam name="T">The type of elements in the matrix.</typeparam>
        /// <typeparam name="C">The calculator for the matrix elements type.</typeparam>
        /// <param name="list">The list object containing points of type <typeparamref name="T"/>.</param>
        /// <returns>The matrix of size (Nx2) which rows are exactly the point coordinates.</returns>
        public static Matrix<T, C> convertToMatrixRows<T, C>(this IList<Point<T>> list) where C : ICalc<T>, new()
        {
            list.Assert_NotNull("The list of points should not be null.");

            Matrix_SDA<T, C> matrix = new Matrix_SDA<T, C>(list.Count, 2);

            for (int i = 0; i < matrix.RowCount; i++)
            {
                matrix[i, 0] = list[i].X;
                matrix[i, 1] = list[i].Y;
            }

            return matrix;
        }
    }
}
