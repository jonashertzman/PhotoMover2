namespace PhotoMover;

public class Program
{

	[STAThread]
	public static void Main(string[] args)
	{
		Dictionary<string, string> namedArgs = ParseArgs(args);

		if (namedArgs.ContainsKey("--no_gui"))
		{
			if (WinApi.AttachConsole(WinApi.ATTACH_PARENT_PROCESS))
			{
				Console.WriteLine("funkar");
				Console.WriteLine("funkar");
				Console.WriteLine("funkar");

				WinApi.FreeConsole();
			}
		}
		else
		{
			var application = new App();
			application.InitializeComponent();
			application.Run();
		}
	}

	private static Dictionary<string, string> ParseArgs(string[] args)
	{
		Dictionary<string, string> namedArgs = new();

		for (int i = 0; i < args.Length; i++)
		{
			string key = args[i];
			string value = "";
			if (key.StartsWith("--"))
			{
				if (args.Length > i + 1 && !args[i + 1].StartsWith("--"))
				{
					value = args[i + 1];
				}
				namedArgs.Add(key.ToLower(), value);
			}
		}

		return namedArgs;
	}

}
