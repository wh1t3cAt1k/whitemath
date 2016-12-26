namespace WhiteMath.ArithmeticLong
{
	/// <summary>
	/// Provides the required information on numeric precision and serves as the 
	/// type argument for the LongExp class.
	/// 
	/// No two numbers with different IPrecision could be used in the same
	/// arithmetic operation unless the user explicitly calls the 
	/// precisionConvert() method for the number (precision may be extended or lost depending
	/// on the calculator to convert to).
	/// </summary>
	public interface IPrecision : IBase
	{
		/// <summary>
		/// Returns the precision, in decimal signs, for the LongExp mantiss.
		/// </summary>
		int Precision { get; }
	}
}
