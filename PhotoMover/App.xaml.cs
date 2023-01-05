using System.IO;
using System.Windows;

namespace PhotoMover;

static class Flags
{
	public static readonly string Help = "--help";
	public static readonly string ShortHelp = "-h";
	public static readonly string Console = "--console";
	public static readonly string ShortConsole = "-c";
	public static readonly string Delete = "--delete";
	public static readonly string ShortDelete = "-d";
}

public partial class App : Application
{

	protected override void OnStartup(StartupEventArgs e)
	{
		bool showHelp = e.Args.Length > 0 && new[] { Flags.Help, Flags.ShortHelp }.Contains(e.Args[0]);
		Dictionary<string, string> namedArgs = ParseArgs(e.Args);

		// If command line arguments require console mode, we open a console window instead of the default GUI.
		if (namedArgs.ContainsKey(Flags.Help) || namedArgs.ContainsKey(Flags.ShortHelp) || namedArgs.ContainsKey(Flags.Console) || namedArgs.ContainsKey(Flags.ShortConsole))
		{
			bool consoleAllocated = false;

			if (!WinApi.AttachConsole(WinApi.ATTACH_PARENT_PROCESS))
			{
				// If the application is started from a shortcut or we are debugging the application in Visual Studio,
				// AttachConsole will fail and we must create the a new console window to see the output.
				consoleAllocated = WinApi.AllocConsole();
			}

			if (e.Args.Length > 0)
			{
				if (new[] { Flags.Help, Flags.ShortHelp }.Any(key => namedArgs.ContainsKey(key)))
				{
					PrintUsage();
				}
				else
				{
					if (Path.Exists(e.Args[0]))
					{
						bool deleteSourceFiles = namedArgs.ContainsKey(Flags.Delete) || namedArgs.ContainsKey(Flags.ShortDelete);
						Import(e.Args[0], deleteSourceFiles);
					}
					else
					{
						Console.WriteLine($"Import folder {e.Args[0]} does not exist.");
					}
				}
			}

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
			// Open default GUI
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

	private void Import(string importFolder, bool deleteSourceFiles)
	{
		AppSettings.ReadSettingsFromDisk();

		Console.WriteLine($"Importing from {importFolder}");

		List<FileItem> files = BackgroundAnalyzeImport.FindFiles(importFolder);

		Console.WriteLine($"{files.Count} files found");

		foreach (FileItem file in files)
		{
			if (file.Selected)
			{
				Console.WriteLine($"{(deleteSourceFiles ? "Moving" : "Copying")} {file.SourcePath} to {file.DestinationPath}");
				try
				{
					if (!Directory.Exists(Path.GetDirectoryName(file.DestinationPath)))
					{
						Directory.CreateDirectory(Path.GetDirectoryName(file.DestinationPath));
					}
					if (deleteSourceFiles)
					{
						File.Move(file.SourcePath, file.DestinationPath);
					}
					else
					{
						File.Copy(file.SourcePath, file.DestinationPath);
					}
				}
				catch (Exception e)
				{
					Console.WriteLine($"Import failed: {e.Message}");
				}
			}
			else
			{
				Console.WriteLine($"Ignoring {file.SourcePath} - {file.Status}");
			}
		}

	}

	private void PrintUsage()
	{
		Console.WriteLine(
			$$"""
			Usage:

			{{Path.GetFileName(Environment.ProcessPath)}} [IMPORT_DIRECTORY] [OPTIONS]

			IMPORT_DIRECTORY  Folder to import from.

			OPTIONS
			-c --console      Run the application in the console.
			-d --delete       Delete source files from the import direcory after import is complete.
			-h --help         Shows this message.
			"""
		);
	}

}
