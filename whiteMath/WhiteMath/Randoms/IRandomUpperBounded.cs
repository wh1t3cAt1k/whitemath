namespace WhiteMath.Randoms
{
	/// <summary>
	/// A generic interface providing means for generating non-negative
	/// random numbers that are less than the specified value.
	/// </summary>
	/// <typeparam name="T">The type of generated numbers.</typeparam>
	public interface IRandomUpperBounded<T>
	{
		/// <summary>
		/// Returns the next random value in the
		/// <c>[min; max)</c> interval.
		/// </summary>
		/// <param name="maxExclusive">The upper exclusive numeric boundary of the generated number.</param>
		/// <returns>A random number laying in the [min; max) interval.</returns>
		T Next(T maxExclusive);
	}
}
