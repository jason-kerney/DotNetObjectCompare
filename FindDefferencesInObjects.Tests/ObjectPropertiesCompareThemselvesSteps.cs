using FindDifferencesInObjects;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace FindDefferencesInObjects.Tests
{
	[Binding]
	public class ObjectPropertiesCompareThemselvesSteps
	{
		private const string NameKey = "name";

		[Given(@"Two ObjectProperties with a name of '(.*)'")]
		public void GivenTwoObjectPropertiesWithANameOf(string p0)
		{
			ScenarioContext.Current.Add(NameKey, p0);
		}
		
		[Given(@"a value of (.*)")]
		public void GivenAValueOf(int p0)
		{
			var name = ScenarioContext.Current[NameKey].ToString();
			ScenarioContext.Current.Add("First", new ObjectProperty(name, typeof(int), p0));
			ScenarioContext.Current.Add("Second", new ObjectProperty(name, typeof(int), p0));
		}
		
		[When(@"One is used to compare the other")]
		public void WhenOneIsUsedToCompareTheOther()
		{
			var first = ((ObjectProperty) ScenarioContext.Current["First"]);
			var second = ((ObjectProperty) ScenarioContext.Current["Second"]);

			ScenarioContext.Current.Set(first.Compare(second));
		}

		[Then(@"the result should be SameResult")]
		public void ThenTheResultShouldBe()
		{
			var compareResult = ScenarioContext.Current.Get<CompareResult>();

			Assert.That(compareResult.IsSameResult);
		}

		[Given(@"the '(.*)' having a value of (.*)")]
		public void GivenTheHavingAValueOf(string key, int value)
		{
			var name = ScenarioContext.Current[NameKey].ToString();
			ScenarioContext.Current.Add(key, new ObjectProperty(name, typeof(int), value));
		}

		[When(@"the '(.*)' is compared to the '(.*)'")]
		public void WhenTheIsComparedToThe(string mainKey, string secondaryKey)
		{
			var main = ((ObjectProperty) ScenarioContext.Current[mainKey]);
			var secondary = ((ObjectProperty) ScenarioContext.Current[secondaryKey]);

			ScenarioContext.Current.Set(main.Compare(secondary));
		}

		[Then(@"the result should be DifferentResult\('(.*)', '(.*)', '(.*)'\)")]
		public void ThenTheResultShouldBeDifferentResult(string name, int leftValue, int rightValue)
		{
			var result = ScenarioContext.Current.Get<CompareResult>();

			Assert.That(result.IsDifferentResult);

			var differentResult = ((CompareResult.DifferentResult) result);
			Assert.That(differentResult.Item1, Is.EqualTo(name));
			Assert.That(differentResult.Item2, Is.EqualTo(leftValue));
			Assert.That(differentResult.Item3, Is.EqualTo(rightValue));
		}

		[Given(@"the '(.*)' ObjectProperty with a name of '(.*)'")]
		public void GivenTheObjectPropertyWithANameOf(string key, string name)
		{
			ScenarioContext.Current.Add(key, name);
		}

		[Given(@"the '(.*)' has a value of (.*)")]
		public void GivenTheHasAValueOf(string key, int value)
		{
			var name = ScenarioContext.Current[key].ToString();
			ScenarioContext.Current.Remove(key);

			ScenarioContext.Current.Add(key, new ObjectProperty(name, value.GetType(), value));
		}

		[Then(@"the result should be DifferentNameResult\('(.*)', '(.*)'\)")]
		public void ThenTheResultShouldBeDifferentNameResult(string leftName, string rightName)
		{
			var result = ScenarioContext.Current.Get<CompareResult>();

			Assert.That(result.IsDifferentNameResult);

			var differentNameResult = ((CompareResult.DifferentNameResult) result);

			Assert.That(differentNameResult.Item1, Is.EqualTo(leftName));
			Assert.That(differentNameResult.Item2, Is.EqualTo(rightName));
		}
	}
}
