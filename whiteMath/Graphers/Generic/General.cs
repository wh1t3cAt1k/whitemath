using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics.Contracts;

namespace whiteMath.Graphers
{
    /// <summary>
    /// The enumeration is used to specify the line type for <c>GraphingArgs</c> class.
    /// </summary>
    /// <see cref="GraphingArgs"/>
    public enum LineType
    {
        /// <summary>
        /// With this <c>LineType</c>, the graph will be created 
        /// as a sequence of simple lines connecting consecutive pairs of points.
        /// </summary>
        Line,
        /// <summary>
        /// With this <c>LineType</c>, the graph will be created
        /// as a smooth cardinal spline going through the points.
        /// </summary>
        CardinalCurve,
        /// <summary>
        /// Whith this <c>LineType</c>, the graph will be created
        /// as a closed polygon.
        /// </summary>
        Polygon
    }

    /// <summary>
    /// This structure encapsulates the drawing parameters 
    /// for the <c>Graph()</c> method of <c>Graphers</c>.
    /// </summary>
    /// <see cref="IGrapher"/>
    /// <see cref="AbstractGrapher"/>
    /// <see cref="StandardGrapher"/>
    [ContractVerification(true)]
    [Serializable]
    public struct GraphingArgs: ICloneable
    {
        /// <summary>
        /// Gets or sets the indent from the boundaries of drawing area, in pixels.
        /// </summary>
        public int IndentFromBounds { get; set; }

        /// <summary>
        /// Gets or sets the pen which will be used to draw the graph curve. 
        /// If <c>null</c>, the graph curve won't be drawn.
        /// </summary>
        public Pen CurvePen { get; set; }

        /// <summary>
        /// Gets or sets the pen which will be used to draw the axes.
        /// If <c>null</c>, no axes will be drawn.
        /// </summary>
        public Pen CoordPen { get; set; }

        /// <summary>
        /// Gets or sets the font which will be used to draw the values
        /// on the axes.
        /// If <c>null</c>, no values will be drawn on the axes.
        /// </summary>
        public Font CoordFont { get; set; } 

        /// <summary>
        /// Gets or sets the pen which will be used to draw the
        /// coordinate grid.
        /// If <c>null</c>, no grid will be drawn.
        /// </summary>
        public Pen GridPen { get; set; }

        /// <summary>
        /// Gets or sets the brush which will be used to fill
        /// the background of the drawing area.
        /// If <c>null</c>, no background will be drawn.
        /// </summary>
        public Brush BackgroundBrush { get; set; }

        /// <summary>
        /// Gets or sets the curve type of the graph.
        /// </summary>
        public LineType CurveType { get; set; }

        /// <summary>
        /// Initializes the <c>GraphingArgs</c> structure
        /// with drawing parameters.
        /// </summary>
        /// <param name="indentFromBounds">
        /// The indent (in pixels) from the boundaries of drawing area.
        /// </param>
        /// <param name="backgroundBrush">
        /// The brush which will be used to fill
        /// the background of the drawing area.
        /// If <c>null</c>, no background will be drawn.
        /// </param>
        /// <param name="coordPen">
        /// The pen which will be used to draw the axes.
        /// If <c>null</c>, no axes will be drawn.
        /// </param>
        /// <param name="coordFont">
        /// The font which will be used to draw the values
        /// on the axes.
        /// If <c>null</c>, no values will be drawn on the axes.
        /// </param>
        /// <param name="gridPen">
        /// The pen which will be used to draw the
        /// coordinate grid.
        /// If <c>null</c>, no grid will be drawn.
        /// </param>
        /// <param name="curvePen">
        /// The pen which will be used to draw the graph curve. 
        /// If <c>null</c>, the graph curve won't be drawn.
        /// </param>
        /// <param name="curveType"></param>
        public GraphingArgs(
            int indentFromBounds, 
            Brush backgroundBrush, 
            Pen coordPen, 
            Font coordFont, 
            Pen gridPen, 
            Pen curvePen, 
            LineType curveType = LineType.Line): this()
        {
            Contract.Requires<ArgumentOutOfRangeException>(indentFromBounds >=0, "The indent from image bounds must be a non-negative value.");

            this.IndentFromBounds = indentFromBounds;
            
            this.CurvePen = curvePen;
            this.CoordPen = coordPen;
            this.GridPen = gridPen;
            this.BackgroundBrush = backgroundBrush;

            this.CoordFont = coordFont;
            this.CurveType = curveType;
        }

