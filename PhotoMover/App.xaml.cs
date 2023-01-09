using System.IO;
using System.Windows;

namespace PhotoMover;


public partial class App : Application
{

	protected override void OnStartup(StartupEventArgs e)
	{
		CommandLineOptions commandLineOptions = new(e.Args);

		// If command line arguments require console mode, we open a console window instead of the default GUI.
		if (commandLineOptions.ShowHelp || commandLineOptions.RunInConsole)
		{
			bool consoleAllocated = false;

			if (!WinApi.AttachConsole(WinApi.ATTACH_PARENT_PROCESS))
			{
				// If the application is started from a shortcut or we are debugging the application in Visual Studio,
				// AttachConsole will fail and we must create the a new console window to see the output.
				consoleAllocated = WinApi.AllocConsole();
			}

			if (commandLineOptions.ShowHelp)
			{
				PrintUsage();
			}
			else
			{
				Import(commandLineOptions);
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

	private void Import(CommandLineOptions options)
	{
		AppSettings.ReadSettingsFromDisk();

		if (AppSettings.ImportConfigurations.Count == 0)
		{
			Console.WriteLine("No import configuration found, run the application in window mode to configure import.");
			return;
		}

		if (!Path.Exists(options.ImportPath))
		{
			Console.WriteLine($"Import folder {options.ImportPath} does not exist.");
			return;
		}

		Console.WriteLine($"Importing from {options.ImportPath}");
		foreach (var config in AppSettings.ImportConfigurations)
		{
			Console.WriteLine($"  {config.Description}");
		}
		Console.WriteLine();

		List<FileItem> files = BackgroundAnalyzeImport.FindFiles(options.ImportPath);
		Console.WriteLine($"{files.Count} files found");

		foreach (FileItem file in files)
		{
			if (file.Selected)
			{
				try
				{
					if (!Directory.Exists(Path.GetDirectoryName(file.DestinationPath)))
					{
						Directory.CreateDirectory(Path.GetDirectoryName(file.DestinationPath));
					}

					if (options.DeleteSourceFiles)
					{
						Console.WriteLine($"Moving {file.Description}");
						File.Move(file.SourcePath, file.DestinationPath);
					}
					else
					{
						Console.WriteLine($"Copying {file.Description}");
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
			$"""

			Usage:

			{Path.GetFileName(Environment.ProcessPath)} [IMPORT_DIRECTORY] [OPTIONS]

			IMPORT_DIRECTORY  Folder to import from.

			OPTIONS
			-c --console      Run the application in the console.
			-d --delete       Delete source files from the import direcory when importing.
			-h --help         Shows this message.
			"""
		);
	}

}
