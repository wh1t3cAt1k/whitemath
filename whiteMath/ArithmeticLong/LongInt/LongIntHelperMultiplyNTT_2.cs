﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Numerics;

using whiteMath.Algorithms;
using whiteMath.General;

using whiteStructs.Conditions;

namespace whiteMath.ArithmeticLong
{
    public partial class LongInt<B> where B: IBase, new()
    {
        /// <summary>
        /// This struct contains the info used the information about the transform.
        /// </summary>
        internal struct NTTTransformInfo
        {
            /// <summary>
            /// Gets the numeric base of transformed vectors.
            /// </summary>
            public int numericBase      { get; private set; }
            
            /// <summary>
            /// Gets the length of vectors transformed.
            /// </summary>
            public int transformLength  { get; private set; }

            public NTTTransformInfo(int numericBase, int transformLength): this()
            {
                this.numericBase = numericBase;
                this.transformLength = transformLength;
            }

            public override bool Equals(object obj)
            {
                if (obj is NTTTransformInfo)
                {
                    NTTTransformInfo otherTransformInfo = (NTTTransformInfo)obj;

                    return
                        this.numericBase == otherTransformInfo.numericBase &&
                        this.transformLength == otherTransformInfo.transformLength;
                }

                return false;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    // Start with a prime.
                    // -
                    int result = 23;

                    // Multiply by a prime not to run into hash(a;b)==(b;a).
                    // -
                    result = result * 31 + numericBase;
                    result = result * 31 + transformLength;

                    return result;
                }
            }
        }

        /// <summary>
        /// This class contains information associated with the finite field used to
        /// multiply two numbers using NTT.
        /// </summary>
        internal class NTTFiniteFieldInfo
        {
            /// <summary>
            /// Represents the prime modulus used in the process of multiplication.
            /// </summary>
            public BigInteger primeModulus { get; private set; }
            
            /// <summary>
            /// Represents the primitive root of unity used in the process of multiplication.
            /// </summary>
            public BigInteger rootOfUnity { get; private set; }

            public NTTFiniteFieldInfo(BigInteger primeModulus, BigInteger rootOfUnity)
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
            private static bool  NTT_PREPARED = false;
            
            private static int[] NTT_BASES;
            private static int[] NTT_TRANSFORM_LENGTHS;
            private static NTTFiniteFieldInfo[,] NTT_FIELD_INFO_OBJECTS;

            /// <summary>
            /// This method prepares the Helper class for fast NTT multiplication
            /// by loading finite field information from the resource file.
            /// 
            /// This can take a few seconds of time, so consider calling this method in advance.
            /// </summary>
            public static void PrepareNTTMultiplication()
            {
                // If already prepared, return at once.
                // -
                if (NTT_PREPARED)
                {
                    return;
                }

                // Pattern: 
                // BASE N P ROOT \r\n
                // -
                Regex pattern = new Regex(@"(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\r\n");

                Dictionary<NTTTransformInfo, NTTFiniteFieldInfo> fieldInfoDictionary =
                    new Dictionary<NTTTransformInfo, NTTFiniteFieldInfo>();

                SortedSet<int> acquiredBases = new SortedSet<int>();
                SortedSet<int> acquiredTransformLengths = new SortedSet<int>();

                foreach (Match match in pattern.Matches(whiteMath.Properties.Resources.NTTPrimeModuli))
                {
                    int currentNumericBase             = int.Parse(match.Groups[1].Value);
                    int currentTransformLength         = int.Parse(match.Groups[2].Value);

                    BigInteger currentPrimeModulus     = BigInteger.Parse(match.Groups[3].Value);
                    BigInteger currentRootOfUnity      = BigInteger.Parse(match.Groups[4].Value);

                    acquiredBases.Add(currentNumericBase);
                    acquiredTransformLengths.Add(currentTransformLength);

                    fieldInfoDictionary.Add(
                        new NTTTransformInfo(currentNumericBase, currentTransformLength),
                        new NTTFiniteFieldInfo(currentPrimeModulus, currentRootOfUnity));
                }

                // Convert the sets to arrays.
                // -
                LongInt<B>.Helper.NTT_BASES = acquiredBases.ToArray();
                LongInt<B>.Helper.NTT_TRANSFORM_LENGTHS = acquiredTransformLengths.ToArray();

                // Now fill the 2D array with appropriate bases and transform lengths.
                // -
                LongInt<B>.Helper.NTT_FIELD_INFO_OBJECTS =
                    new NTTFiniteFieldInfo[NTT_BASES.Length, NTT_TRANSFORM_LENGTHS.Length];

                // Associate the finite field information with bases and transform lengths.
                // -
                for (int baseIndex = 0; baseIndex < LongInt<B>.Helper.NTT_BASES.Length; ++baseIndex)
                {
                    for (
                        int transformLengthIndex = 0; 
                        transformLengthIndex < LongInt<B>.Helper.NTT_TRANSFORM_LENGTHS.Length; 
                        ++transformLengthIndex)
                    {
                        LongInt<B>.Helper.NTT_FIELD_INFO_OBJECTS[baseIndex, transformLengthIndex] =
                            fieldInfoDictionary[
                                new NTTTransformInfo(NTT_BASES[baseIndex], NTT_TRANSFORM_LENGTHS[transformLengthIndex])];
                    }
                }

                // Set flag that everything's okay.
                // -
                LongInt<B>.Helper.NTT_PREPARED = true;
            }

