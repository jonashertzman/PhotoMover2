using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace PhotoMover;

public partial class MainWindow : Window
{

	#region Members

	MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

	readonly string regPath = @"Folder\shell\photomover";
	readonly string shellExecutePath = $"\"{new FileInfo(Process.GetCurrentProcess().MainModule.FileName)}\" \"%1\"";

	// Keep a reference to the import files data grids scroll viewer so it can be scrolled even if the GUI is frozen
	ScrollViewer importFilesScrollViewer;

	#endregion

	#region Constructor

	public MainWindow()
	{
		InitializeComponent();

		DataContext = ViewModel;

		if (ViewModel.IsAdministrator)
		{
			ViewModel.ShellExtensionsInstalled = Registry.ClassesRoot.CreateSubKey(regPath + "\\command").GetValue("")?.ToString() == shellExecutePath;
		}

		AddShellExtention.Checked += AddShellExtension_Checked;
		AddShellExtention.Unchecked += AddShellExtension_Unchecked;
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
		this.WindowState = AppSettings.WindowState;
	}

	private void ShowInExplorer(string sourcePath)
	{
		if (File.Exists(sourcePath))
		{
			string args = $"/Select, {sourcePath}";
			ProcessStartInfo pfi = new ProcessStartInfo("Explorer.exe", args);
			Process.Start(pfi);
		}
		else
		{
			Process.Start("Explorer.exe", Path.GetDirectoryName(sourcePath));
		}
	}

