using System;
using System.Collections.Generic;

using whiteMath.General;

using WhiteMath = whiteMath.Algorithms.WhiteMath<int, whiteMath.Calculators.CalcInt>;

namespace whiteMath.ArithmeticLong
{
    public partial class LongInt<B> where B: IBase, new()
    {
        public static partial class Helper
        {
            // ----------------------------------------------------------------
            // ------------------KARATSUBA MULTIPLICATION----------------------
            // ----------------------------------------------------------------

            private static int karatsubaCutoffDimension = 4;

            // THIS SHOULD BE OPTIMIZED FOR KARATSUBA MULTIPLICATION BY METHOD BELOW
            // ---------------------------------------------------------------------

            /// <summary>
            /// The Karatsuba multiplying algorithm.
            /// The lengths of numbers are completed to the nearest bigger power of 2.
            /// </summary>
            /// <param name="one"></param>
            /// <param name="two"></param>
            /// <returns></returns>
            public static LongInt<B> MultiplyKaratsuba(LongInt<B> one, LongInt<B> two)
            {
                LongInt<B> bigger = one.Length > two.Length ? one : two;
                
				int twoPower = 1;

                while (bigger.Length > twoPower)
                    twoPower <<= 1;

                LongInt<B> result = new LongInt<B>();
                result.Negative = one.Negative ^ two.Negative;
                result.Digits.AddRange(new int[twoPower * 2]);

                one.Digits.AddRange(new int[twoPower - one.Length]);
                two.Digits.AddRange(new int[twoPower - two.Length]);

                MultiplyKaratsuba(LongInt<B>.BASE, result.Digits, one.Digits, two.Digits, twoPower);

                result.DealWithZeroes();
                one.DealWithZeroes();
                two.DealWithZeroes();

                return result;
            }

            // ---------------------------- TODO -----------------------------------

            public static void MultiplyOptimization()
            {
            }

            // ---------------------------------------------------------------------

            private static void MultiplyKaratsuba(int BASE, IList<int> result, IList<int> one, IList<int> two, int dim)
            {
                int half = dim / 2;

                // Отбрасываем при некотором значении
                if (dim <= karatsubaCutoffDimension)
                {
                    LongIntegerMethods.MultiplySimple(BASE, result, one, two);
                    return;
                }

                ListSegment<int> a = new ListSegment<int>(one, 0, half);
                ListSegment<int> b = new ListSegment<int>(one, half, one.Count - half);

                ListSegment<int> c = new ListSegment<int>(two, 0, half);
                ListSegment<int> d = new ListSegment<int>(two, half, two.Count - half);

                int[] ac = new int[dim];
                int[] bd = new int[b.Count + d.Count];
                int[] abcd = new int[b.Count + d.Count + 2];

                MultiplyKaratsuba(BASE, ac, a, c, half);
                MultiplyKaratsuba(BASE, bd, b, d, half);
                
                int[] apb = new int[b.Count + 1];
                int[] cpd = new int[d.Count + 1];
                int[] acpbd = new int[b.Count + d.Count + 2];

                LongIntegerMethods.Sum(BASE, apb, a, b);
                LongIntegerMethods.Sum(BASE, cpd, c, d);

                MultiplyKaratsuba(BASE, abcd, apb, cpd, half);

                LongIntegerMethods.Sum(BASE, acpbd, ac, bd);
                
                int[] difference = new int[b.Count + d.Count + 2];
                
                if (LongIntegerMethods.Subtract(BASE, difference, abcd, acpbd))
                    Console.WriteLine("Lower-level difference error.");

                SumPrivate(BASE, result, ac, 0, difference, half);
                SumPrivate(BASE, result, result, 0, bd, dim);

                return;
            }
        }
    }
}
