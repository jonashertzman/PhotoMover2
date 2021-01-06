using System.ComponentModel;

namespace PhotoMover
{
	public class LibraryRoot : INotifyPropertyChanged
	{

		#region Constructor

		public LibraryRoot()
		{

		}

		public LibraryRoot(string newPath)
		{
			Path = newPath;
		}

		#endregion

		#region Properties

		string path;
		public string Path
		{
			get { return path; }
			set { path = value; OnPropertyChanged(nameof(Path)); OnPropertyChanged(nameof(Valid)); }
		}

		public bool Valid
		{
			get
			{
				return System.IO.Directory.Exists(path);
			}
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
