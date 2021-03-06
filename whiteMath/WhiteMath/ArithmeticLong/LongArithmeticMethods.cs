﻿using System;
using System.Collections.Generic;
using System.Linq;

using WhiteMath.General;

using WhiteStructs.Conditions;
using WhiteStructs.Collections;

namespace WhiteMath.ArithmeticLong
{
    /// <summary>
    /// This class provides static methods for various long 
	/// integer calculation purposes.
    /// </summary>
    public static class LongIntegerMethods
    {
        /// <summary>
        /// Checks whether the specified long integer number is even.
        /// </summary>
        /// <param name="BASE">The base of digits of the <paramref name="operand"/>. If even, the checking takes O(1) time. If odd, the time is O(n).</param>
        /// <param name="operand">A list which contains the digits of the long integer modulo <paramref name="BASE"/>, starting with the least significant digit.</param>
        /// <returns>True if the long integer number defined by <paramref name="operand"/> is even, false otherwise.</returns>
        public static bool IsEven(int BASE, IReadOnlyList<int> operand)
        {
			Condition.Validate(BASE > 1).OrArgumentOutOfRangeException(Messages.DigitBaseNeedsToBeBiggerThanOne);
			Condition.ValidateNotNull(operand, nameof(operand));
			Condition.ValidateNotEmpty(operand, Messages.DigitArrayNeedsToHaveAtLeastOneDigit);

			if (BASE % 2 == 0)
			{
				return operand[0] % 2 == 0;
			}
            else
            {
                // Можно считать, что существует "виртуальная нулевая цифра".
                // Она четна.

                // Потом мы как будто накапливаем сумму с произведениями вида
                // a_i * BASE^i.  Так как BASE^i всегда нечетная при нечетном BASE,
                // то этот член четен, если a_i четно.

                // На каждом шаге к текущей сумме и текущему члену применяем правило:

                // Чет + Нечет = Нечет.
                // Чет + Чет = Чет.
                // Нечет + Нечет = Чет.

                bool currentSumEven = true;

                for (int i = 0; i < operand.Count; ++i)
                {
                    currentSumEven = (currentSumEven == (operand[i] % 2 == 0));
                }

                return currentSumEven;
            }
        }

        /// <summary>
        /// Checks whether the specified long integer number is divisible by five.
        /// </summary>
        /// <param name="BASE">The base of digits of the <paramref name="operand"/>. If divisible by five, the checking takes O(1) time. If not, the time is O(n).</param>
        /// <param name="operand">A list which contains the digits of the long integer modulo <paramref name="BASE"/>, starting with the least significant digit.</param>
        /// <returns>True if the long integer number defined by <paramref name="operand"/> is divisible by five, false otherwise.</returns>
        public static bool IsDivisibleByFive(int BASE, IReadOnlyList<int> operand)
        {
			Condition.Validate(BASE > 1).OrArgumentOutOfRangeException(Messages.DigitBaseNeedsToBeBiggerThanOne);
			Condition.ValidateNotNull(operand, nameof(operand));
			Condition.ValidateNotEmpty(operand, Messages.DigitArrayNeedsToHaveAtLeastOneDigit);

            if (BASE % 5 == 0)
                return operand[0] % 5 == 0;
            else
            {
                int lastBaseDigit = BASE % 10;

				// We work with series of remainders of BASE divided by five.
				// -
				int[] remainderSeries;

                switch (lastBaseDigit)
                {
                    case 0:
                    case 5: remainderSeries = new int[] { 0 }; break;

                    case 1:
                    case 6: remainderSeries = new int[] { 1 }; break;

                    case 2:
                    case 7: remainderSeries = new int[] { 2, 4, 3, 1 }; break;

                    case 3:
                    case 8: remainderSeries = new int[] { 3, 4, 2, 1 }; break;

                    case 4:
                    case 9: remainderSeries = new int[] { 4, 1 }; break;

                    default: throw new InvalidProgramException();
                }

                int currentRemainder = operand[0] % 5;

                for (int i = 1; i < operand.Count; ++i)
                {
                    currentRemainder = (currentRemainder + operand[i] * remainderSeries[(i - 1) % remainderSeries.Length] % 5) % 5;
                }

                return (currentRemainder % 5 == 0);
            }
        }

