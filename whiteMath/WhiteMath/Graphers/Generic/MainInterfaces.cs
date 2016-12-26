using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WhiteMath.Graphers
{
    /// <summary>
    /// Interface written to support minimal grapher functionality.
    /// </summary>
    public interface IGrapher
    {
        /// <summary>
        /// Graph method supported by graphers with bounded graphing range.
        /// The method signature assumes that the grapher knows on which range to graph.
        /// 
        /// CONTRACT:
        /// 
        /// Recommended to provide a working implementation.
        /// 
        /// If the grapher is unbounded(i.e. cannot graph without explicitly
        /// specifying the range), then the method should throw
        /// a NotSupportedException.
        /// </summary>
        /// <param name="dest">Image object on which the graph should be painted.</param>
        /// <param name="ga">GraphingArgs object incapsulating the graphing parameters.</param>
        void Graph(Image dest, GraphingArgs ga);

        /// <summary>
        /// Graph method supported by graphers with either bounded or unbounded range.
        /// 
        /// The method signature assumes that the implementation class knows how to calculate
        /// the function bounds (yMin and yMax) knowing the argument bounds.
        /// 
        /// CONTRACT:
        /// 
        /// A working implementation SHOULD be provided for any grapher.
        /// However, the method is allowed throw a NotSupportedException, if:
        /// 
        ///     a) The implementation class allows the function go to infinity on the argument range, 
        ///     so the graphing should only be made on a fully specified ranges,
        ///     both X and Y interval.
        ///     
        /// </summary>
        /// <param name="dest">Image object on which the graph should be painted.</param>
        /// <param name="ga">GraphingArgs object incapsulating the graphing parameters.</param>
        /// <param name="xMin">Lower inclusive bound of the argument graphing range.</param>
        /// <param name="xMax">Higher inclusive bound of the argument graphing range.</param>
        void Graph(Image dest, GraphingArgs ga, double xMin, double xMax);

        /// <summary> 
        /// Graph method supported by any graphers with either bounded or unbounded range.
        /// 
        /// CONTRACT: the implementation MUST be provided for every grapher.
        /// </summary>
        /// <param name="dest">Image object on which the graph should be painted.</param>
        /// <param name="ga">GraphingArgs object incapsulating the graphing parameters.</param>
        /// <param name="xMin">Lower inclusive bound of the argument graphing range.</param>
        /// <param name="xMax">Higher inclusive bound of the argument graphing range.</param>
        /// <param name="yMin">Lower inclusive bound of the function graphing range.</param>
        /// <param name="yMax">Higher inclusive bound of the agrument graphing range.</param>
        void Graph(Image dest, GraphingArgs ga, double xMin, double xMax, double yMin, double yMax);
    }

    /// <summary>
    /// A grapher interface type that allows user to know about the minimums and maximums 
    /// of X and Y axis respectively.
    ///
    /// Grapher implementing this interface is expected to be bounded
    /// (by argument or by both argument and function values)
    /// 
    /// CONTRACT: at least Axis1 properties should return non-infinity values
    /// (because otherwise the bounded grapher makes no sense), 
    /// 
    /// except for the MultiGrapher, which can be fully unbounded.
    /// MultiGrapher implements this interface for the sake
    /// of knowing the argument and function extreme values - in case of containing only 
    /// bounded graphers inside.
    /// 
    /// <see>MultiGrapher</see>
    /// </summary>
    public interface IGrapherBounded: IGrapher
    {
        /// <summary>
        /// Returns the minimum value of the grapher argument.
        /// </summary>
        double MinAxis1 { get; }

        /// <summary>
        /// Returns the maximum value of the grapher argument.
        /// </summary>
        double MaxAxis1 { get; }

        /// <summary>
        /// Returns the minimum value of the grapher function.
        /// </summary>
        double MinAxis2 { get; }

        /// <summary>
        /// Returns the maximum value of the grapher function.
        /// </summary>
        double MaxAxis2 { get; }
    }
}
