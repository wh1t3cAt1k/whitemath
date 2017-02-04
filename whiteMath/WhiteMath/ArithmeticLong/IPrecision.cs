namespace WhiteMath.ArithmeticLong
{
	/// <summary>
	/// Provides the required information on numeric precision and 
	/// serves as the type argument for the <see cref="LongExp{B, P}"/> class.
	/// </summary>
	public interface IPrecision
	{
		/// <summary>
		/// Returns the precision, in decimal signs, for the LongExp mantiss.
		/// </summary>
		int Precision { get; }
	}
}
