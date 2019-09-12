﻿using System.IO;

namespace PhotoMover
{
	public static class Utils
	{

		public static bool DirectoryAllowed(string path)
		{
			try
			{
				Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
			}
			catch
			{
				return false;
			}
			return true;
		}

	}
}