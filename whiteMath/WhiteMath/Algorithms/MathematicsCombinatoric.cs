using System;

using WhiteMath.Calculators;

namespace WhiteMath.Mathematics
{
    public partial class Mathematics<T, C> where C: ICalc<T>, new()
    {
        public static T Combinations(T n, T k)
        {
			throw new NotImplementedException();
			/*
            // Numeric<T,C> nNumeric = n;
            // Numeric<T,C> kNumeric = k;

            if (nNumeric < Numeric<T, C>.Zero && k > Numeric<T,C>.Zero)
            {
				throw new NotImplementedException();
            }

            if (calc.mor(calc.zero, k) || calc.mor(k, n))
                return calc.zero;

            return default(T);
			*/
        }

        public static T CombinationsRepeated(T n, T k)
        {
			throw new NotImplementedException();
            // return default(T);
            // return Combinations(calc.dif(calc.sum(n, k), calc.fromInt(1)), k);
        }
    }
}
