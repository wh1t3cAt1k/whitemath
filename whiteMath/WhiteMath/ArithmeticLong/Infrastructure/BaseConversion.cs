using System;
using System.Collections.Generic;

using WhiteMath.Mathematics;
using WhiteMath.Calculators;
using WhiteMath.General;

namespace WhiteMath.ArithmeticLong.Infrastructure
{
    /// <summary>
    /// This class provides methods for converting from one numeric base to another.
    /// </summary>
    public static class BaseConversion
    {
        /// <summary>
        /// Returns the value specifying how many digits with base <paramref name="baseTo"/> will be enough
        /// to handle <paramref name="baseFromDigitCount"/> digits with base <paramref name="baseFrom"/>.
        /// </summary>
        /// <param name="baseFrom">The base to be converted.</param>
        /// <param name="baseTo">The base to be converted to.</param>
        /// <param name="baseFromDigitCount">Amount of digits in <paramref name="baseFrom"/></param>
        /// <returns>The value specifying how many digits in <paramref name="baseTo"/> will be enough
        /// to handle the <paramref name="baseFromDigitCount"/> digits in <paramref name="baseFrom"/></returns>
		public static int GetRequiredDigitCountForConversion(
			int baseFrom, 
			int baseTo, 
			int baseFromDigitCount)
        {
            return (int)Math.Ceiling(Math.Log(baseFrom, baseTo)*baseFromDigitCount);
        }

        /// <summary>
        /// Возвращает массив различных степеней основания.
        /// </summary>
		private static int[] GetBasePowers(int @base, int maxPower)
        {
            int[] powers = new int[maxPower];

            int current = 1;

            for (int i = 0; i < maxPower; i++)
            {
                powers[i] = current;
                current *= @base;
            }

            return powers;
        }

        // -------------------------------------------------
        // --------------------- CONVERT BASE --------------
        // -------------------------------------------------

        /// <summary>
        /// Converts the long integer digit list from one numeric base to another.
        /// </summary>
        /// <param name="from">The integer list containing the digits of base <paramref name="fromBase"/></param>
        /// <param name="fromBase">The numeric base of incoming list.</param>
        /// <param name="newBase">The numeric base for the incoming list to be converted to.</param>
        /// <returns>The list containing digits of numeric base <paramref name="newBase"/>, whilst equal to the incoming list.</returns>
        public static int[] BaseConvert(IList<int> from, int fromBase, int newBase)
        {
            int[] to = new int[GetRequiredDigitCountForConversion(fromBase, newBase, from.Count)];

            BaseConvert(from, to, fromBase, newBase);

            return to.Cut();
        }

		public static void BaseConvert(IList<int> from, IList<int> to, int fromBase, int newBase)
        {
            // проверяем, не кратное ли основание

            int? power;

            if (Mathematics<int, CalcInt>.IsNaturalIntegerPowerOf(fromBase, newBase, out power) ||
                Mathematics<int, CalcInt>.IsNaturalIntegerPowerOf(newBase, fromBase, out power))
				ConvertPowered(from, to, fromBase, newBase, power.Value);
         
            // в противном случае - не избежать последовательного деления.
            
            else
            {
                int k = 0;
                IList<int> divide = from;

                while (divide.CountSignificant() > 1 || divide[0] > 0)
                {
                    int remainder;
                    divide = LongIntegerMethods.DivideByInteger(fromBase, divide, newBase, out remainder);

                    to[k++] = remainder;
                }
            }
        }

        /// <summary>
        /// Производит конвертацию из одной системы счисления в другую
        /// в том случае, если основания кратны.
        /// </summary>
		private static void ConvertPowered(
			IList<int> sourceDigits, 
			IList<int> targetDigits, 
			int sourceBase, 
			int targetBase, 
			int power)
        {
			if (sourceBase > targetBase)
			{
				// Конвертируемое основание больше того, в которое конвертируем;
				// Значит, на каждую цифру исходного числа приходится k цифр выходного.

				int k = GetRequiredDigitCountForConversion(sourceBase, targetBase, 1);

				for (int i = 0; i < sourceDigits.Count; i++)
				{
					int current = sourceDigits[i];

					for (int j = 0; j < k; j++)
					{
						targetDigits[i * k + j] = current % targetBase;
						current /= targetBase;
					}
				}
			}
			else if (sourceBase < targetBase)
			{
				// конвертируемое основание меньше того, в которое конвертируем;
				// Значит, каждые k цифр входного числа составляют только 1 цифру выходного.

				int[] powers = GetBasePowers(sourceBase, power);
				int k = GetRequiredDigitCountForConversion(targetBase, sourceBase, 1);

				for (int i = 0; i < targetDigits.Count; i++)
				{
					int sum = 0;

					for (int j = 0; j < k; j++)
					{
						int index = i * k + j;

						if (index < sourceDigits.Count)
							sum += sourceDigits[index] * powers[j];
					}

					targetDigits[i] = sum;
				}
			}
			else
			{
				ServiceMethods.Copy(sourceDigits, 0, targetDigits, 0, (sourceDigits.Count < targetDigits.Count ? sourceDigits.Count : targetDigits.Count));
			}
        }
    }
}
