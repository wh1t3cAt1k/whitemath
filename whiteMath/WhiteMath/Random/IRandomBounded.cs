namespace WhiteMath.Random
{
	/// <summary>
	/// A generic interface providing means for generating random numbers 
	/// in the specified range.
	/// </summary>
	/// <typeparam name="T">The type of generated numbers.</typeparam>
	public interface IRandomBounded<T>
	{
		/// <summary>
		/// Returns the next random value in the
		/// <c>[min; max)</c> interval.
		/// </summary>
		/// <param name="min">The lower inclusive numeric boundary of the generated number.</param>
		/// <param name="max">The upper exclusive numeric boundary of the generated number.</param>
		/// <returns>A random number laying in the [min; max) interval.</returns>
		T Next(T min, T max);
	}
}
