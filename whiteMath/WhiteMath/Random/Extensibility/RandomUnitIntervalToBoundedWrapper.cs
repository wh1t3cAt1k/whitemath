﻿using WhiteMath.Calculators;

using WhiteStructs.Conditions;

namespace WhiteMath.Random.Extensibility
{
	/// <summary>
	/// This class wraps around an <see cref="IRandomUnitInterval{T}"/>
	/// to treat it as an unbounded generator in a certain interval 
	/// (absolute minimum and absolute maximum values should be provided).
	/// </summary>
	/// <remarks>
	/// Please notice that for bigger intervals the quality of the distribution
	/// may seriously suffer due to scale irregularity of some numeric types, e.g. <c>double</c>.
	/// </remarks>
	/// <typeparam name="T">The type of generated numbers.</typeparam>
	/// <typeparam name="C">A calculator type for the <typeparamref name="T"/> type.</typeparam>
	public class RandomUnitIntervalToBoundedWrapper<T, C> : IRandomBounded<T> 
		where C : ICalc<T>, new()
	{
		public IRandomUnitInterval<T> Generator { get; private set; }

		/// <summary>
		/// Returns the next random number in the specified interval.
		/// </summary>
		/// <remarks>
		/// Please notice that for bigger intervals the quality of the distribution
		/// may seriously suffer due to scale irregularity of some numeric types, e.g. <c>double</c>.
		/// </remarks>
		/// <param name="minInclusive">The lower inclusive bound of generated numbers.</param>
		/// <param name="maxExclusive">The upper exclusive bound of generated numbers.</param>
		public T Next(T minInclusive, T maxExclusive)
		{
			Condition
				.Validate(Numeric<T, C>.Calculator.GreaterThan(maxExclusive, minInclusive))
				.OrArgumentOutOfRangeException("The lower boundary should be less than the upper boundary.");

			Numeric<T, C> minValue = minInclusive;
			Numeric<T, C> maxValue = maxExclusive;

			return minValue + Generator.NextInUnitInterval() * (maxValue - minValue);
		}

		/// <summary>
		/// Initializes the wrapper instance with a floating-point generator.
		/// </summary>
		/// <param name="generator">A floating-point generator for the type <typeparamref name="T"/></param>
		public RandomUnitIntervalToBoundedWrapper(IRandomUnitInterval<T> generator)
		{
			Condition.ValidateNotNull(generator, nameof(generator));

			this.Generator = generator;
		}
	}
}
