using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using whiteMath.ArithmeticLong;

namespace whiteMath.ArithmeticLong
{
    /// <summary>
    /// The default calculator for whiteMath long integer numbers.
    /// </summary>
    public class CalcLongInt<P>: ICalc<LongInt<P>> where P: IBase, new()
    {
        public bool isIntegerCalculator { get { return true; } }

        public LongInt<P> sum(LongInt<P> one, LongInt<P> two) { return one + two; }
        public LongInt<P> dif(LongInt<P> one, LongInt<P> two) { return one - two; }
        public LongInt<P> mul(LongInt<P> one, LongInt<P> two) { return one * two; }
        public LongInt<P> div(LongInt<P> one, LongInt<P> two) { return one / two; }
        public LongInt<P> rem(LongInt<P> one, LongInt<P> two) { return one % two; }

        public LongInt<P> increment(LongInt<P> one) { return ++one; }
        public LongInt<P> decrement(LongInt<P> one) { return --one; }

        public LongInt<P> negate(LongInt<P> num) { return -num; }

        public LongInt<P> intPart(LongInt<P> num) { return num.Clone() as LongInt<P>; }

        public bool mor(LongInt<P> one, LongInt<P> two) { return one > two; }
        public bool eqv(LongInt<P> one, LongInt<P> two) { return one == two; }

        public bool isEven(LongInt<P> num) { return num[0] % 2 == 0; }
        public bool isNaN(LongInt<P> num) { return false; }
        public bool isPosInf(LongInt<P> num) { return false; }
        public bool isNegInf(LongInt<P> num) { return false; }

        public LongInt<P> getCopy(LongInt<P> num) { return num.Clone() as LongInt<P>; }
        public LongInt<P> zero { get { return new LongInt<P>(); } }

        public LongInt<P> fromInt(long equivalent)      { return new LongInt<P>(equivalent); }
        public LongInt<P> fromDouble(double equivalent) { throw new NonFractionalTypeException("LongInt"); }

        public LongInt<P> parse(string value) { return LongInt<P>.Parse(value); }
    }
}
