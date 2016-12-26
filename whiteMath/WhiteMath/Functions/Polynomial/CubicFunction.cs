using WhiteMath.Calculators;
using WhiteMath.General;
using WhiteMath.Matrices;
using WhiteMath.Vectors;

namespace WhiteMath.Functions
{
    /// <summary>
    /// Represents a standard cubic function:
    /// y = a(x-x0)^3 + b(x-x0)^2 + c(x-x0) + d.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="C"></typeparam>
    public class CubicFunction<T,C>: IFunction<T, T> where C: ICalc<T>, new()
    {
        private static C calc = Numeric<T, C>.Calculator;

        public Numeric<T, C> a { get; private set; }
        public Numeric<T, C> b { get; private set; }
        public Numeric<T, C> c { get; private set; }
        public Numeric<T, C> d { get; private set; }

        public Numeric<T, C> x0 { get; private set; }

        // -------------------------
        // -------- ctors ----------
        // -------------------------

        /// <summary>
        /// Creates a new instance of cubic function with
        /// explicitly specified coefficients.
        /// </summary>
        /// <param name="a">The cubic coefficient.</param>
        /// <param name="b">The quadratic coefficient.</param>
        /// <param name="c">The linear coefficient.</param>
        /// <param name="d">The free coefficient.</param>
        /// <param name="x0">The shift value. Positive value shifts the function to the right.</param>
        public CubicFunction(T a, T b, T c, T d, T x0)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;

            this.x0 = x0;

            return;
        }

        /// <summary>
        /// Creates the cubic function basing on the two points and two derivative values in the first point.
        /// </summary>
        /// <param name="firstPoint">The first point for the function to pass.</param>
        /// <param name="secondPoint">The second point for the function to pass.</param>
        /// <param name="firstDerivative">The value of the first derivative in the first point.</param>
        /// <param name="secondDerivative">The value of the second derivative in the first point.</param>
        public CubicFunction(Point<T> firstPoint, Point<T> secondPoint, T firstDerivative, T secondDerivative)
        {
            T xaPow2 = calc.Multiply(firstPoint.X, firstPoint.X);
            T xbPow2 = calc.Multiply(secondPoint.X, secondPoint.X);

            T xaPow3 = calc.Multiply(firstPoint.X, xaPow2);
            T xbPow3 = calc.Multiply(secondPoint.X, xbPow2);

            T one = calc.FromInteger(1);

            Matrix_SDA<T, C> matrix = new Matrix_SDA<T, C>(4, 4);
 
            matrix.convertFromArray(new T[,]
            {
                { xaPow3, xaPow2, firstPoint.X, one },
                { xbPow3, xbPow2, secondPoint.X, one },
                { calc.Multiply(calc.FromInteger(3), xaPow2), calc.Multiply(calc.FromInteger(2), firstPoint.X), one, calc.Zero },
                { calc.Multiply(calc.FromInteger(6), firstPoint.X), calc.FromInteger(2), calc.Zero, calc.Zero },
            });

            Vector<T,C> result;
            Vector<T,C> rights = new T[] { firstPoint.Y, secondPoint.Y, firstDerivative, secondDerivative };

            WhiteMath.Matrices.SLAESolving.LU_FactorizationSolving(matrix, rights, out result);

            this.a = result[0];
            this.b = result[1];
            this.c = result[2];
            this.d = result[3];

            this.x0 = calc.Zero;
        }

        // -----------------------
        // ------ VALUE ----------
        // -----------------------

        public T Value(T x)
        {
            Numeric<T, C> xNew = x - x0;

            return ((a*xNew + b)*xNew + c)*xNew + d;
        }
    }
}
