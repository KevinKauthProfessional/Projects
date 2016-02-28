//------------------------------------------------------------------
// <copyright file="PathUtility.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.Utilities
{
	using System;
	using System.IO;
	using UnityEngine;

	/// <summary>
	/// Path utility.
	/// </summary>
	public static class PathUtility
	{
		/// <summary>
		/// The DNA file extension.
		/// </summary>
		public const string DNAFileExtension = ".dna";

		private const string GenePoolDirectoryName = "GenePool";

		/// <summary>
		/// Gets the gene pool directory info.
		/// </summary>
		/// <returns>The gene pool directory info.</returns>
		public static DirectoryInfo GetGenePoolDirectoryInfo()
		{
			string path = Path.Combine (Application.persistentDataPath, GenePoolDirectoryName);
			if (!Directory.Exists (path)) 
			{
				Directory.CreateDirectory(path);
			}

			return new DirectoryInfo(path);
		}

		/// <summary>
		/// Generates the new DNA file path.
		/// </summary>
		/// <returns>The new DNA file path.</returns>
		/// <param name="winLossCount">Win loss count.</param>
		public static string GenerateNewDNAFilePath(int winLossCount)
		{
			if (winLossCount <= 0)
			{
				LogUtility.LogError("Creating DNA file with non positive win loss count");
			}

			for (int i = 0; i < 2000; ++i)
			{
				string fileName = string.Format (
					"{0}_{1}_{2}{3}",
					winLossCount,
					CommonHelperMethods.ConvertToUnixTimestamp (DateTime.Now),
					i,
					DNAFileExtension);

				string path = Path.Combine (GetGenePoolDirectoryInfo ().FullName, fileName);
				if (!File.Exists(path))
				{
					return path;
				}
			}

			LogUtility.LogErrorFormat("Unable to produce new file name");
			return string.Empty;
		}
	}
}

