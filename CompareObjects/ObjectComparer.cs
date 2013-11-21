using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CompareObjects
{
	/// <summary>
	/// A helper utility to dynamically compare 2 objects with properties that share the same name.
	/// </summary>
	public static class PropertyComparer
	{
		public static Dictionary<string, object> GetProperties(this object obj)
		{
			List<string> skipped;

			return obj.GetProperties(out skipped);
		}

		/// <summary>
		/// This method will return a dictionary of all the properties and their values for the object passed in.
		/// It will also out a List of indexed properties which it can not get the value of. 
		/// </summary>
		/// <param name="obj">The object that who's properties are to be retrieved.</param>
		/// <param name="skipped">properties that were skipped for some reason.</param>
		/// <returns>Returns a dictionary of all the properties and their values for the object passed in.</returns>
		public static Dictionary<string, object> GetProperties(this object obj, out List<string> skipped)
		{
			skipped                    = new List<string>();
			var objectType             = obj.GetType();
			var properties             = obj.GetItemProperties();
			var propertyValueContainer = new Dictionary<string, object>();

			foreach (var propertyInfo in properties)
			{
				if (!propertyInfo.CanRead)
				{
					continue;
				}

				var paramInfo = propertyInfo.GetIndexParameters();
				if (paramInfo.Length == 0)
				{
					propertyValueContainer.Add(propertyInfo.Name, propertyInfo.GetValue(obj, null));
				}
				else
				{
					skipped.Add(propertyInfo.Name);
				}
			}

			return propertyValueContainer;
		}

		/// <summary>
		/// Compares the equality of two objects based on values of properties that share a name.
		/// </summary>
		/// <typeparam name="T">The common base type of the two objects.</typeparam>
		/// <param name="leftObject">The left hand object to be compared.</param>
		/// <param name="rightObject">The right hand object to be compared.</param>
		/// <param name="returnOnlyFailed">This will filter out properties that were skipped from the results.</param>
		/// <param name="skips">A list of property names that are not to be used for the comparison.</param>
		/// <returns>An enumerable set of properties that failed comparison for any reason. If returnOnlyFailed is true, this list will not contain skipped properties.</returns>
		public static IEnumerable<PropertyNotPassedInfo> PropertiesEqual<T>(this T leftObject, T rightObject, bool returnOnlyFailed, params string[] skips) where T : class
		{
			return PropertiesEqual(leftObject, rightObject, skips).Where(notPassed => (!returnOnlyFailed) || (notPassed.FailReason == PropertyNotPassedResult.Failed));
		}

		/// <summary>
		/// This method returns a list of properties that are not equal between two objects. 
		/// It will skip any indexed properties and not tell you which were skipped. 
		/// </summary>
		/// <typeparam name="T">The base type of object to be compared.</typeparam>
		/// <param name="leftObject">The left hand object being compared.</param>
		/// <param name="rightObject">The right hand object being compared.</param>
		/// <param name="skips">A list of property names not to be used in the comparison.</param>
		/// <returns>An enumerable set of properties that failed comparison for any reason. If returnOnlyFailed is true, this list will not contain skipped properties.</returns>
		public static IEnumerable<PropertyNotPassedInfo> PropertiesEqual<T>(this T leftObject, T rightObject, params string[] skips) where T : class
		{
			var results = new List<PropertyNotPassedInfo>();
			var mutableSkips = new List<string>(skips);

			if (ReferenceEquals(leftObject, rightObject))
			{
				return (new PropertyNotPassedInfo[0]);
			}

			if (leftObject == null)
			{
				return rightObject == null 
					? new PropertyNotPassedInfo[0]
					: GetSingleSidedResults(rightObject, (name, result, value) => new PropertyNotPassedInfo(name, result, rightValue: value), mutableSkips);
			}

			if (rightObject == null)
			{
				return GetSingleSidedResults(leftObject, (name, result, value) => new PropertyNotPassedInfo(name, result, leftValue: value), mutableSkips);
			}

			if (leftObject is IEnumerable || rightObject is IEnumerable)
			{
				mutableSkips.Add("SyncRoot");
			}

			results.AddRange(CompareNonStringEnumerables(leftObject, rightObject, mutableSkips.ToArray()));

			var rightObjectProperties = GetItemProperties(rightObject).Where(prop => !prop.GetIndexParameters().Any());

			List<string> skippedProperties;
			var leftObjectPropertyValues = GetProperties(leftObject, out skippedProperties);

			if (leftObject is string)
			{
				results.AddRange(CompareStrings(leftObject, rightObject));
			}
			else
			{
				foreach (var rightObjectProperty in rightObjectProperties)
				{
					var currentResults = new List<PropertyNotPassedInfo>();
					if (mutableSkips.Contains(rightObjectProperty.Name))
					{
						continue;
					}

					var rightObjectPropertyValue = rightObjectProperty.GetValue(rightObject, null);

					EvaluateNulls(rightObjectProperty, leftObjectPropertyValues, rightObjectPropertyValue, currentResults);
					EvaluateDateTimes(leftObjectPropertyValues[rightObjectProperty.Name], rightObjectPropertyValue, rightObjectProperty.Name, rightObjectProperty.PropertyType, currentResults);
					EvaluateSubCollections(leftObjectPropertyValues[rightObjectProperty.Name], rightObjectPropertyValue, rightObjectProperty.Name, rightObjectProperty.PropertyType, mutableSkips.ToArray(), currentResults);
					EvaluateBaseTypes(leftObjectPropertyValues[rightObjectProperty.Name], rightObjectPropertyValue, rightObjectProperty.Name, mutableSkips.ToArray(), currentResults);

					results.AddRange(currentResults);
				}

				results.AddRange(skippedProperties.Select(skipped => new PropertyNotPassedInfo(skipped, PropertyNotPassedResult.Skipped)));
			}

			return (results.ToArray());
		}

		private static IEnumerable<PropertyNotPassedInfo> GetSingleSidedResults<T>(T obj, Func<string, PropertyNotPassedResult, string, PropertyNotPassedInfo> constructResult, IEnumerable<string> skips)
		{
			var results = new List<PropertyNotPassedInfo>();

			List<string> skipped;
			var properties = GetProperties(obj, out skipped);

			results.AddRange
				(
					properties
						.Select
						(
							property =>
							skips.Contains(property.Key)
								? PropertyNotPassedInfo.NewSkipped(property.Key)
								: constructResult(property.Key, PropertyNotPassedResult.Failed, property.Value.ToString())
						)
				);

			results.AddRange(skipped.Select(PropertyNotPassedInfo.NewSkipped));
			return results;
		}

		private static void EvaluateBaseTypes(object leftObjectPropertyValue, object rightObjectPropertyValue, string propertyName, string[] skips, List<PropertyNotPassedInfo> results)
		{
			if (results.Any())
			{
				return;
			}

			if (leftObjectPropertyValue.Equals(rightObjectPropertyValue))
			{
				return;
			}

			var notPassed = 
				leftObjectPropertyValue is ValueType 
				? new[] { new PropertyNotPassedInfo(propertyName, PropertyNotPassedResult.Failed, leftObjectPropertyValue.ToString(), rightObjectPropertyValue.ToString()) } 
				: PropertiesEqual(leftObjectPropertyValue, rightObjectPropertyValue, skips).Select(bad => new PropertyNotPassedInfo(string.Format("{0}.{1}", propertyName, bad.Name), bad.FailReason, bad.ValueOfLeftHandObjetProperty, bad.ValueOfRightHandObjetProperty));

			results.AddRange(notPassed);
		}

		private static void EvaluateSubCollections(object leftObjectPropertyValue, object rightObjectPropertyValue, string propertyName, Type propertyType, string[] skips, List<PropertyNotPassedInfo> results)
		{
			if (results.Any())
			{
				return;
			}

			if ((leftObjectPropertyValue is string) || (propertyType.FindInterfaces(GetTypeFilter(), new[] { "IEnumerable" })).Length <= 0)
			{
				return;
			}

			var leftAsEnumerable = (IEnumerable)leftObjectPropertyValue;
			var rightAsEnumerable = (IEnumerable)rightObjectPropertyValue;

			var leftItems = leftAsEnumerable.Cast<object>().ToList();
			var rightItems = rightAsEnumerable.Cast<object>().ToList();

			if (leftItems.Count != rightItems.Count)
				results.Add(new PropertyNotPassedInfo(string.Format("{0}.Count", propertyName), PropertyNotPassedResult.Failed, leftItems.Count.ToString(), rightItems.Count.ToString()));
			else
			{
				for (var index = 0; index < leftItems.Count; index++)
				{
					if (leftItems[index].GetType().IsValueType && !(leftItems[index].Equals(rightItems[index])))
					{
						results.Add(new PropertyNotPassedInfo(string.Format("{0}[{1}]", propertyName, index), PropertyNotPassedResult.Failed, leftItems[index].ToString(), rightItems[index].ToString()));
					}
					else
					{
						var current = index;
						results.AddRange(PropertiesEqual(leftItems[index], rightItems[index], skips).Select(subBad => new PropertyNotPassedInfo(string.Format("{0}[{1}].{2}", propertyName, current, subBad.Name), subBad.FailReason, subBad.ValueOfLeftHandObjetProperty, subBad.ValueOfRightHandObjetProperty)));
					}
				}
			}
		}

		private static void EvaluateDateTimes(object leftObjectPropertyValue, object rightObjectPropertyValue, string propertyName, Type propertyType, List<PropertyNotPassedInfo> results)
		{
			if (results.Any())
			{
				return;
			}

			if ((propertyType.Equals(typeof(DateTime))) || (propertyType.Equals(typeof(DateTime?))))
			{
				results.AddRange(CompareDateTimes(leftObjectPropertyValue, rightObjectPropertyValue, propertyName));
			}
		}

		private static void EvaluateNulls(PropertyInfo rightObjectProperty, IDictionary<string, object> leftObjectPropertyValues, object rightObjectPropertyValue, ICollection<PropertyNotPassedInfo> results)
		{
			if (results.Any())
			{
				return;
			}

			if (leftObjectPropertyValues[rightObjectProperty.Name] == null)
			{
				if (rightObjectPropertyValue != null)
				{
					results.Add(new PropertyNotPassedInfo(rightObjectProperty.Name, PropertyNotPassedResult.Failed, "NULL", rightObjectPropertyValue.ToString()));
				}
			}
			else if (rightObjectPropertyValue == null)
			{
				results.Add(new PropertyNotPassedInfo(rightObjectProperty.Name, PropertyNotPassedResult.Failed, leftObjectPropertyValues[rightObjectProperty.Name].ToString(), "NULL"));
			}
		}

		private static IEnumerable<PropertyNotPassedInfo> CompareDateTimes(object leftObjectPropertyValue, object rightObjectPropertyValue, string propertyName)
		{
			var rightDateTime = (DateTime)rightObjectPropertyValue;
			var leftDateTime = (DateTime)leftObjectPropertyValue;

			return leftDateTime == rightDateTime 
				? new PropertyNotPassedInfo[] { } 
				: new[] {new PropertyNotPassedInfo(propertyName, PropertyNotPassedResult.Failed, leftDateTime.ToString("F"), rightDateTime.ToString("F"))};
		}

		private static IEnumerable<PropertyNotPassedInfo> CompareStrings<T>(T leftObject, T rightObject)
		{
			return leftObject.Equals(rightObject) 
				? new PropertyNotPassedInfo[] { } 
				: new[] { new PropertyNotPassedInfo("Value", PropertyNotPassedResult.Failed, leftObject.ToString(), rightObject.ToString()) };
		}

		private static TypeFilter GetTypeFilter()
		{
			return delegate(Type type, object obj)
			       	{
			       		var typeNames = (string[])obj;
			       		return typeNames.Any(typeName => type.FullName != null && type.FullName.Contains(typeName));
			       	};
		}

		private static IEnumerable<PropertyInfo> GetItemProperties<T>(this T item)
		{
			return item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(info => info.CanRead);
		}

		private static IEnumerable<PropertyNotPassedInfo> CompareNonStringEnumerables<T>(T leftObject, T rightObject, string[] skips)
		{
			var results = new List<PropertyNotPassedInfo>();

			if ((!(leftObject is IEnumerable)) || (leftObject is string))
			{
				return results;
			}

			var leftObjectAsEnumerable = (IEnumerable)leftObject;
			var leftItems = leftObjectAsEnumerable.Cast<object>().ToList();

			var rightObjectAsEnumerable = (IEnumerable)rightObject;
			var rightItems = rightObjectAsEnumerable.Cast<object>().ToList();

			for (var index = 0; index < leftItems.Count; index++)
			{
				if (index >= rightItems.Count)
				{
					break;
				}

				if (leftItems[index].GetType().IsValueType)
				{
					if (!leftItems[index].Equals(rightItems[index]))
					{
						results.Add(new PropertyNotPassedInfo(string.Format("[{0}]", index), PropertyNotPassedResult.Failed, leftItems[index].ToString(), rightItems[index].ToString()));
					}
				}
				else
				{
					var subBads = PropertiesEqual(leftItems[index], rightItems[index], skips);
					var current = index;
					results.AddRange(subBads.Select(subBad => new PropertyNotPassedInfo(string.Format("[{0}].{1}", current, subBad.Name), subBad.FailReason, subBad.ValueOfLeftHandObjetProperty, subBad.ValueOfRightHandObjetProperty)));
				}
			}

			return results;
		}
	}
}
