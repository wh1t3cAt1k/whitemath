#if (INCLUDE_UNIT_TESTS)

using System;
using System.Collections.Generic;
using System.Linq;


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
				   new RandomMersenneTwister(),
				   Math.Log,
				   Math.Sqrt);

			IEnumerable<double> sequence = Enumerable.Range(1, 1000).Select(x => generator.Next());

			Assert.That(Math.Abs(Math.Sqrt(sequence.SampleUnbiasedVariance) - standardDeviation) < standardDeviation / 100);
			Assert.That(Math.Abs(sequence.SampleAverage - mean) < mean / 100);
		}
	}
}

#endif