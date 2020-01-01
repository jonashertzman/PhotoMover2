using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Xml;

namespace PhotoMover
{
	public static class AppSettings
	{

		#region Members

		private const string SETTINGS_DIRECTORY = "PhotoMover2";
		private const string SETTINGS_FILE_NAME = "Settings.xml";

		private static SettingsData Settings = new SettingsData();

		#endregion

		#region Constructor

		static AppSettings()
		{
			DateFormats.Add(new DateFormat("year", "yyyy", "Year"));
			DateFormats.Add(new DateFormat("month", "MM", "Month number"));
			DateFormats.Add(new DateFormat("short_month", "MMM", "Short month name"));
			DateFormats.Add(new DateFormat("long_month", "MMMM", "Long month name"));
			DateFormats.Add(new DateFormat("day", "dd", "Day"));
		}

		#endregion

		#region Properies

		public static ObservableCollection<DateFormat> DateFormats = new ObservableCollection<DateFormat>();

		public static ObservableCollection<ImportConfiguration> ImportConfigurations
		{
			get { return Settings.ImportConfigurations; }
			set { Settings.ImportConfigurations = value; }
		}

		public static ObservableCollection<LibraryRoot> LibraryRootDirectories
		{
			get { return Settings.LibraryRootDirectories; }
			set { Settings.LibraryRootDirectories = value; }
		}

		public static double PositionLeft
		{
			get { return Settings.PositionLeft; }
			set { Settings.PositionLeft = value; }
		}

		public static double PositionTop
		{
			get { return Settings.PositionTop; }
			set { Settings.PositionTop = value; }
		}

		public static double Width
		{
			get { return Settings.Width; }
			set { Settings.Width = value; }
		}

		public static double Height
		{
			get { return Settings.Height; }
			set { Settings.Height = value; }
		}

		public static WindowState WindowState
		{
			get { return Settings.WindowState; }
			set { Settings.WindowState = value; }
		}

		#endregion

		#region Methods

		internal static void ReadSettingsFromDisk()
		{
			string settingsPath = Path.Combine(Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), SETTINGS_DIRECTORY), SETTINGS_FILE_NAME);
			DataContractSerializer xmlSerializer = new DataContractSerializer(typeof(SettingsData));

			if (File.Exists(settingsPath))
			{
				using (var xmlReader = XmlReader.Create(settingsPath))
				{
					try
					{
						Settings = (SettingsData)xmlSerializer.ReadObject(xmlReader);
					}
					catch (Exception e)
					{
						MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}

			if (Settings == null)
			{
				Settings = new SettingsData();
			}
		}

		internal static void WriteSettingsToDisk()
		{
			try
			{
				string settingsPath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), SETTINGS_DIRECTORY);

				DataContractSerializer xmlSerializer = new DataContractSerializer(typeof(SettingsData));
				var xmlWriterSettings = new XmlWriterSettings { Indent = true, IndentChars = " " };

				if (!Directory.Exists(settingsPath))
				{
					Directory.CreateDirectory(settingsPath);
				}

				using (var xmlWriter = XmlWriter.Create(Path.Combine(settingsPath, SETTINGS_FILE_NAME), xmlWriterSettings))
				{
					xmlSerializer.WriteObject(xmlWriter, Settings);
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		#endregion

	}
}
