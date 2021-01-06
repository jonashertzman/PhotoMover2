using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace PhotoMover
{
	public static class BackgroundPerformImport
	{

		#region Members

		public static IProgress<Tuple<float, string>> progressHandler;

		static bool abortPosted = false;
		static DateTime lastStatusUpdateTime = DateTime.UtcNow;

		#endregion

		#region Methods

		public static void CopyFiles(List<FileItem> importFiles)
		{
			abortPosted = false;
			int fileCount = 0;

			ReportProgress(0, $"Importing files...", true);

			foreach (FileItem importFile in importFiles)
			{
				if (abortPosted) return;

				//Thread.Sleep(300);

				try
				{
					if (!Directory.Exists(Path.GetDirectoryName(importFile.DestinationPath)))
					{
						Directory.CreateDirectory(Path.GetDirectoryName(importFile.DestinationPath));
					}

					File.Copy(importFile.SourcePath, importFile.DestinationPath);
					importFile.Selected = false;
					importFile.Status = "Copied";
				}
				catch (Exception e)
				{
					importFile.Status = "Copy failed. " + e.Message;
					MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}

				ReportProgress((float)fileCount++ / importFiles.Count, $"Importing files... {fileCount} files processed...");
			}
		}

		public static void Cancel()
		{
			abortPosted = true;
		}

		private static void ReportProgress(float progress, string status, bool foreceUpdate = false)
		{
			if (foreceUpdate || (DateTime.UtcNow - lastStatusUpdateTime).TotalMilliseconds >= 50)
			{
				progressHandler.Report(new Tuple<float, string>(progress, status));

				lastStatusUpdateTime = DateTime.UtcNow;
			}
		}

		#endregion

	}
}
