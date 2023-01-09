namespace PhotoMover;

class CommandLineOptions
{
	#region Members

	readonly string Help = "--help";
	readonly string ShortHelp = "-h";
	readonly string Console = "--console";
	readonly string ShortConsole = "-c";
	readonly string Delete = "--delete";
	readonly string ShortDelete = "-d";

	#endregion

	#region Constructor

	public CommandLineOptions(string[] args)
	{
		ImportPath = args.Length > 0 ? args[0] : "";

		Dictionary<string, string> namedArgs = ParseArgs(args);

		ShowHelp = namedArgs.ContainsKey(Help) || namedArgs.ContainsKey(ShortHelp);
		RunInConsole = namedArgs.ContainsKey(Console) || namedArgs.ContainsKey(ShortConsole);
		DeleteSourceFiles = namedArgs.ContainsKey(Delete) || namedArgs.ContainsKey(ShortDelete);
	}

	#endregion

	#region Properties

	public string ImportPath { get; private set; }

	public bool ShowHelp { get; private set; }

	public bool RunInConsole { get; private set; }

	public bool DeleteSourceFiles { get; private set; }

	#endregion

	#region Methods

	private Dictionary<string, string> ParseArgs(string[] args)
	{
		Dictionary<string, string> namedArgs = new();

		for (int i = 0; i < args.Length; i++)
		{
			string key = args[i];
			string value = "";
			if (key.StartsWith("--"))
			{
				if (args.Length > i + 1 && !args[i + 1].StartsWith("-"))
				{
					value = args[i + 1];
					i++;
				}
				namedArgs.Add(key.ToLower(), value);
			}
			else if (key.StartsWith("-"))
			{
				foreach (char c in key[1..])
				{
					namedArgs.Add($"-{c}", "");
				}

			}
		}

		return namedArgs;
	}

	#endregion

}
