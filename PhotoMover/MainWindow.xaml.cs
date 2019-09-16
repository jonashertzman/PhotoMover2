using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
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
			if (Environment.GetCommandLineArgs().Length > 1)
			{
				ViewModel.ImportPath = Environment.GetCommandLineArgs()[1];
			}
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

		private void ButtonFindFiles_Click(object sender, RoutedEventArgs e)
		{
			if (Directory.Exists(ViewModel.ImportPath))
			{
				ViewModel.GuiFrozen = true;

				ProgressBarWork.Value = 0;

				Debug.Print("------ ButtonFindFiles_Click");
				BackgroundWork.progressHandler = new Progress<int>(FindFilesStatusUpdate);
				Task.Run(() => BackgroundWork.FindFiles()).ContinueWith(AnalyzeFinnished, TaskScheduler.FromCurrentSynchronizationContext());
			}
			else
			{
				MessageBox.Show("Import folder does not exist.", "Error");
			}
		}

		private void FindFilesStatusUpdate(int progress)
		{
			Debug.Print("------ FindFilesStatusUpdate");
			ProgressBarWork.Value = progress;
		}

		private void AnalyzeFinnished(Task task)
		{
			Debug.Print("------ CompareFilesFinnished");
			ViewModel.GuiFrozen = false;

		}

		private void ButtonCopy_Click(object sender, RoutedEventArgs e)
		{

		}

		#region Commands

		private void CommandExit_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			this.Close();
		}

		private void CommandAbout_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			AboutWindow aboutWindow = new AboutWindow() { Owner = this };
			aboutWindow.ShowDialog();
		}

		private void CommandOpenContainingFolder_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{

		}

		private void CommandOpenContainingFolder_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
		{

		}

		#endregion

		#endregion

	}
}
