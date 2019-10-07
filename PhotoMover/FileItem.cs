using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PhotoMover
{
	public class FileItem : INotifyPropertyChanged
	{
		#region Constructor

		public FileItem(string path, WIN32_FIND_DATA findData)
		{
			sourcePath = path;

			Name = findData.cFileName;
			Size = (long)Combine(findData.nFileSizeHigh, findData.nFileSizeLow);
			FileDate = DateTime.FromFileTime((long)Combine(findData.ftLastWriteTime.dwHighDateTime, findData.ftLastWriteTime.dwLowDateTime));
		}

		#endregion

		#region Overrides

		public override string ToString()
		{
			return $"{SourcePath} -> {DestinationPath}";
		}

		#endregion

		#region Properties

		bool selected = false;
		public bool Selected
		{
			get { return selected; }
			set { selected = value; OnPropertyChanged(nameof(Selected)); }
		}

		string sourcePath;
		public string SourcePath
		{
			get { return sourcePath; }
			set { sourcePath = value; OnPropertyChanged(nameof(SourcePath)); }
		}

		string destinationPath;
		public string DestinationPath
		{
			get { return destinationPath; }
			set { destinationPath = value; OnPropertyChanged(nameof(DestinationPath)); }
		}

		string status;
		public string Status
		{
			get { return status; }
			set { status = value; OnPropertyChanged(nameof(Status)); }
		}

		public string Name { get; }

		public long Size { get; }

		public DateTime FileDate { get; }

		DateTime dateTaken = DateTime.MinValue;
		public DateTime DateTaken
		{
			get
			{
				if (dateTaken == DateTime.MinValue)
				{
					dateTaken = GetDateTaken();
				}
				return dateTaken;
			}
		}

		private string checksum = "";
		public string Checksum
		{
			get
			{
				if (checksum == "")
				{
					checksum = GetChecksum();
				}
				return checksum;
			}
		}

		#endregion

		#region Methods

		private DateTime GetDateTaken()
		{
			try
			{
				DateTime date = new DateTime();
				IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(SourcePath);

				foreach (MetadataExtractor.Directory directory in directories)
				{
					if (directory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out date))
					{
						return date;
					}
				}

				foreach (MetadataExtractor.Directory directory in directories)
				{
					if (directory.TryGetDateTime(ExifDirectoryBase.TagDateTime, out date))
					{
						return date;
					}
				}
			}
			catch (Exception) { }

			return FileDate;
		}

		private ulong Combine(uint highValue, uint lowValue)
		{
			return (ulong)highValue << 32 | lowValue;
		}

		private string GetChecksum()
		{
			Debug.Print($"Slow checksum calculation on {sourcePath}");

			StringBuilder s = new StringBuilder();

			using (MD5 md5 = MD5.Create())
			{
				using (FileStream stream = File.OpenRead(SourcePath))
				{
					foreach (byte b in md5.ComputeHash(stream))
					{
						s.Append(b.ToString("x2"));
					}
				}
			}

			return s.ToString();
		}

		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		#endregion

	}
}
