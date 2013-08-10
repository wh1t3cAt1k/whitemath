using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Diagnostics.Contracts;

using whiteMath.General;

namespace whiteMath.ArithmeticLong
{
    public partial class LongInt<B> where B: IBase, new()
    {
        public static partial class Helper_NTT_OLD
        {
            /// <summary>
            /// Represents the exponent of two related to the root of unity
            /// <see cref="NTT_MAX_ROOT_OF_UNITY_2_30"/> 
            /// </summary>
            public const long NTT_MAX_ROOT_OF_UNITY_DEGREE_EXPONENT = 30;

            public const long NTT_MODULUS                     = 70383776563201;
            public const long NTT_MAX_ROOT_OF_UNITY_2_30      = 31696988370702;
            public const long NTT_MAX_ROOT_OF_UNITY_INVERSE   = 32091152699528;
                                                                
                                                                // 9223372036854775807 - ulong.maxValue
                                                                // -
                                                                // 1017192893919361109684428656 - product of the root and its inverse.
                                                                // ulong overflows here. So never multiply these. :-)

            public static LongInt<B> MultiplyNTT(LongInt<B> one, LongInt<B> two)
            {
                Contract.Requires<ArgumentNullException>(one != null, "one");
                Contract.Requires<ArgumentNullException>(two != null, "two");

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
            /// Computes the product of two long integer numbers using integer NTT in finite fields.
            /// 
            /// The numbers are not necessarily required to be of two's power long;
            /// this algorithm performs all needed operations automatically.
            /// </summary>
            /// <param name="result"></param>
            /// <param name="one"></param>
            /// <param name="two"></param>
            public static void MultiplyNTT(int BASE, IList<long> result, IList<int> one, IList<int> two)
            {
                Contract.Requires<ArgumentNullException>(result != null, "result");
                Contract.Requires<ArgumentNullException>(one != null, "one");
                Contract.Requires<ArgumentNullException>(two != null, "two");
                
                int maxLength = Math.Max(one.Count, two.Count);

                // Ищем минимальную степень двойки
                // -
                int twoPower = 1;
                
                while (twoPower < maxLength)
                    twoPower *= 2;

                twoPower *= 2;  // Еще раз умножаем на два, для однозначной интерполяции многочленов

                // ---------------------------------

                BigInteger[] bigIntegerResult = new BigInteger[twoPower];

                // ---------------------------------

                // Если один операнд возводится в квадрат,
                // можно сэкономить на одном преобразовании Фурье
                // -
                if (object.ReferenceEquals(one, two))
                {
                    BigInteger[] op = new BigInteger[twoPower];

                    for (int i = 0; i < maxLength; i++)
                        op[i] = one[i];

                    BigInteger[] fft = Recursive_NTT(op);
                    bigIntegerResult = Recursive_NTT_Inverse(componentMultiplyGalois(fft, fft));
                }
                // Если разные операнды
                else
                {
                    BigInteger[] op1 = new BigInteger[twoPower];
                    BigInteger[] op2 = new BigInteger[twoPower];
                    
                    for (int i = 0; i < maxLength; i++)
                    {
                        if (i < one.Count) op1[i] = one[i];      
                        if (i < two.Count) op2[i] = two[i];      
                    }

#if(DEBUG)
                    BigInteger[] ntt1 = Recursive_NTT(op1);
                    BigInteger[] ntt2 = Recursive_NTT(op2);

                    BigInteger[] ntt1_STRAIGHT = __DB_Quadratic_NTT(op1, false);
                    BigInteger[] ntt2_STRAIGHT = __DB_Quadratic_NTT(op2, false);

                    if (!ntt1.SequenceEqual(ntt1_STRAIGHT))
                    {
                        Console.WriteLine("NO1");
                    }
                    if (!ntt2.SequenceEqual(ntt2_STRAIGHT))
                    {
                        Console.WriteLine("NO2");
                    }
#endif

                    bigIntegerResult = Recursive_NTT_Inverse(componentMultiplyGalois(Recursive_NTT(op1), Recursive_NTT(op2)));
                }

                // -------------------------------------------
                // последние штрихи и осуществление переносов
                // -
                for (int i = 0; i < result.Count; i++)
                {
                    result[i] = (long)bigIntegerResult[i];
                }
                
                LongInt<B>.Helper.performCarry(BASE, result);     // осуществим переносы
                return;
            }

            /// <summary>
            /// FFT
            /// </summary>
            /// <param name="coefficients"></param>
            /// <returns></returns>
            internal static BigInteger[] Recursive_NTT(IList<BigInteger> coefficients)
            {
                int n = coefficients.Count;

                BigInteger[] rootsHalf = rootsOfUnityHalfGalois(n, false);
   
                return 
                    Recursive_NTT_Skeleton(coefficients, rootsHalf, 1, 0);
            }

            /// <summary>
            /// BACK FFT
            /// </summary>
            /// <param name="coefficients"></param>
            /// <returns></returns>
            internal static BigInteger[] Recursive_NTT_Inverse(IList<BigInteger> coefficients)
            {
                int n = coefficients.Count;

                BigInteger[] rootsHalf = rootsOfUnityHalfGalois(n, true);
                BigInteger[] result = Recursive_NTT_Skeleton(coefficients, rootsHalf, 1, 0);

                // We should multiply the result by the multiplicative inverse of length.
                // -
                BigInteger lengthMultiplicativeInverse =
                    WhiteMath<BigInteger, CalcBigInteger>.MultiplicativeInverse(n, NTT_MODULUS);

                for (int i = 0; i < result.Length; i++)
                    result[i] = (result[i] * lengthMultiplicativeInverse) % NTT_MODULUS;

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
                    Recursive_NTT_Skeleton(coefficients, rootsOfUnity, step * 2, offset);
                
                IList<BigInteger> resultOdds = 
                    Recursive_NTT_Skeleton(coefficients, rootsOfUnity, step * 2, offset + step);

                for (int k = 0; k < n / 2; k++)
                {
                    BigInteger bfly = (rootsOfUnity[k * step] * resultOdds[k]) % NTT_MODULUS;
                    
                    results[k]          = (resultEvens[k] + bfly) % NTT_MODULUS;
                    results[k + n / 2]  = (resultEvens[k] - bfly) % NTT_MODULUS;

                    // Negative numbers may appear here.
                    // -
                    while (results[k + n / 2] < 0)
                    {
                        results[k + n / 2] += NTT_MODULUS;
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
            public static BigInteger[] rootsOfUnityHalfGalois(int rootDegree, bool inverted)
            {
                Contract.Requires<ArgumentOutOfRangeException>(rootDegree > 0, "The degree of the root should be a positive power of two.");                
                Contract.Ensures(
                    Contract.ForAll(
                        Contract.Result<BigInteger[]>(), 
                        (x => WhiteMath<BigInteger, CalcBigInteger>.PowerIntegerModular(x, (ulong)rootDegree, NTT_MODULUS) == 1)));

                BigInteger[] result = new BigInteger[rootDegree / 2];

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

                // Calculate the desired roots of unity.
                // -
                for (int i = 1; i < rootDegree / 2; i++)
                {
                    // All the desired roots of unity are obtained as powers of the principal root. 
                    // -
                    result[i] = result[i-1] * principalRoot % NTT_MODULUS;
                }

                // If performing the inverse NTT, we should invert the roots.
                // -
                if (inverted)
                {
                    for (int i = 0; i < rootDegree / 2; ++i)
                    {
                        result[i] = WhiteMath<BigInteger, CalcBigInteger>.MultiplicativeInverse(result[i], NTT_MODULUS);
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
            internal static BigInteger[] componentMultiplyGalois(IList<BigInteger> one, IList<BigInteger> two)
            {
                BigInteger[] tmp = new BigInteger[one.Count];

                for (int i = 0; i < one.Count; i++)
                {
                    tmp[i] = (one[i] * two[i]) % NTT_MODULUS;
                }

                return tmp;
            }

#if(DEBUG)
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