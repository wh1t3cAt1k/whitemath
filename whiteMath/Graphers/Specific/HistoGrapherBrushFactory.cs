using System;
using System.Collections.Generic;
using System.Drawing;

using whiteStructs.Conditions;

namespace whiteMath.Graphers
{
    public static class HistoGrapherBrushFactory
    {
        public static Dictionary<string, Brush> CreateValueDependentBrushes(this HistoGrapher histographer, double minValue, double maxValue, Color minColor, Color maxColor)
        {
			Condition.ValidateNotNull(histographer, nameof(histographer));
			Condition
				.Validate(minValue < maxValue)
				.OrArgumentException("The minimum value should not exceed the maximum value.");
			Condition
				.Validate(histographer.MinValue >= minValue && histographer.MaxValue <= maxValue)
				.OrArgumentException("HistoGrapher's values should not exceed the explicitly specified bounds of colour.");

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

            return histographer.CreateValueDependentBrushes(minValue, maxValue, colorMapper);
        }

        public static Dictionary<string, Brush> CreateValueDependentBrushes(this HistoGrapher histographer, double minValue, double maxValue, Func<double, Color> colorMapper)
        {
			Condition.ValidateNotNull(histographer, nameof(histographer));
			Condition.ValidateNotNull(colorMapper, nameof(colorMapper));
			Condition
				.Validate(minValue < maxValue)
				.OrArgumentException("The minimum value should not exceed the maximum value.");
			Condition
				.Validate(histographer.MinValue >= minValue && histographer.MaxValue <= maxValue)
				.OrArgumentException("HistoGrapher's values should not exceed the explicitly specified bounds of colour.");
			
            return histographer.CreateValueDependentBrushes(minValue, maxValue, (x => new SolidBrush(colorMapper(x))));
        }

        public static Dictionary<string, Brush> CreateValueDependentBrushes(this HistoGrapher histographer, double minValue, double maxValue, Func<double, Brush> brushMapper)
        {
			Condition.ValidateNotNull(histographer, nameof(histographer));
			Condition.ValidateNotNull(brushMapper, nameof(brushMapper));
			Condition
				.Validate(minValue < maxValue)
				.OrArgumentException("The minimum value should not exceed the maximum value.");
			Condition
				.Validate(histographer.MinValue >= minValue && histographer.MaxValue <= maxValue)
				.OrArgumentException("HistoGrapher's values should not exceed the explicitly specified bounds of colour.");
			
            Dictionary<string, Brush> result = new Dictionary<string, Brush>();

            double rangeLength = maxValue - minValue;

            foreach (KeyValuePair<string, double> kvp in histographer.pointValuePairs)
            {
                double coefficient = (kvp.Value - minValue) / rangeLength;
                result.Add(kvp.Key, brushMapper(coefficient));
            }

            return result;
        }
    }
}
