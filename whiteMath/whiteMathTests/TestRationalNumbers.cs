﻿using NUnit.Framework;

using WhiteMath.Numeric;

using IntRational = WhiteMath.RationalNumbers.Rational<int, WhiteMath.Calculators.CalcInt>;
using IntRationalCalculator = WhiteMath.Calculators.CalcRational<int, WhiteMath.Calculators.CalcInt>;

namespace WhiteMathTests
{
	[TestFixture]
	public class TestRationalNumbers
	{
		private static readonly IntRationalCalculator calculator = new IntRationalCalculator();

		[Test]
		public void TestNegativeNumberIsNegative()
		{
			Assert.That(new IntRational(-5, 7).IsNegative);
		}

		[Test]
		public void TestPositiveNumberIsPositive()
		{
			Assert.That(!(new IntRational(5, 7)).IsNegative);
		}

		[Test]
		public void TestNumberPlusItsNegationIsZero(
			[Random(-10, 10, 5)] int numerator,
			[Random(-10, 10, 5)] int denominator
		)
		{
			IntRational number = new IntRational(numerator, denominator);
			IntRational negatedNumber = -number;

			if (denominator == 0)
			{
				Assert.Pass();
			}
			else
			{
				Assert.That(
					number + negatedNumber, 
					Is.EqualTo(calculator.Zero));
			}
		}

		[Test]
		public void TestZeroDenominatorNumberIsNotNormal()
		{
			IntRational zeroDenominatorNumber = new IntRational(50, 0);
			Assert.That(!zeroDenominatorNumber.IsNormalNumber);
		}

		[Test]
		public void TestPositiveNumeratorAndZeroDenominatorNumberIsPositiveInfinity()
		{
			IntRational number = new IntRational(50, 0);
			Assert.That(number.IsPositiveInfinity);
		}

		[Test]
		public void TestNegativeNumeratorAndZeroDenominatorNumberIsNegativeInfinity()
		{
			IntRational number = new IntRational(-50, 0);
			Assert.That(number.IsNegativeInfinity);
		}

		[Test]
		public void TestZeroNumeratorAndZeroDenominatorNumberIsNaN()
		{
			IntRational number = new IntRational(0, 0);
			Assert.That(number.IsNaN);
		}

		[Test]
		public void AssertThatNonNormalNumberHasZeroNumeratorAndZeroDenominator()
		{
			IntRational number = new IntRational(-538, 0);
			Assert.That(number.Numerator == 0 && number.Denominator == 0);
		}

		[Test]
		public void AssertThatZeroNumberHasUnitDenominator()
		{
			IntRational number = new IntRational(0, 1000);
			Assert.That(number.Denominator == 1);
		}

		[Test]
		public void TestNaNIsNotEqualToNaN()
		{
			IntRational firstNaN = new IntRational(SpecialNumberType.NaN);
			IntRational secondNaN = new IntRational(SpecialNumberType.NaN);

			Assert.That(firstNaN != secondNaN);
		}

		[Test]
		public void TestPositiveInfinityPlusPositiveNumberIsPositiveInfinity()
		{
			IntRational positiveInfinity = new IntRational(SpecialNumberType.PositiveInfinity);
			IntRational normalNumber = new IntRational(5, 7);
			Assert.That((positiveInfinity + normalNumber).IsPositiveInfinity);
		}

		[Test]
		public void TestPositiveInfinityPlusNegativeNumberIsPositiveInfinity()
		{
			IntRational positiveInfinity = new IntRational(SpecialNumberType.PositiveInfinity);
			IntRational normalNumber = new IntRational(-5, 7);
			Assert.That((positiveInfinity + normalNumber).IsPositiveInfinity);
		}

		[Test]
		public void TestPositiveInfinityPlusPositiveInfinityIsPositiveInfinity()
		{
			IntRational firstInfinity = new IntRational(SpecialNumberType.PositiveInfinity);
			IntRational secondInfinity = new IntRational(SpecialNumberType.PositiveInfinity);
			Assert.That((firstInfinity + secondInfinity).IsPositiveInfinity);
		}

		[Test]
		public void TestPositiveInfinityPlusNegativeInfinityIsNaN()
		{
			IntRational positiveInfinity = new IntRational(SpecialNumberType.PositiveInfinity);
			IntRational negativeInfinity = new IntRational(SpecialNumberType.NegativeInfinity);
			Assert.That((positiveInfinity + negativeInfinity).IsNaN);
		}

