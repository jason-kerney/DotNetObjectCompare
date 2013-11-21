using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CompareObjects.Tests
{
	[TestClass]
	[UseReporter(typeof(BeyondCompareReporter))]
	public class PropertiesEqualShould
	{
		[TestMethod]
		public void ReturnNothingWithEqualObjects()
		{
			var out1 = ObjectUnderTest.GetDefaultCase();
			var out2 = ObjectUnderTest.GetDefaultCase();

			var propertyNotPassedInfos = out1.PropertiesEqual(out2);

			Assert.IsTrue(!propertyNotPassedInfos.Any());
		}

		[TestMethod]
		public void ReturnNothingWhenOnlySkippedPropertiesAreNotEqual()
		{
			var out1 = ObjectUnderTest.GetDefaultCase();
			var out2 = ObjectUnderTest.GetDefaultCase();

			out2.IntProperty = 7000;
			out2.StringProperty = "This is not the string you are looking for";

			var propertyNotPassedInfos = out1.PropertiesEqual(out2, new[] {"IntProperty", "StringProperty"});


			Assert.IsTrue(!propertyNotPassedInfos.Any());
		}

		[TestMethod]
		public void ReturnsAllPropertiesIfRightObjectIsNull()
		{
			var out1 = ObjectUnderTest.GetDefaultCase();
			ObjectUnderTest out2 = null;

			var propertyNotPassedInfos = out1.PropertiesEqual(out2);

			propertyNotPassedInfos.VerifyAll();
		}

		[TestMethod]
		public void ReturnsAllPropertiesIfLeftObjectIsNull()
		{
			ObjectUnderTest out1 = null;
			var out2 = ObjectUnderTest.GetDefaultCase();

			var propertyNotPassedInfos = out1.PropertiesEqual(out2);

			propertyNotPassedInfos.VerifyAll();
		}

		[TestMethod]
		public void HandleArraysOfObjectsWithSamePropertyValues()
		{
			var outList = new[] {ObjectUnderTest.GetDefaultCase(), ObjectUnderTest.GetDefaultCase(), ObjectUnderTest.GetDefaultCase()};
			var expectedList = new[] {ObjectUnderTest.GetDefaultCase(), ObjectUnderTest.GetDefaultCase(), ObjectUnderTest.GetDefaultCase()};

			var propertyNotPassedInfos = outList.PropertiesEqual(expectedList);

			Assert.IsTrue(!propertyNotPassedInfos.Any());
		}


		[TestMethod]
		public void FailIfOneItemInArrayDiffers()
		{
			var outList = new[] {ObjectUnderTest.GetDefaultCase(), new ObjectUnderTest("string2", 4, false, "private property"), ObjectUnderTest.GetDefaultCase()};
			var expectedList = new[] {ObjectUnderTest.GetDefaultCase(), ObjectUnderTest.GetDefaultCase(), ObjectUnderTest.GetDefaultCase()};

			var propertyNotPassedInfos = outList.PropertiesEqual(expectedList);

			Approvals.VerifyAll(propertyNotPassedInfos, "Failed");
		}

		[TestMethod]
		public void FailIf2EnumerablesHaveUnevenAmounts()
		{
			var outList = new[] {ObjectUnderTest.GetDefaultCase(), ObjectUnderTest.GetDefaultCase(), ObjectUnderTest.GetDefaultCase()};
			var expectedList = new[] {ObjectUnderTest.GetDefaultCase(), ObjectUnderTest.GetDefaultCase()};

			var propertyNotPassedInfos = outList.PropertiesEqual(expectedList);

			Approvals.VerifyAll(propertyNotPassedInfos, "Failed:");
		}

		[TestMethod]
		public void FailIf2BaseTypeEnumerablesHaveUnevenAmounts()
		{
			var outList = new[] {5, 6, 7};
			var expectedList = new[] {5, 4, 7};

			var propertyNotPassedInfos = outList.PropertiesEqual(expectedList);

			Approvals.VerifyAll(propertyNotPassedInfos, "Failed:");
		}

		[TestMethod]
		public void FailIf2BaseValueEnumerablesHaveUnevenAmounts()
		{
			var outList = new[] {5, 6, 7};
			var expectedList = new[] {5, 6};

			var propertyNotPassedInfos = outList.PropertiesEqual(expectedList);

			Approvals.VerifyAll(propertyNotPassedInfos, "Failed:");
		}

		[TestMethod]
		public void HandleArraysOfBaseTypes()
		{
			var outList = new[] {5, 6, 7};
			var expectedList = new[] {5, 6, 7};

			var propertyNotPassedInfos = outList.PropertiesEqual(expectedList);

			Assert.IsTrue(!propertyNotPassedInfos.Any());
		}

		[TestMethod]
		public void HandleSubCollections()
		{
			var out1 = SubCollectionUnderTest.GetDefaultCase();
			var out2 = SubCollectionUnderTest.GetDefaultCase();

			var propertyNotPassedInfos = out1.PropertiesEqual(out2, true);

			Assert.AreEqual(0, propertyNotPassedInfos.Count());
		}

		[TestMethod]
		public void HandleSubCollectionsWithMismatch()
		{
			var out1 = SubCollectionUnderTest.GetDefaultCase();
			var out2 = SubCollectionUnderTest.GetModifiedCase(new[] {0, 1, 2, 3, 3, 5, 6, 7, 8, 9});

			var propertyNotPassedInfos = out1.PropertiesEqual(out2, true);

			Approvals.VerifyAll(propertyNotPassedInfos, "Failed");
		}

		[TestMethod]
		public void HandleSubCollectionsWithDiffrentLengths()
		{
			var out1 = SubCollectionUnderTest.GetDefaultCase();
			var out2 = SubCollectionUnderTest.GetModifiedCase(new[] {0, 1, 2, 3, 4, 5, 6, 7, 8});

			var propertyNotPassedInfos = out1.PropertiesEqual(out2, true);

			Approvals.VerifyAll(propertyNotPassedInfos, "Failed");
		}

		[TestMethod]
		public void TestTheSameObjectAsEqualToItself()
		{
			var out1 = ObjectUnderTest.GetDefaultCase();

			var propertyNotPassedInfos = out1.PropertiesEqual(out1);

			Assert.AreEqual(0, propertyNotPassedInfos.Count());
		}

		[TestMethod]
		public void HandleDateTimes()
		{
			var out1 = DateTimeUnderTest.GetDefaultCase();
			var out2 = DateTimeUnderTest.GetDefaultCase();

			var propertyNotPassedInfos = out1.PropertiesEqual(out2);

			Assert.AreEqual(0, propertyNotPassedInfos.Count());
		}

		[TestMethod]
		public void HandleUnMatchedDateTimes()
		{
			var out1 = DateTimeUnderTest.GetDefaultCase();
			var out2 = new DateTimeUnderTest(DateTime.Parse("6-May-2006"));

			var propertyNotPassedInfos = out1.PropertiesEqual(out2);

			Approvals.VerifyAll(propertyNotPassedInfos, "Failed");
		}

		[TestMethod]
		public void HandleNullPropertiesOnRightHandSide()
		{
			var out1 = ObjectUnderTest.GetDefaultCase();
			var out2 = ObjectUnderTest.GetDefaultCase();

			out2.StringProperty = null;

			var propertyNotPassedInfos = out1.PropertiesEqual(out2);

			Approvals.VerifyAll(propertyNotPassedInfos, "Failed");
		}

		[TestMethod]
		public void HandleNullPropertiesOnLeftHandSide()
		{
			var out1 = ObjectUnderTest.GetDefaultCase();
			var out2 = ObjectUnderTest.GetDefaultCase();

			out1.StringProperty = null;

			var propertyNotPassedInfos = out1.PropertiesEqual(out2);

			Approvals.VerifyAll(propertyNotPassedInfos, "Failed");
		}
	}
}