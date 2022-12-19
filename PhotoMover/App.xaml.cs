using System.Windows;

namespace PhotoMover;

public partial class App : Application
{

	private void Application_Startup(object sender, StartupEventArgs e)
	{
		Dictionary<string, string> namedArgs = new();

		for (int i = 0; i < e.Args.Length; i++)
		{
			string key = e.Args[i];
			string value = "";
			if (key.StartsWith("--"))
			{

				if (e.Args.Length > i + 1 && !e.Args[i + 1].StartsWith("--"))
				{
					value = e.Args[i + 1];
				}
				namedArgs.Add(key, value);
			}
		}

		if (namedArgs.ContainsKey("--no_gui"))
		{
			WinApi.AttachConsole(WinApi.ATTACH_PARENT_PROCESS);
			Console.WriteLine("funkar");
		}
		else
		{
			var mainWindow = new MainWindow();
			mainWindow.Show();
		}
	}

}
