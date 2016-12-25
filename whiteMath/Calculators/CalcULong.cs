using whiteStructs.Conditions;

namespace whiteMath.Calculators
{
    /// <summary>
    /// Standard calculator for unsigned long integers.
    /// </summary>
    public class CalcULong : ICalc<ulong>
    {
        public bool IsIntegerCalculator     { get { return true; } }

        public ulong Add(ulong one, ulong two)    { return one + two; }
        public ulong Subtract(ulong one, ulong two)    { return one - two; }
        public ulong Multiply(ulong one, ulong two)    { return one * two; }
        public ulong Divide(ulong one, ulong two)    { return one / two; }
        public ulong Modulo(ulong one, ulong two)    { return one % two; }
        
        public ulong Negate(ulong one)          { throw new NonNegativeTypeException("ulong"); }

        public ulong Increment(ulong one)       { return ++one; }
        public ulong Decrement(ulong one)       { return --one; }

        public bool GreaterThan(ulong one, ulong two)   { return one > two; }
        public bool Equal(ulong one, ulong two)   { return one == two; }

        public ulong IntegerPart(ulong one)                 { return one; }

        public bool IsEven(ulong one)                 { return one % 2 == 0; }
        public bool IsNaN(ulong one)                  { return false; }
        public bool IsPositiveInfinity(ulong one)               { return false; }
        public bool IsNegativeInfinity(ulong one)               { return false; }

        public ulong GetCopy(ulong val)                 { return val; }
        public ulong Zero                             { get { return 0; } }

        public ulong FromInteger(long equivalent)         
        {
			Condition.ValidateNonNegative(equivalent, Messages.CannotConvertNegativeValueToUnsignedType);

			return (ulong)equivalent; 
        }
        
        public ulong FromDouble(double equivalent) 
		{ 
			throw new NonFractionalTypeException(typeof(ulong).Name); 
		}

        public ulong Parse(string value) 
		{
			return ulong.Parse(value); 
		}
    }
}
