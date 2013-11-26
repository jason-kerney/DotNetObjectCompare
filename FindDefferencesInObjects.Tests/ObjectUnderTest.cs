using System;
using System.Collections.Generic;

namespace CompareObjects.Tests
{
	public class ObjectUnderTest
	{
		private string trueWriteOnly;

		public  string StringProperty        {         get; set; }
		public  int    IntProperty           {         get; set; }
		public  bool   ReadonlyProperty      {         get; set; }
		public  string HiddenGetProperty     { private get; set; }
		private string PrivateProperty       {         get; set; }
		public string  WriteOnlyProperty { set { trueWriteOnly = value; } }

		public ObjectUnderTest(string stringProperty, int intProperty, bool readonlyProperty, string privateProperty)
		{
			StringProperty = stringProperty;
			IntProperty = intProperty;
			ReadonlyProperty = readonlyProperty;
			PrivateProperty = privateProperty;
		}

		public string GetWriteOnlyProperty()
		{
			return trueWriteOnly;
		}

		public string GetPrivateProperty()
		{
			return PrivateProperty;
		}

		public string GetHiddenGetProperty()
		{
			return HiddenGetProperty;
		}

		public static ObjectUnderTest GetDefaultCase()
		{
			return new ObjectUnderTest("string property", 5, false, "private property") {WriteOnlyProperty = "write only property", HiddenGetProperty = "Hidden get property"};
		}
	}

	public class IndexedObjectUnderTest : ObjectUnderTest
	{
		private readonly List<string> items;

		public IndexedObjectUnderTest(string stringProperty, int intProperty, bool readonlyProperty, string privateProperty, IEnumerable<string> items) : base(stringProperty, intProperty, readonlyProperty, privateProperty)
		{
			this.items = new List<string>(items);
		}

		public string this[int index]
		{
			get { return items[index]; }
			set { items[index] = value; }
		}

		public static IndexedObjectUnderTest GetDefaultCase()
		{
			return new IndexedObjectUnderTest("string property", 5, false, "private property", new[] {"item1", "item2"}) {WriteOnlyProperty = "write only property", HiddenGetProperty = "Hidden get property"};
		}
	}

	public class SubCollectionUnderTest : ObjectUnderTest
	{
		private readonly List<int> numbers;

		public SubCollectionUnderTest(string stringProperty, int intProperty, bool readonlyProperty, string privateProperty, IEnumerable<int> numbers) : base(stringProperty, intProperty, readonlyProperty, privateProperty)
		{
			this.numbers = new List<int>(numbers);
		}

		public IEnumerable<int> Numbers { get { return numbers; } } 

		public static SubCollectionUnderTest GetDefaultCase()
		{
			return GetModifiedCase(new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9});
		}

		public static SubCollectionUnderTest GetModifiedCase(IEnumerable<int> numbers)
		{
			return new SubCollectionUnderTest("string property", 5, false, "private property", numbers) {WriteOnlyProperty = "write only property", HiddenGetProperty = "Hidden get property"};
		}
	}

	public class DateTimeUnderTest
	{
		public DateTimeUnderTest(DateTime dateTime)
		{
			DateTime = dateTime;
		}

		public DateTime DateTime { get; set; }

		public static DateTimeUnderTest GetDefaultCase()
		{
			return new DateTimeUnderTest(DateTime.Parse("5-MAY-2005"));
		}
	}
}