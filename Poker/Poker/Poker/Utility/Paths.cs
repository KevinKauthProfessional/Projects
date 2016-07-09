namespace Poker.Utility
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public static class Paths
	{
		public static string AbsoluteRoot
		{
			get
			{
				string currentDirectory = Environment.CurrentDirectory;
				string[] filesInDirectory = Directory.GetFiles(currentDirectory);

				foreach (string file in filesInDirectory)
				{
					if (string.Compare(Path.GetExtension(file), ".root", StringComparison.OrdinalIgnoreCase) == 0)
					{
						return currentDirectory;
					}
				}

				Environment.CurrentDirectory = Directory.GetParent(Environment.CurrentDirectory).FullName;
				return AbsoluteRoot;
			}
		}

		public static string EvaluatedHandDatFilePath
		{
			get
			{
				return Path.Combine(AbsoluteRoot, @"Utility\HandEvaluation\HandRanks.dat");
			}
		}

		public static string PathForCardName(string cardName)
		{
			string result = Path.Combine(AbsoluteRoot, string.Format(@"Utility\Cards\images\{0}.png", cardName));
			if (!File.Exists(result))
			{
				throw new IOException(string.Format("Could not find file for card {0} at path \"{1}\"", cardName, result));
			}

			return result;
		}
	}
}
