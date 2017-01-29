using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using WhiteMath;
using WhiteMath.Calculators;

using WhiteStructs.Conditions;
using WhiteStructs.Collections;

namespace WhiteStructs.Drawing
{
    /// <summary>
    /// This class provides a way of mapping a real coefficient
    /// in the [0; 1] segment to RGB color space using a pre-specified sequence of colors and
    /// the linear gradient method.
    /// </summary>
    public class ColorPath
    {
        BoundedInterval<double, CalcDouble>[] intervals;
        Color[] colors;

        /// <summary>
        /// Returns the <c>Func</c> delegate that maps double coefficients
        /// in the [0; 1] segment to Color values according to the current
        /// <c>ColorPath</c> object's color sequence.
        /// </summary>
        public Func<double, Color> ColorFunction { get { return x => this.Map(x); } }

        /// <summary>
        /// Maps a real coefficient in [0; 1] to a position on
        /// linear color path made of <c>ColorPath</c>'s specified colors.
        /// </summary>
        /// <param name="coefficient">A coefficient in the [0; 1] segment.</param>
        /// <returns>A <c>Color</c> structure 'between' the first color and the last color of the linear path, according to the coefficient's value.</returns>
        public Color Map(double coefficient)
        {
			Condition
				.Validate(coefficient >= 0 && coefficient <= 1)
				.OrArgumentOutOfRangeException("The coefficient must belong to [0; 1] segment.");

            int i = 0;

            if (this.intervals.Length > 1)
            {
                for (; i < intervals.Length; i++)
                {
                    if (this.intervals[i].Contains(coefficient))
                        break;
                }
            }

            if (i == intervals.Length)
                throw new MissingMemberException("The color mapper couldn't find the specified color. Internal inconsistence. Do not use this class anymore.");

            Color lower = this.colors[i];
            Color upper = this.colors[i + 1];

            double aDif = upper.A - lower.A;
            double rDif = upper.R - lower.R;
            double gDif = upper.G - lower.G;
            double bDif = upper.B - lower.B;

            return Color.FromArgb(
                (int)Math.Round(lower.A + coefficient * aDif),
                (int)Math.Round(lower.R + coefficient * rDif),
                (int)Math.Round(lower.G + coefficient * gDif),
                (int)Math.Round(lower.B + coefficient * bDif));
        }

        /// <summary>
        /// Initializes the color path with a sequence of colors.
        /// </summary>
        /// <param name="colors">A parameter array of two or more colors.</param>
        public ColorPath(params Color[] colors)
        {
			Condition.ValidateNotNull(colors, nameof(colors));
			Condition
				.Validate(colors.Length >= 2)
				.OrArgumentException("The color path must consist of at least two colors.");

            __init(colors);
        }

        /// <summary>
        /// Initializes the color path with a sequence of colors.
        /// </summary>
        /// <param name="colorSequence">A sequence of two or more colors.</param>
        public ColorPath(IEnumerable<Color> colorSequence)
        {
			Condition.ValidateNotNull(colorSequence, nameof(colorSequence));
			Condition
				.Validate(!colorSequence.IsEmpty() && !colorSequence.IsSingleElement())
				.OrArgumentException("The color path must consist of at least two colors.");

            __init(colorSequence);
        }

        private void __init(IEnumerable<Color> colorSequence)
        {
            int colorCount = colorSequence.Count();

            this.intervals = new BoundedInterval<double,CalcDouble>[colorCount - 1];
            this.colors = colorSequence.ToArray();

            double intervalLength = (double)1 / (colorCount - 1);

            double leftBound;
            double rightBound = 0;

            for (int i = 0; i < colorCount - 2; i++)
            {
                leftBound = rightBound;
                rightBound = (i + 1) * intervalLength;

                this.intervals[i] = new BoundedInterval<double, CalcDouble>(leftBound, rightBound, true, false);
            }

            leftBound = rightBound;
            rightBound = 1;

            this.intervals[colorCount - 2] = new BoundedInterval<double, CalcDouble>(leftBound, rightBound, true, true);
        }
    }
}