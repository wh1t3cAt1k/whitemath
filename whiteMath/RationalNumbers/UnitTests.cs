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
	}
}

#endif