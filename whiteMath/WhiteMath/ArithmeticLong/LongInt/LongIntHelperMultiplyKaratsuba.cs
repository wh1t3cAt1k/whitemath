using System;
using System.Collections.Generic;

using WhiteMath.General;

using WhiteStructs.Collections;

namespace WhiteMath.ArithmeticLong
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
            public static LongInt<B> MultiplyKaratsuba(LongInt<B> one, LongInt<B> two)
            {
                LongInt<B> bigger = one.Length > two.Length ? one : two;
                
				int twoPower = 1;

                while (bigger.Length > twoPower)
                    twoPower <<= 1;

                LongInt<B> result = new LongInt<B>();
                result.IsNegative = one.IsNegative ^ two.IsNegative;
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

			private static void MultiplyKaratsuba(int digitBase, IList<int> result, IList<int> one, IList<int> two, int dim)
            {
                int half = dim / 2;

                if (dim <= karatsubaCutoffDimension)
                {
					LongIntegerMethods.MultiplySimple(digitBase, result, one.AsReadOnly(), two.AsReadOnly());
                    return;
                }

                ListSegment<int> a = new ListSegment<int>(one, 0, half);
                ListSegment<int> b = new ListSegment<int>(one, half, one.Count - half);

                ListSegment<int> c = new ListSegment<int>(two, 0, half);
                ListSegment<int> d = new ListSegment<int>(two, half, two.Count - half);

                int[] ac = new int[dim];
                int[] bd = new int[b.Count + d.Count];
                int[] abcd = new int[b.Count + d.Count + 2];

                MultiplyKaratsuba(digitBase, ac, a, c, half);
                MultiplyKaratsuba(digitBase, bd, b, d, half);
                
                int[] apb = new int[b.Count + 1];
                int[] cpd = new int[d.Count + 1];
                int[] acpbd = new int[b.Count + d.Count + 2];

                LongIntegerMethods.Sum(digitBase, apb, a, b);
                LongIntegerMethods.Sum(digitBase, cpd, c, d);

                MultiplyKaratsuba(digitBase, abcd, apb, cpd, half);

                LongIntegerMethods.Sum(digitBase, acpbd, ac, bd);
                
                int[] difference = new int[b.Count + d.Count + 2];

				if (LongIntegerMethods.Subtract(digitBase, difference, abcd, acpbd))
				{
					throw new Exception("Lower-level difference error.");
				}

                SumPrivate(digitBase, result, ac, 0, difference, half);
				SumPrivate(digitBase, result, result.AsReadOnly(), 0, bd, dim);

                return;
            }
        }
    }
}
