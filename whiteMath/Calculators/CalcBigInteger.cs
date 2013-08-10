using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath
{
    /// <summary>
    /// Standard calculator for integers.
    /// </summary>
    public class CalcInt : ICalc<int>
    {
        public bool isIntegerCalculator     { get { return true; } }

        public int sum(int one, int two)    { return one + two; }
        public int dif(int one, int two)    { return one - two; }
        public int mul(int one, int two)    { return one * two; }
        public int div(int one, int two)    { return one / two; }
        public int rem(int one, int two)    { return one % two; }
        public int negate(int one)          { return -one; }

        public int increment(int one)       { return ++one; }
        public int decrement(int one)       { return --one; }

        public bool mor(int one, int two)   { return one > two; }
        public bool eqv(int one, int two)   { return one == two; }

        public int intPart(int one)                 { return one; }

        public bool isEven(int one)                 { return one % 2 == 0; }
        public bool isNaN(int one)                  { return false; }
        public bool isPosInf(int one)               { return false; }
        public bool isNegInf(int one)               { return false; }

        public int getCopy(int val)                 { return val; }
        public int zero                             { get { return 0; } }

        public int fromInt(long equivalent)         { return (int)equivalent; }
        public int fromDouble(double equivalent)    { throw new NonFractionalTypeException("int"); }

        public int parse(string value) { return int.Parse(value); }
    }
}
