using System.Windows;

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

		#region Events

		private void Window_Initialized(object sender, System.EventArgs e)
		{

		}

		private void Window_ContentRendered(object sender, System.EventArgs e)
		{

		}

		private void Window_Closed(object sender, System.EventArgs e)
		{

		}

		private void ButtonAnalyze_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.ImportFiles.Add(new ImportFile() { SourcePath = "A", DestinationPath = "B", Status = "C" });
		}

		#endregion

	}
}
