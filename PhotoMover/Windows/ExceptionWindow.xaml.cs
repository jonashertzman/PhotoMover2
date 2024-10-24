using System.ComponentModel;
using System.Windows;

namespace PhotoMover;

public partial class ExceptionWindow : Window, INotifyPropertyChanged
{

	#region Constructor

	public ExceptionWindow()
	{
		InitializeComponent();
		DataContext = this;
	}

	#endregion

	#region Properties

	string exceptionType;
	public string ExceptionType
	{
		get { return exceptionType; }
		set { exceptionType = value; OnPropertyChanged(nameof(ExceptionType)); }
	}

	string exceptionMessage;
	public string ExceptionMessage
	{
		get { return exceptionMessage; }
		set { exceptionMessage = value; OnPropertyChanged(nameof(ExceptionMessage)); }
	}

	string source;
	public string Source
	{
		get { return source; }
		set { source = value; OnPropertyChanged(nameof(Source)); }
	}

	string stackTrace;
	public string StackTrace
	{
		get { return stackTrace; }
		set { stackTrace = value; OnPropertyChanged(nameof(StackTrace)); }
	}

	#endregion

	#region Events

	private void CloseButton_Click(object sender, RoutedEventArgs e)
	{
		this.Close();
	}

	private void OpenLogFileButton_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			using Process p = new();

			p.StartInfo.FileName = AppSettings.LogPath;
			p.StartInfo.ErrorDialog = true;
			p.StartInfo.UseShellExecute = true;
			p.Start();
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Cannot open log file. {ex.Message}");
		}
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
