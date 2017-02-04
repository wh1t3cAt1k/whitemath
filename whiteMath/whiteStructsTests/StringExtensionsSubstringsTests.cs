using System.Collections.Generic;

using WhiteStructs.Strings;

using NUnit.Framework;

namespace WhiteStructsTests
{
	[TestFixture]
	public class StringExtensionsSubstringsTests
	{
		[Test]
		[TestCase("aaa", "a", new[] { 0, 1, 2 })]
		[TestCase("aaa", "aa", new[] { 0, 1 })]
		[TestCase("aba", "a", new[] { 0, 2 })]
		[TestCase("a", "a", new[] { 0 })]
		[TestCase("ababa", "aba", new[] { 0, 2 })]
		[TestCase("aaa", "", new[] { 0, 1, 2 })]
		public void Test_IndicesOfUsingZFunction_ReturnsAllIndicesOfSubstring(
			string text,
			string sample,
			IEnumerable<int> expectedIndices)
		{
			Assert.That(
				text.IndicesOfUsingZFunction(sample),
				Is.EquivalentTo(expectedIndices));
		}
	}
}
