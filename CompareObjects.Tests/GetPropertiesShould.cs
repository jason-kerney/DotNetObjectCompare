using System.Collections.Generic;
using ApprovalTests;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CompareObjects.Tests
{
	[TestClass]
	[UseReporter(typeof(BeyondCompareReporter))]
	public class GetPropertiesShould
	{
		[TestMethod]
		public void GetOnlyReadablePublicPropertiesFromAnObject()
		{
			var tested = ObjectUnderTest.GetDefaultCase();

			List<string> skipped;

			var properties = tested.GetProperties(out skipped);

			Assert.IsTrue(!skipped.Any());
			Approvals.VerifyAll(properties);
		}

		[TestMethod]
		public void SkippIndexedItems()
		{
			var tested = IndexedObjectUnderTest.GetDefaultCase();

			List<string> skipped;

			tested.GetProperties(out skipped);

			Approvals.VerifyAll(skipped, "skipped");
		}
	}
}