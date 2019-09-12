using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace PhotoMover
{
	public partial class MainWindow : Window
	{

		#region Members

		MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

		#endregion

		#region Constructor

		public MainWindow()
		{
			InitializeComponent();

			DataContext = ViewModel;
		}

		#endregion

		#region Methods

		private void SaveSettings()
		{
			AppSettings.PositionLeft = this.Left;
			AppSettings.PositionTop = this.Top;
			AppSettings.Width = this.Width;
			AppSettings.Height = this.Height;
			AppSettings.WindowState = this.WindowState;

			AppSettings.WriteSettingsToDisk();
		}

		private void LoadSettings()
		{
			AppSettings.ReadSettingsFromDisk();

			this.Left = AppSettings.PositionLeft;
			this.Top = AppSettings.PositionTop;
			this.Width = AppSettings.Width;
			this.Height = AppSettings.Height;
		}

		#endregion

		#region Events

		private void Window_Initialized(object sender, System.EventArgs e)
		{
			FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

			LoadSettings();
		}

		private void Window_ContentRendered(object sender, System.EventArgs e)
		{

		}

		private void Window_Closed(object sender, System.EventArgs e)
		{
			SaveSettings();
		}

		private void ButtonBrowseImportFolder_Click(object sender, RoutedEventArgs e)
		{
			BrowseFolderWindow browseFolderWindow = new BrowseFolderWindow() { Owner = this, SelectedPath = ViewModel.ImportPath };
			browseFolderWindow.ShowDialog();

			if (browseFolderWindow.DialogResult == true)
			{
				ViewModel.ImportPath = browseFolderWindow.SelectedPath;
			}
		}

		private void ButtonEditConfiguration_Click(object sender, RoutedEventArgs e)
		{
			var oldImportConfigurations = new ObservableCollection<ImportConfiguration>(ViewModel.ImportConfigurations);

			ConfigurationWindow configurationWindow = new ConfigurationWindow() { Owner = this, DataContext = ViewModel };
			configurationWindow.ShowDialog();

			if (configurationWindow.DialogResult == true)
			{
				SaveSettings();
			}
			else
			{
				ViewModel.ImportConfigurations = new ObservableCollection<ImportConfiguration>(oldImportConfigurations);
			}
		}


		private void ButtonAnalyze_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.ImportFiles.Add(new ImportFile() { SourcePath = "A", DestinationPath = "B", Status = "C" });
		}

		private void ButtonCopy_Click(object sender, RoutedEventArgs e)
		{

		}

		#endregion

	}
}
