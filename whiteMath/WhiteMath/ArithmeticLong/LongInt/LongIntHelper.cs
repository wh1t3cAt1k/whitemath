using System;
using System.Collections.Generic;

using WhiteMath.Mathematics;
using WhiteMath.Calculators;

using WhiteStructs.Conditions;

namespace WhiteMath.ArithmeticLong
{
	public partial class LongInt<B> where B : IBase, new()
	{
		public static partial class Helper
		{
			/// <summary>
			/// Adds together two long integer numbers with the
			/// specified shift values.
			/// </summary>
			private static void SumPrivate(
				int digitBase, 
				IList<int> result, 
				IReadOnlyList<int> firstOperand, 
				int firstShift, 
				IReadOnlyList<int> secondOperand, 
				int secondShift)
			{
				int i;
				long tmp;
				int carry = 0;

				for (i = 0; i < result.Count; i++)
				{
					tmp = carry;

					if (i - firstShift >= 0 && i - firstShift < firstOperand.Count)
						tmp += firstOperand[i - firstShift];
					if (i - secondShift >= 0 && i - secondShift < secondOperand.Count)
						tmp += secondOperand[i - secondShift];

					result[i] = (int)(tmp % digitBase);
					carry = (int)(tmp / digitBase);
				}

				return;
			}

			#region Number Division

			/// <summary>
			/// Performs the integral division of two long integer numbers.
			/// </summary>
			/// <param name="one">The first LongInt number.</param>
			/// <param name="two">The second LongInt number.</param>
			/// <returns>The result of integral division without the remainder.</returns>
			public static LongInt<B> Divide(LongInt<B> one, LongInt<B> two)
			{
				Condition.ValidateNotNull(one, nameof(one));
				Condition.ValidateNotNull(two, nameof(two));

				LongInt<B> _;

				return Divide(one, two, out _);
			}

			/// <summary>
			/// Performs the division (with remainder) of two long integer numbers.
			/// </summary>
			/// <param name="one">The first LongInt number.</param>
			/// <param name="two">The second LongInt number.</param>
			/// <param name="remainder">The reference to contain the remainder.</param>
			/// <returns>The result of integral division.</returns>
			public static LongInt<B> Divide(LongInt<B> one, LongInt<B> two, out LongInt<B> remainder)
			{
				Condition.ValidateNotNull(one, nameof(one));
				Condition.ValidateNotNull(two, nameof(two));

				// Corner cases - dividing by a single digit or 
				// dividing by a larger number.
				// -
				if (one.Length < two.Length)
				{
					remainder = one.Clone() as LongInt<B>;
					return new LongInt<B>();                                  // zero number should be returned
				}

				LongInt<B> result;

				if (two.Length == 1)
				{
					result = new LongInt<B>(one.Length);
					result.Digits.AddRange(new int[one.Length]);

					remainder = LongIntegerMethods.DivideByInteger(LongInt<B>.BASE, result.Digits, one.Digits, two[0]);

					// Deal with negativeness.
					// -
					result.IsNegative = one.IsNegative ^ two.IsNegative;
					remainder.IsNegative = one.IsNegative;

					result.DealWithZeroes();
					remainder.DealWithZeroes();

					return result;
				}

				result = new LongInt<B>();
				result.Digits.AddRange(new int[one.Length - two.Length + 1]);

				remainder = new LongInt<B>(two.Length);

				IList<int> remainderDigits = LongIntegerMethods.Divide(LongInt<B>.BASE, result.Digits, one.Digits, two.Digits);
				remainder.Digits.AddRange(remainderDigits);

				// Deal with negativeness
				// -
				result.IsNegative = one.IsNegative ^ two.IsNegative;
				remainder.IsNegative = one.IsNegative;

				// Deal with zeroes
				// -
				result.DealWithZeroes();
				remainder.DealWithZeroes();

				return result;
			}

			#endregion

			#region Number Multiplication

			/// <summary>
			/// Performs a simple, square time complexity multiplication of two LongInt numbers.
			/// </summary>
			public static LongInt<B> MultiplySimple(LongInt<B> one, LongInt<B> two)
			{
				// the resulting number can have double length.

				LongInt<B> res = new LongInt<B>(one.Length + two.Length);
				res.Digits.AddRange(new int[one.Length + two.Length]);
				res.IsNegative = one.IsNegative ^ two.IsNegative;

				// The big one

				LongIntegerMethods.MultiplySimple(LongInt<B>.BASE, res.Digits, one.Digits, two.Digits);

				// Cut the leading zeroes

				res.DealWithZeroes();
				return res;
			}

			#endregion

			#region Integer Square Root

			// Need to statically calculate whether the BASE is a full square.
			// -
			private static readonly int BASE_SQRT_FLOOR 
				= Mathematics<int, CalcInt>.SquareRootInteger(LongInt<B>.BASE, LongInt<B>.BASE);

