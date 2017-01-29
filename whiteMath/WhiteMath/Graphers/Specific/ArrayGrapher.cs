using System.Collections.Generic;
using System.Linq;

using WhiteMath.General;

using WhiteStructs.Conditions;

namespace WhiteMath.Graphers
{
    public class ArrayGrapher: StandardGrapher
    {
        /// <summary>
        /// Constructs a new <see cref="ArrayGrapher"/> object
        /// using a list of (x, y) points.
        /// </summary>
        /// <param name="pointSequence">An enumerable list of (x, y) points.</param>
        public ArrayGrapher(IEnumerable<Point<double>> pointSequence)
        {
			Condition.ValidateNotNull(pointSequence, nameof(pointSequence));
			Condition.ValidateNotEmpty(pointSequence, "The point sequence should contain at least one point.");

            this.PointsArray = pointSequence.ToArray();
            FindMaximumsAndMinimums();
        }

        /// <summary>
        /// Constructs a new <see cref="ArrayGrapher"/> object
        /// using separate point lists for 'x' and 'y'.
        /// </summary>
        /// <param name="xSequence">An enumerable list of 'x' points.</param>
        /// <param name="ySequence">An enumerable list of 'y' points.</param>
        public ArrayGrapher(IEnumerable<double> xSequence, IEnumerable<double> ySequence)
        {
			Condition.ValidateNotNull(xSequence, nameof(xSequence));
			Condition.ValidateNotNull(ySequence, nameof(ySequence));
			Condition.ValidateNotEmpty(xSequence, "The sequence of x values should not be empty.");
			Condition
				.Validate(xSequence.Count() == ySequence.Count())
				.OrArgumentException("The lengths of 'x' and 'y' point sequences must be equal.");

            this.PointsArray = xSequence
                .Zip(ySequence, (x, y) => new Point<double>(x, y))
                .ToArray();

            FindMaximumsAndMinimums();
        }
    }
}
