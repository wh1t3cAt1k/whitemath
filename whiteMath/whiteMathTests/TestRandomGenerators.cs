using System;
using System.Collections.Generic;
using System.Linq;

using WhiteMath;
using WhiteMath.Calculators;
using WhiteMath.Statistics;
using WhiteMath.Random;

using NUnit.Framework;

namespace Randoms
{
	[TestFixture]
	public class TestRandomGenerators
	{
		private const int SEED = 1;
		private const int SAMPLE_SIZE = 10000;
		private const double EPSILON = 0.01;

		[Test]
		public void Test_RandomBoxMuller_GeneratesAccurateStandardNormalDistribution()
		{
			RandomNormalBoxMuller<double, CalcDouble> generator 
				= new RandomNormalBoxMuller<double, CalcDouble>(
				new RandomMersenneTwister(SEED),
					Math.Log,
					Math.Sqrt);

			IEnumerable<double> sequence = Enumerable.Range(1, 10000).Select(x => generator.Next());

			double sampleAverage = sequence.SampleAverage<double, CalcDouble>();
			double sampleUnbiasedVariance = sequence.SampleUnbiasedVariance<double, CalcDouble>(sampleAverage);

			Assert.LessOrEqual(Math.Abs(Math.Sqrt(sampleUnbiasedVariance) - 1), EPSILON);
			Assert.LessOrEqual(Math.Abs(sampleAverage), EPSILON);
		}
	}
}