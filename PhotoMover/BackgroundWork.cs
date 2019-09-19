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
			int progress = 0;

			List<FileItem> existingFiles = new List<FileItem>();

			foreach (string s in libraryRootDirectories)
			{
				foreach (FileItem f in GetFilesInDirectory(s))
				{
					existingFiles.Add(f);
				}
			}

			importResults = new List<FileItem>();

			foreach (FileItem f in GetFilesInDirectory(importPath))
			{
				Thread.Sleep(300);
				importResults.Add(f);
				ReportProgress(progress, "Files found...");
			}

			ReportProgress(progress, "Files found...", true);
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

		private static void ReportProgress(int progress, string status, bool finalUpdate = false)
		{
			if (finalUpdate || (DateTime.UtcNow - lastStatusUpdateTime).TotalMilliseconds >= 1000)
			{
				progressHandler.Report(new Tuple<int, string, List<FileItem>>(progress, status, importResults));

				lastStatusUpdateTime = DateTime.UtcNow;
			}
		}

		#endregion

	}
}
