﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

		readonly string regPath = @"Folder\shell\photomover";
		readonly string shellexecutePath = $"\"{System.Reflection.Assembly.GetExecutingAssembly().Location}\" \"%1\"";

		#endregion

		#region Constructor

		public MainWindow()
		{
			InitializeComponent();

			DataContext = ViewModel;

			if (ViewModel.IsAdministrator)
			{
				ViewModel.ShellExtentionsInstalled = Registry.ClassesRoot.CreateSubKey(regPath + "\\command").GetValue("")?.ToString() == shellexecutePath;
			}

			AddShellExtention.Checked += AddShellExtention_Checked;
			AddShellExtention.Unchecked += AddShellExtention_Unchecked;
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
				Process.Start(Path.GetDirectoryName(sourcePath));
			}
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
			var oldLibraryRootDirectories = new ObservableCollection<LibraryRoot>(ViewModel.LibraryRootDirectories);

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
				MessageBox.Show("Import folder does not exist.", "Error");
			}
		}

		private void FindFilesStatusUpdate(Tuple<float, string, List<FileItem>> progress)
		{
			ProgressBarWork.Value = progress.Item1 * 100;
			ProgressLabel.Content = progress.Item2;

			for (int i = ViewModel.ImportFiles.Count; i < progress.Item3.Count; i++)
			{
				ViewModel.ImportFiles.Add(progress.Item3[i]);
			}
		}

		private void TaskDone(Task task)
		{
			ViewModel.GuiFrozen = false;
		}

		private void ButtonCopy_Click(object sender, RoutedEventArgs e)
		{
			List<FileItem> filesToCopy = new List<FileItem>();

			foreach (FileItem f in ViewModel.ImportFiles)
			{
				if (f.Selected)
				{
					filesToCopy.Add(f);
				}
			}

			if (filesToCopy.Count == 0)
			{
				MessageBox.Show("No files selected for import.", "Error");
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
			ProgressLabel.Content = progress.Item2;
		}

		private void AddShellExtention_Checked(object sender, RoutedEventArgs e)
		{
			using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(regPath))
			{
				key.SetValue(null, "Import With Photo Mover");
			}
			using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(regPath + "\\command"))
			{
				key.SetValue(null, shellexecutePath);
			}
		}

		private void AddShellExtention_Unchecked(object sender, RoutedEventArgs e)
		{
			Registry.ClassesRoot.DeleteSubKeyTree(regPath);
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
			FileItem selectedItem = DataGridImportFiles.SelectedItem as FileItem;

			e.CanExecute = selectedItem != null && Directory.Exists(Path.GetDirectoryName(selectedItem.DestinationPath));
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
}
