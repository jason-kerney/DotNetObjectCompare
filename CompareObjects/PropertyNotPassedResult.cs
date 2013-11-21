namespace CompareObjects
{
	/// <summary>
	/// An enumeration representing why a property did not pass inspection.
	/// </summary>
	public enum PropertyNotPassedResult
	{
		/// <summary>
		/// The properties compared were not equal.
		/// </summary>
		Failed,
		/// <summary>
		/// The properties where skipped for some reason.
		/// </summary>
		Skipped
	}
}