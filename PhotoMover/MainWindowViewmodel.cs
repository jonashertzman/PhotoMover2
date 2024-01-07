using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Security.Principal;
using System.Text;

namespace PhotoMover;

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
		get { return "2.0"; }
	}

	public string BuildNumber
	{
		get
		{
			DateTime buildDate = new FileInfo(Process.GetCurrentProcess().MainModule.FileName).LastWriteTime;
			return $"{buildDate:yy}{buildDate.DayOfYear:D3}";
		}
	}

	bool newBuildAvailable = false;
	public bool NewBuildAvailable
	{
		get { return newBuildAvailable; }
		set { newBuildAvailable = value; OnPropertyChanged(nameof(NewBuildAvailable)); }
	}

	public bool IsAdministrator
	{
		get
		{
			WindowsIdentity wi = WindowsIdentity.GetCurrent();
			WindowsPrincipal wp = new WindowsPrincipal(wi);

			return wp.IsInRole(WindowsBuiltInRole.Administrator);
		}
	}

	public string ApplicationName
	{
		get { return $"{Title} {Version}"; }
	}

	public string FullApplicationName
	{
		get { return $"{Title} {Version} (Build {BuildNumber})"; }
	}

	private bool shellExtensionsInstalled;
	public bool ShellExtensionsInstalled
	{
		get { return shellExtensionsInstalled; }
		set { shellExtensionsInstalled = value; OnPropertyChanged(nameof(ShellExtensionsInstalled)); }
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
				s.Append(c.Description);
			}

			return s.ToString();
		}
	}

	ObservableCollection<FileItem> importFiles = [];
	public ObservableCollection<FileItem> ImportFiles
	{
		get { return importFiles; }
		set { importFiles = value; OnPropertyChanged(nameof(ImportFiles)); }
	}

	public ObservableCollection<DateFormat> DateFormats
	{
		get { return AppSettings.DateFormats; }
	}

	#endregion

	public void Refresh()
	{
		OnPropertyChanged(nameof(ImportConfigurationsLabel));
	}

	#region INotifyPropertyChanged

	public event PropertyChangedEventHandler PropertyChanged;

	public void OnPropertyChanged(string name)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}

	#endregion

}