        [ContractInvariantMethod]
        private void __invariant()
        {
            Contract.Invariant(this.IndentFromBounds >= 0);
        }

        /// <summary>
        /// Creates an exact, independent copy of the GraphingArgs structure.
        /// </summary>
        /// <returns>An exact, independent copy of the GraphingArgs structure.</returns>
        public object Clone()
        {
            return new GraphingArgs(
                IndentFromBounds,
                (BackgroundBrush    == null ? null : BackgroundBrush.Clone() as Brush),
                (CoordPen           == null ? null : CoordPen.Clone() as Pen),
                (CoordFont          == null ? null : CoordFont.Clone() as Font),                
                (GridPen            == null ? null : GridPen.Clone() as Pen),
                (CurvePen           == null ? null : CurvePen.Clone() as Pen),
                CurveType
                );
        }
    }

    public enum HistoGrapherIncrementDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum HistoGrapherValueDrawingStyle
    {
        AutoFixedInterval,
        PointValues,
        Both
    }

    public enum HistoGrapherColumnValueTextPosition
    {
        TopOutside,
        TopInside,
        MiddleInside,
        BottomInside
    }

    /// <summary>
    /// This structure encapsulates the drawing parameters for
    /// graphing methods of <c>HistoGraphers</c>.
    /// </summary>
    /// <see cref="HistoGrapher"/>
    [ContractVerification(true)]
    [Serializable]
    public struct HistoGraphingArgs : ICloneable
    {
        /// <summary>
        /// Gets or sets the orientation of the histogram, that is,
        /// the increment direction of the columns.
        /// </summary>
        public HistoGrapherIncrementDirection IncrementDirection { get; set; }

        /// <summary>
        /// Gets or sets the dictionary that maps <c>HistoGrapher</c>'s point names
        /// to brushes which will be used to draw corresponding columns on the histogram.
        /// If <c>null</c>, no columns will be drawn.
        /// This dictionary must contain all point names of <c>HistoGrapher</c>.
        /// </summary>
        /// <see cref="HistoGrapher.PointNames"/>
        /// <see cref="HistoGrapher.CreateBrushes"/>
        public Dictionary<string, Brush> ColumnBrushes { get; set; }

        /// <summary>
        /// Gets or sets the dictionary that maps <c>HistoGrapher</c>'s point names
        /// to aliases which will instead be drawn
        /// on the histogram.
        /// If <c>null</c>, existing point names will be used.
        /// </summary>
        public Dictionary<string, string> PointAliases { get; set; }

        /// <summary>
        /// Gets or sets the pen which will be used to separate columns vertically from each other.
        /// If <c>null</c>, no separator will be drawn.
        /// </summary>
        public Pen ColumnSeparatorPen { get; set; }

        /// <summary>
        /// Gets or sets the pen which will be used to connect the uppermost edge of the column
        /// with the axis (where the text specifying the column's value is usually located).
        /// If <c>null</c>, no such lines will be drawn.
        /// </summary>
        public Pen ColumnValueLinePen { get; set; }

        /// <summary>
        /// Gets pr sets the pen which will be used to draw lines going out from
        /// axis' automatic values and to the opposite border of the histogram.
        /// </summary>
        public Pen AutoValueLinePen { get; set; }

        /// <summary>
        /// A font which will be used to draw values inside the columns.
        /// If <c>null</c>, no values will be drawn inside the column.
        /// </summary>
        public Font InsideColumnValueFont { get; set; }

        /// <summary>
        /// A value indicating the alignment of values inside the columns.
        /// (if these values are drawn at all).
        /// </summary>
        public HistoGrapherColumnValueTextPosition InsideColumnValuePosition { get; set; }

        /// <summary>
        /// A font which will be used to draw values on the axis.
        /// </summary>
        public Font AxisValueFont { get; set; }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
