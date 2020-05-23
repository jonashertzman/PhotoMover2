﻿using System;
using System.Diagnostics;
using System.Windows;

namespace PhotoMover
{
	public partial class AboutWindow : Window
	{

		public AboutWindow()
		{
			InitializeComponent();
		}

		private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(AppSettings.HOMEPAGE));
			e.Handled = true;
		}

		private void Feedback_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			MainWindowViewModel viewModel = DataContext as MainWindowViewModel;

			string mailto = Uri.EscapeUriString($"mailto:jonashertzmansoftware@gmail.com?Subject={viewModel.FullApplicationName}&Body=Hello");

			Process.Start(new ProcessStartInfo(mailto));
			e.Handled = true;
		}

	}
}