            /// <summary>
            /// Returns the "minimal" finite field info for a given pair of values for BASE and transform length.
            /// </summary>
            private static NTTFiniteFieldInfo getFiniteFieldInfo(int BASE, int transformLength)
            {
                // If the multiplication field information is unprepared, we should 
                // do it now.
                // -
                if (!LongInt<B>.Helper.NTT_PREPARED)
                {
                    LongInt<B>.Helper.PrepareNTTMultiplication();
                }

                // Do binary search of minimal BASE more or equal to the desired in
                // the array NTT_BASES
                // - 
                int baseIndex = Array.BinarySearch(LongInt<B>.Helper.NTT_BASES, BASE);

                if (baseIndex < 0)
                {
                    baseIndex = ~baseIndex;

                    // If the base is too big, we can't help.
                    // -
                    if (baseIndex >= LongInt<B>.Helper.NTT_BASES.Length)
                    {
                        return null;
                    }
                }
                
                // Do binary search of the EXACT transform length.
                // - 
                int transformLengthIndex = Array.BinarySearch(LongInt<B>.Helper.NTT_TRANSFORM_LENGTHS, transformLength);

                // If no such transform length, then it is impossible to perform NTT multiplication of such numbers.
                // -
                if (transformLengthIndex < 0)
                {
                    return null;
                }

                // Everything is okay otherwise.
                // -
                return NTT_FIELD_INFO_OBJECTS[baseIndex, transformLengthIndex];
            }

