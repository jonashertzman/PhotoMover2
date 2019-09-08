using System.ComponentModel;

namespace PhotoMover
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{

		#region Members


		#endregion

		#region Constructor

		public MainWindowViewModel()
		{

		}

		#endregion

		#region Properties

		bool guiFrozen = false;
		public bool GuiFrozen
		{
			get { return guiFrozen; }
			set { guiFrozen = value; OnPropertyChanged(nameof(GuiFrozen)); }
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
