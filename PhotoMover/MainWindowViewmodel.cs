﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace PhotoMover
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{

		#region Constructor

		public MainWindowViewModel()
		{

		}

		#endregion

		#region Properties

		public string Title
		{
			get { return "Photo Mover"; }
		}

		public string Version
		{
			get { return "2 - Alpha 1"; }
		}

		public string BuildNumber
		{
			get
			{
				DateTime buildDate = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;
				return $"{buildDate.ToString("yy")}{buildDate.DayOfYear}";
			}
		}

		public string FullName
		{
			get { return $"{Title} {Version}  (Build {BuildNumber})"; }
		}

		bool guiFrozen = false;
		public bool GuiFrozen
		{
			get { return guiFrozen; }
			set { guiFrozen = value; OnPropertyChanged(nameof(GuiFrozen)); }
		}

		string importPath = "";
		public string ImportPath
		{
			get { return importPath; }
			set { importPath = value; OnPropertyChanged(nameof(ImportPath)); }
		}

		public ObservableCollection<ImportConfiguration> ImportConfigurations
		{
			get { return AppSettings.ImportConfigurations; }
			set { AppSettings.ImportConfigurations = value; OnPropertyChanged(nameof(ImportConfigurations)); }
		}

		public ObservableCollection<LibraryRoot> LibraryRootDirectories
		{
			get { return AppSettings.LibraryRootDirectories; }
			set { AppSettings.LibraryRootDirectories = value; OnPropertyChanged(nameof(LibraryRootDirectories)); }
		}

		public string ImportConfigurationsLabel
		{
			get
			{
				StringBuilder s = new StringBuilder();

				foreach (ImportConfiguration c in ImportConfigurations)
				{
					if (s.Length > 0)
					{
						s.Append("  |  ");
					}
					s.Append($"{c.Files} -> {c.DestinationFormat}");
				}

				return s.ToString();
			}
		}

		ObservableCollection<FileItem> importFiles = new ObservableCollection<FileItem>();
		public ObservableCollection<FileItem> ImportFiles
		{
			get { return importFiles; }
			set { importFiles = value; OnPropertyChanged(nameof(ImportFiles)); }
		}
		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		#endregion

	}
}
