using whiteStructs.Conditions;

namespace whiteMath.Calculators
{
    /// <summary>
    /// Standard calculator for unsigned long integers.
    /// </summary>
    public class CalcULong : ICalc<ulong>
    {
        public bool isIntegerCalculator     { get { return true; } }

        public ulong sum(ulong one, ulong two)    { return one + two; }
        public ulong dif(ulong one, ulong two)    { return one - two; }
        public ulong mul(ulong one, ulong two)    { return one * two; }
        public ulong div(ulong one, ulong two)    { return one / two; }
        public ulong rem(ulong one, ulong two)    { return one % two; }
        
        public ulong negate(ulong one)          { throw new NonNegativeTypeException("ulong"); }

        public ulong increment(ulong one)       { return ++one; }
        public ulong decrement(ulong one)       { return --one; }

        public bool mor(ulong one, ulong two)   { return one > two; }
        public bool eqv(ulong one, ulong two)   { return one == two; }

        public ulong intPart(ulong one)                 { return one; }

        public bool isEven(ulong one)                 { return one % 2 == 0; }
        public bool isNaN(ulong one)                  { return false; }
        public bool isPosInf(ulong one)               { return false; }
        public bool isNegInf(ulong one)               { return false; }

        public ulong getCopy(ulong val)                 { return val; }
        public ulong zero                             { get { return 0; } }

        public ulong fromInt(long equivalent)         
        {
			Condition.ValidateNonNegative(equivalent, Calculators.Messages.CannotConvertNegativeValueToUnsignedType);

			return (ulong)equivalent; 
        }
        
        public ulong fromDouble(double equivalent) 
		{ 
			throw new NonFractionalTypeException("ulong"); 
		}

        public ulong parse(string value) 
		{
			return ulong.Parse(value); 
		}
    }
}
