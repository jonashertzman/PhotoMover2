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
					var ViewModel = DataContext as MainWindowViewModel;
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
			DialogResult = true;
		}

		#endregion

		private void ButtonHelp_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Supported date formats:\n\nyyyy\tYear (2017)\nMM\tMonth number (01)\nMMM\tShort month name (Jan)\nMMMM\tLong month name (January)\ndd\tDay (01)", "Help");
		}
	}
}
