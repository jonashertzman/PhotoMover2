using System.IO;
using System.Windows;

namespace PhotoMover;

public static class Utils
{

	public static bool DirectoryAllowed(string path)
	{
		try
		{
			Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static void HideMinimizeAndMaximizeButtons(Window window)
	{
		window.SourceInitialized += (s, e) =>
		{
			IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
			int style = WinApi.GetWindowLong(hwnd, WinApi.GWL_STYLE);

			_ = WinApi.SetWindowLong(hwnd, WinApi.GWL_STYLE, style & ~WinApi.WS_MAXIMIZEBOX & ~WinApi.WS_MINIMIZEBOX);
		};
	}

}


public static class DateTimeHelper
{

	public static string Yesterday
	{
		get { return string.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(-1)); }
	}

	public static string YearYesterday
	{
		get { return string.Format("{0:yyyy}", DateTime.Now.AddDays(-1)); }
	}

	public static string MonthYesterday
	{
		get { return string.Format("{0:MM}", DateTime.Now.AddDays(-1)); }
	}

	public static string DayYesterday
	{
		get { return string.Format("{0:dd}", DateTime.Now.AddDays(-1)); }
	}


	public static string Today
	{
		get { return string.Format("{0:yyyy-MM-dd}", DateTime.Now); }
	}

	public static string YearToday
	{
		get { return string.Format("{0:yyyy}", DateTime.Now); }
	}

	public static string MonthToday
	{
		get { return string.Format("{0:MM}", DateTime.Now); }
	}

	public static string DayToday
	{
		get { return string.Format("{0:dd}", DateTime.Now); }
	}


	public static string MonthYear
	{
		get { return string.Format("{0:MMMM yyyy}", DateTime.Now); }
	}

	public static string PreviousMonthYear
	{
		get { return string.Format("{0:MMMM yyyy}", DateTime.Now.AddMonths(-1)); }
	}


}