//------------------------------------------------------------------
// <copyright file="GenePoolManager.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.GameLogic
{
	using System;
	using System.Collections.Generic;

	using AssemblyCSharp.Scripts.UnityGameObjects;
	using Assets.Scripts.Utilities;
	using UnityEngine;

	/// <summary>
	/// Gene pool manager.
	/// </summary>
	public static class GenePoolManager
	{
		private const int GenePoolDirectoryFileLimit = 10;

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

                // Reward survivors with reproduction
                AddSurvivorsToGenePool();

                // Update parent files with win loss info
                UpdateGenePoolFilesUponExtinction ();

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
            int minWinCount = 1;
			foreach (KeyValuePair<string, GeneData> keyValuePair in teamNameToGeneDataDic) 
			{
				string teamName = keyValuePair.Key;
				GeneData geneData = keyValuePair.Value;

				int teamPopCount = TeamManager.GetTeamPopulationCount(teamName);
				int winCount = 1 + teamPopCount - AverageTeamPopulationCount();

				if (winCount < minWinCount)
				{
					// Losers don't get laid
					continue;
				}
                else
                {
                    minWinCount++;
                }

				// Have to place a cap on number of files, otherwise this would flood the machine
				if (FileIOManager.GetFileNames().Count < GenePoolDirectoryFileLimit)
				{
					LogUtility.LogInfoFormat(
						"{0} writes DNA to disk",
						teamName);

					geneData.DNA.PossiblyMutateAndWriteToDisk(FileIOManager.GenerateNewDNAFilePath(winCount));
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
				string parentFileName = keyValuePair.Value.ParentDNAFilePath;

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
					int startingParentWinLoss = ParseWinLoss(keyValuePair.Value.ParentDNAFilePath);
					fileNameToModificationDic.Add(parentFileName, startingParentWinLoss + modification);
				}
			}

            // Move or delete the parent files as appropriate
            int winLossTotal = 0;
			foreach (KeyValuePair<string, int> keyValuePair in fileNameToModificationDic) 
			{
				string oldFileName = keyValuePair.Key;
				int winLossCount = keyValuePair.Value;

                winLossTotal += winLossCount;
                int winLossAverage = (int)Math.Round((float)winLossTotal / (float)fileNameToModificationDic.Count);

				if (winLossCount < winLossAverage)
				{
					// Delete losers
					LogUtility.LogInfoFormat(
						"Removing {0} from gene pool",
						oldFileName);

					FileIOManager.Delete(oldFileName);
					continue;
				}
                else if (winLossCount >= winLossAverage)
                {
                    // Falsely inflate average to break ties
                    winLossTotal += winLossAverage;
                }

				// Move winners to new file name
				string newFileName = FileIOManager.GenerateNewDNAFilePath(winLossCount);
				FileIOManager.Move(oldFileName, newFileName);

				LogUtility.LogInfoFormat(
					"Gene pool file {0} became {1}",
					oldFileName,
					newFileName);
			}
		}

		private static IList<string> GetNotTakenParentFiles()
		{			
			List<string> fileNameList = FileIOManager.GetFileNames();
			
			// Ensure no repeats
			List<string> candidates = new List<string>();
			foreach(string fileName in fileNameList)
			{
				if (!CommonHelperMethods.StringContainsSubString(fileName, FileIOManager.DNAFileExtension))
				{
					// Ingnore any other files that might get in there
					continue;
				}

				if (teamNameToGeneDataDic.Count > 0)
				{
					foreach(GeneData data in teamNameToGeneDataDic.Values)
					{
						if (CommonHelperMethods.StringsAreEqual(data.ParentDNAFilePath, fileName))
						{
							continue;
						}

						candidates.Add(fileName);
					}
				}
				else
				{
					candidates.Add(fileName);
				}
			}

			if (candidates.Count == 0) 
			{
				string defaultDNAFilePath = FileIOManager.GenerateNewDNAFilePath(1);
				candidates.Add(defaultDNAFilePath);
			}

			return candidates;
		}

		private static string ChooseDNAParentFile_Random()
		{
			IList<string> candidates = GetNotTakenParentFiles();
			int choiceIndex = CommonHelperMethods.GetRandomPositiveInt0ToValue(candidates.Count - 1);
			return candidates[choiceIndex];
		}

		private static string ChooseDNAParentFile_WeightedTowardsWinners()
		{
			// Prevent teams from having the same parent file
			IList<string> candidates = GetNotTakenParentFiles();

			int winLossTotal = 0;
			foreach (string fileName in candidates) 
			{
				int fileWinLoss = ParseWinLoss(fileName);

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
			foreach (string fileName in candidates) 
			{				
				int fileWinLoss = ParseWinLoss(fileName);
				winLossTotal += fileWinLoss;

				if (winLossTotal >= randInt)
				{
					return fileName;
				}
			}

			throw new InvalidOperationException ("Should never get here");
		}

		private static int ParseWinLoss(string filePath)
		{
            string[] directorySplit = filePath.Split('\\');
			string[] fileNameSplit = directorySplit[directorySplit.Length - 1].Split ('_');

			int result;
			if (!int.TryParse (fileNameSplit [0], out result)) 
			{
				throw new InvalidOperationException(string.Format("File name: {0} could not be parsed", filePath));
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

