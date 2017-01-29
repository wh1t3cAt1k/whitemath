using WhiteMath.Calculators;

using WhiteStructs.Conditions;

namespace WhiteMath.Randoms.Extensibility
{
	/// <summary>
	/// This class wraps around a <see cref="IRandomUnitInterval{T}"/>
	/// generator to treat it as an unbounded generator within the provided
	/// interval.
	/// </summary>
	/// <remarks>
	/// Please notice that for bigger intervals the quality of the distribution
	/// may seriously suffer due to scale irregularity of some numeric types,
	/// e.g. <see cref="double"/>.
	/// </remarks>
	/// <typeparam name="T">The type of generated numbers.</typeparam>
	/// <typeparam name="C">A calculator type for the <typeparamref name="T"/> type.</typeparam>
	public class RandomUnitIntervalToUnboundedWrapper<T, C> : IRandomUnbounded<T> where C : ICalc<T>, new()
	{
		/// <summary>
		/// Gets a floating-point number generator which is internally used 
		/// to produce numbers in the <c>[0; 1)</c> interval.
		/// </summary>
		public IRandomUnitInterval<T> Generator { get; private set; }

		/// <summary>
		/// Gets the lower boundary of generated numbers.
		/// This boundary is inclusive.
		/// </summary>
		public Numeric<T, C> Minimum { get; private set; }

		/// <summary>
		/// Gets the upper boundary for generated numbers.
		/// This boundary is exclusive.
		/// </summary>
		public Numeric<T, C> Maximum { get; private set; }

		/// <summary>
		/// Returns the next random number in the <c>[AbsoluteMinimum; AbsoluteMaximum)</c> interval.
		/// </summary>
		public T Next()
		{
			return Minimum + Generator.NextInUnitInterval() * (Maximum - Minimum);
		}

		/// <summary>
		/// Initializes the wrapper instance with a floating-point generator
		/// and a numeric interval.
		/// </summary>
		/// <param name="generator">A floating-point generator.</param>
		/// <param name="absoluteInclusiveMinimum">The lower inclusive boundary for generated numbers.</param>
		/// <param name="absoluteExclusiveMaximum">The upper exclusive boundary for generated numbers.</param>
		public RandomUnitIntervalToUnboundedWrapper(
			IRandomUnitInterval<T> generator, 
			Numeric<T, C> absoluteInclusiveMinimum, 
			Numeric<T, C> absoluteExclusiveMaximum)
		{
			Condition.ValidateNotNull(generator, nameof(generator));
			Condition
				.Validate(absoluteExclusiveMaximum > absoluteInclusiveMinimum)
				.OrArgumentOutOfRangeException("The lower boundary should be less than the upper boundary.");

			this.Generator = generator;
			this.Minimum = absoluteInclusiveMinimum;
			this.Maximum = absoluteExclusiveMaximum;
		}
	}
}
