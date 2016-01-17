#if (DEBUG)
using NUnit.Framework;

using IntRational = whiteMath.RationalNumbers.Rational<int, whiteMath.CalcInt>;
using IntRationalCalculator = whiteMath.RationalNumbers.CalcRational<int, whiteMath.CalcInt>;

namespace whiteMath.RationalNumbers
{
	[TestFixture]
	public class UnitTests
	{
		private static readonly IntRationalCalculator calculator = new IntRationalCalculator();

		[Test]
		public void TestNegationWorksProperly(
			[Random(-10, 10, 50)] int numerator,
			[Random(-10, 10, 50)] int denominator
		)
		{
			var number = new IntRational(numerator, denominator);
			var negatedNumber = -number;

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
	}
}
#endif