			private static readonly bool IS_BASE_FULL_SQUARE 
				= (BASE_SQRT_FLOOR * BASE_SQRT_FLOOR == LongInt<B>.BASE);

			/// <summary>
			/// Returns the integer part of the square root
			/// of the number.
			/// </summary>
			/// <remarks>This method works only for integer numeric types.</remarks>
			/// <param name="number">A strictly positive number for which the integer part of its square root is to be found.</param>
			/// <returns>The integer part of the square root of the <paramref name="number"/>.</returns>
			public static LongInt<B> SquareRootInteger(LongInt<B> number)
			{
				Condition.ValidateNotNull(number, nameof(number));
				Condition
					.Validate(number > 0)
					.OrArgumentOutOfRangeException("Cannot extract a square root of a negative number.");

				LongInt<B> firstEstimate = number;

				// An initial estimate better than the number itself
				// is only reasonable to look for in case of digit
				// length more than 1.
				// -
				if (number.Length > 1)
				{
					if (!IS_BASE_FULL_SQUARE || number.Length % 2 == 0)
					{
						firstEstimate = LongInt<B>.CreatePowerOfBase((number.Length + 1) / 2);
					}
					else
					{
						firstEstimate = LongInt<B>.CreatePowerOfBase(number.Length / 2);
						firstEstimate[firstEstimate.Length - 1] = LongInt<B>.Helper.BASE_SQRT_FLOOR;
					}
				}

				return
					Mathematics<LongInt<B>, CalcLongInt<B>>.SquareRootInteger(number, firstEstimate);
			}

			#endregion

			#region Exponentiation

			[Obsolete]
			public static LongInt<B> PowerIntegerModularSlow<B>(
				LongInt<B> number,
				LongInt<B> power,
				LongInt<B> modulus) where B : IBase, new()
			{
				Condition.ValidateNotNull(number, nameof(number));
				Condition.ValidateNotNull(power, nameof(power));
				Condition.ValidateNotNull(modulus, nameof(modulus));

				if (power == 0)
					return 1;

				LongInt<B> res = number % modulus;

				for (LongInt<B> i = 1; i < power; ++i)
					res = res * number % modulus;

				return res;
			}

			/// <summary>
			/// Performs fast modular exponentiation of a <c>LongInt</c> number modulo another number.
			/// </summary>
			/// <typeparam name="B">A type specifying the numeric base of <c>LongInt</c> digits.</typeparam>
			/// <param name="number">A <c>LongInt</c> number to be exponentiated.</param>
			/// <param name="power">A non-negative exponent.</param>
			/// <param name="modulus">The modulus of the operation.</param>
			/// <returns>The result of raising <paramref name="number"/> to power <paramref name="power"/> modulo <paramref name="modulus"/>.</returns>
			public static LongInt<B> PowerIntegerModular<B>(
				LongInt<B> number,
				ulong power,
				LongInt<B> modulus) where B : IBase, new()
			{
				Condition.ValidateNotNull(number, nameof(number));
				Condition.ValidateNotNull(modulus, nameof(modulus));

				LongInt<B> result = 1;

				while (power > 0)
				{
					if ((power & 1) == 1)
						result = (result * number) % modulus;

					power >>= 1;
					number = (number * number) % modulus;
				}

				return result;
			}

			/// <summary>
			/// Performs a fast exponentiation of a <c>LongInt</c> number modulo another number.
			/// </summary>
			/// <typeparam name="B">A type specifying the numeric base of exponentiated number.</typeparam>
			/// <typeparam name="T">A type specifying the numeric base of the exponent. It is very recommended that it be an integer power of two for performance reasons.</typeparam>
			/// <remarks>If <typeparamref name="T"/> <c>IBase</c> type stands for a base that is an integer power of two, the algorithm will speed up significantly.</remarks>
			/// <param name="number">A <c>LongInt</c> number to be exponentiated.</param>
			/// <param name="power">A non-negative exponent.</param>
			/// <param name="modulus">The modulus of the operation.</param>
			/// <returns>The result of raising <paramref name="number"/> to power <paramref name="power"/> modulo <paramref name="modulus"/>.</returns>
			public static LongInt<B> PowerIntegerModular<B, T>(
				LongInt<B> number,
				LongInt<T> power,
				LongInt<B> modulus)
				where B : IBase, new()
				where T : IBase, new()
			{
				Condition.ValidateNotNull(number, nameof(number));
				Condition.ValidateNotNull(power, nameof(power));
				Condition.ValidateNotNull(modulus, nameof(modulus));

				Condition
					.Validate(modulus > 0)
					.OrArgumentOutOfRangeException("The modulus should be a positive number");

				Condition
					.Validate(!power.IsNegative)
					.OrArgumentOutOfRangeException("The power should be a non-negative number.");

				LongInt<B> result = 1;

				while (power > 0)
				{
					if ((power[0] & 1) == 1)
						result = (result * number) % modulus;

					power >>= 1;
					number = (number * number) % modulus;
				}

				return result;
			}

			#endregion
		}
	}
}
