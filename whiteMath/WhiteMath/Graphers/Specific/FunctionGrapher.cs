using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WhiteMath.Functions;
using WhiteMath.General;

namespace WhiteMath.Graphers
{
    public class FunctionGrapher: AbstractGrapher
    {
        IFunction<double, double> function;
        int dotCount=1000; // количество точек для построения. По умолчанию - 500.

        /// <summary>
        /// How many points to calculate in the mentioned diap.
        /// Graphing quality depends on it. Usually 1000 is enough.
        /// </summary>
        public int DotCount { get { return dotCount; } set { if (value > 2) dotCount = value; } }

        public FunctionGrapher(IFunction<double,double> function)
        {
            if (function != null) this.function = function;
            else
                throw new GrapherSettingsException("Невозможно создать объект FunctionGrapher: переданная ссылка функции является ссылкой на null!");
        }

        public override void Graph(System.Drawing.Image destinationImage, GraphingArgs graphingArgs, double xMin, double xMax)
        {
            if (xMin >= xMax) throw new GrapherGraphException("Диапазоны должны задаваться от меньшего числа к большему.");

            double step = (xMax - xMin) / (dotCount - 1);
            double yMin, yMax;

            IList<Point<double>> pointsArray = GetPointsArraySkeleton(dotCount, xMin, xMax, out yMin, out yMax);
            GraphSkeleton(destinationImage, graphingArgs, xMin, xMax, yMin, yMax, pointsArray);
        }

        public override void Graph(System.Drawing.Image destinationImage, GraphingArgs graphingArgs, double xMin, double xMax, double yMin, double yMax)
        {
            if (xMin >= xMax || yMin >= yMax) throw new GrapherGraphException("Диапазоны должны задаваться от меньшего числа к большему.");

            double dummy;
            IList<Point<double>> pointsArray = GetPointsArraySkeleton(dotCount, xMin, xMax, out dummy, out dummy);

            GraphSkeleton(destinationImage, graphingArgs, xMin, xMax, yMin, yMax, pointsArray);
        }

        private void GraphSkeleton(System.Drawing.Image destinationImage, GraphingArgs graphingArgs, double xMin, double xMax, double yMin, double yMax, IList<Point<double>> pointsArray)
        {
            ArrayGrapher tmp = new ArrayGrapher(pointsArray);
            copyGrapherSignature(this, tmp);

            tmp.Graph(destinationImage, graphingArgs, xMin, xMax, yMin, yMax);
        }

        private IList<Point<double>> GetPointsArraySkeleton(int dotCount, double xMin, double xMax, out double yMin, out double yMax)
        {
            yMin = double.PositiveInfinity;
            yMax = double.NegativeInfinity;

            List<Point<double>> temPoints = new List<Point<double>>();

            double xtw;
            double ytw = double.NaN; // значение функции в указанной точке
            double yprev; // предыдущее значение функции

            double step = (xMax - xMin) / (dotCount - 1);

            for (double i=1; i<=dotCount; i++)
            {
                yprev = ytw;

                xtw = xMin + (i / dotCount)*(xMax - xMin);
                ytw = function.GetValue(xtw);

                // около границ области определения должна многократно повышаться точность.
                // изначально повышается более чем стократно

                // если переходим границу точности нуля, то уменьшаем точность.

                int k = 128;

                while(step/k == 0 && k>1)
                    k/=2;

                // если k = 0, то нет смысла увеличивать точность.
                // забиваем. если нет, то go.

                if (k > 1)
                    if ((!ytw.isNormalNumber() && yprev.isNormalNumber()) || (!yprev.isNormalNumber() && ytw.isNormalNumber()))
                    {
                        double xtmp;
                        double ytmp;
                        
                        for (double j=1; j<k; j++)
                        {
                            xtmp = xtw - (step - step * (j / k));
                            ytmp = function.GetValue(xtmp);

                            if (ytmp < yMin) yMin = ytmp;
                            else if (ytmp > yMax) yMax = ytmp;
                            temPoints.Add(new Point<double>(xtmp, ytmp));
                        }
                    }

                temPoints.Add(new Point<double>(xtw, ytw));
                
                if (ytw < yMin) yMin = ytw;
                else if (ytw > yMax) yMax = ytw;
            }

            // ASSERT xtw = xmax.

            return temPoints;
        }

    }

    // ----------------------------------------------------------------------------

    /// <summary>
    /// Методы расширения, используемые при работе с функциями.
    /// </summary>
    public static class GrapherExtensionMethods
    {
        /// <summary>
        /// Checks whether the double value is 'normal', i.e. not an infinity or NaN.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static bool isNormalNumber(this double a)
        {
            return (!double.IsNaN(a) && !double.IsInfinity(a));
        }

        /// <summary>
        /// Creates the FunctionGrapher for the calling IFunction object.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static FunctionGrapher CreateFunctionGrapher(this IFunction<double, double> func)
        {
            FunctionGrapher grapher = new FunctionGrapher(func);

            if (func is AnalyticFunction)
                grapher.Name = (func as AnalyticFunction).FunctionString;

            return grapher;

        }
    }
}
