using System;
using System.ComponentModel;

namespace PhotoMover
{
	public class ImportConfiguration : INotifyPropertyChanged
	{

		#region Constructor

		public ImportConfiguration()
		{

		}

		#endregion

		#region Properties

		string files = "";
		public string Files
		{
			get { return files; }
			set { files = value; OnPropertyChanged(nameof(Files)); }
		}

		string destinationFormat = "";
		public string DestinationFormat
		{
			get { return destinationFormat; }
			set { destinationFormat = value; OnPropertyChanged(nameof(DestinationFormat)); }
		}

		#endregion

		#region Methods

		public string GetDestinationFolder(DateTime date)
		{
			string formatedString = destinationFormat;

			formatedString = ReplaceDatePattern(formatedString, date, "yyyy");
			formatedString = ReplaceDatePattern(formatedString, date, "MMMM");
			formatedString = ReplaceDatePattern(formatedString, date, "MMM");
			formatedString = ReplaceDatePattern(formatedString, date, "MM");
			formatedString = ReplaceDatePattern(formatedString, date, "dd");

			return formatedString;
		}

		private string ReplaceDatePattern(string formatedString, DateTime date, string formatPattern)
		{
			while (formatedString.Contains(formatPattern))
			{
				formatedString = formatedString.Replace(formatPattern, string.Format("{0:" + formatPattern + "}", date));
			}
			return formatedString;
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
}