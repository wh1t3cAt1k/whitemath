#if(OLD_VERSION)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using whiteMath.Geometry;

namespace whiteMath.Graphers
{
    /// <summary>
    /// This class' purpose is to transform coordinates between
    /// image and functional coordinates.
    /// </summary>
    [Serializable]
    public class CoordinateTransformer
    {
        private double kx, ky;
        private double xMin, xMax, yMin, yMax;
        
        private int imgWidth, imgHeight;
        private int imgBorderIndent;

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
            // ---------------
            // Center point

            float xType;

            if (xMin >= 0)
                xType = imgBorderIndent;
            else if (xMax <= 0)
                xType = imgWidth - imgBorderIndent;
            else
                xType = (float)(imgWidth - imgBorderIndent - xMax * kx);

            float yType;

            if (yMin >= 0)
                yType = imgHeight - imgBorderIndent;
            else if (yMax <= 0)
                yType = imgBorderIndent;
            else
                yType = (float)(imgBorderIndent + yMax * ky);

            this.CoordinateSystemCenter = new PointF(xType, yType);

            // ---------------
            // Left point

            xType = imgBorderIndent;

            if (yMin >= 0)
                yType = imgHeight - imgBorderIndent;
            else if (yMax <= 0)
                yType = imgBorderIndent;
            else
                yType = (float)(imgBorderIndent + yMax * ky);

            this.CoordinateSystemLeft = new PointF(xType, yType);

            // ---------------
            // Right point

            xType = imgWidth - imgBorderIndent;

            if (yMin >= 0)
                yType = imgHeight - imgBorderIndent;
            else if (yMax <= 0)
                yType = imgBorderIndent;
            else
                yType = (float)(imgBorderIndent + yMax * ky);

            this.CoordinateSystemRight = new PointF(xType, yType);
            
            // ---------------
            // Up point

            if (xMin >= 0)
                xType = imgBorderIndent;
            else if (xMax <= 0)
                xType = imgWidth - imgBorderIndent;
            else
                xType = (float)(imgWidth - imgBorderIndent - xMax * kx);

            yType = imgBorderIndent;

            this.CoordinateSystemUp = new PointF(xType, yType);

            // ---------------
            // Down point

            if (xMin >= 0)
                xType = imgBorderIndent;
            else if (xMax <= 0)
                xType = imgWidth - imgBorderIndent;
            else
                xType = (float)(imgWidth - imgBorderIndent - xMax * kx);

            yType = imgHeight - imgBorderIndent;

            this.CoordinateSystemDown = new PointF(xType, yType);
        }

        // ------------------ public properties

        public double MinAxis1 { get { return xMin; } }

        public double MaxAxis1 { get { return xMax; } }
        
        public double MinAxis2 { get { return yMin; } }
        
        public double MaxAxis2 { get { return yMax; } }

        public double Axis1ScaleFactor { get { return kx; } }
        
        public double Axis2ScaleFactor { get { return ky; } }

        /// <summary>
        /// Returns the image size that current CoordinateTransformer is set to.
        /// </summary>
        /// <returns></returns>
        public Size ImageSize { get { return new Size(imgWidth, imgHeight); } }

        /// <summary>
        /// Returns the value of the image border indent for the current CoordinateTransformer
        /// </summary>
        public int ImageBorderIndent
        {
            get { return imgBorderIndent; }
        }
    
        // ------------------------------------

        public CoordinateTransformer(double xMin, double xMax, double yMin, double yMax, Size imgSize, int imgBorderIndent)
            : this(xMin, xMax, yMin, yMax, imgSize.Width, imgSize.Height, imgBorderIndent) { }

