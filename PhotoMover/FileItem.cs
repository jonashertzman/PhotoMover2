﻿using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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
			return $"{SourcePath}";
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