        /// <summary>
        /// Checks whether one natural long integer number is bigger than another.
        /// Digit arrays can contain leading zeroes, digits should be of the same 
		/// base for both parameters. Need not specify BASE.
        /// </summary>
        /// <param name="one">The first long integer digits array.</param>
        /// <param name="two">The second long integer digits array.</param>
        /// <param name="equals">The out argument showing if the second number is actually equal to the first one.</param>
        /// <returns></returns>
        public static bool GreaterThan(IReadOnlyList<int> one, IReadOnlyList<int> two, out bool equals)
        {
			Condition.ValidateNotNull(one, nameof(one));
			Condition.ValidateNotNull(two, nameof(two));

            equals = true;

            int i;

            int smallerLength = (one.Count < two.Count ? one.Count : two.Count);
            int biggerLength = (one.Count < two.Count ? two.Count : one.Count);

            for (i = biggerLength - 1; i >= smallerLength; i--)
            {
                // Если первое число длиннее другого, идем с конца, как только не ноль - точно больше.
                // -
                if (one.Count == biggerLength && one[i] != 0)
                {
                    equals = false;
                    return true;
                }

                // Если второе число длиннее другого, идем с конца, как только не ноль - точно меньше.
                // -
                else if (two.Count == biggerLength && two[i] != 0)
                {
                    equals = false;
                    return false;
                }
            }

            for (; i >= 0; i--)
            {
                if (one[i] > two[i])
                {
                    equals = false;
                    return true;
                }
                else if (one[i] < two[i])
                {
                    equals = false;
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether the two natural long integers store the same value.
        /// Digits array can contain leading zeroes, digits should be of the same BASE. (need not specify)
        /// </summary>
        public static bool Equals(IReadOnlyList<int> one, IReadOnlyList<int> two)
        {
			Condition.ValidateNotNull(one, nameof(one));
			Condition.ValidateNotNull(two, nameof(two));

            int i;

            int smallerLength = (one.Count < two.Count ? one.Count : two.Count);
            int biggerLength = (one.Count < two.Count ? two.Count : one.Count);

            // Длины чисел не равны, если в большем встретится нечто, что не ноль - числа точно не равны.
            
            for (i = biggerLength - 1; i >= smallerLength; i--)
                if (one.Count == biggerLength && one[i] != 0 || two[i] != 0) 
                    return false;

            // Теперь проверяем поцифирькно

            for (; i >= 0; i--)
                if (one[i] != two[i]) 
                    return false;

            return true;
        }

        // -------------------------------------
        // ---------------- SUM ----------------
        // -------------------------------------

        /// <summary>
        /// Returns the sum of several long integer numbers.
        /// </summary>
        /// <param name="BASE">The base of digits in numbers (ex.: 10, 100, 1000)</param>
        /// <param name="sumOperands">An array of sum operands.</param>
        /// <returns>The digit array containing the sum and NO trailing zeroes.</returns>
		public static int[] Sum(int BASE, IList<int>[] sumOperands)
        {
			Condition.Validate(BASE > 1).OrArgumentOutOfRangeException(Messages.DigitBaseNeedsToBeBiggerThanOne);
			Condition.ValidateNotNull(sumOperands, nameof(sumOperands));
			Condition.Validate(sumOperands.Count() > 1).OrArgumentException(Messages.MoreThanOneOperandRequired);

            int n = sumOperands.Max(delegate(IList<int> operand) { return operand.Count; });
			int m = sumOperands.Length;

            int[] result = new int[n + (int)Math.Ceiling(Math.Log(m, BASE))];

            Sum(BASE, result, sumOperands);

            return result.Cut();
        }

        /// <summary>
        /// Calculates the sum of several long integer numbers.
        /// </summary>
        /// <remarks>
        /// WARNING: if the length of the largest operand is N, and there are M operands, 
        /// the result should be safe to store at least N + ceil(log(M, BASE)) digits. Note that no checking 
        /// is performed, thus the operation may result in a loss.
        /// </remarks>
        /// <param name="result">The result digits array. Should be safe to store at least N + ceil(log(M, BASE)) digits.</param>
        /// <param name="BASE">The base of digits in numbers (ex.: 10, 100, 1000)</param>
        public static void Sum(int BASE, IList<int> result, params IList<int>[] sumOperands)
        {
			Condition.Validate(BASE > 1).OrArgumentOutOfRangeException(Messages.DigitBaseNeedsToBeBiggerThanOne);
			Condition.ValidateNotNull(result, nameof(result));
			Condition.ValidateNotNull(sumOperands, nameof(sumOperands));
			Condition.Validate(sumOperands.Length > 1).OrArgumentException(Messages.MoreThanOneOperandRequired);

            int i;
            long tmp;
            int carry = 0;

            for (i = 0; i < result.Count; i++)
            {
                tmp = carry;
                foreach (IList<int> op in sumOperands)
                    if (i < op.Count)
                        tmp += op[i];

                result[i] = (int)(tmp % BASE);
                carry = (int)(tmp / BASE);
            }
        }

        /// <summary>
        /// Returns the sum of two long integer numbers, each shifted
        /// left by certain amounts of BASE-based digits.
        /// </summary>
        /// <param name="BASE">The base of digits in the numbers.</param>
        /// <param name="operand1">The digits list of the first number.</param>
        /// <param name="shift1">A positive amount of digits for the first number to shift left by.</param>
        /// <param name="operand2">The digits list of the second number.</param>
        /// <param name="shift2">A positive amount of digits for the second number to shift left by.</param>
        /// <returns>The digits array of the result containing only significant digits.</returns>
        public static int[] SumShifted(int BASE, IList<int> operand1, int shift1, IList<int> operand2, int shift2)
        {
			Condition
				.Validate(shift1 >= 0)
				.OrArgumentOutOfRangeException(Messages.CannotShiftByNegativeNumberOfDigits);
			Condition
				.Validate(shift2 >= 0)
				.OrArgumentOutOfRangeException(Messages.CannotShiftByNegativeNumberOfDigits);
			Condition.ValidateNotNull(operand1, nameof(operand1));
			Condition.ValidateNotNull(operand2, nameof(operand2));

            int[] result = new int[Math.Max(operand1.Count + shift1, operand2.Count + shift2) + 1];

            SumShifted(BASE, result, operand1, shift1, operand2, shift2);

            return result.Cut();
        }

        /// <summary>
        /// Calculates the sum of two long integer numbers, each shifted
        /// left by certain amounts of BASE-based digits.
        /// </summary>
        /// <param name="BASE">The base of digits in the numbers.</param>
        /// <param name="result">The digits list to store the result. If N = max(operand1.Count + shift1, operand2.Count + shift2), this list should be able to store at least N+1 digits. Otherwise, the result will be cut.</param>
        /// <param name="operand1">The digits list of the first number.</param>
        /// <param name="shift1">A positive amount of digits for the first number to shift left by.</param>
        /// <param name="operand2">The digits list of the second number.</param>
        /// <param name="shift2">A positive amount of digits for the second number to shift left by.</param>
        public static void SumShifted(int BASE, IList<int> result, IList<int> operand1, int shift1, IList<int> operand2, int shift2)
        {
			Condition
				.Validate(shift1 >= 0)
				.OrArgumentOutOfRangeException(Messages.CannotShiftByNegativeNumberOfDigits);
			Condition
				.Validate(shift2 >= 0)
				.OrArgumentOutOfRangeException(Messages.CannotShiftByNegativeNumberOfDigits);
			Condition.ValidateNotNull(operand1, nameof(operand1));
			Condition.ValidateNotNull(operand2, nameof(operand2));
			Condition.ValidateNotNull(result, nameof(result));

            int i;
            long tmp;
            int carry = 0;

            for (i = 0; i < result.Count; i++)
            {
                tmp = carry;

                if (i - shift1 >= 0 && i - shift1 < operand1.Count)
                    tmp += operand1[i - shift1];
                if (i - shift2 >= 0 && i - shift2 < operand2.Count)
                    tmp += operand2[i - shift2];

                result[i] = (int)(tmp % BASE);
                carry = (int)(tmp / BASE);
            }

            return;
        }

        // ---------------------------------------
        // ----------- DIFFERENCE ----------------
        // ---------------------------------------

        /// <summary>
        /// Calculates the difference between two natural long integer numbers.
        /// WARNING: if the length of the bigger number is N, then the result
        /// digits array should be able to store N digits as well.
        /// No checking is performed, thus an <c>IndexOutOfBoundsException</c> can be thrown.
        /// We assume that number 'one' is greater than number 'two'.
        /// If, nevertheless, <paramref name="two"/> is bigger than <paramref name="one"/>, the function would return <c>true</c>
        /// and the result would be equal to (<c>BASE^result.Length - trueResultValue</c>);
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="result"></param>
        /// <param name="BASE"></param>
        public static bool Subtract(int BASE, IList<int> result, IReadOnlyList<int> one, IReadOnlyList<int> two)
        {
			Condition.ValidateNotNull(one, nameof(one));
			Condition.ValidateNotNull(two, nameof(two));
			Condition.ValidateNotNull(result, nameof(result));

            // TODO: turn this into a unit test.
			// Contract.Ensures(Contract.ForAll(result, x => (x >= 0)));

            int i;
            int temp;
            int carry = 0;

            for (i = 0; i < result.Count; i++)
            {
                temp = -carry;

                if (i < one.Count) temp += one[i];
                if (i < two.Count) temp -= two[i];

                carry = 0;

                while (temp < 0)
                {
                    carry++;
                    temp += BASE;
                }

                result[i] = temp;
            }

            return (carry != 0);
        }

        // ---------------------------------------
        // ---------------- DIVISION -------------
        // ---------------------------------------

        /// <summary>
        /// Performs the division (with remainder) of two long integer numbers.
        /// Returns the quotient containing only significant digits.
        /// 
        /// The remainder containing only significant digits is passed as an 'out' parameter.
        /// </summary>
        /// <param name="BASE">The digit base integers to divide one by another.</param>
        /// <param name="one">The dividend.</param>
        /// <param name="two">The divisor.</param>
        /// <param name="remainder">The reference to store the remainder.</param>
        /// <returns>The quotient containing only significant digits (no trailing zeroes).</returns>
		public static IList<int> Divide(int BASE, IReadOnlyList<int> one, IReadOnlyList<int> two, out IList<int> remainder)
        {
			Condition.ValidateNotNull(one, nameof(one));
			Condition.ValidateNotNull(two, nameof(two));

			/*
			 * TODO: make this into a unit test
			 * 
            Contract.Ensures(Contract.Result<IList<int>>() != null);                            // result's not null
            Contract.Ensures(Contract.ValueAtReturn<IList<int>>(out remainder) != null);        // remainder's not null
            Contract.Ensures(Contract.ForAll(Contract.Result<IList<int>>(), (x => (x >= 0))));  // the result has positive digits
            Contract.Ensures(Contract.ForAll(Contract.ValueAtReturn<IList<int>>(out remainder), (x => (x >= 0))));  // the remainder has positive digits
			*/

            int[] result = new int[one.CountSignificant() - two.CountSignificant() + 1];

			remainder = Divide(BASE, result, one, two).AsReadOnly().Cut();

            return result.Cut();
        }

        /// <summary>
        /// Performs the division (with remainder) of two long integer numbers.
        /// The method would return the list containing the remainder digits.
        /// </summary>
        /// <returns>The list containing the remainder digits.</returns>
        /// <param name="BASE">The numberic base of the digits, ex. 10 000</param>
        /// <param name="result">The digits array to store the result. WARNING! Should be able to store AT LEAST one.Count - two.Count + 1 digits.</param>
        /// <param name="one">The digits array containing the first operand.</param>
        /// <param name="two">The digits array containing the second operand.</param>
        public static IList<int> Divide(int BASE, IList<int> result, IReadOnlyList<int> one, IReadOnlyList<int> two)
        {
			Condition.ValidateNotNull(one, nameof(one));
			Condition.ValidateNotNull(two, nameof(two));
			Condition.ValidateNotNull(result, nameof(result));
			Condition.Validate(one.All(digit => digit >= 0)).OrArgumentException(Messages.OperandShouldNotContainNegativeDigits);
			Condition.Validate(two.All(digit => digit >= 0)).OrArgumentException(Messages.OperandShouldNotContainNegativeDigits);

			/*
            Contract.Requires(Contract.ForAll(one, x => x >= 0));    // first contains only positive digits
            Contract.Requires(Contract.ForAll(two, x => x >= 0));    // second contains only positive digits
			*/

			/*
			 * TODO: turn this into a unit test
            Contract.Ensures(Contract.Result<IList<int>>() != null);                        // remainder's not null
            Contract.Ensures(Contract.ForAll(Contract.Result<IList<int>>(), x => x >= 0));  // remainder has positive digits
            Contract.Ensures(Contract.ForAll(result, x => x >= 0));                         // quot has positive digits
			*/
            // Обнуляем результат
            result.FillByAssign(0);
            // ------------------

			IList<int> u;
			IList<int> v;

            int uShift, vShift, i;
            long temp, temp1, temp2;

            int scale;
            int qGuess, remainder;
            int borrow, carry;

			// Check if we need to scale. If yes, alas,
			// additional memory needs to be allocated.
			// -
            scale = BASE / (two[two.CountSignificant() - 1] + 1);

            if (scale > 1)
            {
                u = new int[one.Count + 2];
                v = new int[two.Count + 2];

				int[] scaleDigitsArray = { scale };

                MultiplySimple(BASE, u, one, scaleDigitsArray);
                MultiplySimple(BASE, v, two, scaleDigitsArray);
            }
            else
            {
                // The number will be spoiled, so we need to 
				// create a copy.
				// -
                u = new int[one.CountSignificant() + 1];
				ServiceMethods.Copy(one, 0, u, 0, u.Count - 1);

				v = new int[two.CountSignificant()];
				ServiceMethods.Copy(one, 0, v, 0, v.Count);
            }

			int n = v.AsReadOnly().CountSignificant();
			int m = u.AsReadOnly().CountSignificant() - n;

            // Added -1.
			// -
            for (vShift = m, uShift = n + vShift; vShift >= 0; --vShift, --uShift)
            {
                qGuess = (int)(((long)u[uShift] * BASE + u[uShift - 1]) / v[n - 1]);
                remainder = (int)(((long)u[uShift] * BASE + u[uShift - 1]) % v[n - 1]);

                while (remainder < BASE)
                {
                    temp2 = (long)v[n - 2] * qGuess;
                    temp1 = (long)remainder * BASE + u[uShift - 2];

                    if ((temp2 > temp1) || (qGuess == BASE))
                    {
                        --qGuess;
                        remainder += v[n - 1];
                    }
                    else break;
                }

                // Теперь qGuess - правильное частное или на единицу больше q
                // Вычесть делитель B, умноженный на qGuess из делимого U,
                // начиная с позиции vJ+i
                carry = 0; borrow = 0;

                // Loop over the second number's digits.
				// -
                for (i = 0; i < n; i++)
                {
                    // получить в temp цифру произведения B*qGuess

                    temp1 = (long)v[i] * qGuess + carry;
                    carry = (int)(temp1 / BASE);
                    temp1 -= (long)carry * BASE;

                    // Сразу же вычесть из U

                    temp2 = (long)u[i + vShift] - temp1 + borrow;

                    if (temp2 < 0)
                    {
                        u[i + vShift] = (int)(temp2 + BASE);
                        borrow = -1;
                    }
                    else
                    {
                        u[i + vShift] = (int)temp2;
                        borrow = 0;
                    }
                }

                // возможно, умноженое на qGuess число B удлинилось.
                // Если это так, то после умножения остался
                // неиспользованный перенос carry. Вычесть и его тоже.

                temp2 = (long)u[i + vShift] - carry + borrow;

                if (temp2 < 0)
                {
                    u[i + vShift] = (int)temp2 + BASE;
                    borrow = -1;
                }
                else
                {
                    u[i + vShift] = (int)temp2;
                    borrow = 0;
                }

                // Did the subtraction go normally?
				// -
                if (borrow == 0)
                {
					// Yes, the quotient guess was correct.
					// -
                    result[vShift] = qGuess;
                }
                else
                {
					// No, the last borrow during subtraction is -1,
					// which means qGuess is 1 larger than the true
					// quotient.
					// -
                    result[vShift] = qGuess - 1;

                    // добавить одно, вычтенное сверх необходимого B к U
                    carry = 0;

                    for (i = 0; i < n; i++)
                    {
                        temp = (long)u[i + vShift] + v[i] + carry;

                        if (temp >= BASE)
                        {
                            u[i + vShift] = (int)(temp - BASE);
                            carry = 1;
                        }
                        else
                        {
                            u[i + vShift] = (int)temp;
                            carry = 0;
                        }
                    }

                    u[i + vShift] = u[i + vShift] + carry - BASE;
                }

                // Обновим размер U, который после вычитания мог уменьшиться
                // i = u.Length - 1;
                // while ((i > 0) && (u[i] == 0))
                // u.digits.RemoveAt(i--);
            }

			// Return the remainder, also need to scale back.
			// -
			int[] remainderDigits = new int[u.Count];

			DivideByInteger(BASE, remainderDigits, u.AsReadOnly(), scale);

            return remainderDigits;
        }

        /// <summary>
        /// Divides the natural long integer number by a natural integer value.
        /// The method would return an array containing only significant digits of the quotient.
        /// The integer remainder is passed as an out parameter.
        /// </summary>
        /// <param name="BASE">The base of dividend's digits.</param>
        /// <param name="one">The natural long integer dividend.</param>
        /// <param name="two">The natural integer divisor.</param>
        /// <param name="remainder">The reference to store the remainder.</param>
        /// <returns>An array containing only significant digits of the quotient.</returns>
        public static int[] DivideByInteger(int BASE, IReadOnlyList<int> one, int two, out int remainder)
        {
			Condition.ValidateNotNull(one, nameof(one));
			Condition.Validate(two > 0).OrArgumentOutOfRangeException(Messages.DivisorShouldBePositive);

            int[] result = new int[one.Count];

            remainder = DivideByInteger(BASE, result, one, two);

            return result.Cut();
        }

        /// <summary>
        /// Divides the natural long integer number by a natural integer value.
        /// The method would return the remainder of the operation.
        /// </summary>
        /// <param name="BASE">The base of the digits in operands.</param>
        /// <param name="result">The digit array to contain the result. WARNING! Should be safe to store all [one.Count] digits</param>
        /// <param name="one">The first operand. Should be non-negative.</param>
        /// <param name="two">The second operand. Should be positive.</param>
        public static int DivideByInteger(int BASE, IList<int> result, IReadOnlyList<int> one, int two)
        {
			Condition.ValidateNotNull(result, nameof(result));
			Condition.ValidateNotNull(one, nameof(one));
			Condition.Validate(two > 0).OrArgumentOutOfRangeException(Messages.DivisorShouldBePositive);
			Condition.Validate(one.All(digit => digit >= 0)).OrArgumentException(Messages.OperandShouldNotContainNegativeDigits);
            
            int i;

            long remainder = 0;
            long temp;

            for (i = result.Count - 1; i >= one.Count; i--)
                result[i] = 0;

            for (; i >= 0; i--)
            {
                temp = remainder * BASE + one[i];
                result[i] = (int)(temp / two);
            
                remainder = temp - (long)result[i] * two;
            }

            // the remainder now equals 
            // the overall remainder
            
            return (int)remainder;
        }

        // --------------------------------
        // -------- MULTIPLICATION --------
        // --------------------------------

        /// <summary>
        /// Multiplies one natural long integer by another.
        /// The result digits array must be safe to contain one.Count + two.Count elements.
        /// </summary>
        public static void MultiplySimple(int BASE, IList<int> result, IReadOnlyList<int> one, IReadOnlyList<int> two)
        {
			Condition.ValidateNotNull(result, nameof(result));
			Condition.ValidateNotNull(one, nameof(one));
			Condition.ValidateNotNull(two, nameof(two));
			Condition.Validate(one.All(digit => digit >= 0)).OrArgumentException(Messages.OperandShouldNotContainNegativeDigits);
			Condition.Validate(two.All(digit => digit >= 0)).OrArgumentException(Messages.OperandShouldNotContainNegativeDigits);

            result.FillByAssign(0);

            int i, j;

            long shift;     // перенос
            long temp;      // временный результат

            for (i = 0; i < one.Count; i++)
            {
                shift = 0;

                for (j = 0; j < two.Count; j++)
                {
                    temp            = shift + (long)one[i] * two[j] + result[i + j];
                    shift           = temp / BASE;
                    result[i + j]   = (int)(temp - shift * BASE);
                }

                result[i + j] = (int)shift;
            }
        }
    }
}
