using System.ComponentModel;

namespace PhotoMover
{
	public class ImportFile : INotifyPropertyChanged
	{

		#region Constructor

		public ImportFile()
		{

		}

		#endregion

		#region Properties

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