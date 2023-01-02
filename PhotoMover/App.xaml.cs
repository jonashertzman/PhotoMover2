using System.Windows;

namespace PhotoMover;

public partial class App : Application
{

	protected override void OnStartup(StartupEventArgs e)
	{

		Dictionary<string, string> namedArgs = ParseArgs(e.Args);

		if (namedArgs.ContainsKey("--no_gui"))
		{
			if (WinApi.AttachConsole(WinApi.ATTACH_PARENT_PROCESS))
			{
				Console.WriteLine(WinApi.GetConsoleWindow() != 0);
				Console.WriteLine("funkar");
				Console.WriteLine("funkar");
				Console.WriteLine("funkar");

				WinApi.FreeConsole();
			}
			Environment.Exit(0);
		}
		else
		{
			base.OnStartup(e);
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