            /// <summary>
            /// Computes the product of two long integer numbers using NTT in finite fields.
            /// 
            /// The numbers are not necessarily required to be of two's power long;
            /// this algorithm performs all needed operations automatically.
            /// </summary>
            /// <param name="one"></param>
            /// <param name="two"></param>
            public static LongInt<B> MultiplyNTT(LongInt<B> one, LongInt<B> two)
            {
				Condition.ValidateNotNull(one, nameof(one));
				Condition.ValidateNotNull(two, nameof(two));

                LongInt<B> result = new LongInt<B>(one.Length + two.Length);                
                long[] longResult = new long[one.Length + two.Length];

                MultiplyNTT(LongInt<B>.BASE, longResult, one.Digits, two.Digits);

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
            public static void MultiplyNTT(int BASE, IList<long> result, IList<int> one, IList<int> two)
            {
				Condition.ValidateNotNull(one, nameof(one));
				Condition.ValidateNotNull(two, nameof(two));
				Condition.ValidateNotNull(result, nameof(result));
                
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
                NTTFiniteFieldInfo finiteFieldInfo = 
                    LongInt<B>.Helper.getFiniteFieldInfo(BASE, transformLength);

                // Prepare the result storage.
                // - 
                BigInteger[] bigIntegerResult = new BigInteger[transformLength];

                // Squaring one operand saves us one FFT.
                // -
                if (object.ReferenceEquals(one, two))
                {
                    BigInteger[] op = new BigInteger[transformLength];

                    for (int i = 0; i < maxLength; i++)
                        op[i] = one[i];

                    BigInteger[] nttResult = Recursive_NTT(op, finiteFieldInfo);
                    BigInteger[] componentWiseProduct = componentMultiplyGalois(nttResult, nttResult, finiteFieldInfo);

                    bigIntegerResult = Recursive_NTT_Inverse(componentWiseProduct, finiteFieldInfo);
                }
                // If operands differ, nothing can be done.
                // -
                else
                {
                    BigInteger[] op1 = new BigInteger[transformLength];
                    BigInteger[] op2 = new BigInteger[transformLength];
                    
                    for (int i = 0; i < maxLength; i++)
                    {
                        if (i < one.Count) op1[i] = one[i];      
                        if (i < two.Count) op2[i] = two[i];      
                    }

                    BigInteger[] nttResultOne = Recursive_NTT(op1, finiteFieldInfo);
                    BigInteger[] nttResultTwo = Recursive_NTT(op2, finiteFieldInfo);
                    BigInteger[] componentWiseProduct = componentMultiplyGalois(nttResultOne, nttResultTwo, finiteFieldInfo);

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
            internal static BigInteger[] Recursive_NTT(
                IList<BigInteger> coefficients, 
                NTTFiniteFieldInfo finiteFieldInfo)
            {
                int n = coefficients.Count;

                BigInteger[] rootsHalf = rootsOfUnityHalfGalois(finiteFieldInfo, n, false);
   
                return 
                    Recursive_NTT_Skeleton(coefficients, rootsHalf, finiteFieldInfo, 1, 0);
            }

            /// <summary>
            /// BACK FFT
            /// </summary>
            /// <param name="coefficients"></param>
            /// <returns></returns>
            internal static BigInteger[] Recursive_NTT_Inverse(
                IList<BigInteger> coefficients,
                NTTFiniteFieldInfo finiteFieldInfo)
            {
                int n = coefficients.Count;

                BigInteger[] rootsHalf = rootsOfUnityHalfGalois(finiteFieldInfo, n, true);
                BigInteger[] result = Recursive_NTT_Skeleton(coefficients, rootsHalf, finiteFieldInfo, 1, 0);

                // We should multiply the result by the multiplicative inverse of length 
                // in contrast to the forward-NTT.
                // -
                BigInteger lengthMultiplicativeInverse =
                    WhiteMath<BigInteger, CalcBigInteger>.MultiplicativeInverse(n, finiteFieldInfo.primeModulus);

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
            private static BigInteger[] Recursive_NTT_Skeleton(
                IList<BigInteger> coefficients, 
                IList<BigInteger> rootsOfUnity, 
                NTTFiniteFieldInfo finiteFieldInfo,
                int step, 
                int offset)
            {
                // Calculate the length of vectors at the current step of recursion.
                // -
                int n = coefficients.Count / step - offset / step;

                if (n == 1)
                {
                    return new BigInteger[] { coefficients[offset] };
                }
                
                BigInteger[] results = new BigInteger[n];

                IList<BigInteger> resultEvens = 
                    Recursive_NTT_Skeleton(coefficients, rootsOfUnity, finiteFieldInfo, step * 2, offset);
                
                IList<BigInteger> resultOdds = 
                    Recursive_NTT_Skeleton(coefficients, rootsOfUnity, finiteFieldInfo, step * 2, offset + step);

                for (int k = 0; k < n / 2; k++)
                {
                    BigInteger bfly = (rootsOfUnity[k * step] * resultOdds[k]) % finiteFieldInfo.primeModulus;
                    
                    results[k]          = (resultEvens[k] + bfly) % finiteFieldInfo.primeModulus;
                    results[k + n / 2]  = (resultEvens[k] - bfly) % finiteFieldInfo.primeModulus;

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
            /// Returns the first half of the [rootDegree]th roots of unity series in the BigInteger field.
            /// Used in the recursive FFT algorithm in LongInt.Helper class.
            /// 
            /// Root degree should be an exact power of two.
            /// </summary>
            /// <param name="rootDegree"></param>
            /// <returns></returns>
            internal static BigInteger[] rootsOfUnityHalfGalois(
                NTTFiniteFieldInfo finiteFieldInfo, 
                int rootDegree, 
                bool inverted)
            {
				Condition
					.Validate(rootDegree > 0)
					.OrArgumentOutOfRangeException("The degree of the root should be a positive power of two.");                
                
				/*
				Contract.Ensures(
                    Contract.ForAll(
                        Contract.Result<BigInteger[]>(), 
                        (x => WhiteMath<BigInteger, CalcBigInteger>.PowerIntegerModular(x, (ulong)rootDegree, finiteFieldInfo.primeModulus) == 1)));
				*/

                BigInteger[] result = new BigInteger[rootDegree / 2];

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
                    result[i] = result[i-1] * finiteFieldInfo.rootOfUnity % finiteFieldInfo.primeModulus;
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
            internal static BigInteger[] componentMultiplyGalois(
                IList<BigInteger> one, 
                IList<BigInteger> two, 
                NTTFiniteFieldInfo finiteFieldInfo)
            {
                BigInteger[] result = new BigInteger[one.Count];

                for (int i = 0; i < one.Count; i++)
                {
                    result[i] = (one[i] * two[i]) % finiteFieldInfo.primeModulus;
                }

                return result;
            }

#if(DEBUG_1)
            /// <summary>
            /// For debug purposes. Performs the NTT in a straightforward (quadratic) manner.
            /// </summary>
            internal static BigInteger[] __DB_Quadratic_NTT(IList<BigInteger> coefficients, bool back = false)
            {
                BigInteger[] rootsOfUnity = __DB__rootsOfUnityGalois(coefficients.Count, back);
                BigInteger[] result = new BigInteger[coefficients.Count];

                for (int i = 0; i < rootsOfUnity.Length; ++i)
                {
                    BigInteger currentRoot = rootsOfUnity[i];
                    
                    BigInteger hornerResult = 0;

                    for (int j = coefficients.Count - 1; j >= 0; --j)
                    {
                        hornerResult = (hornerResult * currentRoot + coefficients[j]) % NTT_MODULUS;
                    }

                    result[i] =
                        hornerResult *
                        (back ? WhiteMath<BigInteger, CalcBigInteger>.MultiplicativeInverse(coefficients.Count, NTT_MODULUS) : 1) % NTT_MODULUS;
                }

                return result;
            }

            /// <summary>
            /// Returns all the [rootDegree]th roots of unity series in the BigInteger field.
            /// Used in the recursive FFT algorithm in LongInt.Helper class.
            /// 
            /// Root degree should be an exact power of two.
            /// </summary>
            public static BigInteger[] __DB__rootsOfUnityGalois(int rootDegree, bool inverted)
            {
                Contract.Requires<ArgumentOutOfRangeException>(rootDegree > 0, "The degree of the root should be a positive power of two.");
                Contract.Ensures(
                    Contract.ForAll(
                        Contract.Result<BigInteger[]>(),
                        (x => WhiteMath<BigInteger, CalcBigInteger>.PowerIntegerModular(x, (ulong)rootDegree, NTT_MODULUS) == 1)));

                BigInteger[] result = new BigInteger[rootDegree];

                // The first root of unity is obviously one.
                // -
                result[0] = 1;

                // Now we will learn what k it is in rootDegree == 2^k
                // -
                int rootDegreeCopy = rootDegree;
                int rootDegreeExponent = 0;

                while (rootDegreeCopy > 1)
                {
                    rootDegreeCopy /= 2;
                    ++rootDegreeExponent;
                }

                // Now we obtain the principal 2^rootDegreeExponent-th root of unity.
                // -
                BigInteger principalRoot = WhiteMath<BigInteger, CalcBigInteger>.PowerIntegerModular(
                    NTT_MAX_ROOT_OF_UNITY_2_30,
                    WhiteMath<ulong, CalcULong>.PowerInteger(2, NTT_MAX_ROOT_OF_UNITY_DEGREE_EXPONENT - rootDegreeExponent),
                    NTT_MODULUS);

                for (int i = 1; i < rootDegree; i++)
                {
                    // All the desired roots of unity are obtained as powers of the principal root. 
                    // -
                    result[i] = result[i - 1] * principalRoot % NTT_MODULUS;
                }

                if (inverted)
                {
                    for (int i = 0; i < rootDegree; ++i)
                    {
                        result[i] = WhiteMath<BigInteger, CalcBigInteger>.MultiplicativeInverse(result[i], NTT_MODULUS);
                    }
                }

                return result;
            }
#endif

        }
    }
}