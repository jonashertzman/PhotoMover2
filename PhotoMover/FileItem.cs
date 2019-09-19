using System;
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
			Date = DateTime.FromFileTime((long)Combine(findData.ftLastWriteTime.dwHighDateTime, findData.ftLastWriteTime.dwLowDateTime));
		}

		#endregion

		#region Overrides

		public override string ToString()
		{
			return $"{SourcePath}";
		}

		#endregion

		#region Properties

		bool selected = true;
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

		public DateTime Date { get; }

		#endregion

		#region Methods

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
