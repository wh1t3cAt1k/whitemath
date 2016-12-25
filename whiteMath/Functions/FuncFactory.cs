using System;

using whiteMath.Algorithms;
using whiteMath.Calculators;

namespace whiteMath.Functions
{
    public static class FuncFactory
    {
        public static Func<T, T> SigmoidFunction<T, C>(T exponentDelta, int taylorMemberCount = 100) where C : ICalc<T>, new()
        {
            return delegate(T x)
            {
                ICalc<T>    calc = Numeric<T, C>.Calculator;
                T           temp = WhiteMath<T, C>.Exponent(calc.Multiply(exponentDelta, x), taylorMemberCount);

                return Numeric<T, C>._1 / (Numeric<T, C>._1 + temp);
            };
        }
    }
}
