using System;
using System.Collections.Generic;
using System.Linq;

using WhiteMath.Calculators;
using WhiteMath.General;
using WhiteMath.Matrices;

namespace WhiteMath.Functions
{
    /// <summary>
    /// Represents a polynom in the Lagrange interpolation form.
    /// Used to create a polynom of degree N basing on its equivalent form
    /// of argument/value pairs in (N+1) points.
    /// 
    /// Although the simple Polynom(T,C) can be used if the appropriate SLAE
    /// is solved to explicitly find all the coefficients, the system's matrix
    /// is very ill-conditioned and thus there is a big probability 
    /// of badly calculated result.
    /// 
    /// On the other side, LagrangePolynom's calculation method 
    /// would take much more time than of the simple Polynom using Horner's scheme.
    /// </summary>
    /// <see cref="Polynom&lt;T, C&gt;"/>
    /// <typeparam name="T">The type of polynom coefficients/value.</typeparam>
    /// <typeparam name="C">The calculator for the coefficient type.</typeparam>
    public class LagrangePolynom<T, C>: IFunction<T, T>, ICloneable where C: ICalc<T>, new() 
    {
        private static ICalc<T> calc = Numeric<T, C>.Calculator;

        Point<T>[] points;
        MatrixSDA<T, C> difMatrix;

        /// <summary>
        /// Returns the formal degree of the polynom.
        /// Meanwhile, if the lagrange polynom is a line built on
        /// three points, it will return degree 2 even though
        /// the coefficient of x^2 will be zero.
        /// </summary>
        public int FormalDegree
        {
            get { return points.Length - 1; }
        }

        // ----------------------------------------
        // ------------ get basis poly ------------
        // ----------------------------------------

        /// <summary>
        /// Gets the basis polynom of zero-based index i for
        /// the current Lagrange interpolation polynom.
        /// </summary>
        /// <param name="i">The index of the basis polynom to get. Lies in interval [0; n-1].</param>
        /// <returns>The i-th basis polynom for the current Lagrange interpolation polynom.</returns>
        public Polynom<T, C> BasisPolynom(int i)
        {
            Polynom<T, C> mul = new Polynom<T, C>((Numeric<T,C>)1);

            for (int j = 0; j < this.points.Length; j++)
                if (i != j)
                    mul *= new Polynom<T, C>(this.points[j].X / difMatrix[i, j], Numeric<T,C>._1 / difMatrix[i, j]);

            return mul;
        }

        // ----------------------------------------

        private LagrangePolynom(IList<Point<T>> points, bool checkSorted, bool createMatrix)
        {
            this.points = points.ToArray();

            IComparer<Point<T>> comparer = Point<T>.GetComparerOnX(Numeric<T, C>.UnderlyingTypeComparer);

            // Если списокъ не отсортированъ, 
            // то это нужно сделать сей же часъ.

            if (checkSorted && !this.points.IsSorted(comparer))
                this.points.SortShell(comparer);

            // Создаем матрицу разностей.

            if (createMatrix)
            {
                int n = this.points.Length;

                difMatrix = new MatrixSDA<T, C>(n, n);

                for (int i=0; i<n; i++)
                {
                    difMatrix[i, i] = Numeric<T, C>.Zero;

                    for (int j = 0; j < i; j++)
                        difMatrix[i, j] = calc.Subtract(this.points[i].X, this.points[j].X);
                }

                // Теперь заполняем то, что выше главной диагонали, противоположными значениями.

                for (int i = 0; i < n; i++)
                    for (int j = i+1; j < n; j++)
                        difMatrix[i, j] = -difMatrix[j, i];
            }
        }

        /// <summary>
        /// Creates a new instance of Lagrange polynom object using the points list.
        /// </summary>
        /// <param name="points">The points list.</param>
        public LagrangePolynom(IList<Point<T>> points)
            : this(points, true, true)
        { }

        /// <summary>
        /// Creates a new instance of Lagrange polynom object using the points parameters.
        /// </summary>
        /// <param name="points">The points parameter array.</param>
        public LagrangePolynom(params Point<T>[] points)
            : this(points, true, true)
        { }


        // ----------------------------------------
        // ------- conversion methods -------------
        // ----------------------------------------

        public Polynom<T, C> AsStandardPolynom
        {
            get
            {
                Numeric<T,C> one = Numeric<T,C>._1;

                // -------------------------------------

                Polynom<T, C> sum = new Polynom<T, C>(Numeric<T,C>.Zero);
                
                for (int i = 0; i < this.points.Length; i++)
                {
                    Polynom<T, C> mul = new Polynom<T,C>(one);

                    for (int j = 0; j < this.points.Length; j++)
                        if(i!=j)
                            mul *= new Polynom<T,C>(this.points[j].X/difMatrix[i,j], one/difMatrix[i,j]);

                    sum += mul * this.points[i].Y;
                }

                return sum;
            }
        }

        public static explicit operator Polynom<T, C>(LagrangePolynom<T, C> obj)
        {
            return obj.AsStandardPolynom;
        }

        // ----------------------------------------
        // ------- function methods ---------------
        // ----------------------------------------

        public T Value(T x)
        {
            Numeric<T, C> mul;
            Numeric<T, C> result = Numeric<T, C>.Zero;      // результат
            
            for (int i = 0; i < points.Length; i++)
            {
                mul = Numeric<T,C>._1;

                for (int j = 0; j < points.Length && mul != Numeric<T,C>.Zero; j++)
                    if (i != j)
                        mul *= calc.Subtract(x, points[j].X) / difMatrix[i, j];

                result += mul*points[i].Y;
            }

            return result;
        }

        // ----------------------------------------
        // ------- object methods overriding ------
        // ----------------------------------------

        public object Clone()
        {
            LagrangePolynom<T,C> newObject = new LagrangePolynom<T,C>(this.points, false, false);
            newObject.difMatrix = this.difMatrix;

            return newObject;
        }

        public override bool Equals(object obj)
        {
            if (obj is LagrangePolynom<T, C>)
            {
                LagrangePolynom<T, C> poly = obj as LagrangePolynom<T, C>;

                // Если количество точек совпадает, проверяем поточечно.

                if (poly.points.Length == this.points.Length)
                {
                    for (int i = 0; i < poly.points.Length; i++)
                        if (!calc.Equal(this.points[i].X, poly.points[i].X) || !calc.Equal(this.points[i].Y, poly.points[i].Y))
                            return false;

                    return true;
                }

                // Если количество точек не совпадает, то...
                // К сожалению, может иметь место линейная зависимость. 
                // Например, параболу тоже при желании можно определить по пяти точкам.
                // Единственный выход - приводить все к канонической форме и сравнивать по коэффициентам.

                return this.AsStandardPolynom.Equals(poly.AsStandardPolynom);
            }

            else if (obj is Polynom<T, C>)
                return this.AsStandardPolynom.Equals(obj as Polynom<T, C>);

            return false;
        }
    }
}