	private async void CheckForNewVersion(bool forced = false)
	{
		if (AppSettings.LastUpdateTime < DateTime.Now.AddDays(-5) || forced)
		{
			try
			{
				Debug.Print("Checking for new version...");

				HttpClient httpClient = new();
				string result = await httpClient.GetStringAsync("https://jonashertzman.github.io/PhotoMover2/download/version.txt");

				Debug.Print($"Latest version found: {result}");
				ViewModel.NewBuildAvailable = int.Parse(result) > int.Parse(ViewModel.BuildNumber);
			}
			catch (Exception exception)
			{
				Debug.Print($"Version check failed: {exception.Message}");
			}

			AppSettings.LastUpdateTime = DateTime.Now;
		}
	}
	private ScrollViewer GetScrollViewer(UIElement element)
	{
		if (element == null) return null;

		ScrollViewer retour = null;
		for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element) && retour == null; i++)
		{
			if (VisualTreeHelper.GetChild(element, i) is ScrollViewer viewer)
			{
				retour = viewer;
			}
			else
			{
				retour = GetScrollViewer(VisualTreeHelper.GetChild(element, i) as UIElement);
			}
		}
		return retour;
	}

	#endregion

	#region Events

	private void Window_Initialized(object sender, System.EventArgs e)
	{
		FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

		LoadSettings();
		CheckForNewVersion();
	}

	private void Window_ContentRendered(object sender, System.EventArgs e)
	{
		if (Environment.GetCommandLineArgs().Length > 1)
		{
			ViewModel.ImportPath = Environment.GetCommandLineArgs()[1];
		}

		importFilesScrollViewer = GetScrollViewer(DataGridImportFiles);
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
		var oldImportConfigurations = new ObservableCollection<ImportConfiguration>();
		foreach (ImportConfiguration i in ViewModel.ImportConfigurations)
		{
			oldImportConfigurations.Add(new ImportConfiguration() { DestinationFormat = i.DestinationFormat, Files = i.Files });
		}

		var oldLibraryRootDirectories = new ObservableCollection<LibraryRoot>();
		foreach (LibraryRoot l in ViewModel.LibraryRootDirectories)
		{
			oldLibraryRootDirectories.Add(new LibraryRoot(l.Path));
		}

		ConfigurationWindow configurationWindow = new ConfigurationWindow() { Owner = this, DataContext = ViewModel };
		configurationWindow.ShowDialog();

		if (configurationWindow.DialogResult == true)
		{
			SaveSettings();
		}
		else
		{
			ViewModel.ImportConfigurations = new ObservableCollection<ImportConfiguration>(oldImportConfigurations);
			ViewModel.LibraryRootDirectories = new ObservableCollection<LibraryRoot>(oldLibraryRootDirectories);
		}

		ViewModel.Refresh();
	}

	private void ButtonFindFiles_Click(object sender, RoutedEventArgs e)
	{
		if (Directory.Exists(ViewModel.ImportPath))
		{
			ViewModel.ImportFiles.Clear();

			ViewModel.GuiFrozen = true;

			ProgressBarWork.Value = 0;

			BackgroundAnalyzeImport.progressHandler = new Progress<Tuple<float, string, List<FileItem>>>(FindFilesStatusUpdate);
			Task.Run(() => BackgroundAnalyzeImport.FindFiles(ViewModel.ImportPath)).ContinueWith(TaskDone, TaskScheduler.FromCurrentSynchronizationContext());
		}
		else
		{
			MessageBox.Show("Import folder does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	private void FindFilesStatusUpdate(Tuple<float, string, List<FileItem>> progress)
	{
		ProgressBarWork.Value = progress.Item1 * 100;
		ProgressLabel.Text = progress.Item2;

		for (int i = ViewModel.ImportFiles.Count; i < progress.Item3.Count; i++)
		{
			ViewModel.ImportFiles.Add(progress.Item3[i]);
		}
	}

	private void TaskDone(Task task)
	{
		ViewModel.GuiFrozen = false;
		ProgressLabel.Text = "";
		ButtonFindFiles.Focus();
	}

	private void ButtonCopy_Click(object sender, RoutedEventArgs e)
	{
		List<FileItem> filesToCopy = [];

		foreach (FileItem f in ViewModel.ImportFiles)
		{
			if (f.Selected)
			{
				filesToCopy.Add(f);
			}
		}

		if (filesToCopy.Count == 0)
		{
			MessageBox.Show("No files selected for import.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		ViewModel.GuiFrozen = true;

		ProgressBarWork.Value = 0;

		BackgroundPerformImport.progressHandler = new Progress<Tuple<float, string>>(CopyFilesStatusUpdate);
		Task.Run(() => BackgroundPerformImport.CopyFiles(filesToCopy)).ContinueWith(TaskDone, TaskScheduler.FromCurrentSynchronizationContext());
	}

	private void CopyFilesStatusUpdate(Tuple<float, string> progress)
	{
		ProgressBarWork.Value = progress.Item1 * 100;
		ProgressLabel.Text = progress.Item2;
	}

	private void AddShellExtension_Checked(object sender, RoutedEventArgs e)
	{
		using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(regPath))
		{
			key.SetValue(null, "Import With Photo Mover");
		}
		using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(regPath + "\\command"))
		{
			key.SetValue(null, shellExecutePath);
		}
	}

	private void AddShellExtension_Unchecked(object sender, RoutedEventArgs e)
	{
		Registry.ClassesRoot.DeleteSubKeyTree(regPath);
	}

	private void Hyperlink_OpenHomepage(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
	{
		Process.Start(new ProcessStartInfo(e.Uri.ToString()) { UseShellExecute = true });
		e.Handled = true;
	}

	private void WaitPanel_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
	{
		int lines = SystemParameters.WheelScrollLines * e.Delta / 120;

		importFilesScrollViewer.ScrollToVerticalOffset(importFilesScrollViewer.VerticalOffset - lines);
	}

	#region Commands

	private void CommandExit_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
	{
		this.Close();
	}

	private void CommandAbout_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
	{
		CheckForNewVersion(true);

		AboutWindow aboutWindow = new AboutWindow() { Owner = this, DataContext = ViewModel };
		aboutWindow.ShowDialog();
	}

	private void CommandOpenSourceFolder_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
	{
		e.CanExecute = DataGridImportFiles.SelectedItem != null && ((FileItem)DataGridImportFiles.SelectedItem).SourcePath != "";
	}

	private void CommandOpenSourceFolder_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
	{
		ShowInExplorer(((FileItem)DataGridImportFiles.SelectedItem).SourcePath);
	}

	private void CommandOpenDestinationFolder_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
	{
		e.CanExecute = DataGridImportFiles.SelectedItem is FileItem selectedItem && Directory.Exists(Path.GetDirectoryName(selectedItem.DestinationPath));
	}

	private void CommandOpenDestinationFolder_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
	{
		ShowInExplorer(((FileItem)DataGridImportFiles.SelectedItem).DestinationPath);
	}

	private void CommandCancelWork_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
	{
		BackgroundAnalyzeImport.Cancel();
		BackgroundPerformImport.Cancel();
	}

	#endregion

	#endregion

}
