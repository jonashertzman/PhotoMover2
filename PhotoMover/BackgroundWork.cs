using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace PhotoMover
{
	public static class BackgroundWork
	{

		#region Members

		public static IProgress<Tuple<int, string, List<FileItem>>> progressHandler;

		static bool abortPosted = false;
		static DateTime lastStatusUpdateTime = DateTime.UtcNow;

		static List<FileItem> importResults = new List<FileItem>();
		static Dictionary<long, List<FileItem>> libraryFiles = new Dictionary<long, List<FileItem>>();

		#endregion

		#region Methods

		public static void FindFiles(string importPath)
		{
			int progress = -1;
			int counter = 0;

			libraryFiles = new Dictionary<long, List<FileItem>>();

			ReportProgress(progress, $"Scanning library...", true);

			foreach (string libraryRoot in AppSettings.LibraryRootDirectories.Select(x => x.Path))
			{
				foreach (FileItem fileItem in GetFilesInDirectory(libraryRoot))
				{
					ReportProgress(progress, $"Scanning library... {counter++} files found...");
					AddToCollection(fileItem, libraryFiles);
				}
			}

			importResults = new List<FileItem>();
			counter = 0;

			foreach (FileItem f in GetFilesInDirectory(importPath))
			{
				ImportFile(f);

				importResults.Add(f);
				ReportProgress(progress, $"Analyzing import... {counter++} files found...");
			}

			ReportProgress(progress, $"Analyzing import... {counter} files found...", true);
		}

		private static void ImportFile(FileItem f)
		{

			foreach (ImportConfiguration configuration in AppSettings.ImportConfigurations)
			{
				foreach (string file in configuration.Files.Split(' '))
				{
					if (WildcardCompare(f.Name, file, true))
					{
						f.DestinationPath = Path.Combine(configuration.GetDestinationFolder(f.DateTaken), f.Name);
						f.Selected = true;
					}
				}
			}
		}

		private static List<FileItem> GetFilesInDirectory(string path)
		{
			List<FileItem> items = new List<FileItem>();

			SearchDirectory(path, items);

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

		private static void ReportProgress(int progress, string status, bool foreceUpdate = false)
		{
			if (foreceUpdate || (DateTime.UtcNow - lastStatusUpdateTime).TotalMilliseconds >= 50)
			{
				progressHandler.Report(new Tuple<int, string, List<FileItem>>(progress, status, importResults));

				lastStatusUpdateTime = DateTime.UtcNow;
			}
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
}
