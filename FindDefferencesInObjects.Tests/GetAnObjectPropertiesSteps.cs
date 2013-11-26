using System.Collections.Generic;
using System.Reflection;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using CompareObjects.Tests;
using FindDifferencesInObjects;
using TechTalk.SpecFlow;

namespace FindDefferencesInObjects.Tests
{
	[UseReporter(typeof(BeyondCompareReporter))]
    [Binding]
    public class GetAnObjectPropertiesSteps
    {
        [Given(@"the '(.*)' student object with an Age of (.*) and a Class of '(.*)'")]
        public void GivenTheStudentObjectWithAnAgeOfAndAClassOf(string key, int age, string @class)
        {
            ScenarioContext.Current.Add(key, new Student{Age = age, Class = @class});
        }

		[When(@"I get a list of the '(.*)' properties")]
		public void WhenIGetAListOfTheProperties(string key)
		{
			IEnumerable<PropertyInfo> propertyInfos = BasicProperties.getProperties(ScenarioContext.Current[key]);
			ScenarioContext.Current.Set(propertyInfos);
		}

		[Then(@"the '(.*)' list of PropertyInfo objects is retrieved")]
		public void ThenTheListOfPropertyInfoObjectsIsRetrieved(string scenario)
		{
			var propertyInfos = ScenarioContext.Current.Get<IEnumerable<PropertyInfo>>();
			ApprovalResults.ForScenario(scenario);
			Approvals.VerifyAll(propertyInfos, "info", info => string.Format("{0} {1} {2} {3}", info.Name, info.MemberType, info.PropertyType, info.ToString()));
		}

		[Given(@"the '(.*)' object under test with the following values")]
		public void GivenTheObjectUnderTestWithTheFollowingValues(string key, Table table)
		{
			var tableRow = table.Rows[0];

			ScenarioContext.Current.Add(key, new ObjectUnderTest(tableRow["StringProperty"], int.Parse(tableRow["IntProperty"]), bool.Parse(tableRow["ReadonlyProperty"]), tableRow["PrivateProperty"]) { WriteOnlyProperty = tableRow["WriteOnlyProperty"] });
		}

	}

	public class Student
	{
		public int Age { get; set; }
		public string Class { get; set; }
	}
}
