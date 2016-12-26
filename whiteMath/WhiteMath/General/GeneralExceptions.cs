using System;

namespace WhiteMath.General
{
    /// <summary>
    /// If an instance of this exception occurs somewhere,
    /// it seems to be a result of code refactoring, often an unforeseen addition to enums, so 
    /// feel free to immediately write to author's e-mail address and kick his butt.
    /// </summary>
    public class EnumFattenedException : Exception
    {
        public EnumFattenedException(string message)
            : base("The enum has fattened so this function stopped working correctly: " + message)
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

        internal static ArgumentOutOfRangeException __MINVALUE_EXCEED_EQUAL_MAXVALUE
            = new ArgumentOutOfRangeException("The minimum value exceeds or is equal to the maximum value.");

        internal static ArgumentOutOfRangeException __LOWERBOUND_EXCEED_UPPERBOUND_FUNC<T>(T lowerBound, T upperBound)
        {
            return new ArgumentOutOfRangeException(string.Format("The lower bound {0} exceeds the upper bound {1}.", lowerBound, upperBound));
        }

        internal static ArgumentOutOfRangeException __MINVALUE_EXCEED_MAXVALUE_FUNC<T>(T minValue, T maxValue)
        {
            return new ArgumentOutOfRangeException(string.Format("The lower bound {0} exceeds the upper bound {1}.", minValue, maxValue));
        }

        // -------------------------------------------
        // -------------- For random generators ------
        // -------------------------------------------

        internal static NotSupportedException __RNG_INT_SINGLEINTERVAL_NOTSUPPORTED
            = new NotSupportedException("Random integer numbers in the [0; 1) interval will always be equal to zero, so this method is not to be called on integer generators.");
    
        // -------------- For sequences --------------

        internal static ArgumentException __SEQUENCE_EMPTY
            = new ArgumentException("The sequence object passed contains no elements.");
    }
}
