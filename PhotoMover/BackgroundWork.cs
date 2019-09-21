using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace PhotoMover
{
	public static class BackgroundWork
	{

		#region Members

		public static IProgress<Tuple<int, string, List<FileItem>>> progressHandler;

		static bool abortPosted = false;
		static List<FileItem> importResults = new List<FileItem>();
		static DateTime lastStatusUpdateTime = DateTime.UtcNow;

		#endregion

		#region Methods

		public static void FindFiles(string importPath, List<string> libraryRootDirectories)
		{
			int progress = -1;
			int counter = 0;

			Dictionary<long, List<FileItem>> existingFiles = new Dictionary<long, List<FileItem>>();

			foreach (string libraryRoot in libraryRootDirectories)
			{
				foreach (FileItem fileItem in GetFilesInDirectory(libraryRoot))
				{
					ReportProgress(progress, $"Scanning library... {counter++} files found...");
					AddToCollection(fileItem, existingFiles);
				}
			}

			importResults = new List<FileItem>();
			counter = 0;

			foreach (FileItem f in GetFilesInDirectory(importPath))
			{
				importResults.Add(f);
				ReportProgress(progress, $"Analyzing import... {counter++} files found...");
			}

			ReportProgress(progress, $"Analyzing import... {counter} files found...", true);
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

		private static void ReportProgress(int progress, string status, bool finalUpdate = false)
		{
			if (finalUpdate || (DateTime.UtcNow - lastStatusUpdateTime).TotalMilliseconds >= 100)
			{
				progressHandler.Report(new Tuple<int, string, List<FileItem>>(progress, status, importResults));

				lastStatusUpdateTime = DateTime.UtcNow;
			}
		}

		#endregion

	}
}
