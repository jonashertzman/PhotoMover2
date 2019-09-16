using System;
using System.Threading;

namespace PhotoMover
{
	public static class BackgroundWork
	{

		private static int progress;
		public static IProgress<int> progressHandler;
		private static DateTime startTime;
		private static string currentRoot;

		public static void FindFiles()
		{
			Thread.Sleep(2000);
			progressHandler.Report(70);
			Thread.Sleep(2000);
		}

	}
}
