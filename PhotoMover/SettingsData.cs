using System.Collections.ObjectModel;
using System.Windows;

namespace PhotoMover
{
	public class SettingsData
	{
		public ObservableCollection<ImportConfiguration> IgnoredDirectories { get; set; } = new ObservableCollection<ImportConfiguration>();

		public double PositionLeft { get; set; }
		public double PositionTop { get; set; }
		public double Width { get; set; } = 750;
		public double Height { get; set; } = 500;
		public WindowState WindowState { get; set; }

	}
}
