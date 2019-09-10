using System.ComponentModel;

namespace PhotoMover
{
	public class ImportConfiguration : INotifyPropertyChanged
	{

		#region Constructor

		public ImportConfiguration()
		{

		}

		#endregion

		#region Properties

		string files = "";
		public string Files
		{
			get { return files; }
			set { files = value; OnPropertyChanged(nameof(Files)); }
		}

		string destinationFormat = "";
		public string DestinationFormat
		{
			get { return destinationFormat; }
			set { destinationFormat = value; OnPropertyChanged(nameof(DestinationFormat)); }
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