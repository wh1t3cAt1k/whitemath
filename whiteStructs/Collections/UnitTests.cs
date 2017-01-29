#if (INCLUDE_UNIT_TESTS)

using System;
using NUnit.Framework;

namespace WhiteStructs.Collections
{
	[TestFixture]
	public class UnitTests
	{
		[Test]
		public void TestSingletonCollectionIsSingleton()
		{
			int[] singletonCollection = { 1 }; 
			Assert.That(singletonCollection.IsSingleElement());
		}

		[Test]
		public void TestNonSingletonCollectionIsNotSingleton()
		{
			int[] nonSingletonCollection = { 1, 2 };
			Assert.That(!nonSingletonCollection.IsSingleElement());
		}

		[Test]
		public void TestCallingSingletonCheckOnNullThrowsException()
		{
			int[] nullCollection = null;

			Assert.Throws(typeof(ArgumentNullException), delegate {
				nullCollection.IsSingleElement();
			});
		}

		[Test]
		public void TestEmptyCollectionIsEmpty()
		{
			int[] emptyCollection = { };
			Assert.That(emptyCollection.IsEmpty());
		}

		[Test]
		public void TestNonEmptyCollectionIsNotEmpty()
		{
			int[] nonEmptyCollection = { 1 };
			Assert.That(!nonEmptyCollection.IsEmpty());
		}

		[Test]
		public void TestCallingEmptyCheckOnNullThrowsException()
		{
			int[] nullCollection = null;

			Assert.Throws(typeof(ArgumentNullException), delegate {
				nullCollection.IsEmpty();
			});
		}
	}
}

#endif