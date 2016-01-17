using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using whiteMath.General;

namespace whiteMath.ArithmeticLong
{
    /// <summary>
    /// This class provides methods for converting from one numeric base to another.
    /// </summary>
    public static class BaseConversion
    {
        /// <summary>
        /// Returns the value specifying how many digits with base <paramref name="baseTo"/> will be enough
        /// to handle <paramref name="digitCount"/> digits with base <paramref name="baseFrom"/>.
        /// </summary>
        /// <param name="baseFrom">The base to be converted.</param>
        /// <param name="baseTo">The base to be converted to.</param>
        /// <param name="digitCount">Amount of digits in <paramref name="baseFrom"/></param>
        /// <returns>The value specifying how many digits in <paramref name="baseTo"/> will be enough
        /// to handle the <paramref name="digitCount"/> digits in <paramref name="baseFrom"/></returns>
        public static int getDigitEquivalent(int baseFrom, int baseTo, int digitCount)
        {
            return (int)Math.Ceiling(Math.Log(baseFrom, baseTo)*digitCount);
        }

        /// <summary>
        /// Возвращает массив различных степеней основания.
        /// </summary>
        private static int[] getBasePowers(int _base, int maxPower)
        {
            int[] powers = new int[maxPower];

            int current = 1;

            for (int i = 0; i < maxPower; i++)
            {
                powers[i] = current;
                current *= _base;
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
        public static int[] baseConvert(IList<int> from, int fromBase, int newBase)
        {
            int[] to = new int[getDigitEquivalent(fromBase, newBase, from.Count)];

            baseConvert(from, to, fromBase, newBase);

            return to.Cut();
        }

        public static void baseConvert(IList<int> from, IList<int> to, int fromBase, int newBase)
        {
            // проверяем, не кратное ли основание

            int? power;

            if (WhiteMath<int, CalcInt>.IsNaturalIntegerPowerOf(fromBase, newBase, out power) ||
                WhiteMath<int, CalcInt>.IsNaturalIntegerPowerOf(newBase, fromBase, out power))
                convertPowered(from, to, fromBase, newBase, power.Value);
         
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
        private static void convertPowered(IList<int> from, IList<int> to, int fromBase, int newBase, int power)
        {
            if (fromBase > newBase)
            {
                // Конвертируемое основание больше того, в которое конвертируем;
                // Значит, на каждую цифру исходного числа приходится k цифр выходного.

                int k = getDigitEquivalent(fromBase, newBase, 1);

                for (int i = 0; i < from.Count; i++)
                {                    
                    int current = from[i];

                    for (int j = 0; j < k; j++)
                    {
                        to[i * k + j] = current % newBase;
                        current /= newBase;
                    }
                }
            }
            else if (fromBase < newBase)
            {
                // конвертируемое основание меньше того, в которое конвертируем;
                // Значит, каждые k цифр входного числа составляют только 1 цифру выходного.

                int[] powers = getBasePowers(fromBase, power);
                int k = getDigitEquivalent(newBase, fromBase, 1);

                for (int i = 0; i < to.Count; i++)
                {
                    int sum = 0;

                    for (int j = 0; j < k; j++)
                    {
                        int index = i * k + j;

                        if (index < from.Count)
                            sum += from[index] * powers[j];
                    }

                    to[i] = sum;
                }
            }
            else
                General.ServiceMethods.Copy(from, 0, to, 0, (from.Count < to.Count ? from.Count : to.Count));
        }
    }
}
