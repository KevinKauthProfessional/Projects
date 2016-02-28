//------------------------------------------------------------------
// <copyright file="GenePoolManager.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.GameLogic
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	using AssemblyCSharp.Scripts.UnityGameObjects;
	using AssemblyCSharp.Scripts.Utilities;
	using UnityEngine;

	/// <summary>
	/// Gene pool manager.
	/// </summary>
	public static class GenePoolManager
	{
		private const int GenePoolDirectoryFileLimit = 20;

		private static IDictionary<string, GeneData> teamNameToGeneDataDic = new Dictionary<string, GeneData>();

		/// <summary>
		/// Gets the data.
		/// </summary>
		/// <returns>The data.</returns>
		/// <param name="teamName">Team name.</param>
		public static GeneData GetData(string teamName)
		{
			lock (CommonHelperMethods.GlobalFileIOLock)
			{
				GeneData result;
				if (!teamNameToGeneDataDic.TryGetValue (teamName, out result)) 
				{
					LogUtility.LogErrorFormat("Team {0} was not registered", teamName);
				}

				return result;
			}
		}

		/// <summary>
		/// Registers the team.
		/// </summary>
		/// <param name="teamName">Team name.</param>
		public static void RegisterTeam(string teamName)
		{
			lock (CommonHelperMethods.GlobalFileIOLock)
			{
				if (!teamNameToGeneDataDic.ContainsKey (teamName)) 
				{
					GeneData data = new GeneData(teamName, ChooseDNAParentFile_Random());
					teamNameToGeneDataDic.Add(teamName, data);
				}
			}

			TeamManager.ResetTeam(teamName);
		}

		/// <summary>
		/// Refreshs the team DNA upon extinction.
		/// </summary>
		/// <param name="extinctTeamName">Extinct team name.</param>
		public static void RefreshTeamDNA()
		{
			lock (CommonHelperMethods.GlobalFileIOLock)
			{
				if (teamNameToGeneDataDic == null || teamNameToGeneDataDic.Count == 0)
				{
					LogUtility.LogError("GenePoolMamanger has no registered teams");
				}

				// Update parent files with win loss info
				UpdateGenePoolFilesUponExtinction ();

				// Reward survivors with reproduction
				AddSurvivorsToGenePool ();

				// Wipe grid
				GameObjectGrid.KillAllObjects();

				// Assign new DNA to the teams
				IList<string> teamNameList = new List<string>(teamNameToGeneDataDic.Keys);
				teamNameToGeneDataDic.Clear ();
				foreach (string teamName in teamNameList) 
				{
					RegisterTeam(teamName);
				}
			}
		}

		private static void AddSurvivorsToGenePool()
		{
			foreach (KeyValuePair<string, GeneData> keyValuePair in teamNameToGeneDataDic) 
			{
				string teamName = keyValuePair.Key;
				GeneData geneData = keyValuePair.Value;

				int teamPopCount = TeamManager.GetTeamPopulationCount(teamName);
				int winCount = 1 + teamPopCount - AverageTeamPopulationCount();

				if (winCount <= 0)
				{
					// Losers don't get laid
					continue;
				}

				// Have to place a cap on number of files, otherwise this would flood the machine
				if (PathUtility.GetGenePoolDirectoryInfo ().GetFiles().Length < GenePoolDirectoryFileLimit)
				{
					LogUtility.LogInfoFormat(
						"{0} writes DNA to disk",
						teamName);

					geneData.DNA.WriteToDisk(PathUtility.GenerateNewDNAFilePath(winCount));
				}
				else
				{
					LogUtility.LogInfoFormat(
						"Gene Pool has reached max size of {0} files",
						GenePoolDirectoryFileLimit);
				}
			}
		}

		private static void UpdateGenePoolFilesUponExtinction()
		{
			Dictionary<string, int> fileNameToModificationDic = new Dictionary<string, int> ();

			// Tabulate the new win loss info for the parent files
			foreach(KeyValuePair<string, GeneData> keyValuePair in teamNameToGeneDataDic)
			{
				string teamName = keyValuePair.Key;
				string parentFileName = keyValuePair.Value.ParentDNAFile.FullName;

				int teamPopCount = TeamManager.GetTeamPopulationCount(teamName);
				int modification = teamPopCount - AverageTeamPopulationCount();
				
				if (fileNameToModificationDic.ContainsKey(parentFileName))
				{
					// Update
					fileNameToModificationDic[parentFileName] += modification;
				}
				else
				{
					// Create new entry
					int startingParentWinLoss = ParseWinLoss(keyValuePair.Value.ParentDNAFile);
					fileNameToModificationDic.Add(parentFileName, startingParentWinLoss + modification);
				}
			}

			// Move or delete the parent files as appropriate
			foreach (KeyValuePair<string, int> keyValuePair in fileNameToModificationDic) 
			{
				string oldFileName = keyValuePair.Key;
				int winLossCount = keyValuePair.Value;

				if (winLossCount <= 0)
				{
					// Delete losers
					LogUtility.LogInfoFormat(
						"Removing {0} from gene pool",
						oldFileName);

					File.Delete(oldFileName);
					continue;
				}

				// Move winners to new file name
				string newFileName = PathUtility.GenerateNewDNAFilePath(winLossCount);
				File.Move(oldFileName, newFileName);

				LogUtility.LogInfoFormat(
					"Gene pool file {0} became {1}",
					oldFileName,
					newFileName);
			}
		}

		private static IList<FileInfo> GetNotTakenParentFiles()
		{
			DirectoryInfo genePoolDirectory = PathUtility.GetGenePoolDirectoryInfo();
			FileInfo[] files = genePoolDirectory.GetFiles();
			
			// Ensure no repeats
			List<FileInfo> candidates = new List<FileInfo>();
			foreach(FileInfo file in files)
			{
				if (!CommonHelperMethods.StringsAreEqual(file.Extension, PathUtility.DNAFileExtension))
				{
					// Ingnore any other files that might get in there
					continue;
				}

				if (teamNameToGeneDataDic.Count > 0)
				{
					foreach(GeneData data in teamNameToGeneDataDic.Values)
					{
						if (CommonHelperMethods.StringsAreEqual(data.ParentDNAFile.Name, file.Name))
						{
							continue;
						}

						candidates.Add(file);
					}
				}
				else
				{
					candidates.Add(file);
				}
			}

			if (candidates.Count == 0) 
			{
				string defaultDNAFilePath = PathUtility.GenerateNewDNAFilePath(1);
				candidates.Add(new FileInfo(defaultDNAFilePath));
			}

			return candidates;
		}

		private static FileInfo ChooseDNAParentFile_Random()
		{
			IList<FileInfo> candidates = GetNotTakenParentFiles();
			int choiceIndex = CommonHelperMethods.GetRandomPositiveInt0ToValue(candidates.Count - 1);
			return candidates[choiceIndex];
		}

		private static FileInfo ChooseDNAParentFile_WeightedTowardsWinners()
		{
			// Prevent teams from having the same parent file
			IList<FileInfo> candidates = GetNotTakenParentFiles();

			int winLossTotal = 0;
			foreach (FileInfo file in candidates) 
			{
				int fileWinLoss = ParseWinLoss(file);

				// Delete losers
				if (fileWinLoss <= 0)
				{
					throw new InvalidOperationException("Should never get here");
				}

				winLossTotal += fileWinLoss;
			}

			// This random selection favors higher win count files
			int randInt = CommonHelperMethods.GetRandomPositiveInt0ToValue (winLossTotal);
			winLossTotal = 0;
			foreach (FileInfo file in candidates) 
			{				
				int fileWinLoss = ParseWinLoss(file);
				winLossTotal += fileWinLoss;

				if (winLossTotal >= randInt)
				{
					return file;
				}
			}

			throw new InvalidOperationException ("Should never get here");
		}

		private static int ParseWinLoss(FileInfo file)
		{
			string[] fileNameSplit = file.Name.Split ('_');

			int result;
			if (!int.TryParse (fileNameSplit [0], out result)) 
			{
				throw new InvalidOperationException(string.Format("File name: {0} could not be parsed", file.Name));
			}

			return result;
		}

		private static int AverageTeamPopulationCount()
		{
			int total = 0;
			int count = 0;
			foreach(string teamName in teamNameToGeneDataDic.Keys)
			{				
				total += TeamManager.GetTeamPopulationCount(teamName);
				++count;
			}
			
			// Will throw if count == 0
			return total / count;
		}

		private static int AverageOtherTeamPopulationCount(string excludedTeam)
		{
			int total = 0;
			int count = 0;
			foreach(string teamName in teamNameToGeneDataDic.Keys)
			{
				if (CommonHelperMethods.StringsAreEqual(teamName, excludedTeam))
				{
					continue;
				}

				total += TeamManager.GetTeamPopulationCount(teamName);
				++count;
			}

			// Will throw if count == 0
			return total / count;
		}
	}
}

