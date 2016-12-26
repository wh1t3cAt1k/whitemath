using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace WhiteMath.Geometry
{
    /// <summary>
    /// Structure that represents a point whose coordinates
    /// are double-precision values.
    /// </summary>
    public struct PointD
    {
        /// <summary>
        /// Gets the X coordinate of the point.
        /// </summary>
        public double X { get; set; }
        
        /// <summary>
        /// Gets the Y coordinate of the point.
        /// </summary>
        public double Y { get; set; }

        public PointD(double x, double y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        public static explicit operator PointF(PointD point)
        {
            return new PointF((float)point.X, (float)point.Y);
        }

        public static implicit operator PointD(PointF point)
        {
            return new PointD(point.X, point.Y);
        }

        // ---------------------------------------
        // -------------- Comparers --------------

        private class ___XComparer: IComparer<PointD>
        {
            public int Compare(PointD first, PointD second)
            {
                return first.X.CompareTo(second.X);
            }
        }

        private class ___YComparer : IComparer<PointD>
        {
            public int Compare(PointD first, PointD second)
            {
                return first.Y.CompareTo(second.Y);
            }
        }

        private static readonly ___XComparer ___XComparerInstnc = new ___XComparer();
        private static readonly ___YComparer ___YComparerInstnc = new ___YComparer();

        /// <summary>
        /// Returns the comparer that compares PointD objects on their X values.
        /// </summary>
        public static IComparer<PointD> ComparerOnX { get { return ___XComparerInstnc; } }
        
        /// <summary>
        /// Returns the comparer that compares PointD objects on their Y values.
        /// </summary>
        public static IComparer<PointD> ComparerOnY { get { return ___YComparerInstnc; } }
    }
}