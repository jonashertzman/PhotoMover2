using System.Windows.Input;

namespace PhotoMover
{
	public static class Commands
	{

		public static readonly RoutedUICommand Exit = new RoutedUICommand("Exit", "Exit", typeof(Commands),
			new InputGestureCollection()
			{
					new KeyGesture(Key.F4, ModifierKeys.Alt)
			}
		);

		public static readonly RoutedUICommand About = new RoutedUICommand("About", "About", typeof(Commands));

		public static readonly RoutedUICommand CancelWork = new RoutedUICommand("Cancel Compare", "CancelCompare", typeof(Commands),
			new InputGestureCollection()
			{
				new KeyGesture(Key.Escape)
			}
		);

		public static readonly RoutedUICommand OpenContainingFolder = new RoutedUICommand("Open Containing Folder", "OpenContainingFolder", typeof(Commands));

	}
}
