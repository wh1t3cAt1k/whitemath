using System;
using System.Drawing;

using WhiteMath.Calculators;
using WhiteMath.Geometry;
using WhiteMath.General;

using whiteStructs.Conditions;

namespace WhiteMath.Graphers
{
    /// <summary>
    /// A generic class whose purpose is to transform between
    /// image coordinates and 2D function plane coordinates.
	/// <see cref="Numeric{T,C}"/>
	/// <see cref="ICalc{T}"/>
    /// <typeparam name="T">
    /// The numeric type which specifies the coordinates of the function.
    /// Should support fractional numbers and conversion to <c>double</c> type at least
    /// for small absolute numbers.
    /// </typeparam>
    /// <typeparam name="C">The calculator for the <typeparamref name="T"/> type.</typeparam>
    /// </summary>
    [Serializable]
    public class CoordinateTransformer<T,C> where C: ICalc<T>, new()
    {
        private Func<T, double> toDouble;

        /// <summary>
        /// Gets the <c>DimensionTransformer</c> which performs the
        /// transformation of X coordinates between the drawing rectangle
        /// and the function plane rectangle.
        /// </summary>
        public DimensionTransformer<T, C> TransformerAxisX { get; private set; }
        
        /// <summary>
        /// Gets the <c>DimensionTransformer</c> which performs the
        /// transformation of Y coordinates between the drawing rectangle
        /// and the function plane rectangle.
        /// </summary>
        public DimensionTransformer<T, C> TransformerAxisY { get; private set; }

        /// <summary>
        /// Gets the image drawing rectangle
        /// associated with the current <c>CoordinateTransformer</c>.
        /// </summary>
        public RectangleF DrawingArea
        {
            get
            {
                return new RectangleF(
                    new PointF((float)TransformerAxisX.MinImageAxis, (float)TransformerAxisY.MinImageAxis),
                    new SizeF(
                        (float)(TransformerAxisX.MaxImageAxis - TransformerAxisX.MinImageAxis),
                        (float)(TransformerAxisY.MaxImageAxis - TransformerAxisY.MinImageAxis)));
            }
        }

        private bool xMinIsMoreThanZero;
        private bool xMaxIsLessThanZero;
        private bool yMinIsMoreThanZero;
        private bool yMaxIsLessThanZero;

        private Numeric<T, C> xMinInPixels;
        private Numeric<T, C> xMaxInPixels;
        private Numeric<T, C> yMinInPixels;
        private Numeric<T, C> yMaxInPixels;
        
        public PointD CoordinateSystemCenter    { get; private set; }
        public PointD CoordinateSystemUp        { get; private set; }
        public PointD CoordinateSystemDown      { get; private set; }
        public PointD CoordinateSystemLeft      { get; private set; }
        public PointD CoordinateSystemRight     { get; private set; }

        /// <summary>
        /// Вычисляет положение главных точек координатной оси.
        /// </summary>
        private void calculateCoordinateSystemPoints()
        {
            float xType;
            float yType;

            RectangleF drawingArea = this.DrawingArea;

            // ---------------
            // Center point

            if (xMinIsMoreThanZero)         xType = drawingArea.Left;
            else if (xMaxIsLessThanZero)    xType = drawingArea.Right;
            else                            xType = (float)(drawingArea.Right - toDouble(xMaxInPixels));

            
            if (yMinIsMoreThanZero)         yType = drawingArea.Bottom;
            else if (yMaxIsLessThanZero)    yType = drawingArea.Top;
            else                            yType = (float)(drawingArea.Top + toDouble(yMaxInPixels));

            PointF center               = new PointF(xType, yType);
            this.CoordinateSystemCenter = center;

            // ---------------
            // Left point

            xType = drawingArea.Left;
            yType = center.Y;

            this.CoordinateSystemLeft = new PointF(xType, yType);

            // ---------------
            // Right point

            xType = drawingArea.Right;
            yType = center.Y;
            
            this.CoordinateSystemRight = new PointF(xType, yType);
            
            // ---------------
            // Up point

            xType = center.X;
            yType = drawingArea.Top;

            this.CoordinateSystemUp = new PointF(xType, yType);

            // ---------------
            // Down point

            xType = center.X;
            yType = drawingArea.Bottom;

            this.CoordinateSystemDown = new PointF(xType, yType);
        }

        // ------------------------------------

        public CoordinateTransformer(T xMin, T xMax, T yMin, T yMax, Func<T, double> toDoubleConversion, Size imgSize, int imgBorderIndent)
            : this(
            xMin, 
            xMax, 
            yMin, 
            yMax, 
            toDoubleConversion, 
            new RectangleF(imgBorderIndent, imgBorderIndent, imgSize.Width - 2 * imgBorderIndent, imgSize.Height - 2 * imgBorderIndent)) 
        {
        }

