using System;

using WhiteMath.Calculators;
using WhiteMath.General;

namespace WhiteMath.Functions
{
    /// <summary>
    /// Represents the standard linear function: f(x) = kx + b.
    /// </summary>
    /// <typeparam name="T">The type of both function argument and value.</typeparam>
    /// <typeparam name="C">The calculator for the T type.</typeparam>
    public class LinearFunction<T, C> : IFunction<T, T> where C : ICalc<T>, new()
    {
        private static C calc = new C();

        private Numeric<T, C> k;
        private Numeric<T, C> b;

        /// <summary>
        /// Constructs the linear function on the basis of tangent coefficient 'k' and the free member 'b'.
        /// </summary>
        /// <param name="k"></param>
        /// <param name="b"></param>
        public LinearFunction(T k, T b)
        {
            this.k = k;
            this.b = b;
        }

        /// <summary>
        /// Constructs the linear function on the basis of two points passed.
        /// </summary>
        /// <param name="firstPoint"></param>
        /// <param name="secondPoint"></param>
        public LinearFunction(Point<T> firstPoint, Point<T> secondPoint)
        {
            if (calc.Equal(firstPoint.X, secondPoint.X))
                throw new ArgumentException("The X coordinates of the two points must differ.");

            this.k = calc.Divide(calc.Subtract(firstPoint.Y, secondPoint.Y), calc.Subtract(firstPoint.X, secondPoint.X));
            this.b = calc.Subtract(firstPoint.Y, calc.Multiply(firstPoint.X, k));
        }

        // ----------------------- functionality

        public T Value(T x) { return k*x + b; }
    }
}
