namespace WhiteMath.Randoms
{
	/// <summary>
	/// Represents an object that is capable of generating random numbers
	/// in the whole range of numeric type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of generated numbers.</typeparam>
	public interface IRandomUnbounded<T>
	{
		/// <summary>
		/// Returns the next random value from
		/// all of the possible finite values of type <typeparamref name="T"/>.
		/// </summary>
		T Next();
	}
}