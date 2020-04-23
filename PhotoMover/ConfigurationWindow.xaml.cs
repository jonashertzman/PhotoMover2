using System.Windows;
using System.Windows.Controls;

namespace PhotoMover
{
	/// <summary>
	/// Interaction logic for ConfigurationWindow.xaml
	/// </summary>
	public partial class ConfigurationWindow : Window
	{

		#region Members

		ImportConfiguration selectedConfiguration = null;

		#endregion

		#region Constructor

		public ConfigurationWindow()
		{
			InitializeComponent();
		}

		#endregion

		#region Methods

		private bool ConfigurationValid()
		{
			MainWindowViewModel ViewModel = DataContext as MainWindowViewModel;

			for (int i = ViewModel.ImportConfigurations.Count - 1; i >= 0; i--)
			{
				if (string.IsNullOrEmpty(ViewModel.ImportConfigurations[i].Files) && string.IsNullOrEmpty(ViewModel.ImportConfigurations[i].DestinationFormat))
				{
					ViewModel.ImportConfigurations.RemoveAt(i);
				}
				else if (!ViewModel.ImportConfigurations[i].Valid)
				{
					return false;
				}
			}

			for (int i = ViewModel.LibraryRootDirectories.Count - 1; i >= 0; i--)
			{
				if (string.IsNullOrEmpty(ViewModel.LibraryRootDirectories[i].Path))
				{
					ViewModel.LibraryRootDirectories.RemoveAt(i);
				}
				else if (!ViewModel.LibraryRootDirectories[i].Valid)
				{
					return false;
				}
			}

			return true;
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

		private void ButtonInsertPlaceholder_Click(object sender, RoutedEventArgs e)
		{
			selectedConfiguration = (sender as Button).DataContext as ImportConfiguration;

			HelpPopup.IsOpen = true;
		}

		private void ButtonOk_Click(object sender, RoutedEventArgs e)
		{
			if (ConfigurationValid())
			{
				DialogResult = true;
				Close();
			}
		}

		private void ButtonHelp_Click(object sender, RoutedEventArgs e)
		{
			selectedConfiguration = null;

			HelpPopup.IsOpen = true;
		}

		private void ButtonAppendPlaceholder_Click(object sender, RoutedEventArgs e)
		{
			if (selectedConfiguration != null)
			{
				DateFormat dataFormat = (e.Source as Button).DataContext as DateFormat;
				selectedConfiguration.DestinationFormat += dataFormat.PlaceHolder;
			}
		}

		#endregion

	}
}
