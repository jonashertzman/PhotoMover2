using System.Windows;
using System.Windows.Controls;

namespace PhotoMover
{
	/// <summary>
	/// Interaction logic for ConfigurationWindow.xaml
	/// </summary>
	public partial class ConfigurationWindow : Window
	{

		#region Constructor

		public ConfigurationWindow()
		{
			InitializeComponent();
		}

		#endregion

		#region Methods

		private void RemoveEmptyConfigurations()
		{
			MainWindowViewModel ViewModel = DataContext as MainWindowViewModel;

			for (int i = ViewModel.ImportConfigurations.Count - 1; i >= 0; i--)
			{
				if (string.IsNullOrEmpty(ViewModel.ImportConfigurations[i].Files))
				{
					ViewModel.ImportConfigurations.RemoveAt(i);
				}
			}

			for (int i = ViewModel.LibraryRootDirectories.Count - 1; i >= 0; i--)
			{
				if (string.IsNullOrEmpty(ViewModel.LibraryRootDirectories[i].Path))
				{
					ViewModel.LibraryRootDirectories.RemoveAt(i);
				}
			}
		}

		#endregion

		#region Events

		private void ButtonBrowseLibrary_Click(object sender, RoutedEventArgs e)
		{
			LibraryRoot t = (sender as Button).DataContext as LibraryRoot;

			BrowseFolderWindow browseFolderWindow = new BrowseFolderWindow() { Owner = this, SelectedPath = t?.Path };
			browseFolderWindow.ShowDialog();

			if (browseFolderWindow.DialogResult == true)
			{
				if (t == null)
				{
					MainWindowViewModel ViewModel = DataContext as MainWindowViewModel;
					ViewModel.LibraryRootDirectories.Add(new LibraryRoot(browseFolderWindow.SelectedPath));
				}
				else
				{
					t.Path = browseFolderWindow.SelectedPath;
				}
			}
		}

		private void ButtonOk_Click(object sender, RoutedEventArgs e)
		{
			RemoveEmptyConfigurations();

			DialogResult = true;
		}

		private void ButtonHelp_Click(object sender, RoutedEventArgs e)
		{
			HelpPopup.IsOpen = true;
		}

		#endregion

	}
}
