using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics.Contracts;

using whiteMath.General;

namespace whiteMath.Graphers
{
    /// <summary>
    /// This class is capable of drawing 2D and 3D histograms.
    /// </summary>
    [ContractVerification(true)]
    public class HistoGrapher
    {
        /// <summary>
        /// Hidden field.
        /// </summary>
        [ContractPublicPropertyName("PointValuePairs")]
        internal SortedDictionary<string, double> pointValuePairs;

        /// <summary>
        /// Gets a collection of point-value pairs associated with the current <c>HistoGrapher</c>.
        /// </summary>
        public Dictionary<string, double> PointValuePairs { get { return new Dictionary<string, double>(pointValuePairs); } }

        /// <summary>
        /// Gets a sequence of point names associated with the current <c>HistoGrapher</c>.
        /// </summary>
        public IEnumerable<string> PointNames { get { return pointValuePairs.Select(kvp => kvp.Key); } }

        /// <summary>
        /// Gets the minimum value from current HistoGrapher's point-value pairs.
        /// </summary>
        public double MinValue { get { return pointValuePairs.Min(kvp => kvp.Value); } }

        /// <summary>
        /// Gets the maximum value from current HistoGrapher's point-value pairs.
        /// </summary>
        public double MaxValue { get { return pointValuePairs.Max(kvp => kvp.Value); } }

        /// <summary>
        /// Initializes a HistoGrapher object with separate lists of
        /// points and values.
        /// </summary>
        /// <remarks>The lengths of the lists should be equal.</remarks>
        /// <param name="points">A list containing the names of the points.</param>
        /// <param name="values">A list containing the values for corresponding points.</param>
        public HistoGrapher(IList<string> points, IList<double> values)
        {
            Contract.Requires<ArgumentNullException>(points != null, nameof(points));
            Contract.Requires<ArgumentNullException>(values != null, nameof(values));
			Contract.Requires<ArgumentException>(points.Count == values.Count, Messages.SequenceLengthsAreNotEqual);
            Contract.Requires<ArgumentOutOfRangeException>(points.Any(), Messages.SequenceShouldContainAtLeastOneElement);

            _initialize(points.Select((point, pointIndex) => new KeyValuePair<string, double>(point, values[pointIndex])));
        }

        /// <summary>
        /// Initializes the HistoGrapher with a sequence of point-value pairs.
        /// </summary>
        /// <param name="pointValuePairs">A sequence (e.g. a <c>Dictionary</c>) containing point-value pairs to draw.</param>
        public HistoGrapher(IEnumerable<KeyValuePair<string, double>> pointValuePairs)
        {
            Contract.Requires<ArgumentNullException>(pointValuePairs != null, nameof(pointValuePairs));
            Contract.Requires<ArgumentOutOfRangeException>(pointValuePairs.Any(), Messages.SequenceShouldContainAtLeastOneElement);

            _initialize(pointValuePairs);
        }

        private void _initialize(IEnumerable<KeyValuePair<string, double>> pointValuePairs)
        {
            Contract.Assume(pointValuePairs != null);
            Contract.Assume(pointValuePairs.Any());
            
            this.pointValuePairs = new SortedDictionary<string, double>();

            foreach (KeyValuePair<string, double> kvp in pointValuePairs)
                this.pointValuePairs.Add(kvp.Key, kvp.Value);
        }

        [ContractInvariantMethod]
        private void __invariant()
        {
            Contract.Invariant(this.pointValuePairs != null);
        }

        // ---------------------------------------------------

        private class HistoGraphingArgs
        {
            // double minValue
        }

