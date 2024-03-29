﻿using System.ComponentModel;
using System.IO;

namespace PhotoMover;

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
		set { files = value; OnPropertyChanged(nameof(Files)); OnPropertyChanged(nameof(Valid)); }
	}

	string destinationFormat = "";
	public string DestinationFormat
	{
		get { return destinationFormat; }
		set { destinationFormat = value; OnPropertyChanged(nameof(DestinationFormat)); OnPropertyChanged(nameof(Valid)); }
	}

	public string Description
	{
		get { return $"{Files} -> {DestinationFormat}"; }
	}

	public bool Valid
	{
		get
		{
			if (string.IsNullOrEmpty(files) || string.IsNullOrEmpty(destinationFormat))
			{
				return false;
			}
			return DestinationIsValid();
		}
	}

	#endregion

	#region Methods

	public string GetDestinationFolder(DateTime date)
	{
		string formatedString = destinationFormat;

		foreach (DateFormat dateFormat in AppSettings.DateFormats)
		{
			while (formatedString.Contains(dateFormat.PlaceHolder))
			{
				formatedString = formatedString.Replace(dateFormat.PlaceHolder, string.Format("{0:" + dateFormat.Format + "}", date));
			}
		}

		return formatedString;
	}

	private bool DestinationIsValid()
	{
		try
		{
			_ = new DirectoryInfo(GetDestinationFolder(DateTime.Now));
		}
		catch
		{
			return false;
		}

		return true;
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
