namespace WhiteMath.Randoms
{
	/// <summary>
	/// A generic interface providing means for generating random 
	/// floating-point numbers in the <c>[0; 1)</c> interval.
	/// </summary>
	/// <typeparam name="T">The type of generated numbers.</typeparam>
	public interface IRandomUnitInterval<T>
	{
		/// <summary>
		/// Returns the next random value in the
		/// [0; 1) interval.
		/// </summary>
		/// <returns>A random value in the [0; 1) interval.</returns>
		T NextInUnitInterval();
	}

}
