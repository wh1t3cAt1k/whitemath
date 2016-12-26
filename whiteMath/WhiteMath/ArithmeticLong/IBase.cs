namespace WhiteMath.ArithmeticLong
{
	/// <summary>
	/// Provides the needed information on long integers numeric base
	/// and serves as the type argument for the LongInt class.
	/// 
	/// No two numbers with different IBase could be used in the same
	/// arithmetic operation unless the user explicitly calls the
	/// baseConvert() method or the appropriate LongInt constructor.
	/// </summary>
	public interface IBase
	{
		/// <summary>
		/// Returns the value of the digits' numeric base.
		/// </summary>
		int Base { get; }
	}
}
