namespace WhiteMath.Random
{
	/// <summary>
	/// An interface which provides a method for generating
	/// sequences of random bytes.
	/// </summary>
	public interface IRandomBytes
	{
		/// <summary>
		/// Generates a sequence of random bytes and stores them 
		/// in the specified buffer.
		/// </summary>
		/// <param name="buffer">The buffer to store the random sequence of bytes.</param>
		void NextBytes(byte[] buffer);
	}
}
