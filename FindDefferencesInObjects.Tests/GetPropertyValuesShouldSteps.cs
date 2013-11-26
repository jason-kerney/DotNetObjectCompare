using ApprovalTests;
using CompareObjects.Tests;
using FindDifferencesInObjects;
using TechTalk.SpecFlow;

namespace FindDefferencesInObjects.Tests
{
	[Binding]
	public class GetPropertyValuesShouldSteps
	{
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

			Approvals.VerifyAll(properties.Values, "Property", value => string.Format("{0}: '{1}' {2}", value.Name, value.Value, value.PropertyType));
		}
	}
}
