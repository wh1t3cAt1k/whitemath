using System.Collections.Generic;

using whiteMath.Calculators;
using whiteMath.General;

namespace whiteMath.Functions
{
    /// <summary>
    /// This class provides static methods
    /// to create continuous or piece functions
    /// interpolating the points arrays.
    /// <typeparam name="T">The type of numeric arguments of the function.</typeparam>
    /// <typeparam name="C">The calculator for the numeric type T.</typeparam>
    /// </summary>
    public static class Interpolation<T, C> where C: ICalc<T>, new()
    {
        private static ICalc<T> calc = Numeric<T, C>.Calculator;

        /// <summary>
        /// Creates a continuous piece-linear function from the points list.
        /// The user should also specify a default value to return
        /// when the function argument is out of the interpolation area bounds.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static PieceFunction<T, C> CreatePieceLinearFunction(IList<Point<T>> points, T defaultValue)
        {
            BoundedInterval<T, C>[] intervals = new BoundedInterval<T, C>[points.Count - 1];
            IFunction<T, T>[] functions = new IFunction<T, T>[points.Count - 1];

            for (int i = 0; i < points.Count - 1; i++)
            {
                intervals[i] = new BoundedInterval<T, C>(points[i].X, points[i + 1].X, true, false);
                functions[i] = new LinearFunction<T, C>(points[i], points[i + 1]);
            }

            PieceFunction<T, C> fun = new PieceFunction<T, C>(intervals, functions, defaultValue);
            fun.Type = PieceFunctionType.PieceLinearFunction;

            return fun;
        }

        /// <summary>
        /// Creates an interpolational polynom from the points array. 
        /// This polynom, according to the equivalence of the representation, 
        /// is unique.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Polynom<T, C> CreatePolynom(IList<Point<T>> points)
        {
            return new Polynom<T, C>(points);
        }

        /// <summary>
        /// Creates a continuous natural cubic spline of defect 1.
        /// Max. continuous derivative of the spline is second.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static PieceFunction<T, C> CreateNaturalCubicSpline(IList<Point<T>> points, T defaultValue)
        {
            int n = points.Count - 1;   // количество точек

            Numeric<T, C>[] a = new Numeric<T,C>[n];
            Numeric<T, C>[] c = new Numeric<T, C>[n];

            DefaultList<Numeric<T, C>> b = new DefaultList<Numeric<T,C>>(new Numeric<T,C>[n], Numeric<T,C>.Zero);
            
            Numeric<T, C>[] delta = new Numeric<T,C>[n];
            Numeric<T, C>[] lambda = new Numeric<T,C>[n];

            Numeric<T, C>[] h = new Numeric<T,C>[n];
            Numeric<T, C>[] fDiv = new Numeric<T,C>[n];

            KeyValuePair<BoundedInterval<T, C>, IFunction<T,T>>[] pieces = new KeyValuePair<BoundedInterval<T,C>,IFunction<T,T>>[n];

            // count h and fDiv

            for (int i = 0; i < n; i++)
            {
                h[i] = calc.dif(points[i+1].X, points[i].X);
                fDiv[i] = calc.dif(points[i+1].Y, points[i].Y) / h[i];
            }

            // h идут не от 1 до n
            // а от 0 до n-1.

            delta[0] = (Numeric<T,C>)(-0.5) * h[1] / (h[0] + h[1]);
            lambda[0] = (Numeric<T,C>)(1.5) * (fDiv[1] - fDiv[0]) / (h[0] + h[1]);

            // calculating lambda

            Numeric<T, C> two = (Numeric<T, C>)2;
            Numeric<T, C> three = (Numeric<T, C>)3;

            for (int i = 2; i < n; i++)
            {
                delta[i - 1] = -h[i] / (two * (h[i - 1] + h[i]) + h[i - 1] * delta[i - 2]);
                lambda[i - 1] = (three * (fDiv[i] - fDiv[i - 1]) - h[i - 1] * lambda[i - 2]) / (two * (h[i - 1] + h[i]) + h[i - 1] * delta[i - 2]);
            }

            // calculating b

            b[n - 1] = Numeric<T,C>.Zero;

            for (int i = n - 1; i > 0; i--)
                b[i - 1] = delta[i - 1] * b[i] + lambda[i - 1];

            // calculating others

            for (int i = 0; i < n; i++)
            {
                a[i] = (b[i] - b[i - 1]) / (three * h[i]);
                c[i] = fDiv[i] + two * h[i] * b[i] / three + h[i] * b[i - 1] / three;
            
                pieces[i] = new KeyValuePair<BoundedInterval<T,C>,IFunction<T,T>>(
                    new BoundedInterval<T,C>(points[i].X, points[i+1].X, true, false),
                    new CubicFunction<T,C> (a[i], b[i], c[i], points[i+1].Y, points[i+1].X));
            }

            PieceFunction<T, C> function = new PieceFunction<T, C>(defaultValue, pieces);
            function.Type = PieceFunctionType.NaturalCubicSpline;

            return new PieceFunction<T, C>(defaultValue, pieces);
        }
    }
}
