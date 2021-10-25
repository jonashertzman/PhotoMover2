namespace PhotoMover;

public class DateFormat
{

	#region Constructor

	public DateFormat(string placeHolder, string format, string description)
	{
		PlaceHolder = $"<{placeHolder}>";
		Format = format;
		Description = description;
	}

	#endregion

	#region Properties

	public string Format { get; private set; }

	public string Description { get; private set; }

	public string PlaceHolder { get; private set; }

	public string Example
	{
		get
		{
			return string.Format("{0:" + Format + "}", DateTime.Now);
		}
	}

	#endregion

}
