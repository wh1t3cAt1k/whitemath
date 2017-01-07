﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

using WhiteMath.ArithmeticLong;
using WhiteMath.ArithmeticLong.Bases;
using WhiteMath.Cryptography;
using WhiteMath.Randoms;

using NUnit.Framework;

namespace WhiteMathTests
{
	using LongInt = LongInt<B100k>;
	using RandomLongInt = RandomLongIntModular<B100k>;

	[TestFixture]
	public class TestPrimalityTests
	{
		const int NUMBER_ROUNDS = 10;
		const int RANDOM_SEED = 100;
		const double FALSE_POSITIVES_EPSILON = 0.001;
		const double FALSE_NEGATIVES_EPSILON = 0.001;

		private static IEnumerable<LongInt> _primesUpTo10000;
		private static IEnumerable<LongInt> _compositesUpTo10000;

		private static IEnumerable<LongInt> PrimesUpTo10000
		{
			get
			{
				if (_primesUpTo10000 != null) return _primesUpTo10000;

				using (StreamReader primesFileReader = new StreamReader(Assembly
					.GetExecutingAssembly()
					.GetManifestResourceStream("whiteMathTests.Resources.PrimesUpTo10000.txt")))
				{
					_primesUpTo10000 = primesFileReader
						.ReadToEnd()
						.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
						.Select(numberString => LongInt.Parse(numberString))
						.ToArray();
				}

				return _primesUpTo10000;
			}
		}

		private static IEnumerable<LongInt> CompositesUpTo10000
		{
			get
			{
				if (_compositesUpTo10000 != null) return _compositesUpTo10000;

				_compositesUpTo10000 = Enumerable
					.Range(2, 9999)
					.Select(number => new LongInt(number))
					.Except(PrimesUpTo10000)
					.ToArray();

				return _compositesUpTo10000;
			}
		}

		[Test]
		public void Test_IsPrimeWilsonTheorem_ReturnsTrue_When_NumberIsPrime()
		{
			// Don't take too much or unit tests will run forever.
			// -
			foreach (LongInt prime in PrimesUpTo10000.Take(10))
			{
				Assert.That(prime.IsPrimeWilsonTheorem());
			}
		}

		[Test]
		public void Test_IsPrimeWilsonTheorem_ReturnsFalse_When_NumberIsComposite()
		{
			// Don't take too much or unit tests will run forever.
			// -
			foreach (LongInt nonPrime in CompositesUpTo10000.Take(10))
			{
				Assert.IsFalse(nonPrime.IsPrimeWilsonTheorem());
			}
		}

		[Test]
		public void Test_IsPrimeWilsonTheorem_ThrowsException_When_NumberIsLessThanTwo()
		{
			LongInt[] illegalNumbers = { -1, 0, 1 };

			foreach (LongInt illegalNumber in illegalNumbers)
			{
				Assert.Throws<ArgumentOutOfRangeException>(
					() => illegalNumber.IsPrimeWilsonTheorem());
			}
		}

		[Test]
		public void Test_IsPrimeFermat_ReturnsTrue_When_NumberIsPrime()
		{
			RandomLongInt generator = new RandomLongInt(
				new RandomStandard(RANDOM_SEED));

			foreach (LongInt prime in PrimesUpTo10000)
			{
				Assert.That(prime.IsPrimeFermat(generator, NUMBER_ROUNDS));
			}
		}

		[Test]
		public void Test_IsPrimeFermat_ReturnsFalseMostOfTheTime_When_NumberIsComposite()
		{
			RandomLongInt generator = new RandomLongInt(
				new RandomStandard(RANDOM_SEED));

			int totalTested = 0;
			int falsePositives = 0;

			foreach (LongInt composite in CompositesUpTo10000)
			{
				if (composite.IsPrimeFermat(generator, NUMBER_ROUNDS)) 
				{
					++falsePositives;
				}

				++totalTested;
			}

			Assert.That((double)falsePositives / totalTested < FALSE_POSITIVES_EPSILON);
		}

		private void Test_CalculateCompositeProbability_ReturnsLowProbability_When_NumberIsPrime(
			Func<LongInt, RandomLongInt, long, double> calculateCompositeProbability)
		{
			RandomLongInt generator = new RandomLongInt(
				new RandomStandard(RANDOM_SEED));

			foreach (LongInt prime in PrimesUpTo10000)
			{
				Assert.That(
					calculateCompositeProbability(prime, generator, NUMBER_ROUNDS) <= FALSE_POSITIVES_EPSILON);
			}
		}

		private void Test_CalculateCompositeProbability_ReturnsHighProbability_When_NumberIsComposite(
			Func<LongInt, RandomLongInt, long, double> calculateCompositeProbability)
		{
			RandomLongInt generator = new RandomLongInt(
				new RandomStandard(RANDOM_SEED));

			foreach (LongInt composite in CompositesUpTo10000)
			{
				Assert.That(
					calculateCompositeProbability(composite, generator, NUMBER_ROUNDS) >= 1 - FALSE_NEGATIVES_EPSILON);
			}
		}

		[Test]
		public void Test_CalculateCompositeProbabilityMillerRabin_ReturnsLowProbability_When_NumberIsPrime()
		{
			Test_CalculateCompositeProbability_ReturnsLowProbability_When_NumberIsPrime(
				PrimalityTests.CalculateCompositeProbabilityMillerRabin);
		}

		[Test]
		public void Test_CalculateCompositeProbabilityMillerRabin_ReturnsHighProbability_When_NumberIsComposite()
		{
			Test_CalculateCompositeProbability_ReturnsHighProbability_When_NumberIsComposite(
				PrimalityTests.CalculateCompositeProbabilityMillerRabin);
		}

		[Test]
		public void Test_CalculateCompositeProbabilitySolovayStrassen_ReturnsLowProbability_When_NumberIsPrime()
		{
			Test_CalculateCompositeProbability_ReturnsLowProbability_When_NumberIsPrime(
				PrimalityTests.CalculateCompositeProbabilitySolovayStrassen);
		}

		[Test]
		public void Test_CalculateCompositeProbabilitySolovayStrassen_ReturnsHighProbability_When_NumberIsComposite()
		{
			Test_CalculateCompositeProbability_ReturnsHighProbability_When_NumberIsComposite(
				PrimalityTests.CalculateCompositeProbabilitySolovayStrassen);
		}
	}
}