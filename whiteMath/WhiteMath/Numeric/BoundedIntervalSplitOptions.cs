using System;

namespace WhiteMath.Numeric
{
	/// <summary>
	/// This enum contains options to specify when splitting the interval into a sequence of non-intersecting intervals
	/// and the source interval does not contain a whole number of intervals of the desired length. 
	/// </summary>
	public enum BoundedIntervalSplitOptions
	{
		/// <summary>
		/// When using this option, in case of presence of the 'tail' length,
		/// the last interval will have length bigger than the desired.
		/// The overall number of intervals produced will be floor(currentInterval.length / desiredLength).
		/// </summary>
		BiggerLastInterval,

		/// <summary>
		/// When using this option, in case of presence of the 'tail' length,
		/// the last interval will have length smaller than the desired.
		/// The overall number of intervals produced will be ceil(currentInterval.length / desiredLength).
		/// </summary>
		SmallerLastInterval
	}
}
