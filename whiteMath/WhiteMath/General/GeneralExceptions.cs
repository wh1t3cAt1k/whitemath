using System;

namespace WhiteMath.General
{
    /// <summary>
    /// If an instance of this exception occurs somewhere, it seems to be a result 
	/// of code refactoring, often an unforeseen addition to enums, so feel free 
	/// to immediately write to author's e-mail address and kick his butt.
    /// </summary>
    public class EnumFattenedException : ArgumentException
    {
        public EnumFattenedException(string message)
            : base("The enum has increased in size so this function stopped working correctly: " + message)
        { }
    }

    internal static class GeneralExceptions
    {
        internal static ArgumentOutOfRangeException __LOWERBOUND_EXCEED_UPPERBOUND 
            = new ArgumentOutOfRangeException("The lower bound exceeds the upper bound.");

        internal static ArgumentOutOfRangeException __LOWERBOUND_EXCEED_EQUAL_UPPERBOUND
            = new ArgumentOutOfRangeException("The lower bound exceeds or is equal to the upper bound.");

        internal static ArgumentOutOfRangeException __MINVALUE_EXCEED_MAXVALUE 
            = new ArgumentOutOfRangeException("The minimum value exceeds the maximum value.");
    }
}
