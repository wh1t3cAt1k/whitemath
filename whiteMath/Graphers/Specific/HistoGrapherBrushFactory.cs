using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics.Contracts;

namespace whiteMath.Graphers
{
    [ContractVerification(true)]
    public static class HistoGrapherBrushFactory
    {
        public static Dictionary<string, Brush> CreateValueDependentBrushes(this HistoGrapher hg, double minValue, double maxValue, Color minColor, Color maxColor)
        {
            Contract.Requires<ArgumentNullException>(hg != null, "hg");
            Contract.Requires<ArgumentException>(minValue < maxValue, "The minimum value should be less than the maximum value.");
            Contract.Requires<ArgumentException>(hg.MinValue >= minValue && hg.MaxValue <= maxValue, "HistoGrapher's values should not exceed the explicitly specified bounds of colour.");

            double rDiff = maxColor.R - minColor.R;
            double gDiff = maxColor.G - minColor.G;
            double bDiff = maxColor.B - minColor.B;
            double aDiff = maxColor.A - minColor.A;

            Func<double, Color> colorMapper = delegate(double k)
            {
                int rNew = minColor.R + (int)(k * rDiff);
                int gNew = minColor.G + (int)(k * gDiff);
                int bNew = minColor.B + (int)(k * bDiff);
                int aNew = minColor.A + (int)(k * aDiff);

                return Color.FromArgb(aNew, rNew, gNew, bNew);
            };

            return hg.CreateValueDependentBrushes(minValue, maxValue, colorMapper);
        }

        public static Dictionary<string, Brush> CreateValueDependentBrushes(this HistoGrapher hg, double minValue, double maxValue, Func<double, Color> colorMapper)
        {
            Contract.Requires<ArgumentNullException>(hg != null, "hg");
            Contract.Requires<ArgumentNullException>(colorMapper != null, "colorMapper");
            Contract.Requires<ArgumentException>(minValue < maxValue, "The minimum value should be less than the maximum value.");
            Contract.Requires<ArgumentException>(hg.MinValue >= minValue && hg.MaxValue <= maxValue, "HistoGrapher's values should not exceed the explicitly specified bounds of colour.");

            return hg.CreateValueDependentBrushes(minValue, maxValue, (x => new SolidBrush(colorMapper(x))));
        }

        public static Dictionary<string, Brush> CreateValueDependentBrushes(this HistoGrapher hg, double minValue, double maxValue, Func<double, Brush> brushMapper)
        {
            Contract.Requires<ArgumentNullException>(hg != null, "hg");
            Contract.Requires<ArgumentNullException>(brushMapper != null, "brushMapper");
            Contract.Requires<ArgumentException>(minValue < maxValue, "The minimum value should be less than the maximum value.");
            Contract.Requires<ArgumentException>(hg.MinValue >= minValue && hg.MaxValue <= maxValue, "HistoGrapher's values should not exceed the explicitly specified bounds of colour.");

            Dictionary<string, Brush> result = new Dictionary<string, Brush>();

            double rangeLength = maxValue - minValue;

            foreach (KeyValuePair<string, double> kvp in hg.pointValuePairs)
            {
                double coefficient = (kvp.Value - minValue) / rangeLength;
                result.Add(kvp.Key, brushMapper(coefficient));
            }

            return result;
        }
    }
}
