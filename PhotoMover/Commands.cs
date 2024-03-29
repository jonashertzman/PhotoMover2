﻿using System.Windows.Input;

namespace PhotoMover;

public static class Commands
{

	public static readonly RoutedUICommand Exit = new RoutedUICommand("Exit", "Exit", typeof(Commands),
		[new KeyGesture(Key.F4, ModifierKeys.Alt)]
	);

	public static readonly RoutedUICommand About = new RoutedUICommand("About", "About", typeof(Commands));

	public static readonly RoutedUICommand CancelWork = new RoutedUICommand("Cancel Compare", "CancelCompare", typeof(Commands),
		[new KeyGesture(Key.Escape)]
	);

	public static readonly RoutedUICommand OpenSourceFolder = new RoutedUICommand("Open Source Folder", "OpenSourceFolder", typeof(Commands));
	public static readonly RoutedUICommand OpenDestinationFolder = new RoutedUICommand("Open Destination Folder", "OpenDestinationFolder", typeof(Commands));

}
