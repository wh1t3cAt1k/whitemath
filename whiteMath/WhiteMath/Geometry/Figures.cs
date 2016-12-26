using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteMath.Geometry
{
    public static class Figures
    {
        /// <summary>
        /// Returns a list of regular polygon's vertices, which has a 
        /// specified amount of sides and is inscribed into
        /// a circle of certain radius and center point.
        /// 
        /// The first vertice will be located at specified angle (counting counterclockwise)
        /// from the circle's rightmost point.
        /// </summary>
        /// <param name="sideCount">The total amount of polygon's sides. May be equal to 2 (then the resulting vertices will make up a line).</param>
        /// <param name="circleCenter">The center of the surrounding circle.</param>
        /// <param name="circleRadius">The radius of the surrounding circle.</param>
        /// <param name="initialAngle">The angle (in radians, counting counterclockwise from the circle's rightmost point) at which the first vertice will be located.</param>
        /// <returns>The list of regular polygon's vertices.</returns>
        public static List<PointD> RegularPolygonInscribedIntoCircle(int sideCount, PointD circleCenter, double circleRadius, double initialAngle = 0)
        {
            // Вектор в ноль градусов.
            VectorD initialVector = new VectorD(circleCenter, new PointD(circleCenter.X + circleRadius, circleCenter.Y));

            List<PointD> points = new List<PointD>(sideCount);

            // Вектор установлен на начальный угол.
            
            if(initialAngle != 0)
                initialVector = initialVector.VectorRotatedNewStartPoint(initialAngle, initialVector.StartPoint);
    
            points.Add(initialVector.EndPoint);

            double rotationAngle = 2 * Math.PI / sideCount;

            VectorD currentVector;

            for(int i=1; i<sideCount; i++)
            {
                currentVector = initialVector.VectorRotatedNewStartPoint(i * rotationAngle, initialVector.StartPoint);
                points.Add(currentVector.EndPoint);
            }

            return points;
        }
    }
}
