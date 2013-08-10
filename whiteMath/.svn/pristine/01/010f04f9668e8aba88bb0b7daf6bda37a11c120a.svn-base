#if(OLD_VERSION)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace whiteMath.Graphers
{
    /// <summary>
    /// This class' purpose is to transform coordinates between
    /// image and functional coordinates.
    /// </summary>
    [Serializable]
    public class CoordinateTransformer
    {
        private double xMin, xMax, yMin, yMax;
        private int imgWidth, imgHeight;
        private int imgBorderIndent;
        private double kx, ky;

        #region PointCalculators

        public PointF CoordinateSystemZero
        {
            get
            {
                calculateKX();
                calculateKY();

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

                return new PointF(xType, yType);
            }
        }

        public PointF CoordinateSystemLeft
        {
            get
            {
                calculateKY();

                float xType = imgBorderIndent;

                float yType;

                if (yMin >= 0)
                    yType = imgHeight - imgBorderIndent;
                else if (yMax <= 0)
                    yType = imgBorderIndent;
                else
                    yType = (float)(imgBorderIndent + yMax * ky);

                return new PointF(xType, yType);
            }
        }

        public PointF CoordinateSystemRight
        {
            get
            {
                calculateKY();

                float xType = imgWidth - imgBorderIndent;

                float yType;

                if (yMin >= 0)
                    yType = imgHeight - imgBorderIndent;
                else if (yMax <= 0)
                    yType = imgBorderIndent;
                else
                    yType = (float)(imgBorderIndent + yMax * ky);

                return new PointF(xType, yType);
            }
        }

        public PointF CoordinateSystemUp
        {
            get
            {
                calculateKX();

                float xType;

                if (xMin >= 0)
                    xType = imgBorderIndent;
                else if (xMax <= 0)
                    xType = imgWidth - imgBorderIndent;
                else
                    xType = (float)(imgWidth - imgBorderIndent - xMax * kx);

                float yType = imgBorderIndent;

                return new PointF(xType, yType);
            }
        }

        public PointF CoordinateSystemDown
        {
            get
            {
                calculateKX();

                float xType;

                if (xMin >= 0)
                    xType = imgBorderIndent;
                else if (xMax <= 0)
                    xType = imgWidth - imgBorderIndent;
                else
                    xType = (float)(imgWidth - imgBorderIndent - xMax * kx);

                float yType = imgHeight - imgBorderIndent;

                return new PointF(xType, yType);
            }
        }

        #endregion

        // ------------------ public properties

        public double MinAxis1 { get { return xMin; } }
        public double MaxAxis1 { get { return xMax; } }
        public double MinAxis2 { get { return yMin; } }
        public double MaxAxis2 { get { return yMax; } }

        public double Axis1ScaleFactor { get { calculateKX(); return kx; } }
        public double Axis2ScaleFactor { get { calculateKY(); return ky; } }

        /// <summary>
        /// Returns the image size that current CoordinateTransformer is set to.
        /// </summary>
        /// <returns></returns>
        public Size getImageSize()
        {
            return new Size(imgWidth, imgHeight);
        }

        /// <summary>
        /// Sets the image size for current CoordinateTransformer.
        /// </summary>
        /// <param name="imgSize"></param>
        public void setImageSize(Size imgSize)
        {
            imgWidth = imgSize.Width; 
            imgHeight = imgSize.Height;
        }

        /// <summary>
        /// Returns the value of the image border indent for the current CoordinateTransformer
        /// </summary>
        public int getImageBorderIndent()
        {
            return imgBorderIndent;
        }
    
        /// <summary>
        /// Sets the value of image border indent for the current CoordinateTransformer
        /// </summary>
        /// <param name="imgBorderIndent"></param>
        public void setImageBorderIndent(int imgBorderIndent)
        {
            this.imgBorderIndent = imgBorderIndent;
        }

        // ------------------------------------

        private void calculateKX()
        {
            if (xMax - xMin == 0)
                throw new ArgumentException("The length of Axis1 graphing range is zero. The CoordinateTransformer possibly hasn't been initialized.");

            kx = (this.imgWidth - imgBorderIndent * 2) / (xMax - xMin);
            return;
        }

        private void calculateKY()
        {
            if (yMax - yMin == 0)
                throw new ArgumentException("The length of Axis1 graphing range is zero. The CoordinateTransformer possibly hasn't been initialized.");

            ky = (this.imgHeight - imgBorderIndent * 2) / (yMax - yMin);
            return;
        }

        // ------------------------------------

        public void setAxis1GraphingRange(double xMin, double xMax)
        {
            this.xMin = xMin;
            this.xMax = xMax;
        }

        public void setAxis2GraphingRange(double yMin, double yMax)
        {
            this.yMin = yMin;
            this.yMax = yMax;
        }

        public void setGraphingRange(double xMin, double xMax, double yMin, double yMax)
        {
            setAxis1GraphingRange(xMin, xMax);
            setAxis2GraphingRange(yMin, yMax);
        }

        // ------------------------------------

        public CoordinateTransformer() 
            {}

        public CoordinateTransformer(double xMin, double xMax, double yMin, double yMax, Size imgSize, int imgBorderIndent)
            : this(xMin, xMax, yMin, yMax, imgSize.Width, imgSize.Height, imgBorderIndent) { }

        public CoordinateTransformer(double xMin, double xMax, double yMin, double yMax, int imgWidth, int imgHeight, int imgBorderIndent)
        {
            this.xMin = xMin;
            this.xMax = xMax;
            this.yMin = yMin;
            this.yMax = yMax;
            this.imgWidth = imgWidth;
            this.imgHeight = imgHeight;
            this.imgBorderIndent = imgBorderIndent;
        }

        // ------------Transforming methods--------------------

        public void transformPixelToFunction(PointF pixel, out double x, out double y)
        {
            PointF zero = CoordinateSystemZero;     // calculates kx and ky inside

            x = transformImageXtoFunctionX(pixel.X);
            y = transformImageYToFunctionY(pixel.Y);
            
            return;
        }

        public double transformImageXtoFunctionX(float pixelX)
        {
            PointF zero = CoordinateSystemZero;
            return (pixelX + (xMax * kx * (xMax <= 0 ? 1 : 0)) + (xMin * kx * (xMin >= 0 ? 1 : 0)) - zero.X) / kx;
        }

        public double transformImageYToFunctionY(float pixelY)
        {
            PointF zero = CoordinateSystemZero;
            return -(pixelY - (yMax * ky * (yMax <= 0 ? 1 : 0)) - (yMin * ky * (yMin >= 0 ? 1 : 0)) - zero.Y) / ky;
        }

        /// <summary>
        /// Transforms the pair of function coordinates (x; y) to the image pixel coordinates.
        /// </summary>
        /// <param name="x">The X function coordinate</param>
        /// <param name="y">The Y function coordinate</param>
        /// <returns></returns>
        public PointF transformFunctionToPixel(double x, double y)
        {
            PointF zero = CoordinateSystemZero;     // calculates kx and ky inside

            return new PointF((float)transformFunctionXToPixelX(x), (float)transformFunctionYToPixelY(y));
        }

        /// <summary>
        /// Transforms the X coordinate from the functional coordinate system to the relative image coordinate system.
        /// </summary>
        /// <param name="x">The X coordinate from the functional coordinate system</param>
        /// <returns></returns>
        public double transformFunctionXToPixelX(double x)
        {
            return (CoordinateSystemZero.X + x * kx - (xMax * kx * (xMax <= 0 ? 1 : 0)) - xMin * kx * (xMin >= 0 ? 1 : 0));
        }

        /// <summary>
        /// Transforms the Y coordinate from the functional coordinate system to the relative image coordinate system.
        /// </summary>
        /// <param name="y">The Y coordinate from the functional coordinate system</param>
        /// <returns></returns>
        public double transformFunctionYToPixelY(double y)
        {
            return CoordinateSystemZero.Y - y * ky + yMax * ky * (yMax <= 0 ? 1 : 0) + yMin * ky * (yMin >= 0 ? 1 : 0);        
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