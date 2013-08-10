using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath.Geometry
{
    /// <summary>
    /// Represents a vector which is built on top of two
    /// points (PointD objects).
    /// </summary>
    public struct VectorD
    {
        /// <summary>
        /// Gets the starting point of the vector.
        /// </summary>
        public PointD StartPoint { get; private set; }

        /// <summary>
        /// Gets the ending point of the vector.
        /// </summary>
        public PointD EndPoint { get; private set; }

        /// <summary>
        /// Gets the middle point of the vector.
        /// </summary>
        public PointD MiddlePoint
        {
            get
            {
                return new PointD((EndPoint.X + StartPoint.X) / 2, (EndPoint.Y + StartPoint.Y) / 2);
            }
        }

        /// <summary>
        /// Returns the vector that has current vector's end point as his start point
        /// and the same coordinates.
        /// </summary>
        public VectorD Reverse
        {
            get
            {
                return new VectorD(this.EndPoint, this.StartPoint);
            }
        }

        /// <summary>
        /// Gets the X coordinate of the vector.
        /// </summary>
        public double X
        {
            get { return EndPoint.X - StartPoint.X; }
        }

        /// <summary>
        /// Gets the Y coordinate of the vector.
        /// </summary>
        public double Y
        {
            get { return EndPoint.Y - StartPoint.Y; }
        }

        /// <summary>
        /// Gets the length of the vector.
        /// </summary>
        public double Length
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y);
            }
        }

        /// <summary>
        /// Vector with new start point
        /// with the length scaled by specified double factor.
        /// </summary>
        /// <param name="coefficient">Should be from 0 to 1.</param>
        /// <param name="newStartPoint">New start point.</param>
        public VectorD VectorScaledNewStartPoint(double coefficient, PointD newStartPoint)
        {
            double newX = this.X * coefficient;
            double newY = this.Y * coefficient;

            return new VectorD(newStartPoint, new PointD(newStartPoint.X + newX, newStartPoint.Y + newY));
        }

        /// <summary>
        /// Vector with new end point
        /// with the length scaled by specified double factor.
        /// </summary>
        /// <param name="coefficient">Should be from 0 to 1.</param>
        /// <param name="newEndPoint">New end point.</param>
        public VectorD VectorScaledNewEndPoint(double coefficient, PointD newEndPoint)
        {
            double newX = this.X * coefficient;
            double newY = this.Y * coefficient;

            return new VectorD(new PointD(newEndPoint.X - newX, newEndPoint.Y - newY), newEndPoint);
        }

        /// <summary>
        /// Returns a vector of same length
        /// with a new start point.
        /// </summary>
        /// <param name="newStartPoint">The new starting point for the vector.</param>
        /// <returns>A vector with a new starting point.</returns>
        public VectorD VectorNewStartPoint(PointD newStartPoint)
        {
            return new VectorD(newStartPoint, new PointD(newStartPoint.X + this.X, newStartPoint.Y + this.Y));
        }

        /// <summary>
        /// Returns a vector of same length
        /// with a new end point.
        /// </summary>
        /// <param name="newEndPoint">The new ending point for the vector.</param>
        /// <returns>A vector with the new ending point.</returns>
        public VectorD VectorNewEndPoint(PointD newEndPoint)
        {
            return new VectorD(new PointD(newEndPoint.X - this.X, newEndPoint.Y - this.Y), newEndPoint);
        }

        /// <summary>
        /// Returns a vector rotated on the specified angle
        /// with new start point.
        /// </summary>
        public VectorD VectorRotatedNewStartPoint(double angle, PointD newStartPoint)
        {
            double cosFi = Math.Cos(angle);
            double sinFi = Math.Sin(angle);

            double x = this.X;
            double y = this.Y;

            double newX = cosFi * x - sinFi * y;
            double newY = sinFi * x + cosFi * y;

            return new VectorD(newStartPoint, new PointD(newStartPoint.X + newX, newStartPoint.Y + newY));
        }

        /// <summary>
        /// Returns a vector rotated on the specified angle
        /// with new end point.
        /// </summary>
        public VectorD VectorRotatedNewEndPoint(double angle, PointD newEndPoint)
        {
            double cosFi = Math.Cos(angle);
            double sinFi = Math.Sin(angle);

            double x = this.X;
            double y = this.Y;

            double newX = cosFi * x - sinFi * y;
            double newY = sinFi * y + cosFi * y;

            return new VectorD(new PointD(newEndPoint.X - newX, newEndPoint.Y - newY), newEndPoint);
        }

        // ---------------------------------
        // ----------- Constructor ---------
        // ---------------------------------

        public VectorD(PointD first, PointD second)
            : this()
        {
            this.StartPoint = first;
            this.EndPoint = second;
        }

        public VectorD(PointD first, double vectorX, double vectorY)
            : this()
        {
            this.StartPoint = first;
            this.EndPoint = new PointD(first.X + vectorX, first.Y + vectorY);
        }

        // ---------------------------------
        // ---------- multiplication -------
        // ---------------------------------

        /// <summary>
        /// Returns the scalar product of two vectors.
        /// 
        /// The scalar product of vectors A and B is equal to the value |A|*|B|*sin(phi),
        /// where phi is the angle between A to B.
        /// </summary>
        /// <param name="another">Another vector to find the scalar product to find the scalar product between the current vector and the specified.</param>
        /// <returns>The scalar product of two vectors.</returns>
        public double multiplyScalar(VectorD another)
        {
            return this.X * another.X + this.Y * another.Y;
        }

        /// <summary>
        /// Returns the wedge product of
        /// two vectors.
        /// 
        /// The wedge product of vectors A and B is equal to the value |A|*|B|*sin(phi),
        /// where phi is the angle of rotating A to B counterclockwise to become collinear.
        /// </summary>
        /// <param name="another">Another vector to find the wedge product between the current vector and the specified.</param>
        /// <returns>The wedge product of two vectors.</returns>
        public double multiplyWedge(VectorD another)
        {
            return this.X * another.Y - this.Y * another.X;
        }

        // --------------------------------------------
        // ---------------- STATIC MATHS --------------

        /// <summary>
        /// Returns the cosine of the angle between two vectors.
        /// </summary>
        /// <param name="first">The first vector.</param>
        /// <param name="second">The second vector.</param>
        /// <returns>The cosine of the angle between vectors.</returns>
        public static double angleCosBetweenVectors(VectorD first, VectorD second)
        {
            return (first.multiplyScalar(second)) / (first.Length * second.Length);
        }

        /// <summary>
        /// Returns the sine of the angle between two vectors.
        /// </summary>
        /// <param name="first">The first vector.</param>
        /// <param name="second">The second vector.</param>
        /// <returns>
        /// The sine of the angle that is required by the first vector to be 
        /// rotated counterclockwise to become collinear with the second.
        /// </returns>
        public static double angleSinBetweenVectors(VectorD first, VectorD second)
        {
            return (first.multiplyWedge(second)) / (first.Length * second.Length);
        }

        /// <summary>
        /// Returns the angle, in radians, that is required by the first vector
        /// to be rotated counterclockwise to become collinear with the second.
        /// </summary>
        /// <param name="first">The first vector.</param>
        /// <param name="second">The second vector.</param>
        /// <returns>
        /// The angle, in radians, that is required by the first vector
        /// to be rotated counterclockwise to become collinear with the second.
        /// </returns>
        public static double angleBetweenVectors(VectorD first, VectorD second)
        {
            double x1 = first.X;
            double y1 = first.Y;
            double x2 = second.X;
            double y2 = second.Y;

            return Math.Atan2(x1 * y2 - y1 * x2, x1 * x2 + y1 * y2);
        }
    }
}
