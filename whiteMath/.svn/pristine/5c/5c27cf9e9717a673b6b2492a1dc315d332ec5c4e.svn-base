#if (OLD_VERSION)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using whiteMath.Geometry;

using System.Diagnostics.Contracts;
using whiteMath.General;

namespace whiteMath.Graphers
{
    /// <summary>
    /// A generic class whose purpose is to transform between
    /// image coordinates and 2D function plane coordinates.
    /// <see cref="Numeric&lt;T,C&gt;"/>
    /// <see cref="ICalc&lt;T&gt;"/>
    /// <typeparam name="T">
    /// The numeric type which specifies the coordinates of the function.
    /// Should support fractional numbers and conversion to <c>double</c> type at least
    /// for small absolute numbers.
    /// </typeparam>
    /// <typeparam name="C">The calculator for the <typeparamref name="T"/> type.</typeparam>
    /// </summary>
    [Serializable]
    [ContractVerification(true)]
    public class CoordinateTransformer<T,C> where C: ICalc<T>, new()
    {
        private Numeric<T, C> kx, ky;                       // сколько пикселей стоит единичный интервал икса и игрека соответственно
        private Numeric<T, C> xMin, xMax, yMin, yMax;       // интервал функции

        private Func<T, double> toDouble;                   // функция, переводящая тип T в double

        private int imgWidth, imgHeight;
        private int imgBorderIndent;

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

            // ---------------
            // Center point

            if (xMinIsMoreThanZero)         xType = imgBorderIndent;
            else if (xMaxIsLessThanZero)    xType = imgWidth - imgBorderIndent;
            else                            xType = (float)(imgWidth - imgBorderIndent - toDouble(xMaxInPixels));

            
            if (yMinIsMoreThanZero)         yType = imgHeight - imgBorderIndent;
            else if (yMaxIsLessThanZero)    yType = imgBorderIndent;
            else                            yType = (float)(imgBorderIndent + toDouble(yMaxInPixels));

            this.CoordinateSystemCenter = new PointF(xType, yType);

            // ---------------
            // Left point

            xType = imgBorderIndent;

            if (yMinIsMoreThanZero)         yType = imgHeight - imgBorderIndent;
            else if (yMaxIsLessThanZero)    yType = imgBorderIndent;
            else                            yType = (float)(imgBorderIndent + toDouble(yMaxInPixels));

            this.CoordinateSystemLeft = new PointF(xType, yType);

            // ---------------
            // Right point

            xType = imgWidth - imgBorderIndent;

            if (yMinIsMoreThanZero)         yType = imgHeight - imgBorderIndent;
            else if (yMaxIsLessThanZero)    yType = imgBorderIndent;
            else                            yType = (float)(imgBorderIndent + toDouble(yMaxInPixels));

            this.CoordinateSystemRight = new PointF(xType, yType);
            
            // ---------------
            // Up point

            if (xMinIsMoreThanZero)         xType = imgBorderIndent;
            else if (xMaxIsLessThanZero)    xType = imgWidth - imgBorderIndent;
            else                            xType = (float)(imgWidth - imgBorderIndent - toDouble(xMaxInPixels));

            yType = imgBorderIndent;

            this.CoordinateSystemUp = new PointF(xType, yType);

            // ---------------
            // Down point

            if (xMinIsMoreThanZero)         xType = imgBorderIndent;
            else if (xMaxIsLessThanZero)    xType = imgWidth - imgBorderIndent;
            else                            xType = (float)(imgWidth - imgBorderIndent - toDouble(xMaxInPixels));

            yType = imgHeight - imgBorderIndent;

            this.CoordinateSystemDown = new PointF(xType, yType);
        }

        // ------------------ public properties

        public T MinAxis1 { get { return xMin; } }

        public T MaxAxis1 { get { return xMax; } }

        public T MinAxis2 { get { return yMin; } }

        public T MaxAxis2 { get { return yMax; } }

        public T Axis1ScaleFactor { get { return kx; } }

        public T Axis2ScaleFactor { get { return ky; } }

        /// <summary>
        /// Returns the image size that current CoordinateTransformer is set to.
        /// </summary>
        /// <returns>The image size that current CoordinateTransformer is set to.</returns>
        public Size ImageSize { get { return new Size(imgWidth, imgHeight); } }

        /// <summary>
        /// Returns the value of the image border indent for the current CoordinateTransformer
        /// </summary>
        public int ImageBorderIndent
        {
            get { return imgBorderIndent; }
        }
    
        // ------------------------------------

        public CoordinateTransformer(T xMin, T xMax, T yMin, T yMax, Func<T, double> toDoubleConversion, Size imgSize, int imgBorderIndent)
            : this(xMin, xMax, yMin, yMax, toDoubleConversion, imgSize.Width, imgSize.Height, imgBorderIndent) 
        {
            Contract.Assume(xMin != null);
            Contract.Assume(xMax != null);
            Contract.Assume(yMin != null);
            Contract.Assume(yMax != null);
            Contract.Assume(toDoubleConversion != null);
        }

