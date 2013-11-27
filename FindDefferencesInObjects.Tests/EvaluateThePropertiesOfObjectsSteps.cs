using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Namers;
using CompareObjects.Tests;
using FindDifferencesInObjects;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace FindDefferencesInObjects.Tests
{
	[Binding]
	public class EvaluateThePropertiesOfObjectsSteps
	{
		private const string ScenarioKey = "scenario";

		[Given(@"the scenario '(.*)'")]
		public void GivenTheScenario(string scenario)
		{
			ScenarioContext.Current.Add(ScenarioKey, scenario);
		}

		[Given(@"the '(.*)' object under test with")]
		public void GivenTheObjectUnderTestWith(string key, Table table)
		{
			var row = table.Rows[0];
			ScenarioContext.Current.Add(key, new ObjectUnderTest(stringProperty: row["StringProperty"], privateProperty: row["PrivateProperty"], intProperty: int.Parse(row["IntProperty"]), readonlyProperty: bool.Parse(row["ReadonlyProperty"])));
		}

		[When(@"I contruct a PropertyValues object from the '(.*)'")]
		public void WhenIContructAPropertyValuesObjectFromThe(string key)
		{
			var objectUnderTest = (ObjectUnderTest) ScenarioContext.Current[key];

			ScenarioContext.Current.Remove(key);
			ScenarioContext.Current.Add(key, new PropertyValues(objectUnderTest));
		}

		[Then(@"the '(.*)' result should be a verifiable list of properties")]
		public void ThenTheResultShouldBeAVerifiableListOfProperties(string key)
		{
			var properties = (PropertyValues) ScenarioContext.Current[key];
			ApprovalResults.ForScenario(ScenarioContext.Current[ScenarioKey].ToString());
			var propertyDefs = properties.Values.Select(kvp => kvp.Value);
			Approvals.VerifyAll(propertyDefs, "Property");
		}

		[When(@"I compare the '(.*)' object with the '(.*)' object")]
		public void WhenICompareTheObjectWithTheObject(string leftKey, string rightKey)
		{
			var left = ScenarioContext.Current[leftKey];
			var right = ScenarioContext.Current[rightKey];

			var leftProperties = new PropertyValues(left);
			var rightProperties = new PropertyValues(right);

			ScenarioContext.Current.Set(leftProperties.Compare(rightProperties).Where(r => !r.IsSameResult));
		}

		[Then(@"there should be no difference")]
		public void ThenThereShouldBeNoDifference()
		{
			var result = ScenarioContext.Current.Get<IEnumerable<CompareResult>>();
			Assert.That(result, Is.Empty);
		}

		[Then(@"there should be verifiable differences")]
		public void ThenThereShouldBeVerifiableDifferences()
		{
			var results = ScenarioContext.Current.Get<IEnumerable<CompareResult>>();

			ApprovalResults.ForScenario(ScenarioContext.Current[ScenarioKey].ToString());

			Approvals.VerifyAll(results, "Result");
		}
	}
}
