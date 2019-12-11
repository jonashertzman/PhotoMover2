namespace PhotoMover
{
	public class DateFormat
	{

		#region Constructor

		public DateFormat(string format, string description)
		{
			Format = format;
			Description = description;
		}

		#endregion

		#region Properties

		public string Format { get; private set; }

		public string Description { get; private set; }

		public string PlaceHolder
		{
			get
			{
				return $"<{Format}>";
			}
		}

		#endregion

	}
}