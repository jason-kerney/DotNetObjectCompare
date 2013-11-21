using System.Collections.Generic;
using ApprovalTests;

namespace CompareObjects.Tests
{
	public static class PropertyNotPassedTestExtensions
	{
		public static void VerifyAll(this IEnumerable<PropertyNotPassedInfo> failedInfo)
		{
			Approvals.VerifyAll(failedInfo, info => string.Format("{0}: '{1}' != '{2}' -- > '{3}'", info.Name, info.ValueOfLeftHandObjetProperty, info.ValueOfRightHandObjetProperty, info.FailReason));
		}
	}
}