        public CoordinateTransformer(double xMin, double xMax, double yMin, double yMax, int imageWidth, int imageHeight, int imageBorderIndent)
        {
            this.xMin = xMin;
            this.xMax = xMax;
            this.yMin = yMin;
            this.yMax = yMax;
            
            this.imgWidth = imageWidth;
            this.imgHeight = imageHeight;
            this.imgBorderIndent = imageBorderIndent;

            kx = (this.imgWidth - imageBorderIndent * 2) / (xMax - xMin);
            ky = (this.imgHeight - imageBorderIndent * 2) / (yMax - yMin);

            calculateCoordinateSystemPoints();
        }

        // ------------Transforming methods--------------------

        public PointD transformPixelToFunction(PointD pixel)
        {
            return new PointD(transformImageXtoFunctionX(pixel.X), transformImageYToFunctionY(pixel.Y));
        }

        public double transformImageXtoFunctionX(double pixelX)
        {
            PointD zero = CoordinateSystemCenter;
            return (pixelX + (xMax * kx * (xMax <= 0 ? 1 : 0)) + (xMin * kx * (xMin >= 0 ? 1 : 0)) - zero.X) / kx;
        }

        public double transformImageYToFunctionY(double pixelY)
        {
            PointD zero = CoordinateSystemCenter;
            return -(pixelY - (yMax * ky * (yMax <= 0 ? 1 : 0)) - (yMin * ky * (yMin >= 0 ? 1 : 0)) - zero.Y) / ky;
        }

        /// <summary>
        /// Transforms the pair of function coordinates (x; y) to the image pixel coordinates.
        /// </summary>
        /// <param name="x">The X function coordinate</param>
        /// <param name="y">The Y function coordinate</param>
        /// <returns></returns>
        public PointD transformFunctionToPixel(double x, double y)
        {
            PointD zero = CoordinateSystemCenter;     // calculates kx and ky inside
            return new PointD(transformFunctionXToPixelX(x), transformFunctionYToPixelY(y));
        }

        /// <summary>
        /// Transforms the X coordinate from the functional coordinate system to the relative image coordinate system.
        /// </summary>
        /// <param name="x">The X coordinate from the functional coordinate system</param>
        /// <returns></returns>
        public double transformFunctionXToPixelX(double x)
        {
            return (CoordinateSystemCenter.X + x * kx - (xMax * kx * (xMax <= 0 ? 1 : 0)) - xMin * kx * (xMin >= 0 ? 1 : 0));
        }

        /// <summary>
        /// Transforms the Y coordinate from the functional coordinate system to the relative image coordinate system.
        /// </summary>
        /// <param name="y">The Y coordinate from the functional coordinate system</param>
        /// <returns></returns>
        public double transformFunctionYToPixelY(double y)
        {
            return CoordinateSystemCenter.Y - y * ky + yMax * ky * (yMax <= 0 ? 1 : 0) + yMin * ky * (yMin >= 0 ? 1 : 0);        
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
        public double transformPixelXRangeToFunctionXRange(int range)
        {
            return range / kx;
        }

        /// <summary>
        /// Transforms the pixel Y range to the equivalent functional Y range.
        /// For example, 10 pixels may be equal to 100 units on the Y axis.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public double transformPixelYRangeToFunctionYRange(int range)
        {
            return range / ky;
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
            if (this.GetType().IsInstanceOfType(obj))
            {
                CoordinateTransformer ct = obj as CoordinateTransformer;
                return ct.imgHeight == this.imgHeight &&
                       ct.imgWidth == this.imgWidth &&
                       ct.xMin == this.xMin &&
                       ct.xMax == this.xMax &&
                       ct.yMin == this.yMin &&
                       ct.yMax == this.yMax;
            }
            else return false;
        }

        public override string ToString()
        {
            return String.Format("Coordinate transformer: " +
                "xRange = {0}-{1} yRange = {2}-{3} " +
                "imgWidth = {4} imgHeight = {5}", xMin, xMax, yMin, yMax, imgWidth, imgHeight);
        }

        public override int GetHashCode()
        {
            return (int)xMax % imgWidth + (int)xMin % imgWidth +
                (int)yMin % imgHeight + (int)yMax % imgHeight;
        }
    }
}

#endif