        public CoordinateTransformer(T xMin, T xMax, T yMin, T yMax, Func<T, double> toDoubleConversion, int imageWidth, int imageHeight, int imageBorderIndent)
        {
            Contract.Requires<ArgumentNullException>(xMin != null, "xMin");
            Contract.Requires<ArgumentNullException>(xMax != null, "xMax");
            Contract.Requires<ArgumentNullException>(yMin != null, "yMin");
            Contract.Requires<ArgumentNullException>(yMax != null, "yMax");
            Contract.Requires<ArgumentNullException>(toDoubleConversion != null, "toDoubleConversion");

            Contract.Requires<ArgumentOutOfRangeException>(imageWidth > 0);
            Contract.Requires<ArgumentOutOfRangeException>(imageHeight > 0);
            Contract.Requires<ArgumentOutOfRangeException>(imageBorderIndent >= 0);

            this.xMin = xMin;
            this.xMax = xMax;
            this.yMin = yMin;
            this.yMax = yMax;

            xMinIsMoreThanZero = (xMin >= Numeric<T, C>.Zero);
            yMinIsMoreThanZero = (yMin >= Numeric<T, C>.Zero);

            xMaxIsLessThanZero = (xMax <= Numeric<T, C>.Zero);
            yMaxIsLessThanZero = (yMax <= Numeric<T, C>.Zero);

            this.imgWidth = imageWidth;
            this.imgHeight = imageHeight;
            this.imgBorderIndent = imageBorderIndent;

            this.toDouble = toDoubleConversion;

            kx = (Numeric<T,C>)(this.imgWidth - imageBorderIndent * 2) / ((Numeric<T,C>)xMax - xMin);
            ky = (Numeric<T,C>)(this.imgHeight - imageBorderIndent * 2) / ((Numeric<T,C>)yMax - yMin);

            xMinInPixels = xMin * kx;
            xMaxInPixels = xMax * kx;

            yMinInPixels = yMin * ky;
            yMaxInPixels = yMax * ky;

            calculateCoordinateSystemPoints();
        }

        [ContractInvariantMethod]
        private void ContractInvariant()
        {
            Contract.Invariant(this.toDouble != null);
        }

        // ------------Transforming methods--------------------

        public Point<T> transformPixelToFunction(PointD pixel)
        {
            return new Point<T>(transformImageXtoFunctionX(pixel.X), transformImageYToFunctionY(pixel.Y));
        }

        public T transformImageXtoFunctionX(double pixelX)
        {
            Numeric<T, C> result = (Numeric<T,C>)(pixelX - CoordinateSystemCenter.X);

            if (xMaxIsLessThanZero)
                result += xMaxInPixels;

            else if (xMinIsMoreThanZero)
                result += xMinInPixels;

            return result / kx;
        }

        public T transformImageYToFunctionY(double pixelY)
        {
            Numeric<T, C> result = (Numeric<T, C>)(pixelY - CoordinateSystemCenter.Y);

            if (yMaxIsLessThanZero)
                result -= yMaxInPixels;

            else if (yMinIsMoreThanZero)
                result -= yMinInPixels;

            return -(result / ky);
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
        /// Transforms the X coordinate from the functional coordinate system to the relative image coordinate system.
        /// </summary>
        /// <param name="x">The X coordinate from the functional coordinate system</param>
        /// <returns></returns>
        public double transformFunctionXToPixelX(T x)
        {
            Numeric<T, C> result = (Numeric<T, C>)CoordinateSystemCenter.X + x * kx;

            if (xMaxIsLessThanZero)
                result -= xMaxInPixels;

            else if (xMinIsMoreThanZero)
                result -= xMinInPixels;

            return toDouble(result);
        }

        /// <summary>
        /// Transforms the Y coordinate from the functional coordinate system to the relative image coordinate system.
        /// </summary>
        /// <param name="y">The Y coordinate from the functional coordinate system</param>
        /// <returns></returns>
        public double transformFunctionYToPixelY(T y)
        {
            Numeric<T, C> result = (Numeric<T, C>)CoordinateSystemCenter.Y - y * ky;

            if (yMaxIsLessThanZero)
                result += yMaxInPixels;

            else if (yMinIsMoreThanZero)
                result += yMinInPixels;

            return toDouble(result);        
        }

        // --------------------------------------------------
        // --------------------- Range transform ------------
        // --------------------------------------------------

        /// <summary>
        /// Transforms the pixel X range to the equivalent functional X range.
        /// For example, 10 pixels may be equal to 50 units on the X axis.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public T transformPixelXRangeToFunctionXRange(double range)
        {
            return (Numeric<T,C>)range / kx;
        }

        /// <summary>
        /// Transforms the pixel Y range to the equivalent functional Y range.
        /// For example, 10 pixels may be equal to 100 units on the Y axis.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public T transformPixelYRangeToFunctionYRange(double range)
        {
            return (Numeric<T,C>)range / ky;
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

                return ct.imgHeight == this.imgHeight   &&
                       ct.imgWidth == this.imgWidth     &&
                       ct.xMin == this.xMin             &&
                       ct.xMax == this.xMax             &&
                       ct.yMin == this.yMin             &&
                       ct.yMax == this.yMax;
            }
            
            return false;
        }

        public override string ToString()
        {
            return String.Format("Coordinate transformer: " +
                "xRange = {0}-{1} yRange = {2}-{3} " +
                "imgWidth = {4} imgHeight = {5}", xMin, xMax, yMin, yMax, imgWidth, imgHeight);
        }

        public override int GetHashCode()
        {
            return 
                imgHeight.GetHashCode() + imgWidth.GetHashCode() + 
                xMin.GetHashCode() + xMax.GetHashCode() + 
                yMin.GetHashCode() + yMax.GetHashCode();
        }
    }
}

#endif