using System.IO;
using System.Windows;

namespace PhotoMover;

public partial class App : Application
{

	protected override void OnStartup(StartupEventArgs e)
	{
		bool showHelp = new[] { "/?", "-h", "--help" }.Contains(e.Args[0]);
		Dictionary<string, string> namedArgs = ParseArgs(e.Args);

		if (namedArgs.ContainsKey("--no_gui") || namedArgs.ContainsKey("-h") || namedArgs.ContainsKey("--help"))
		{
			bool consoleAllocated = false;

			if (!WinApi.AttachConsole(WinApi.ATTACH_PARENT_PROCESS))
			{
				// If the application is started from a shortcut or we are debugging the application in Visual Studio,
				// AttachConsole has no console to attach to and we must create the a new console window to see the output.
				consoleAllocated = WinApi.AllocConsole();
			}

			if (e.Args.Length > 0)
			{
				if (new[] { "-h", "--help" }.All(key => namedArgs.ContainsKey(key)))
				{
					PrintUsage();
				}

				else
				{
					if (Path.Exists(e.Args[0]))
					{
						Import(e.Args[0]);
					}
					else
					{
						Console.WriteLine($"Import folder {e.Args[0]} does not exist.");
					}
				}
			}

			//for (int i = 0; i < 10; i++)
			//{
			//	Console.WriteLine(i);
			//	Thread.Sleep(400);
			//}

			if (consoleAllocated)
			{
				Console.WriteLine("\nPress any key to continue.\n");
				Console.ReadKey();
				WinApi.FreeConsole();
			}

			Environment.Exit(0);
		}
		else
		{
			base.OnStartup(e);
		}

	}

	private Dictionary<string, string> ParseArgs(string[] args)
	{
		Dictionary<string, string> namedArgs = new();

		for (int i = 0; i < args.Length; i++)
		{
			string key = args[i];
			string value = "";
			if (key.StartsWith("--"))
			{
				if (args.Length > i + 1 && !args[i + 1].StartsWith("-"))
				{
					value = args[i + 1];
					i++;
				}
				namedArgs.Add(key.ToLower(), value);
			}
			else if (key.StartsWith("-"))
			{
				foreach (char c in key[1..])
				{
					namedArgs.Add($"-{c}", "");
				}

			}
		}

		return namedArgs;
	}

	private void Import(string importFolder)
	{
		Console.WriteLine($"Importing from {importFolder} ...");
	}

	private void PrintUsage()
	{
		string usage =
$$"""
Usage:

{{Path.GetFileName(Environment.ProcessPath)}} [import-folder] [--no_gui] [-h | --help]

import-folder  Folder to import from.
--no_gui       Run the import in the console.
-h --help      Shows this message.
""";

		Console.WriteLine(usage);
	}

}
