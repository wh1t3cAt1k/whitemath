using WhiteMath.Calculators;

using WhiteStructs.Conditions;

namespace WhiteMath.Randoms.Extensibility
{
	/// <summary>
	/// This class wraps around an <see cref="IRandomBounded{T}"/>
	/// to treat it as an upper-bounded generator in a certain interval 
	/// (absolute minimum and absolute maximum values should be provided).
	/// </summary>
	/// <remarks>
	/// Please notice that for bigger intervals the quality of the distribution
	/// may seriously suffer due to scale irregularity of some numeric types, e.g. <c>double</c>.
	/// </remarks>
	/// <typeparam name="T">The type of generated numbers.</typeparam>
	/// <typeparam name="C">A calculator type for the <typeparamref name="T"/> type.</typeparam>
	public class RandomBoundedToUpperBoundedWrapper<T, C> : IRandomUpperBounded<T>
		where C: ICalc<T>, new()
	{
		public IRandomBounded<T> BoundedGenerator
		{
			get;
			private set;
		}

		public RandomBoundedToUpperBoundedWrapper(IRandomBounded<T> boundedGenerator)
		{
			Condition.ValidateNotNull(boundedGenerator, nameof(boundedGenerator));

			this.BoundedGenerator = boundedGenerator;
		}

		public T Next(T maxExclusive) 
			=> BoundedGenerator.Next(Numeric<T, C>._0, maxExclusive);
	}
}
