using System.IO;
using System.Linq;
using System.Windows;

namespace PhotoMover;

public static class BackgroundAnalyzeImport
{

	#region Members

	public static IProgress<Tuple<float, string, List<FileItem>>> progressHandler;

	static bool abortPosted = false;
	static DateTime lastStatusUpdateTime = DateTime.UtcNow;

	static readonly List<FileItem> importResults = new List<FileItem>();
	static readonly Dictionary<long, List<FileItem>> libraryFiles = new Dictionary<long, List<FileItem>>();
	static readonly Dictionary<long, List<FileItem>> importedFiles = new Dictionary<long, List<FileItem>>();

	static readonly HashSet<string> importPaths = new HashSet<string>();

	#endregion

	#region Methods

	public static void FindFiles(string importPath)
	{
		abortPosted = false;
		int fileCount = 0;

		importResults.Clear();
		libraryFiles.Clear();
		importedFiles.Clear();
		importPaths.Clear();

		ReadLibraries();

		ReportProgress(0, $"Analyzing import...", true);

		foreach (FileItem importFile in GetFilesInDirectory(importPath, out int itemCount))
		{
			if (abortPosted) return;

			//Thread.Sleep(300);

			if (CheckImport(importFile))
			{
				ReadLibrary(Path.GetDirectoryName(importFile.DestinationPath));

				if (CheckDuplicate(importFile))
				{
					importFile.Selected = false;
				}
				else
				{
					importFile.Selected = true;
					FixNameConflict(importFile);
					importPaths.Add(importFile.DestinationPath.ToUpperInvariant());
					AddToCollection(importFile, importedFiles);
				}
			}
			else
			{
				importFile.Selected = false;
				importFile.Status = "Ignored";
			}

			importResults.Add(importFile);

			ReportProgress((float)fileCount / itemCount, $"Analyzing import... {fileCount++} files found...");
		}

		ReportProgress(1, $"Analyzing import... {fileCount} files found...", true);
	}

	private static void FixNameConflict(FileItem importFile)
	{
		int counter = 2;
		string newPath = importFile.DestinationPath;

		while (importPaths.Contains(newPath.ToUpperInvariant()))
		{
			newPath = Path.Combine(Path.GetDirectoryName(importFile.DestinationPath), $"{Path.GetFileNameWithoutExtension(importFile.Name)} ({counter++}){Path.GetExtension(importFile.Name)}");
			importFile.Status = "Name conflict, file renamed";
		}
		importFile.DestinationPath = newPath;
	}

	private static void ReadLibraries()
	{
		ReportProgress(0, $"Scanning library...", true);

		foreach (string libraryRoot in AppSettings.LibraryRootDirectories.Select(x => x.Path))
		{
			ReadLibrary(libraryRoot, true);
		}
	}

	private static void ReadLibrary(string libraryRoot, bool updateProgress = false)
	{
		int counter = 0;

		foreach (FileItem fileItem in GetFilesInDirectory(libraryRoot, out int itemCount))
		{
			if (abortPosted) return;

			if (updateProgress)
			{
				ReportProgress((float)counter / itemCount, $"Scanning library... {counter++} files found...");
			}

			AddToCollection(fileItem, libraryFiles);

			importPaths.Add(fileItem.SourcePath.ToUpperInvariant());
		}
	}

	private static bool CheckDuplicate(FileItem importFile)
	{
		if (libraryFiles.ContainsKey(importFile.Size))
		{
			foreach (FileItem libraryFile in libraryFiles[importFile.Size])
			{
				if (libraryFile.Checksum == importFile.Checksum)
				{
					importFile.Status = $"Duplicate, file already exists at {libraryFile.SourcePath}";
					return true;
				}
			}
		}

		if (importedFiles.ContainsKey(importFile.Size))
		{
			foreach (FileItem file in importedFiles[importFile.Size])
			{
				if (file.Checksum == importFile.Checksum)
				{
					importFile.Status = $"Duplicate, file already imported from {file.SourcePath}";
					return true;
				}
			}
		}

		return false;
	}

