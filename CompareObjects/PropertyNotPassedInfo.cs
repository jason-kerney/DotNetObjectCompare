
namespace CompareObjects
{
	/// <summary>
	/// A structure representing information about properties that did not pass comparison.
	/// </summary>
	public struct PropertyNotPassedInfo
	{
		private readonly PropertyNotPassedResult failReason;
		private readonly string name;
		private readonly string leftValue;
		private readonly string rightValue;

		/// <summary>
		/// Constructs a value representing information about properties that did not pass comparison.
		/// </summary>
		/// <param name="name">The name of the property that did not pass comparison</param>
		/// <param name="failReason">An enumeration representing why a property did not pass inspection.</param>
		/// <param name="leftValue">The value of the left hand object's property that was compared.</param>
		/// <param name="rightValue">The value of the right hand object's property that was compared.</param>
		public PropertyNotPassedInfo(string name, PropertyNotPassedResult failReason, string leftValue = "", string rightValue = "")
		{
			this.name = name;
			this.failReason = failReason;
			this.leftValue = leftValue;
			this.rightValue = rightValue;
		}

		/// <summary>
		/// Returns an enumeration representing why a property did not pass inspection.
		/// </summary>
		public PropertyNotPassedResult FailReason
		{
			get { return (failReason); }
		}

		/// <summary>
		/// Returns the name of the property that failed comparison.
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Returns the value of the left hand object's property that was compared.
		/// </summary>
		public string ValueOfLeftHandObjetProperty
		{
			get { return (leftValue); }
		}

		/// <summary>
		/// Returns the value of the right hand object's property that was compared.
		/// </summary>
		public string ValueOfRightHandObjetProperty
		{
			get { return (rightValue); }
		}

		/// <summary>
		/// Gets a property that did not pass because it was skipped.
		/// </summary>
		/// <param name="name">The name of the property</param>
		/// <returns>A property that did not pass because it was skipped.</returns>
		public static PropertyNotPassedInfo NewSkipped(string name)
		{
			return new PropertyNotPassedInfo(name, PropertyNotPassedResult.Skipped);
		}

		public override string ToString()
		{
			return string.Format("{0}: '{1}' != '{2}' ::\t{3}", Name, leftValue, rightValue, FailReason);
		}
	}
}