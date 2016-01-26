#if (INCLUDE_UNIT_TESTS)

using NUnit.Framework;

using IntRational = whiteMath.RationalNumbers.Rational<int, whiteMath.Calculators.CalcInt>;
using IntRationalCalculator = whiteMath.Calculators.CalcRational<int, whiteMath.Calculators.CalcInt>;

namespace whiteMath.RationalNumbers
{
	[TestFixture]
	public class UnitTests
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
					Is.EqualTo(calculator.zero));
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
			IntRational firstNaN = new IntRational(IntRational.SpecialNumberType.NaN);
			IntRational secondNaN = new IntRational(IntRational.SpecialNumberType.NaN);

			Assert.That(firstNaN != secondNaN);
		}

		[Test]
		public void TestPositiveInfinityIsEqualToPositiveInfinity()
		{
			IntRational firstPositiveInfinity = new IntRational(IntRational.SpecialNumberType.PositiveInfinity);
			IntRational secondPositiveInfinity = new IntRational(IntRational.SpecialNumberType.PositiveInfinity);

			Assert.That(firstPositiveInfinity == secondPositiveInfinity);
		}

		public void TestNegativeInfinityIsEqualToNegativeInfinity()
		{
			IntRational firstNegativeInfinity = new IntRational(IntRational.SpecialNumberType.NegativeInfinity);
			IntRational secondNegativeInfinity = new IntRational(IntRational.SpecialNumberType.NegativeInfinity);

			Assert.That(firstNegativeInfinity == secondNegativeInfinity);
		}

		[Test]
		public void TestPositiveInfinityPlusPositiveNumberIsPositiveInfinity()
		{
			IntRational positiveInfinity = new IntRational(IntRational.SpecialNumberType.PositiveInfinity);
			IntRational normalNumber = new IntRational(5, 7);
			Assert.That((positiveInfinity + normalNumber).IsPositiveInfinity);
		}

		[Test]
		public void TestPositiveInfinityPlusNegativeNumberIsPositiveInfinity()
		{
			IntRational positiveInfinity = new IntRational(IntRational.SpecialNumberType.PositiveInfinity);
			IntRational normalNumber = new IntRational(-5, 7);
			Assert.That((positiveInfinity + normalNumber).IsPositiveInfinity);
		}

		[Test]
		public void TestPositiveInfinityPlusPositiveInfinityIsPositiveInfinity()
		{
			IntRational firstInfinity = new IntRational(IntRational.SpecialNumberType.PositiveInfinity);
			IntRational secondInfinity = new IntRational(IntRational.SpecialNumberType.PositiveInfinity);
			Assert.That((firstInfinity + secondInfinity).IsPositiveInfinity);
		}

		[Test]
		public void TestPositiveInfinityPlusNegativeInfinityIsNaN()
		{
			IntRational positiveInfinity = new IntRational(IntRational.SpecialNumberType.PositiveInfinity);
			IntRational negativeInfinity = new IntRational(IntRational.SpecialNumberType.NegativeInfinity);
			Assert.That((positiveInfinity + negativeInfinity).IsNaN);
		}

		[Test]
		public void TestPositiveInfinityPlusNaNIsNaN()
		{
			IntRational positiveInfinity = new IntRational(IntRational.SpecialNumberType.PositiveInfinity);
			IntRational NaN = new IntRational(IntRational.SpecialNumberType.NaN);
			Assert.That((positiveInfinity + NaN).IsNaN);
		}
	}
}

#endif