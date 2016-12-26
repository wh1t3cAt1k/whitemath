using System;

using WhiteMath.Calculators;
using WhiteMath.Matrices;
using WhiteMath.Vectors;

namespace WhiteMath.Geometry
{
    using DoubleMatrix = WhiteMath.Matrices.Matrix<double, CalcDouble>;
    using DoubleVector = WhiteMath.Vectors.Vector<double, CalcDouble>;

    public static class AffineTransform
    {
        /// <summary>
        /// Returns the Matrix for the Affine transformation that 
        /// transforms one vector to another taking their starting points into account.
        /// </summary>
        /// <param name="firstVector">The first vector.</param>
        /// <param name="secondVector">The second vector.</param>
        /// <param name="equationSystemSolverFunction">The function that, being provided with matrix 
        /// square matrix of size N (containing the unknown parameters' coefficients) and a vector of 
        /// free terms having length N, will return the vector of unknown parameters.
        /// May be null, an internal function will be used in this case.
        /// </param>
        /// <param name="c">The first free term of transformation matrix. Can be set to any value, affects the inverse matrix calculation precision.</param>
        /// <param name="d">THe second free term of transformation matrix. Can be set to any value, affects the inverse matrix calculation precision.</param>
        /// <returns>An affine transform matrix that would convert the <paramref name="firstVector"/> to the <paramref name="secondVector"/>.</returns>
        public static Matrix_SDA<double, CalcDouble> GetAffineTransformMatrix(
            VectorD firstVector, 
            VectorD secondVector, 
            double c = 0, 
            double d = 0,
            Func<DoubleMatrix, DoubleVector, DoubleVector> equationSystemSolverFunction = null)
        {
			throw new NotImplementedException();
        }
    }
}
