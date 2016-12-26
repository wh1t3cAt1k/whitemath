using System;
using System.Collections.Generic;

namespace WhiteMath.ArithmeticLong
{
    public partial class LongInt<B> where B : IBase, new()
    {
        public static partial class Helper
        {
			public static LongInt<B> MultiplyFFTComplex(
				LongInt<B> first, 
				LongInt<B> second)
            {
				double _;
				long __;

                return MultiplyFFTComplex(first, second, out _, out _, out __);
            }

            public static LongInt<B> MultiplyFFTComplex(
				LongInt<B> one, 
				LongInt<B> two, 
				out double maxRoundError, 
				out double maxImaginaryPart, 
				out long maxLong)
            {
				LongInt<B> result = new LongInt<B>(one.Length + two.Length);
                result.IsNegative = one.IsNegative ^ two.IsNegative;

                long[] resultOverflowProne = new long[one.Length + two.Length];
                
				MultiplyFFTComplex(
					BASE, 
					resultOverflowProne, 
					one.Digits, 
					two.Digits, 
					out maxRoundError, 
					out maxImaginaryPart, 
					out maxLong);

				for (int i = 0; i < resultOverflowProne.Length; ++i)
				{
					result.Digits.Add((int)resultOverflowProne[i]);
				}

                result.DealWithZeroes();
                return result;
            }

            /// <summary>
            /// Computes the product of two long integer numbers using Complex-field FFT algorithm.
            /// Precision is not guaranteed on very large numbers.
            /// 
            /// The numbers are not necessarily required to be of two's power long;
            /// this algorithm performs all needed operations automatically.
            /// </summary>
            /// <param name="result">The result of FFT multiplication.</param>
            /// <param name="first">The first operand.</param>
            /// <param name="second">The second operand.</param>
            public static void MultiplyFFTComplex(
				int BASE, 
				IList<long> result, 
				IList<int> first, 
				IList<int> second, 
				out double maxRoundError, 
				out double maxComplexPart, 
				out long maxLongCoefficient)
            {
                // Initialize risk indicators.
                // -
                maxRoundError = 0;
                maxComplexPart = 0;
                maxLongCoefficient = 0;

                int maxLength = Math.Max(first.Count, second.Count);

                // Search for the minimal power of two bigger than both lengths of
                // the operands.
                // -
                int transformLength = 1;

				while (transformLength < maxLength)
				{
					transformLength *= 2;
				}

                // Multiply by two again, for unambigous interpolation of the product.
                // - 
                transformLength *= 2;

                // Prepare the result storage.
                // - 
                Complex[] complexResult = new Complex[transformLength];

                // Squaring one operand saves us one FFT.
                // -
                if (object.ReferenceEquals(first, second))
                {
                    Complex[] operand = new Complex[transformLength];

                    // Copy the digits into a Complex vector.
                    // -
                    for (int i = 0; i < maxLength; i++)
                    {
                        operand[i] = first[i];
                    }

                    Complex[] operandFFT = RecursiveFFT(operand);
                    complexResult = RecursiveFFTInverse(ComponentWiseMultiply(operandFFT, operandFFT));
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
                        if (i < first.Count) op1[i] = first[i];
                        if (i < second.Count) op2[i] = second[i];
                    }

                    complexResult = RecursiveFFTInverse(ComponentWiseMultiply(RecursiveFFT(op1), RecursiveFFT(op2)));
                }

                // Convert each coefficient to long, compute the risk functions.
                // -
                for (int i = 0; i < result.Count; i++)
                {
					double coefficient = (long)(complexResult[i].RealCounterPart + 0.5);

					double roundingError = Math.Abs(complexResult[i].RealCounterPart - coefficient);

					if (roundingError > maxRoundError)
					{
						maxRoundError = roundingError;
					}

					if (complexResult[i].ImaginaryCounterPart > maxComplexPart)
					{
						maxComplexPart = complexResult[i].ImaginaryCounterPart;
					}

                    result[i] = (long)coefficient;

					if (result[i] > maxLongCoefficient)
					{
						maxLongCoefficient = result[i];
					}
                }

                // And perform all the carries.
                // -
                PerformCarries(BASE, result);
            }