        public CoordinateTransformer(T xMin, T xMax, T yMin, T yMax, Func<T, double> toDouble, RectangleF drawingRectangle)
        {
			Condition.ValidateNotNull(xMin, nameof(xMin));
			Condition.ValidateNotNull(xMax, nameof(xMax));
			Condition.ValidateNotNull(yMin, nameof(yMin));
			Condition.ValidateNotNull(yMax, nameof(yMax));
			Condition.ValidateNotNull(toDouble, nameof(toDouble));

            xMinIsMoreThanZero = (xMin >= Numeric<T, C>.Zero);
            yMinIsMoreThanZero = (yMin >= Numeric<T, C>.Zero);

            xMaxIsLessThanZero = (xMax <= Numeric<T, C>.Zero);
            yMaxIsLessThanZero = (yMax <= Numeric<T, C>.Zero);

            this.toDouble = toDouble;
            this.TransformerAxisX = new DimensionTransformer<T, C>(drawingRectangle.Left, drawingRectangle.Right, xMin, xMax, toDouble);
            this.TransformerAxisY = new DimensionTransformer<T, C>(drawingRectangle.Top, drawingRectangle.Bottom, yMin, yMax, toDouble, true);

            xMinInPixels = (Numeric<T,C>)xMin / this.TransformerAxisX.ScaleFactor;
            xMaxInPixels = (Numeric<T,C>)xMax / this.TransformerAxisX.ScaleFactor;

            yMinInPixels = (Numeric<T,C>)yMin / this.TransformerAxisY.ScaleFactor;
            yMaxInPixels = (Numeric<T,C>)yMax / this.TransformerAxisY.ScaleFactor;

            calculateCoordinateSystemPoints();
        }

        // ------------Transforming methods--------------------

        public Point<T> transformPixelToFunction(PointD pixel)
        {
            return new Point<T>(transformImageXtoFunctionX(pixel.X), transformImageYToFunctionY(pixel.Y));
        }

        public T transformImageXtoFunctionX(double pixelX)
        {
            return TransformerAxisX.TransformImagePointToFunctionPoint(pixelX);
        }

        public T transformImageYToFunctionY(double pixelY)
        {
            return TransformerAxisY.TransformImagePointToFunctionPoint(pixelY);
        }

        /// <summary>
        /// Transforms the pair of function coordinates (x; y) to the image pixel coordinates.
        /// </summary>
        /// <param name="x">The X function coordinate</param>
        /// <param name="y">The Y function coordinate</param>
        /// <returns>The coordinates on the image.</returns>
        public PointD transformFunctionToPixel(T x, T y)
        {
            return new PointD(transformFunctionXToPixelX(x), transformFunctionYToPixelY(y));
        }

        /// <summary>
        /// Transforms the X coordinate from the functional coordinate system to the image coordinate system.
        /// </summary>
        /// <param name="x">The X coordinate from the functional coordinate system</param>
        /// <returns></returns>
        public double transformFunctionXToPixelX(T x)
        {
            return TransformerAxisX.TransformFunctionPointToImagePoint(x);
        }

        /// <summary>
        /// Transforms the Y coordinate from the functional coordinate system to the relative image coordinate system.
        /// </summary>
        /// <param name="y">The Y coordinate from the functional coordinate system</param>
        /// <returns></returns>
        public double transformFunctionYToPixelY(T y)
        {
            return TransformerAxisY.TransformFunctionPointToImagePoint(y);
        }

        // --------------------------------------------------
        // --------------------- helper API -----------------
        // --------------------------------------------------

        /// <summary>
        /// Method is used due to the existence of the milk area caused by 'indentFromImageBorder' parameter
        /// in the graphing argumens.
        /// 
        /// If the point 'position' is out of the graphing area (in the "milk")
        /// this method would return the boundary point.
        /// 
        /// Otherwise, the point returned is equivalent to the point passed.
        /// For example, the method is used by GraphicDrawer to determine if user has clicked an in-graph pixel, not the outer boundary of the image.
        /// </summary>
        /// <param name="position"></param>
        public Point getInBoundPoint(Point position)
        {
            int newX = position.X;
            int newY = position.Y;

            if (position.X < this.CoordinateSystemLeft.X)
                newX = (int)Math.Round(this.CoordinateSystemLeft.X);

            else if (position.X > this.CoordinateSystemRight.X)
                newX = (int)Math.Round(this.CoordinateSystemRight.X);

            if (position.Y < this.CoordinateSystemUp.Y)
                newY = (int)Math.Round(this.CoordinateSystemUp.Y);

            else if (position.Y > this.CoordinateSystemDown.Y)
                newY = (int)Math.Round(this.CoordinateSystemDown.Y);

            return new Point(newX, newY);
        }

        /// <summary>
        /// Checks whether the specified image point is inside the graphing range,
        /// not in the "milk" caused by 'indentFromImageBorder' parameter.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool pointInBound(Point point)
        {
            return
                point.X >= CoordinateSystemLeft.X &&
                point.X <= CoordinateSystemRight.X &&
                point.Y >= CoordinateSystemUp.Y &&
                point.Y <= CoordinateSystemDown.Y;
        }

        // ------------OBJECT METHODS OVERRIDING---------------

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            else if (obj is CoordinateTransformer<T,C>)
            {
                CoordinateTransformer<T,C> ct = obj as CoordinateTransformer<T,C>;

                return 
                    this.TransformerAxisX.Equals(ct.TransformerAxisX) &&
                    this.TransformerAxisY.Equals(ct.TransformerAxisY);
            }
            
            return false;
        }

        public override string ToString()
        {
            return String.Format("CoordinateTransformer:[{0}][{1}] ", TransformerAxisX, TransformerAxisY);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.TransformerAxisX.GetHashCode() + this.TransformerAxisY.GetHashCode();
            }
        }
    }
}
