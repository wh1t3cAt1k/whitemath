using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using whiteMath.General;

namespace whiteMath.ArithmeticLong
{
    public partial class LongInt<B> where B : IBase, new()
    {
        public static partial class Helper
        {
            public static LongInt<B> MultiplyFFTComplex(LongInt<B> one, LongInt<B> two)
            {
                double junk;        // не хотите использовать - не надо!
                long junky;         // ну, что поделать...

                return MultiplyFFTComplex(one, two, out junk, out junk, out junky);
            }

            public static LongInt<B> MultiplyFFTComplex(LongInt<B> one, LongInt<B> two, out double maxRoundError, out double maxImaginaryPart, out long maxLong)
            {
                LongInt<B> res = new LongInt<B>(one.Length + two.Length);
                res.Negative = one.Negative ^ two.Negative;

                long[] resultOverflowProne = new long[one.Length + two.Length];
                MultiplyFFTComplex(LongInt<B>.BASE, resultOverflowProne, one.Digits, two.Digits, out maxRoundError, out maxImaginaryPart, out maxLong);

                for (int i = 0; i < resultOverflowProne.Length; i++)
                    res.Digits.Add((int)resultOverflowProne[i]);

                res.DealWithZeroes();
                return res;
            }

            /// <summary>
            /// Computes the product of two long integer numbers using Complex-field FFT algorithm.
            /// Precision is not guaranteed on very large numbers.
            /// 
            /// The numbers are not necessarily required to be of two's power long;
            /// this algorithm performs all needed operations automatically.
            /// </summary>
            /// <param name="result">The result of FFT multiplication.</param>
            /// <param name="one">The first operand.</param>
            /// <param name="two">The second operand.</param>
            public static void MultiplyFFTComplex(int BASE, IList<long> result, IList<int> one, IList<int> two, out double maxRoundError, out double maxComplexPart, out long maxLongCoefficient)
            {
                // Initialize risk indicators
                // -
                maxRoundError = 0;
                maxComplexPart = 0;
                maxLongCoefficient = 0;

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

                // Prepare the result storage.
                // - 
                Complex[] complexResult = new Complex[transformLength];

                // Squaring one operand saves us one FFT.
                // -
                if (object.ReferenceEquals(one, two))
                {
                    Complex[] operand = new Complex[transformLength];

                    // Copy the digits into a Complex vector.
                    // -
                    for (int i = 0; i < maxLength; i++)
                    {
                        operand[i] = one[i];
                    }

                    Complex[] operandFFT = Recursive_FFT(operand);
                    complexResult = Recursive_FFT_Back(componentMultiply(operandFFT, operandFFT));
                }
                // If operands differ, nothing can be done.
                // -
                else
                {
                    Complex[] op1 = new Complex[transformLength];
                    Complex[] op2 = new Complex[transformLength];

                    // Copy the digits into Complex vectors.
                    // -
                    for (int i = 0; i < maxLength; i++)
                    {
                        if (i < one.Count) op1[i] = one[i];
                        if (i < two.Count) op2[i] = two[i];
                    }

                    complexResult = Recursive_FFT_Back(componentMultiply(Recursive_FFT(op1), Recursive_FFT(op2)));
                }

                // Convert each coefficient to long, compute the risk functions.
                // -
                for (int i = 0; i < result.Count; i++)
                {
                    double tmp = (long)(complexResult[i].RealCounterPart + 0.5);

                    double err = complexResult[i].RealCounterPart - tmp;
                    err *= (err > 0 ? 1 : -1);

                    if (err > maxRoundError)
                        maxRoundError = err;
                    if (complexResult[i].ImaginaryCounterPart > maxComplexPart)
                        maxComplexPart = complexResult[i].ImaginaryCounterPart;

                    result[i] = (long)tmp;
                    if (result[i] > maxLongCoefficient) maxLongCoefficient = result[i];
                }

                // And perform all the carries.
                // -
                performCarry(BASE, result);
            }

            /// <summary>
            /// Computes the Fast Fourier Transform of the <paramref name="coefficients"/>
            /// vector.
            /// </summary>
            /// <param name="coefficients">The vector of coefficients of the polynomial, length=2^k.</param>
            /// <returns>A vector containing the discrete fourier transform.</returns>
            public static Complex[] Recursive_FFT(IList<Complex> coefficients)
            {
                int n = coefficients.Count;

                // ----------------- Cutting off lowest dimensions.

                if (n == 1)
                {
                    return new Complex[] { coefficients[0] };
                }
                else if (n == 2)
                {
                    return new Complex[] { coefficients[0] + coefficients[1], coefficients[0] - coefficients[1] };
                }

                Complex[] rootsHalf = rootsOfUnityHalf(n, false);

                return 
                    Recursive_FFT_Skeleton(coefficients, rootsHalf, 1, 0);
            }

