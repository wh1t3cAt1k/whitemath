using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics.Contracts;

using whiteMath.General;

namespace whiteMath.ArithmeticLong
{
    public partial class LongInt<B> where B: IBase, new()
    {
        /// <summary>
        /// This class contains information associated with the finite field used to
        /// multiply two numbers using NTT.
        /// </summary>
        internal class NTTFastFiniteFieldInfo
        {
            /// <summary>
            /// Represents the prime modulus used in the process of multiplication.
            /// </summary>
            public long primeModulus { get; private set; }

            /// <summary>
            /// Represents the primitive root of unity used in the process of multiplication.
            /// </summary>
            public long rootOfUnity { get; private set; }

            public NTTFastFiniteFieldInfo(long primeModulus, long rootOfUnity)
            {
                this.primeModulus = primeModulus;
                this.rootOfUnity = rootOfUnity;
            }
        }

        public static partial class Helper
        {
            // In the process of multiplication we will do a binary search int NTT_BASES
            // and TRANSFORM_LENGTHS and ---indices--- of objects found will give us the ---coordinates---
            // of minimal NTTMultiplicationInfo suitable for multiplication of this transform length
            // and numeric base.
            //
            // A resource file is used for this purpose.
            // -
            private static bool NTT_FAST_PREPARED = false;

            private static int[] NTT_FAST_BASES;
            private static int[] NTT_FAST_TRANSFORM_LENGTHS;
            private static NTTFastFiniteFieldInfo[,] NTT_FAST_FIELD_INFO_OBJECTS;

            /// <summary>
            /// This method prepares the Helper class for fast NTT multiplication
            /// by loading finite field information from the resource file.
            /// 
            /// This can take a few seconds of time, so consider calling this method in advance.
            /// </summary>
            public static void PrepareFastNTTMultiplication()
            {
                // If already prepared, return at once.
                // -
                if (NTT_FAST_PREPARED)
                {
                    return;
                }

                // Pattern: 
                // BASE N P ROOT \r\n
                // -
                Regex pattern = new Regex(@"(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\r\n");

                Dictionary<NTTTransformInfo, NTTFastFiniteFieldInfo> fieldInfoDictionary =
                    new Dictionary<NTTTransformInfo, NTTFastFiniteFieldInfo>();

                SortedSet<int> acquiredBases = new SortedSet<int>();
                SortedSet<int> acquiredTransformLengths = new SortedSet<int>();

                foreach (Match match in pattern.Matches(whiteMath.Properties.Resources.NTTPrimeModuli))
                {
                    int currentNumericBase = int.Parse(match.Groups[1].Value);
                    int currentTransformLength = int.Parse(match.Groups[2].Value);

                    long currentPrimeModulus = long.Parse(match.Groups[3].Value);
                    long currentRootOfUnity = long.Parse(match.Groups[4].Value);

                    acquiredBases.Add(currentNumericBase);
                    acquiredTransformLengths.Add(currentTransformLength);

                    fieldInfoDictionary.Add(
                        new NTTTransformInfo(currentNumericBase, currentTransformLength),
                        new NTTFastFiniteFieldInfo(currentPrimeModulus, currentRootOfUnity));
                }

                // Convert the sets to arrays.
                // -
                LongInt<B>.Helper.NTT_FAST_BASES = acquiredBases.ToArray();
                LongInt<B>.Helper.NTT_FAST_TRANSFORM_LENGTHS = acquiredTransformLengths.ToArray();

                // Now fill the 2D array with appropriate bases and transform lengths.
                // -
                LongInt<B>.Helper.NTT_FAST_FIELD_INFO_OBJECTS =
                    new NTTFastFiniteFieldInfo[NTT_FAST_BASES.Length, NTT_FAST_TRANSFORM_LENGTHS.Length];

                // Associate the finite field information with bases and transform lengths.
                // -
                for (int baseIndex = 0; baseIndex < LongInt<B>.Helper.NTT_FAST_BASES.Length; ++baseIndex)
                {
                    for (
                        int transformLengthIndex = 0;
                        transformLengthIndex < LongInt<B>.Helper.NTT_FAST_TRANSFORM_LENGTHS.Length;
                        ++transformLengthIndex)
                    {
                        LongInt<B>.Helper.NTT_FAST_FIELD_INFO_OBJECTS[baseIndex, transformLengthIndex] =
                            fieldInfoDictionary[
                                new NTTTransformInfo(NTT_FAST_BASES[baseIndex], NTT_FAST_TRANSFORM_LENGTHS[transformLengthIndex])];
                    }
                }

                // Set flag that everything's okay.
                // -
                LongInt<B>.Helper.NTT_FAST_PREPARED = true;
            }

