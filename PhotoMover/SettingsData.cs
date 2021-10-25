using System.Collections.ObjectModel;
using System.Windows;

namespace PhotoMover;

public class SettingsData
{

	public string Id { get; set; } = Guid.NewGuid().ToString();

	public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;

	public ObservableCollection<ImportConfiguration> ImportConfigurations { get; set; } = new ObservableCollection<ImportConfiguration>();

	public ObservableCollection<LibraryRoot> LibraryRootDirectories { get; set; } = new ObservableCollection<LibraryRoot>();

	public double PositionLeft { get; set; }
	public double PositionTop { get; set; }
	public double Width { get; set; } = 750;
	public double Height { get; set; } = 500;
	public WindowState WindowState { get; set; }

}
