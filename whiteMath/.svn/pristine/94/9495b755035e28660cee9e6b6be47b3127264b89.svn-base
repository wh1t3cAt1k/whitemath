using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace whiteMath.Graphers
{
    /// <summary>
    /// Support multiple graphs on a single surface.
    /// Because the grapher objects passed to the constructor can be a mix of bounded
    /// and unbounded graphers, the graph methods expects you to explicitly mention
    /// the graphing diap.
    /// 
    /// Once again: call ONLY the <code>Graph()</code> methods with ALL of the bounds
    /// written explicitly. Otherwise, it would throw NotSupportedException.
    /// 
    /// Although the grapher derives from the IBoundedGrapher interface,
    /// be careful as its properties <code>MinAxis1</code>, <code>MaxAxis1</code>,
    /// will return non-infinity values --ONLY-- if the graphers passed to the constructor 
    /// are bounded and provide explicit implementation of these methods respectively, 
    /// deriving from the <code>IGrapherBounded</code> interface.
    /// 
    /// Values of the MinAxis2, MaxAxis2 should always be checked for infinities
    /// because of the possibly infinite function range on the bounded argument diap.
    /// 
    /// For example, <example>imagine a FunctionGrapher containing f(x)=tg(x).</example>
    /// The argument range may be finite on (0; 2*pi), but, the function varies from -inf to +inf.
    /// 
    /// <see>FunctionGrapher</see>
    /// 
    /// Otherwise, if a single unbounded grapher comes around, these properties will return
    /// infinity values.
    /// 
    /// <see>IGrapherBounded</see>
    /// 
    /// Author: Pavel Kabir
    /// Version: 0.9
    /// Revised: 04.07.2010
    /// </summary>
    public class MultiGrapher: AbstractGrapher, IGrapherBounded
    {
        private Pen[] pens;
        private AbstractGrapher[] grapherArray = null;

        private double minAxis1, maxAxis1, minAxis2, maxAxis2;

        public MultiGrapher(params AbstractGrapher[] graphers)
        {
            if (graphers == null || graphers.Length == 0)
                throw new GrapherException("Null or zero-length graphers array passed to the constructor.");

            foreach (AbstractGrapher ag in graphers)
                if (ag is MultiGrapher)
                    throw new ArgumentException("MultiGraphers cannot encapsulate multigraphers. Consider calling 'getGrapherList()' and constructing another MultiGrapher.");

            this.grapherArray = graphers;
            findMinsAndMaxes();
        }

        // ---------------------------------------------------------

        public double MinAxis1 { get { return minAxis1; } }
        public double MaxAxis1 { get { return maxAxis1; } }
        public double MinAxis2 { get { return minAxis2; } }
        public double MaxAxis2 { get { return maxAxis2; } }

        /// <summary>
        /// Service method used in "findMinsAndMaxes".
        /// in case when one of the graphers is unbounded.
        /// </summary>
        private void setBoundsToInfinities()
        {
            minAxis1 = minAxis2 = double.NegativeInfinity;
            maxAxis1 = maxAxis2 = double.PositiveInfinity;
        }

        private void findMinsAndMaxes()
        {
            minAxis1 = minAxis2 = double.PositiveInfinity;
            maxAxis1 = maxAxis2 = double.NegativeInfinity;
        
            IGrapherBounded temp;

            foreach (AbstractGrapher obj in grapherArray)
            {
                if (obj is IGrapherBounded)
                {
                    temp = (IGrapherBounded)obj;
                    if(temp.MinAxis1<minAxis1) minAxis1 = temp.MinAxis1;
                    if(temp.MaxAxis1>maxAxis1) maxAxis1 = temp.MaxAxis1;
                    if(temp.MinAxis2<minAxis2) minAxis2 = temp.MinAxis2;
                    if(temp.MaxAxis2>maxAxis2) maxAxis2 = temp.MaxAxis2;
                }
                else { setBoundsToInfinities(); return; }
            }
        }

        /// <summary>
        /// Returns the grapher list previously passed to the constructor.
        /// </summary>
        public AbstractGrapher[] getGrapherList() { return grapherArray; }

        /// <summary>
        /// Method used to set different pens for different graphers previously passed to the
        /// constructor.
        /// 
        /// The number of parameters passed must strictly match the number of graphers 
        /// passed to the constructor. Otherwise, a <code>GrapherException</code> is thrown.
        /// </summary>
        /// <param name="pens">A list of pens matching the graphers list. </param>
        public void setPens(params Pen[] pens)
        {
            if (pens.Length != grapherArray.Length)
                throw new GrapherException("Pens array must be of the same length as the graphers array.");

            this.pens = pens;
        }

        public override void Graph(System.Drawing.Image dest, GraphingArgs ga, double xMin, double xMax, double yMin, double yMax)
        {
            if (pens != null)
                ga.CurvePen = pens[0];                
            
            copyGrapherSignature(this, grapherArray[0]);
            grapherArray[0].Graph(dest, ga, xMin, xMax, yMin, yMax);

            // Отключим рисование сетки, насечек и номеров

            GraphingArgs newArgs = (GraphingArgs)ga.Clone();

            newArgs.GridPen = null;
            newArgs.CoordFont = null;
            newArgs.CoordPen = null;
            newArgs.BackgroundBrush = null;

            for (int i = 1; i < grapherArray.Length; i++)
            {
                if (pens != null)
                    newArgs.CurvePen = pens[i];

                copyGrapherSignature(this, grapherArray[i]);
                grapherArray[i].Graph(dest, newArgs, xMin, xMax, yMin, yMax);
            }
        }
    }
}