            /// <summary>
            /// Returns the "minimal" finite field info for a given pair of values for BASE and transform length.
            /// </summary>
            private static NTTFastFiniteFieldInfo getFastFiniteFieldInfo(int BASE, int transformLength)
            {
                // If the multiplication field information is unprepared, we should 
                // do it now.
                // -
                if (!LongInt<B>.Helper.NTT_FAST_PREPARED)
                {
                    LongInt<B>.Helper.PrepareFastNTTMultiplication();
                }

                // Do binary search of minimal BASE more or equal to the desired in
                // the array NTT_BASES
                // - 
                int baseIndex = Array.BinarySearch(LongInt<B>.Helper.NTT_FAST_BASES, BASE);

                if (baseIndex < 0)
                {
                    baseIndex = ~baseIndex;

                    // If the base is too big, we can't help.
                    // -
                    if (baseIndex >= LongInt<B>.Helper.NTT_FAST_BASES.Length)
                    {
                        return null;
                    }
                }

                // Do binary search of the EXACT transform length.
                // - 
                int transformLengthIndex = Array.BinarySearch(LongInt<B>.Helper.NTT_FAST_TRANSFORM_LENGTHS, transformLength);

                // If no such transform length, then it is impossible to perform NTT multiplication of such numbers.
                // -
                if (transformLengthIndex < 0)
                {
                    return null;
                }

                // Everything is okay otherwise.
                // -
                return NTT_FAST_FIELD_INFO_OBJECTS[baseIndex, transformLengthIndex];
            }

            /// <summary>
            /// Computes the product of two long integer numbers using NTT in finite fields.
            /// 
            /// The numbers are not necessarily required to be of two's power long;
            /// this algorithm performs all needed operations automatically.
            /// </summary>
            /// <param name="one"></param>
            /// <param name="two"></param>
            public static LongInt<B> MultiplyNTTFast(LongInt<B> one, LongInt<B> two)
            {
                Contract.Requires<ArgumentNullException>(one != null, "one");
                Contract.Requires<ArgumentNullException>(two != null, "two");

                LongInt<B> result = new LongInt<B>(one.Length + two.Length);
                long[] longResult = new long[one.Length + two.Length];

                MultiplyNTTFast(LongInt<B>.BASE, longResult, one.Digits, two.Digits);

                for (int i = 0; i < longResult.Length; i++)
                    result.Digits.Add((int)longResult[i]);

                result.Negative = one.Negative ^ two.Negative;
                result.DealWithZeroes();

                return result;
            }

