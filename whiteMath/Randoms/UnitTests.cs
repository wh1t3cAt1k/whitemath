#if (INCLUDE_UNIT_TESTS)

using System;
using System.Collections.Generic;
using System.Linq;

using whiteMath;
using whiteMath.Calculators;
using whiteMath.Statistics;
using whiteMath.Randoms;

using NUnit.Framework;

namespace Randoms
{
	[TestFixture]
	public class UnitTests
	{
		[Test]
		public void TestBoxMullerGeneratesExpectedMeanAndSigma(
			[Random(-100.0, 100.0, 1)] double mean,
			[Random(-100.0, 100.0, 1)] double standardDeviation
		)
		{
			RandomNormalBoxMuller<double, CalcDouble> generator 
				= new RandomNormalBoxMuller<double, CalcDouble>(
					new RandomStandard(),
					Math.Log,
					Math.Sqrt);

			IEnumerable<double> sequence = Enumerable.Range(1, 1000).Select(x => generator.Next());

			Assert.LessOrEqual(
				Math.Abs(Math.Sqrt(sequence.SampleUnbiasedVariance<double, CalcDouble>(sequence.SampleAverage<double, CalcDouble>())) - standardDeviation),
				standardDeviation / 100);
			
			Assert.LessOrEqual(
				Math.Abs(sequence.SampleAverage<double, CalcDouble>() - mean),
				mean / 100);
		}
	}
}

#endif