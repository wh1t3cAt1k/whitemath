using System;

using WhiteMath.Calculators;

using whiteStructs.Conditions;

namespace WhiteMath.Graphers
{
    /// <summary>
    /// A generic class dedicated to perform bidirectional mapping
    /// between an image axis interval and a function plane axis interval.
	/// <see cref="Numeric{T,C}"/>
	/// <see cref="ICalc{T}"/>
    /// <typeparam name="T">
    /// The numeric type which specifies the coordinate of the function.
    /// Should support fractional numbers and conversion to <c>double</c> type at least
    /// for small absolute numbers.
    /// </typeparam>
    /// <typeparam name="C">The calculator for the <typeparamref name="T"/> type.</typeparam>
    /// </summary>
    [Serializable]
    public class DimensionTransformer<T,C> where C: ICalc<T>, new()
    {
		private Numeric<T, C> _distancePerPixel;
		private Numeric<T, C> _functionMin, _functionMax;

        private double imageMin, imageMax;   
        
		private Func<T, double> _toDouble;

        // ------------------ public properties

        /// <summary>
        /// Gets the flag indicating if lower values on the
        /// image axis scale correspond to higher values
        /// on the function plane axis scale.
        /// </summary>
        public bool ReverseImageAxis { get; private set; }

        /// <summary>
        /// Returns the leftmost boundary of function axis
        /// transformation interval.
        /// </summary>
        public T MinFunctionAxis { get { return _functionMin; } }
        
        /// <summary>
        /// Returns the rightmost boundary of function axis
        /// transformation interval.
        /// </summary>
        public T MaxFunctionAxis { get { return _functionMax; } }

        /// <summary>
        /// Returns the leftmost boundary of image axis
        /// transformation interval.
        /// </summary>
        public double MinImageAxis { get { return imageMin; } }

        /// <summary>
        /// Returns the rightmost boundary of image axis
        /// transformation interval.
        /// </summary>
        public double MaxImageAxis { get { return imageMax; } }

        /// <summary>
        /// Returns the scale factor specifying the length of
        /// an interval on the function axis which is equivalent to one-pixel
        /// interval range on the image axis.
        /// </summary>
        public T ScaleFactor { get { return _distancePerPixel; } }

        // ------------------------------------

        /// <summary>
        /// Initializes the <c>DimensionTransformer</c> with
        /// an image axis interval and a function plane axis interval.
        /// </summary>
        /// <param name="imageAxisMin">The leftmost boundary of image axis transformation interval.</param>
        /// <param name="imageAxisMax">The rightmost boundary of image axis transformation interval.</param>
        /// <param name="functionAxisMin">The leftmost boundary of function plane axis transformation interval.</param>
        /// <param name="functionAxisMax">The rightmost boundary of function plane axis transformation interval.</param>
        /// <param name="toDouble">A function which provides conversion of function plane axis coordinates to <c>double</c> numbers - at least for small absolute values.</param>
        /// <param name="reverseImageAxis">A flag indicating if lower values on image axis scale should correspond to higher values on function plane axis scale.</param>
        public DimensionTransformer(double imageAxisMin, double imageAxisMax, T functionAxisMin, T functionAxisMax, Func<T, double> toDouble, bool reverseImageAxis = false)
        {
			Condition.ValidateNotNull(toDouble, nameof(toDouble));
			Condition
				.Validate(imageAxisMin < imageAxisMax)
				.OrArgumentOutOfRangeException("Invalid image axis interval, the specified minimal value exceeds the specified maximal value.");
			Condition
				.Validate((Numeric<T,C>)functionAxisMin <= functionAxisMax)
				.OrArgumentOutOfRangeException("Invalid function axis interval, the specified minimal value exceeds the specified maximal value.");

            this.imageMin = imageAxisMin;
            this.imageMax = imageAxisMax;

            this._functionMin = functionAxisMin;
            this._functionMax = functionAxisMax;

            this._toDouble = toDouble;
            this.ReverseImageAxis = reverseImageAxis;

            this._distancePerPixel = (_functionMax - _functionMin) / (Numeric<T, C>)(imageMax - imageMin);
        }

        /// <summary>
        /// Transforms the coordinate on the image axis
        /// to its  coordinate on the function plane axis.
        /// </summary>
        /// <param name="pixelCoordinate">The coordinate of a point on the image axis.</param>
        /// <returns>The coordinate of the point on the function plane axis.</returns>
        public T TransformImagePointToFunctionPoint(double pixelCoordinate)
        {
			if (ReverseImageAxis)
			{
				return _functionMax - (Numeric<T, C>)(pixelCoordinate - imageMin) * _distancePerPixel;
			}

            return _functionMin + (Numeric<T,C>)(pixelCoordinate - imageMin) * _distancePerPixel;
        }

        /// <summary>
        /// Transforms the coordinate of a point on the function plane axis
        /// to its coordinate on the image axis.
        /// </summary>
        /// <param name="functionCoordinate">The coordinate of a point on the function plane axis.</param>
        /// <returns>The coordinate of the point on the function plane axis.</returns>
        public double TransformFunctionPointToImagePoint(T functionCoordinate)
        {
			if (ReverseImageAxis)
			{
				return imageMax - _toDouble((functionCoordinate - _functionMin) / _distancePerPixel);
			}

            return imageMin + _toDouble((functionCoordinate - _functionMin) / _distancePerPixel);
        }

        // --------------------------------------------------
        // --------------------- Range transform ------------
        // --------------------------------------------------

        /// <summary>
        /// Transforms the length of pixel axis range to the equivalent length of function axis range.
        /// </summary>
        /// <param name="imageAxisRangeLength">The length of pixel axis range.</param>
        /// <returns>The length of function axis range.</returns>
		public T TransformRangeLengthImageToFunction(double imageAxisRangeLength)
        {
            return (Numeric<T,C>)imageAxisRangeLength * _distancePerPixel;
        }

        /// <summary>
        /// Transforms the length of function plane axis range to the equivalent length
        /// of image axis rangel.
        /// </summary>
        /// <param name="functionAxisRangeLength">The length of function plane axis range.</param>
        /// <returns>The length of image axis range.</returns>
		public double TransformRangeLengthFunctionToImage(T functionAxisRangeLength)
        {
            return _toDouble(functionAxisRangeLength / _distancePerPixel);
        }

        // --------------------------------------------------
        // --------------------- helper API -----------------
        // --------------------------------------------------

        // ------------OBJECT METHODS OVERRIDING---------------

        public override bool Equals(object obj)
        {
			if (obj == null)
			{
				return false;
			}

			if (obj is DimensionTransformer<T,C>)
            {
                DimensionTransformer<T,C> dt = obj as DimensionTransformer<T,C>;

                return 
                    this.imageMin == dt.imageMin
				    && this.imageMax == dt.imageMax
                    && this._functionMin == dt._functionMin
                    && this._functionMax == dt._functionMax
					&& this._toDouble == dt._toDouble;
            }
            
            return false;
        }

        public override string ToString()
        {
            return String.Format(
				"DimensionTransformer[imageAxisRange = {0}-{1}; functionAxisRange = {2}-{3}]", 
				imageMin, imageMax, _functionMin, _functionMax);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return
                    _functionMin.GetHashCode() + _functionMax.GetHashCode() +
                    imageMin.GetHashCode() + _functionMax.GetHashCode();
            }
        }
    }
}