            /// <summary>
            /// Computes the product of two long integer numbers using NTT in finite fields.
            /// 
            /// The numbers are not necessarily required to be of two's power long;
            /// this algorithm performs all needed operations automatically.
            /// </summary>
            /// <param name="result"></param>
            /// <param name="one"></param>
            /// <param name="two"></param>
            public static void MultiplyNTTFast(int BASE, IList<long> result, IList<int> one, IList<int> two)
            {
                Contract.Requires<ArgumentNullException>(result != null, "result");
                Contract.Requires<ArgumentNullException>(one != null, "one");
                Contract.Requires<ArgumentNullException>(two != null, "two");

                int maxLength = Math.Max(one.Count, two.Count);

                // Search for the minimal power of two bigger than both lengths of
                // the operands.
                // -
                int transformLength = 1;

                while (transformLength < maxLength)
                    transformLength *= 2;

                // Multiply by two again, for unambigous interpolation of the product.
                // - 
                transformLength *= 2;

                // Acquire the field info object associated with the current transform length
                // and BASE.
                // -
                NTTFastFiniteFieldInfo finiteFieldInfo =
                    LongInt<B>.Helper.getFastFiniteFieldInfo(BASE, transformLength);

                // Prepare the result storage.
                // - 
                long[] bigIntegerResult = new long[transformLength];

                // Squaring one operand saves us one FFT.
                // -
                if (object.ReferenceEquals(one, two))
                {
                    long[] op = new long[transformLength];

                    for (int i = 0; i < maxLength; i++)
                        op[i] = one[i];

                    long[] nttResult = Recursive_NTT(op, finiteFieldInfo);
                    long[] componentWiseProduct = componentMultiplyGalois(nttResult, nttResult, finiteFieldInfo);

                    bigIntegerResult = Recursive_NTT_Inverse(componentWiseProduct, finiteFieldInfo);
                }
                // If operands differ, nothing can be done.
                // -
                else
                {
                    long[] op1 = new long[transformLength];
                    long[] op2 = new long[transformLength];

                    for (int i = 0; i < maxLength; i++)
                    {
                        if (i < one.Count) op1[i] = one[i];
                        if (i < two.Count) op2[i] = two[i];
                    }

                    long[] nttResultOne = Recursive_NTT(op1, finiteFieldInfo);
                    long[] nttResultTwo = Recursive_NTT(op2, finiteFieldInfo);
                    long[] componentWiseProduct = componentMultiplyGalois(nttResultOne, nttResultTwo, finiteFieldInfo);

                    bigIntegerResult = Recursive_NTT_Inverse(componentWiseProduct, finiteFieldInfo);
                }

                // Convert each coefficient to long and perform all the carries.
                // -
                for (int i = 0; i < result.Count; i++)
                {
                    result[i] = (long)bigIntegerResult[i];
                }

                performCarry(BASE, result);
            }

            /// <summary>
            /// FFT
            /// </summary>
            /// <param name="coefficients"></param>
            /// <returns></returns>
            internal static long[] Recursive_NTT(
                IList<long> coefficients,
                NTTFastFiniteFieldInfo finiteFieldInfo)
            {
                int n = coefficients.Count;

                long[] rootsHalf = rootsOfUnityHalfGalois(finiteFieldInfo, n, false);

                return
                    Recursive_NTT_Skeleton(coefficients, rootsHalf, finiteFieldInfo, 1, 0);
            }

            /// <summary>
            /// BACK FFT
            /// </summary>
            /// <param name="coefficients"></param>
            /// <returns></returns>
            internal static long[] Recursive_NTT_Inverse(
                IList<long> coefficients,
                NTTFastFiniteFieldInfo finiteFieldInfo)
            {
                int n = coefficients.Count;

                long[] rootsHalf = rootsOfUnityHalfGalois(finiteFieldInfo, n, true);
                long[] result = Recursive_NTT_Skeleton(coefficients, rootsHalf, finiteFieldInfo, 1, 0);

                // We should multiply the result by the multiplicative inverse of length 
                // in contrast to the forward-NTT.
                // -
                long lengthMultiplicativeInverse =
                    WhiteMath<long, CalcLong>.MultiplicativeInverse(n, finiteFieldInfo.primeModulus);

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = (result[i] * lengthMultiplicativeInverse) % finiteFieldInfo.primeModulus;
                }

                return result;
            }

