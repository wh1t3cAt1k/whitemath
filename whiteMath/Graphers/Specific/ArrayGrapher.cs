using System;
using System.Collections.Generic;
using System.Linq;

using whiteMath.General;

namespace whiteMath.Graphers
{
    public class ArrayGrapher: StandardGrapher
    {
        /// <summary>
        /// Constructs a new <see cref="ArrayGrapher"/> object
        /// using a list of (x, y) points.
        /// </summary>
        /// <param name="pointsArray">An enumerable list of (x, y) points.</param>
        public ArrayGrapher(IEnumerable<Point<double>> pointsArray)
        {
            Contract.Requires<ArgumentNullException>(pointsArray != null, "pointsArray");
            Contract.Requires<ArgumentException>(pointsArray.Count() > 0, "The point list does not contain any points.");

            // No need to sort?
            // GrapherArrayWork.DoublePointSort(PointsArray);            

            this.PointsArray = pointsArray.ToArray();
            FindMaximumsAndMinimums();
        }

        /// <summary>
        /// Constructs a new <see cref="ArrayGrapher"/> object
        /// using separate point lists for 'x' and 'y'.
        /// </summary>
        /// <param name="xList">An enumerable list of 'x' points.</param>
        /// <param name="yList">An enumerable list of 'y' points.</param>
        public ArrayGrapher(IEnumerable<double> xList, IEnumerable<double> yList)
        {
            Contract.Requires<ArgumentNullException>(xList != null, "xList");
            Contract.Requires<ArgumentNullException>(yList != null, "yList");
            Contract.Requires<ArgumentException>(xList.Count() > 0, "The 'xList' does not contain any points.");
            Contract.Requires<ArgumentException>(xList.Count() == yList.Count(), "The lengths of 'x' and 'y' point lists must be equal.");

            this.PointsArray = xList
                .Zip(yList, (x, y) => new Point<double>(x, y))
                .ToArray();

            FindMaximumsAndMinimums();
        }
    }
}