	private static bool CheckImport(FileItem importFile)
	{
		try
		{
			foreach (ImportConfiguration configuration in AppSettings.ImportConfigurations)
			{
				foreach (string file in configuration.Files.Split(' '))
				{
					if (WildcardCompare(importFile.Name, file, true))
					{
						string destinationFolder = configuration.GetDestinationFolder(importFile.DateTaken);

						importFile.DestinationPath = Path.Combine(destinationFolder, importFile.Name);

						return true;
					}
				}
			}
		}
		catch (Exception exception)
		{
			MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		return false;
	}

	private static List<FileItem> GetFilesInDirectory(string path, out int itemCount)
	{
		List<FileItem> items = new List<FileItem>();

		SearchDirectory(path, items);
		itemCount = items.Count;
		return items;
	}

	private static void SearchDirectory(string path, List<FileItem> items)
	{
		IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
		IntPtr findHandle = WinApi.FindFirstFile(Path.Combine(path, "*"), out WIN32_FIND_DATA findData);

		string newPath;

		if (findHandle != INVALID_HANDLE_VALUE)
		{
			do
			{
				newPath = Path.Combine(path, findData.cFileName);

				// Directory
				if ((findData.dwFileAttributes & FileAttributes.Directory) != 0)
				{
					if (findData.cFileName != "." && findData.cFileName != "..")
					{
						SearchDirectory(newPath, items);
					}
				}

				// File
				else
				{
					items.Add(new FileItem(newPath, findData));
				}
			}
			while (WinApi.FindNextFile(findHandle, out findData) && !abortPosted);
		}

		WinApi.FindClose(findHandle);
	}

	private static void AddToCollection(FileItem newFile, Dictionary<long, List<FileItem>> collection)
	{
		if (!collection.ContainsKey(newFile.Size))
		{
			collection.Add(newFile.Size, new List<FileItem>());
		}
		else
		{
			foreach (FileItem existingFile in collection[newFile.Size])
			{
				if (existingFile.SourcePath.Equals(newFile.SourcePath, StringComparison.InvariantCultureIgnoreCase))
				{
					return;
				}
			}
		}

		collection[newFile.Size].Add(newFile);
	}

	private static void ReportProgress(float progress, string status, bool foreceUpdate = false)
	{
		if (foreceUpdate || (DateTime.UtcNow - lastStatusUpdateTime).TotalMilliseconds >= 50)
		{
			progressHandler.Report(new Tuple<float, string, List<FileItem>>(progress, status, importResults));

			lastStatusUpdateTime = DateTime.UtcNow;
		}
	}

	public static void Cancel()
	{
		abortPosted = true;
	}

	private static bool WildcardCompare(string compare, string wildString, bool ignoreCase)
	{
		if (ignoreCase)
		{
			wildString = wildString.ToUpper();
			compare = compare.ToUpper();
		}

		int wildStringLength = wildString.Length;
		int CompareLength = compare.Length;

		int wildMatched = wildStringLength;
		int compareBase = CompareLength;

		int wildPosition = 0;
		int comparePosition = 0;

		// Match until first wildcard '*'
		while (comparePosition < CompareLength && (wildPosition >= wildStringLength || wildString[wildPosition] != '*'))
		{
			if (wildPosition >= wildStringLength || (wildString[wildPosition] != compare[comparePosition] && wildString[wildPosition] != '?'))
			{
				return false;
			}

			wildPosition++;
			comparePosition++;
		}

		// Process wildcard
		while (comparePosition < CompareLength)
		{
			if (wildPosition < wildStringLength)
			{
				if (wildString[wildPosition] == '*')
				{
					wildPosition++;

					if (wildPosition == wildStringLength)
					{
						return true;
					}

					wildMatched = wildPosition;
					compareBase = comparePosition + 1;

					continue;
				}

				if (wildString[wildPosition] == compare[comparePosition] || wildString[wildPosition] == '?')
				{
					wildPosition++;
					comparePosition++;

					continue;
				}
			}

			wildPosition = wildMatched;
			comparePosition = compareBase++;
		}

		while (wildPosition < wildStringLength && wildString[wildPosition] == '*')
		{
			wildPosition++;
		}

		if (wildPosition < wildStringLength)
		{
			return false;
		}

		return true;
	}

	#endregion

}
