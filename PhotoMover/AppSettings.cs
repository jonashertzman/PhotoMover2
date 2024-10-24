using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Xml;

namespace PhotoMover;

public static class AppSettings
{

	#region Members

#if DEBUG
	private const string SETTINGS_FILE_NAME = "DebugSettings.xml";
#else
	private const string SETTINGS_FILE_NAME = "Settings.xml";
#endif

	private static readonly string AppDataDirectory = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhotoMover2");
	public static readonly string SettingsPath = Path.Combine(AppDataDirectory, SETTINGS_FILE_NAME);
	public static readonly string LogPath = Path.Combine(AppDataDirectory, "PhotoMover.log");

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

	public static string Id
	{
		get { return Settings.Id; }
	}

	public static DateTime LastUpdateTime
	{
		get { return Settings.LastUpdateTime; }
		set { Settings.LastUpdateTime = value; }
	}

	public static ObservableCollection<DateFormat> DateFormats = [];

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
		DataContractSerializer xmlSerializer = new DataContractSerializer(typeof(SettingsData));

		if (File.Exists(SettingsPath))
		{
			using var xmlReader = XmlReader.Create(SettingsPath);

			try
			{
				Settings = (SettingsData)xmlSerializer.ReadObject(xmlReader);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		Settings ??= new SettingsData();
	}

	internal static void WriteSettingsToDisk()
	{
		try
		{
			DataContractSerializer xmlSerializer = new DataContractSerializer(typeof(SettingsData));
			var xmlWriterSettings = new XmlWriterSettings { Indent = true, IndentChars = " " };

			Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));

			using var xmlWriter = XmlWriter.Create(SettingsPath, xmlWriterSettings);

			xmlSerializer.WriteObject(xmlWriter, Settings);
		}
		catch (Exception e)
		{
			MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	#endregion

}