		[Test]
		public void TestPositiveInfinityPlusNaNIsNaN()
		{
			IntRational positiveInfinity = new IntRational(SpecialNumberType.PositiveInfinity);
			IntRational NaN = new IntRational(SpecialNumberType.NaN);
			Assert.That((positiveInfinity + NaN).IsNaN);
		}

		[Test]
		[TestCase(SpecialNumberType.PositiveInfinity, SpecialNumberType.NegativeInfinity, true)]
		[TestCase(SpecialNumberType.PositiveInfinity, SpecialNumberType.None, true)]
		[TestCase(SpecialNumberType.PositiveInfinity, SpecialNumberType.PositiveInfinity, false)]
		[TestCase(SpecialNumberType.PositiveInfinity, SpecialNumberType.NaN, false)]
		[TestCase(SpecialNumberType.None, SpecialNumberType.NegativeInfinity, true)]
		[TestCase(SpecialNumberType.None, SpecialNumberType.PositiveInfinity, false)]
		[TestCase(SpecialNumberType.None, SpecialNumberType.NaN, false)]
		[TestCase(SpecialNumberType.NegativeInfinity, SpecialNumberType.NegativeInfinity, false)]
		[TestCase(SpecialNumberType.NegativeInfinity, SpecialNumberType.None, false)]
		[TestCase(SpecialNumberType.NegativeInfinity, SpecialNumberType.PositiveInfinity, false)]
		[TestCase(SpecialNumberType.NegativeInfinity, SpecialNumberType.NaN, false)]
		[TestCase(SpecialNumberType.NaN, SpecialNumberType.NegativeInfinity, false)]
		[TestCase(SpecialNumberType.NaN, SpecialNumberType.None, false)]
		[TestCase(SpecialNumberType.NaN, SpecialNumberType.PositiveInfinity, false)]
		[TestCase(SpecialNumberType.NaN, SpecialNumberType.NaN, false)]
		public void Test_GreaterThanOperator_ComparesSpecialNumbersCorrectly(
			SpecialNumberType firstValueType,
			SpecialNumberType secondValueType,
			bool expectedResult)
		{
			IntRational firstValue = firstValueType != SpecialNumberType.None
				? new IntRational(firstValueType)
				: new IntRational(0, 1);

			IntRational secondValue = secondValueType != SpecialNumberType.None
				? new IntRational(secondValueType)
				: new IntRational(0, 1);

			Assert.That(firstValue > secondValue, Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase(SpecialNumberType.PositiveInfinity, SpecialNumberType.NegativeInfinity, false)]
		[TestCase(SpecialNumberType.PositiveInfinity, SpecialNumberType.None, false)]
		[TestCase(SpecialNumberType.PositiveInfinity, SpecialNumberType.PositiveInfinity, true)]
		[TestCase(SpecialNumberType.PositiveInfinity, SpecialNumberType.NaN, false)]
		[TestCase(SpecialNumberType.NegativeInfinity, SpecialNumberType.NegativeInfinity, true)]
		[TestCase(SpecialNumberType.NegativeInfinity, SpecialNumberType.None, false)]
		[TestCase(SpecialNumberType.NegativeInfinity, SpecialNumberType.PositiveInfinity, false)]
		[TestCase(SpecialNumberType.NegativeInfinity, SpecialNumberType.NaN, false)]
		[TestCase(SpecialNumberType.NaN, SpecialNumberType.NegativeInfinity, false)]
		[TestCase(SpecialNumberType.NaN, SpecialNumberType.None, false)]
		[TestCase(SpecialNumberType.NaN, SpecialNumberType.PositiveInfinity, false)]
		[TestCase(SpecialNumberType.NaN, SpecialNumberType.NaN, false)]
		public void Test_EqualsOperator_ComparesSpecialNumbersCorrectly(
			SpecialNumberType firstValueType,
			SpecialNumberType secondValueType,
			bool expectedResult)
		{
			IntRational firstValue = firstValueType != SpecialNumberType.None
				? new IntRational(firstValueType)
				: new IntRational(0, 1);

			IntRational secondValue = secondValueType != SpecialNumberType.None
				? new IntRational(secondValueType)
				: new IntRational(0, 1);

			Assert.That(firstValue == secondValue, Is.EqualTo(expectedResult));
		}
	}
}