            /// <summary>
            /// Computes the Fast Fourier Transform of the <paramref name="coefficients"/>
            /// vector.
            /// </summary>
            /// <param name="coefficients">The vector of coefficients of the polynomial, length=2^k.</param>
            /// <returns>A vector containing the discrete fourier transform.</returns>
			public static Complex[] RecursiveFFT(IList<Complex> coefficients)
            {
				int transformLength = coefficients.Count;

                if (transformLength == 1)
                {
                    return new [] { coefficients[0] };
                }
                else if (transformLength == 2)
                {
                    return new [] 
					{ 
						coefficients[0] + coefficients[1], 
						coefficients[0] - coefficients[1] 
					};
                }

                Complex[] rootsHalf = RootsOfUnityHalf(transformLength, false);

                return RecursiveFFTSkeleton(coefficients, rootsHalf, 1, 0);
            }

            /// <summary>
            /// Computes the inverse Fast Fourier Transform of the <paramref name="coefficients"/>
            /// vector.
            /// </summary>
            /// <param name="coefficients">The vector of coefficients of the polynomial, length=2^k.</param>
            /// <returns>A vector containing the inverse discrete fourier transform.</returns>
			public static Complex[] RecursiveFFTInverse(IList<Complex> coefficients)
            {
                int transformLength = coefficients.Count;

                Complex[] result;

                if (transformLength == 1)
                {
                    result = new [] { coefficients[0] };
                }
                else if (transformLength == 2)
                {
                    result = new [] 
					{ 
						coefficients[0] + coefficients[1], 
						coefficients[0] - coefficients[1] 
					};
                }
                else
                {
                    Complex[] rootsHalf = RootsOfUnityHalf(transformLength, true);
                    result = RecursiveFFTSkeleton(coefficients, rootsHalf, 1, 0);
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
			private static Complex[] RecursiveFFTSkeleton(
				IList<Complex> coefficients, 
				IList<Complex> rootsOfUnity, 
				int step, 
				int offset)
            {
                int n = coefficients.Count / step - offset / step;

                if (n == 4)
                {
                    Complex a = coefficients[offset];
                    Complex b = coefficients[offset + step];
                    Complex c = coefficients[offset + 2 * step];
                    Complex d = coefficients[offset + 3 * step];

                    Complex i = rootsOfUnity[coefficients.Count / 4];

                    return new [] 
					{ 
						a + c + b + d, 
						a - c + i * (b - d), 
						a + c - b - d, 
						a - c - i * (b - d) 
					};
                }

                Complex[] results = new Complex[n];

                IList<Complex> resultEvens = RecursiveFFTSkeleton(coefficients, rootsOfUnity, step * 2, offset);
                IList<Complex> resultOdds = RecursiveFFTSkeleton(coefficients, rootsOfUnity, step * 2, offset + step);

                for (int m = 0; m < n / 2; m++)
                {
					Complex butterfly = rootsOfUnity[m * step] * resultOdds[m];
                    results[m] = resultEvens[m] + butterfly;
                    results[m + n / 2] = resultEvens[m] - butterfly;
                }

                return results;
            }

            /// <summary>
            /// Returns the upper half of the [rootDegree]th roots of unity series in the complex field.
            /// Used in the recursive FFT algorithm in LongInt.Helper class.
            /// Root degree should be an exact power of two.
            /// </summary>
			private static Complex[] RootsOfUnityHalf(int rootDegree, bool isInverse)
            {
				Complex[] result = new Complex[rootDegree / 2];

                result[0] = 1.0;

                for (int i = 1; i < rootDegree / 2; i++)
                {
                    double argument = 2.0 * Math.PI * i / rootDegree;

                    result[i] = new Complex(Math.Cos(argument), Math.Sin(argument));

                    if (isInverse)
                    {
                        result[i] = 1.0 / result[i];
                    }
                }

                return result;
            }

            /// <summary>
            /// Performs the component-to-component multiplication of two convolution vectors.
            /// </summary>
            /// <param name="first"></param>
            /// <param name="second"></param>
            /// <returns></returns>
			private static Complex[] ComponentWiseMultiply(
				IList<Complex> first, 
				IList<Complex> second)
            {
				Complex[] result = new Complex[first.Count];

				for (int i = 0; i < first.Count; ++i)
				{
					result[i] = first[i] * second[i];
				}

                return result;
            }

            /// <summary>
            /// Performs the carrying after the FFT multiplication.
            /// </summary>
			internal static void PerformCarries(int digitBase, IList<long> digits)
            {
                long carry = 0;

                for (int i = 0; i < digits.Count; i++)
                {
                    digits[i] += carry;

                    carry = digits[i] / digitBase;
                    digits[i] %= digitBase;
                }
            }
        }
    }
}