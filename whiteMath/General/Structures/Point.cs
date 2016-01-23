using System;
using System.Collections.Generic;

using whiteMath.Calculators;

using whiteStructs.Conditions;

namespace whiteMath.General
{
    /// <summary>
    /// Represents the pair of objects as a logical point.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the point.</typeparam>
    [Serializable]
    public struct Point<T>
    {
        // ----------------------------
        // ----------- Construction ---
        // ----------------------------

        /// <summary>
        /// Constructs the point object using two separate objects.
        /// </summary>
        /// <param name="x">The first object.</param>
        /// <param name="y">The second object.</param>
        public Point(T x, T y): this()
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Provided with a calculator type for the type <typeparamref name="T"/>,
        /// will return a zero-coordinate point where 'zero' value is
        /// specified by the <typeparamref name="C"/> calculator type.
        /// </summary>
        /// <typeparam name="C">A calculator for the <typeparamref name="T"/> numeric type.</typeparam>
        /// <returns>
        /// A zero-coordinate point where 'zero' value is
        /// specified by the <typeparamref name="C"/> calculator type.
        /// </returns>
        public Point<T> ZeroPoint<C>() where C : ICalc<T>, new()
        {
            return new Point<T>(Numeric<T, C>.Zero, Numeric<T, C>.Zero);
        }

        // ----------------------------------------------

        /// <summary>
        /// The X coordinate of the point.
        /// </summary>
        public T X { get; private set; }

        /// <summary>
        /// The Y coordinate of the point.
        /// </summary>
        public T Y { get; private set; }

        // ----------------------------------------------

        /// <summary>
        /// Provides an alternative way of accessing the point elements.
        /// </summary>
        /// <param name="index">The index value. Should be 0 or 1.</param>
        /// <returns>The value of the first or the second point element respectively.</returns>
        public T this[int index]
        {
            get
            {
				Condition.Validate(index == 0 || index == 1)
					.OrIndexOutOfRangeException(Messages.OnlyZeroAndOneCanBeUsedToAccessPointCoordinates);

                if (index == 0) 
					return X;
                else 
					return Y;
            }

            set
            {
				Condition.Validate(index == 0 || index == 1)
					.OrIndexOutOfRangeException(Messages.OnlyZeroAndOneCanBeUsedToAccessPointCoordinates);

                if (index == 0)
					X = value;
                else 
					Y = value;             
            }
        }

        // -------------------------------------
        // ----------- comparers ---------------
        // -------------------------------------

        /// <summary>
        /// Сравниватель точек на основе сравнивателя аргументов точек.
        /// </summary>
        private class PointComparer: IComparer<Point<T>>
        {
            private IComparer<T> tComparer;     // сравниватель точек
            bool onArgument;                    // на аргумент. Если нет, то сравнение по Y.

            public PointComparer(IComparer<T> tComparer, bool onArgument)
            {
				Condition.ValidateNotNull(tComparer, nameof(tComparer));

                this.onArgument = onArgument;
                this.tComparer = tComparer;
            }

            public int Compare(Point<T> one, Point<T> two)
            {
                if (onArgument)
                    return tComparer.Compare(one.X, two.X);
                else
                    return tComparer.Compare(one.Y, two.Y);
            }
        }

        /// <summary>
        /// Returns the comparer for the points using the
        /// comparer for the point components of type <typeparamref name="T"/>.
        /// 
        /// The resulting comparer would compare two points on their X values.
        /// </summary>
        /// <param name="tComparer">A comparer for the point component type.</param>
        /// <returns>The resulting comparer that compares two points on their X values.</returns>
        public static IComparer<Point<T>> GetComparerOnX(IComparer<T> tComparer)
        {
			Condition.ValidateNotNull(tComparer);
            return new PointComparer(tComparer, true);
        }

        /// <summary>
        /// Returns the comparer for the points using the
        /// comparer for the point components of type <typeparamref name="T"/>.
        /// 
        /// The resulting comparer would compare two points on their Y values.
        /// </summary>
        /// <param name="tComparer">A comparer for the point component type.</param>
        /// <returns>The resulting comparer that compares two points on their Y values.</returns>
        public static IComparer<Point<T>> GetComparerOnY(IComparer<T> tComparer)
        {
			Condition.ValidateNotNull(tComparer, nameof(tComparer));
            return new PointComparer(tComparer, false);
        }

        // ----------------------------------------
        // ----------- Non-generic operators ------
        // ----------------------------------------

		// TODO: Not working. Should be moved to a separate (?)

        /*
        /// <summary>
        /// Converts a point of doubles to drawing structure PointF.
        /// Precision may be lost due to the double-->float cast.
        /// </summary>
        /// <see cref="PointF"/>
        /// <param name="point">The point to be converted.</param>
        /// <returns>
        /// A PointF structure containing the same coordinates 
        /// (with possibly less precision) as the converted Point&lt;double&gt; object.
        /// </returns>
        public static explicit operator System.Drawing.PointF(Point<double> point)
        {
            return new System.Drawing.PointF((float)point.X, (float)point.Y);
        }

        /// <summary>
        /// Converts a point of floats to drawing structure PointF.
        /// </summary>
        /// <see cref="PointF"/>
        /// <param name="point">The point to be converted.</param>
        /// <returns>
        /// A PointF structure containing the same coordinates 
        /// as the converted Point&lt;double&gt; object.
        /// </returns>
        public static implicit operator System.Drawing.PointF(Point<float> point)
        {
            return new System.Drawing.PointF(point.X, point.Y);
        }

        /// <summary>
        /// Converts a PointF structure to a point of floats.
        /// </summary>
        /// <see cref="PointF"/>
        /// <param name="point">The point to be converted.</param>
        /// <returns>
        /// A Point&lt;float&gt; structure containing the same coordinates 
        /// as the passed PointF structure.
        /// </returns>
        public static implicit operator Point<float>(System.Drawing.PointF point)
        {
            return new Point<float>(point.X, point.Y);
        }

        /// <summary>
        /// Converts a PointF structure to a point of doubles.
        /// </summary>
        /// <see cref="PointF"/>
        /// <param name="point">The point to be converted.</param>
        /// <returns>
        /// A Point&lt;double&gt; structure containing the same coordinates 
        /// as the passed PointF structure.
        /// </returns>
        public static implicit operator Point<double>(System.Drawing.PointF point)
        {
            return new Point<double>(point.X, point.Y);
        }
         */
    }
}