            /// <summary>
            /// Computes the inverse Fast Fourier Transform of the <paramref name="coefficients"/>
            /// vector.
            /// </summary>
            /// <param name="coefficients">The vector of coefficients of the polynomial, length=2^k.</param>
            /// <returns>A vector containing the inverse discrete fourier transform.</returns>
            public static Complex[] Recursive_FFT_Back(IList<Complex> coefficients)
            {
                // Name the transform length explicitly.
                // -
                int transformLength = coefficients.Count;

                // Prepare storage for the result.
                // -
                Complex[] result;

                // Cutting off lower dimensions.
                // -
                if (transformLength == 1)
                {
                    result = new Complex[] { coefficients[0] };
                }
                else if (transformLength == 2)
                {
                    result = new Complex[] { coefficients[0] + coefficients[1], coefficients[0] - coefficients[1] };
                }
                else
                {
                    Complex[] rootsHalf = rootsOfUnityHalf(transformLength, true);
                    result = Recursive_FFT_Skeleton(coefficients, rootsHalf, 1, 0);
                }

                // For the inverse FFT, we have to divide by the transform length.
                // -
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] /= transformLength;
                }

                return result;
            }

            /// <summary>
            /// Calculates the result of recursive Fast Fourier Transform.
            /// </summary>
            /// <param name="coefficients"></param>
            /// <returns></returns>
            private static Complex[] Recursive_FFT_Skeleton(IList<Complex> coefficients, IList<Complex> rootsOfUnity, int step, int offset)
            {
                int n = coefficients.Count / step - offset / step;

                if (n == 4)
                {
                    Complex a = coefficients[offset];
                    Complex b = coefficients[offset + step];
                    Complex c = coefficients[offset + 2 * step];
                    Complex d = coefficients[offset + 3 * step];

                    Complex i = rootsOfUnity[coefficients.Count / 4];

                    return new Complex[] { a + c + b + d, a - c + i * (b - d), a + c - b - d, a - c - i * (b - d) };
                }

                Complex[] results = new Complex[n];

                IList<Complex> resultEvens = Recursive_FFT_Skeleton(coefficients, rootsOfUnity, step * 2, offset);
                IList<Complex> resultOdds = Recursive_FFT_Skeleton(coefficients, rootsOfUnity, step * 2, offset + step);

                for (int m = 0; m < n / 2; m++)
                {
                    Complex bfly = rootsOfUnity[m * step] * resultOdds[m];
                    results[m] = resultEvens[m] + bfly;
                    results[m + n / 2] = resultEvens[m] - bfly;
                }

                return results;
            }

            /// <summary>
            /// Returns the upper half of the [rootDegree]th roots of unity series in the complex field.
            /// Used in the recursive FFT algorithm in LongInt.Helper class.
            /// 
            /// Root degree should be an exact power of two.
            /// </summary>
            /// <param name="rootDegree"></param>
            /// <returns></returns>
            internal static Complex[] rootsOfUnityHalf(int rootDegree, bool inverted)
            {
                Complex[] tmp = new Complex[rootDegree / 2];

                tmp[0] = 1.0;

                for (int i = 1; i < rootDegree / 2; i++)
                {
                    double argument = 2.0 * Math.PI * i / rootDegree;

                    tmp[i] = new Complex(Math.Cos(argument), Math.Sin(argument));

                    if (inverted)
                    {
                        tmp[i] = 1.0 / tmp[i];
                    }
                }

                return tmp;
            }

            /// <summary>
            /// Performs the component-to-component multiplication of two convolution vectors.
            /// </summary>
            /// <param name="one"></param>
            /// <param name="two"></param>
            /// <returns></returns>
            internal static Complex[] componentMultiply(IList<Complex> one, IList<Complex> two)
            {
                Complex[] tmp = new Complex[one.Count];

                for (int i = 0; i < one.Count; i++)
                    tmp[i] = one[i] * two[i];

                return tmp;
            }

            /// <summary>
            /// Performs the carrying after the FFT multiplication.
            /// </summary>
            /// <param name="BASE"></param>
            /// <param name="?"></param>
            internal static void performCarry(int BASE, IList<long> digits)
            {
                long carry = 0;

                for (int i = 0; i < digits.Count; i++)
                {
                    digits[i] += carry;

                    carry = digits[i] / BASE;
                    digits[i] %= BASE;
                }
            }
        }
    }
}