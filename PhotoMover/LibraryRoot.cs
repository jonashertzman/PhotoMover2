using System.ComponentModel;

namespace PhotoMover
{
	public class LibraryRoot : INotifyPropertyChanged
	{

		#region Constructor

		public LibraryRoot()
		{

		}

		#endregion

		#region Properties

		string text;
		public string Text
		{
			get { return text; }
			set { text = value; OnPropertyChanged(nameof(Text)); }
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
