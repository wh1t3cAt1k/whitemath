using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using WhiteMath.Graphers;

namespace WhiteMath.Graphers
{
    /// <summary>
    /// Represents a bounded or unbounded grapher.
    /// 
    /// Provides a standard interface for all of the graphers.
    /// By default contains no points array, that is, unbounded,
    /// but can be extended to be bounded.
    /// 
    /// For unbounded subclasses, the overriding of methods Graph without
    /// mentioning the argument range is recommended to throw a 
    /// <code>NotSupportedException</code>.
    /// 
    /// !!! It is recommended that you extend from this class, not from the
    /// !!! IGrapher interface.
    /// </summary>
    [Serializable]
    public abstract class AbstractGrapher : IGrapher
    {
        protected string GrapherName = "Grapher"; // название объекта

        public string Name { get { return GrapherName; } set { GrapherName = value; } }

        protected string xName = "X"; // название оси X;
        protected string yName = "Y"; // название оси Y;

        public string Axis1Name { get { return xName; } set { xName = value; } }
        public string Axis2Name { get { return yName; } set { yName = value; } }

        protected string formatter1 = "{0}"; // форматтер чисел на насечках осей
        protected string formatter2 = "{0}";

        public string Axis1NumbersFormatter { get { return formatter1; } set { formatter1 = value; } }
        public string Axis2NumbersFormatter { get { return formatter2; } set { formatter2 = value; } }

        protected double Step1 = 0;
        protected double Step2 = 0;

        public double Axis1CoordinateStep // с каким шагом отмечаются насечки на координатной оси
        {
            get { return Step1; }
            set { if (value > 0) Step1 = value; else throw new GrapherSettingsException("Невозможно назначить нулевой или отрицательный шаг!"); }
        }
        public double Axis2CoordinateStep
        {
            get { return Step2; }
            set { if (value > 0) Step2 = value; else throw new GrapherSettingsException("Невозможно назначить нулевой или отрицательный шаг!"); }
        }

        // ------------------------------------------------------------
        // ------------ all these methods are not supported -----------
        // ------------------------------------------------------------

        private static NotSupportedException ex = new NotSupportedException("The operation is not supported by such kind of grapher.");

        public virtual void Graph(Image dest, GraphingArgs ga) { throw ex; }
        public virtual void Graph(Image dest, GraphingArgs ga, double xMin, double xMax) { throw ex; }
        public virtual void Graph(Image dest, GraphingArgs ga, double xMin, double xMax, double yMin, double yMax) { throw ex; }

        // ------- Service methods for subclasses
        
        /// <summary>
        /// Copies the following characteristics from one grapher to another:
        ///     1. Axis names.
        ///     2. Axis "X" and "Y" coordinate steps.
        ///     3. Axis numbers formatters.
        ///     
        ///     It DOES NOT copy the grapher name, points array etc. 
        ///     Just the signature of the grapher.
        ///     
        ///     It is used in cases when you subclass the AbstractGrapher
        ///     and want to delegate the grapher functionality to the
        ///     inner object using composition.
        ///     (<see>FunctionGrapher</see>)
        /// </summary>
        /// <param name="from">Grapher from which the copying is produced.</param>
        /// <param name="to">Grapher which is to receive the signature.</param>
        protected static void copyGrapherSignature(AbstractGrapher from, AbstractGrapher to)
        {
            to.formatter1 = from.formatter1;
            to.formatter2 = from.formatter2;
            to.GrapherName = from.GrapherName;
            to.Step1 = from.Step1;
            to.Step2 = from.Step2;
            to.xName = from.xName;
            to.yName = from.yName;
        }

        // -------------------------- 

        /// <summary>
        /// Returns the value of recommended indent from image borders 
        /// depending on the font and formatters used to draw axis names and numbers on the graph.
        /// 
        /// The value returned is measured using the screen graphics resolution and axis names length.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public int getRecommendedIndentFromBorder(Graphics G, Font coordinateFont, string Axis1NumbersFormatter, string Axis2NumbersFormatter)
        {
            List<float> possibleShifts = new List<float>();

            possibleShifts.Add(G.MeasureString(this.Axis1Name, coordinateFont).Width);
            possibleShifts.Add(G.MeasureString(this.Axis2Name, coordinateFont).Height);
            possibleShifts.Add(G.MeasureString(string.Format(Axis1NumbersFormatter, double.MinValue), coordinateFont).Width);
            possibleShifts.Add(G.MeasureString(string.Format(Axis1NumbersFormatter, double.MaxValue), coordinateFont).Width);
            possibleShifts.Add(G.MeasureString(string.Format(Axis1NumbersFormatter, 0.0123456789), coordinateFont).Width);
            possibleShifts.Add(G.MeasureString(string.Format(Axis2NumbersFormatter, double.MinValue), coordinateFont).Height);
            possibleShifts.Add(G.MeasureString(string.Format(Axis2NumbersFormatter, double.MaxValue), coordinateFont).Height);
            possibleShifts.Add(G.MeasureString(string.Format(Axis2NumbersFormatter, 0.0123456789), coordinateFont).Height);

            return (int)(possibleShifts.Max() + 0.5);
        }

        /// <summary>
        /// Overloaded version of the method that uses encapsulated Grapher formatters.
        /// </summary>
        /// <param name="G"></param>
        /// <param name="coordinateFont"></param>
        /// <returns></returns>
        public int getRecommendedIndentFromBorder(Graphics G, Font coordinateFont)
        {
            return getRecommendedIndentFromBorder(G, coordinateFont, this.Axis1NumbersFormatter, this.Axis2NumbersFormatter);
        }
    }

 }