            /// <summary>
            /// Calculates the result of the recursive Number Theoretic Transform.
            /// </summary>
            /// <param name="coefficients"></param>
            /// <returns></returns>
            private static long[] Recursive_NTT_Skeleton(
                IList<long> coefficients,
                IList<long> rootsOfUnity,
                NTTFastFiniteFieldInfo finiteFieldInfo,
                int step,
                int offset)
            {
                // Calculate the length of vectors at the current step of recursion.
                // -
                int n = coefficients.Count / step - offset / step;

                if (n == 1)
                {
                    return new long[] { coefficients[offset] };
                }

                long[] results = new long[n];

                IList<long> resultEvens =
                    Recursive_NTT_Skeleton(coefficients, rootsOfUnity, finiteFieldInfo, step * 2, offset);

                IList<long> resultOdds =
                    Recursive_NTT_Skeleton(coefficients, rootsOfUnity, finiteFieldInfo, step * 2, offset + step);

                for (int k = 0; k < n / 2; k++)
                {
                    long bfly = (rootsOfUnity[k * step] * resultOdds[k]) % finiteFieldInfo.primeModulus;

                    results[k] = (resultEvens[k] + bfly) % finiteFieldInfo.primeModulus;
                    results[k + n / 2] = (resultEvens[k] - bfly) % finiteFieldInfo.primeModulus;

                    // Negative numbers may appear here.
                    // -
                    while (results[k + n / 2] < 0)
                    {
                        results[k + n / 2] += finiteFieldInfo.primeModulus;
                    }
                }

                return results;
            }

            /// <summary>
            /// Returns the first half of the [rootDegree]th roots of unity series in the long field.
            /// Used in the recursive FFT algorithm in LongInt.Helper class.
            /// 
            /// Root degree should be an exact power of two.
            /// </summary>
            /// <param name="rootDegree"></param>
            /// <returns></returns>
            internal static long[] rootsOfUnityHalfGalois(
                NTTFastFiniteFieldInfo finiteFieldInfo,
                int rootDegree,
                bool inverted)
            {
                Contract.Requires<ArgumentOutOfRangeException>(rootDegree > 0, "The degree of the root should be a positive power of two.");
                Contract.Ensures(
                    Contract.ForAll(
                        Contract.Result<long[]>(),
                        (x => WhiteMath<long, CalcLong>.PowerIntegerModular(x, (ulong)rootDegree, finiteFieldInfo.primeModulus) == 1)));

                long[] result = new long[rootDegree / 2];

                // The first root of unity is obviously one, 
                // but we start with -1 if we need the inverse roots.
                // -
                if (!inverted)
                {
                    result[0] = 1;
                }
                else
                {
                    result[0] = finiteFieldInfo.primeModulus - 1;
                }

                // Calculate the desired roots of unity.
                // -
                for (int i = 1; i < rootDegree / 2; ++i)
                {
                    // All the desired roots of unity are obtained as powers of the principal root. 
                    // -
                    result[i] = result[i - 1] * finiteFieldInfo.rootOfUnity % finiteFieldInfo.primeModulus;
                }

                // If performing the inverse NTT, we should reverse the array along 
                // with a magic shift by 1 to get the inverted roots in the necessary order.
                //
                if (inverted)
                {
                    result[0] = 1;

                    for (int i = 1; i < rootDegree / 4; ++i)
                    {
                        result.Swap(i, rootDegree / 2 - i);
                    }
                }

                return result;
            }

            /// <summary>
            /// Performs the component-to-component multiplication of two convolution vectors in the finite field.
            /// </summary>
            /// <param name="one"></param>
            /// <param name="two"></param>
            /// <returns></returns>
            internal static long[] componentMultiplyGalois(
                IList<long> one,
                IList<long> two,
                NTTFastFiniteFieldInfo finiteFieldInfo)
            {
                long[] result = new long[one.Count];

                for (int i = 0; i < one.Count; i++)
                {
                    result[i] = (one[i] * two[i]) % finiteFieldInfo.primeModulus;
                }

                return result;
            }
        }
    }
}
