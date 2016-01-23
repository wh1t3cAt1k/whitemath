using System;

using whiteMath.ArithmeticLong;
using whiteMath.Calculators;
using whiteMath.General;

using LongInt = whiteMath.ArithmeticLong.LongInt<whiteMath.ArithmeticLong.Precisions.P_50k_10k>;

using Rational = whiteMath.RationalNumbers.Rational<
	whiteMath.ArithmeticLong.LongInt<whiteMath.ArithmeticLong.Precisions.P_50k_10k>, 
	whiteMath.Calculators.CalcLongInt<whiteMath.ArithmeticLong.Precisions.P_50k_10k>>;

namespace whiteMath.Algorithms
{
    public static partial class WhiteMathConstants
    {
        public static readonly double PI_Double = Math.PI;
        public static readonly double E_Double = Math.E;

        // ----------- Spigot algorithm for E ----------------

        /// <summary>
        /// Runs a spigot algorithm that calculates the first <paramref name="digitsCount"/>
        /// digits for the transcend constant e.
        /// 
        /// The digits returned are of base <paramref name="decimalBase"/>,
        /// which should be a power of ten: 10, 100, 1000 etc.
        /// 
        /// REQUIREMENTS:
        /// 
        /// 1. Memory consumption for the array of [n+<paramref name="safetyStorage"/>] longs and the array of [n] ints.
        /// </summary>
        /// <param name="decimalBase">The decimal base of digits to be returned.</param>
        /// <param name="digitsCount">The overall digits count to be calculated.</param>
        /// <param name="safetyStorage">The safety storage of type long that is used to reduce the errors of calculations.</param>
        /// <returns>The array of 'e' digits.</returns>
        public static int[] E_SpigotAlgorithm(int decimalBase, int digitsCount, int safetyStorage, out long maxValue)
        {
            int[] result = new int[digitsCount];
            long[] arr = new long[digitsCount + safetyStorage];
            
            arr.FillByAssign(1);

            maxValue = 1;

            int count = 0;
            long quot;

            while (count < digitsCount)
            {
                quot = 0;

                for (int i = arr.Length - 1; i >= 0; i--)
                {
                    arr[i] = arr[i] * decimalBase + quot; // умножаем на мета (10)

                    if (arr[i] > maxValue)
                        maxValue = arr[i];

                    quot = arr[i] / (i + 2);
                    arr[i] = arr[i] % (i + 2);
                }

                result[count++] = (int)quot;
            }

            return result;
        }

        // ---------------------------------------------------

        /// <summary>
        /// Returns the rational approximation of transcend PI constant
        /// using the Leibnitz series. Works slower than the other variation, but consumes
        /// significantly less memory.
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        public static Rational PI_Rational_Leibnitz_Slow(int members)
        {
            Rational sum = new Rational(0, 1);

            for (int i = 0; i < members; i++)
            {
                Rational tmp = new Rational(4, i * 2 + 1);
                if ((i % 2) == 0) sum += tmp;
                else sum -= tmp;
            }

            return sum;
        }

        /// <summary>
        /// Returns the rational approximation of transcend PI constant
        /// using the Leibnitz series.
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        public static Rational PI_Rational_Leibnitz_Quick(int members)
        {
            LongInt num = 0;
            LongInt denom = 1;

            for (int i = members - 1; i >= 0; i--)
            {
                num = LongInt.Helper.MultiplyKaratsuba(num, (i * 2 + 1));

                if ((i % 2) == 0) num += denom;
                else num -= denom;

                denom = LongInt.Helper.MultiplyKaratsuba(denom, (i * 2 + 1));

                Console.WriteLine("Step " + i + " performed.");
            }

            return new Rational(num * 4, denom);
        }

        // ---------------------------------------

    }

    // ------------------------------- NON-GENERIC PART GOES THERE

    public static partial class WhiteMath<T, C> where C : ICalc<T>, new()
    {
        /// <summary>
        /// --- For real number types only! ---
        /// --- Higher precision recommended ---
        /// 
        /// Returns the rational approximation of transcend PI constant
        /// using the Leibnitz series. May be inaccurate, non-generic version is recommended if possible.
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        public static T PI_Leibnitz(long members)
        {
            T sum = calc.zero;

            for (long i = members - 1; i >= 0; i--)
            {
                T tmp = calc.div(calc.fromInt(4), calc.fromInt(i * 2 + 1));
                if ((i % 2) == 0) sum = calc.sum(sum, tmp);
                else sum = calc.dif(sum, tmp);
            }

            return sum;
        }
    }


}
