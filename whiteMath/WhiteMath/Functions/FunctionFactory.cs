using System;

using WhiteMath.Mathematics;
using WhiteMath.Calculators;

namespace WhiteMath.Functions
{
    public static class FunctionFactory
    {
		public static Func<T, T> GetSigmoidFunction<T, C>(T exponentDelta, int taylorMemberCount) 
			where C : ICalc<T>, new()
        {
            return x =>
            {
                ICalc<T> calc = Numeric<T, C>.Calculator;
                T temp = Mathematics<T, C>.Exponent(calc.Multiply(exponentDelta, x), taylorMemberCount);

                return Numeric<T, C>._1 / (Numeric<T, C>._1 + temp);
            };
        }
    }
}