        /// <summary>
        /// Draws a 2-dimensional histogram within specified rectangle.
        /// </summary>
        /// <param name="minValue">The absolute minimum of the graphing range. Must be less than the minimal value within current <c>HistoGrapher</c>'s point-value pairs.</param>
        /// <param name="baseValue">The absolute base-line of the graphing range. Columns' bottoms will have the coordinate of the baseline.</param>
        /// <param name="maxValue">The absolute maximum of the graphing range. Must be bigger that the maximal value within current <c>HistoGrapher</c>'s point-value pairs.</param>
        /// <param name="columnPortion">A value from (0, 1] interval which specifies what percentage of width columns will take from automatically allocated.</param>
        /// <param name="G">A <c>Graphics</c> object to draw with.</param>
        /// <param name="drawingArea">The area to contain the histogram. Full width of this rectangle will be used.</param>
        /// <param name="pointBrushes">A dictionary containing brushes for all <c>HistoGrapher</c>'s points.</param>
        /// <param name="contourPen">The point used to draw columns' contours. May be null.</param>
        public void HistoGraph2D(double minValue, double baseValue, double maxValue, double columnPortion, Graphics G, RectangleF drawingArea, Dictionary<string, Brush> pointBrushes, Pen contourPen)
        {
            Contract.Requires<ArgumentNullException>(G != null, nameof(G));
            Contract.Requires<ArgumentNullException>(pointBrushes != null, nameof(pointBrushes));
            Contract.Requires<ArgumentException>(minValue <= baseValue && baseValue <= maxValue && minValue < maxValue, "The condition minValue <= baseValue <= maxValue AND minValue < maxValue must be met.");
            Contract.Requires<ArgumentException>(!drawingArea.IsEmpty, "The drawing area should be a non-empty rectangle.");
            Contract.Requires<ArgumentException>(Contract.ForAll(pointValuePairs, kvp => (kvp.Value >= minValue && kvp.Value <= maxValue)), "The graphing range does not contain one or more HistoGrapher internal values.");
            Contract.Requires<ArgumentException>(Contract.ForAll(pointValuePairs, kvp => pointBrushes.ContainsKey(kvp.Key)), "The brushes dictionary must contain brushes for all grapher points.");

            double absoluteDataHeight = maxValue - minValue;
            double coefficient = absoluteDataHeight / drawingArea.Height;   // сколько реальной величины приходится на 1 пиксель

            float columnAllocatedWidth = drawingArea.Width / pointValuePairs.Count;  // ширина пространства под колонку в гистограмме
            float columnFactWidth = (float)(columnAllocatedWidth * columnPortion);   // фактическая ширина колонки.

            float baseLinePixelY = (float)((maxValue - baseValue) / coefficient);    // координата Y на картинке базовой линии

            int k = 0;

            Dictionary<string, RectangleF> rectangleDictionary = new Dictionary<string, RectangleF>();

            foreach (KeyValuePair<string, double> kvp in pointValuePairs)
            {
                float leftXPosition = k * columnAllocatedWidth + (columnAllocatedWidth - columnFactWidth) / 2;      // координата X нижнего левого угла колонки.

                RectangleF columnRectangle;

                if (kvp.Value >= minValue && kvp.Value <= baseValue)
                {
                    // The bar "grows down"
                    // -
                    double currentDataHeight = baseValue - kvp.Value;
                    float pixelCurrentDataHeight = (float)(currentDataHeight / coefficient);

                    columnRectangle = new RectangleF(new PointF(drawingArea.X + leftXPosition, drawingArea.Y + baseLinePixelY), new SizeF(columnFactWidth, pixelCurrentDataHeight));
                }
                else
                {
                    // The bar "grows up"
                    // -
                    double currentDataHeight = kvp.Value - baseValue;
                    float pixelCurrentDataHeight = (float)(currentDataHeight / coefficient);

                    columnRectangle = new RectangleF(new PointF(drawingArea.X + leftXPosition, drawingArea.Y + baseLinePixelY - pixelCurrentDataHeight), new SizeF(columnFactWidth, pixelCurrentDataHeight));
                }

                rectangleDictionary.Add(kvp.Key, columnRectangle);
                ++k;
            }

            foreach(KeyValuePair<string, RectangleF> rectPair in rectangleDictionary)
            {
                RectangleF columnRectangle = rectPair.Value;
                G.FillRectangle(pointBrushes[rectPair.Key], columnRectangle);
            }

            if (contourPen != null)
            {
                foreach (KeyValuePair<string, RectangleF> rectPair in rectangleDictionary)
                {
                    RectangleF columnRectangle = rectPair.Value;
                    G.DrawRectangle(contourPen, columnRectangle.X, columnRectangle.Y, columnRectangle.Width, columnRectangle.Height);
                }
            }
        }
    }
}
