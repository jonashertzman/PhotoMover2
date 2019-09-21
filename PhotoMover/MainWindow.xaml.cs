using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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
				ViewModel.ImportFiles.Clear();

				ViewModel.GuiFrozen = true;

				ProgressBarWork.Value = 0;

				Debug.Print("------ ButtonFindFiles_Click");
				BackgroundWork.progressHandler = new Progress<Tuple<int, string, List<FileItem>>>(FindFilesStatusUpdate);
				Task.Run(() => BackgroundWork.FindFiles(ViewModel.ImportPath, ViewModel.LibraryRootDirectories.Select(x => x.Path).ToList())).ContinueWith(FindFilesFinnished, TaskScheduler.FromCurrentSynchronizationContext());
			}
			else
			{
				MessageBox.Show("Import folder does not exist.", "Error");
			}
		}

		private void FindFilesStatusUpdate(Tuple<int, string, List<FileItem>> progress)
		{
			Debug.Print($"------- {progress.Item2}");
			ProgressBarWork.Value = progress.Item1;
			ProgressLabel.Content = progress.Item2;

			for (int i = ViewModel.ImportFiles.Count; i < progress.Item3.Count; i++)
			{
				ViewModel.ImportFiles.Add(progress.Item3[i]);
			}
		}

		private void FindFilesFinnished(Task task)
		{
			Debug.Print("------ FindFilesFinnished");
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
			AboutWindow aboutWindow = new AboutWindow() { Owner = this, DataContext = ViewModel };
			aboutWindow.ShowDialog();
		}

		private void CommandOpenContainingFolder_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{

		}

		private void CommandOpenContainingFolder_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
		{

		}

		private void CommandCancelWork_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			Debug.Print("------ CommandCancelWork_Executed");
		}

		#endregion

		#endregion

	}
}
