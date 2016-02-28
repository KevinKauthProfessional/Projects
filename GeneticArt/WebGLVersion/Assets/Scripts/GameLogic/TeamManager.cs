//------------------------------------------------------------------
// <copyright file="TeamManager.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.GameLogic
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	using AssemblyCSharp.Scripts.GameLogic;
	using AssemblyCSharp.Scripts.UnityGameObjects;
	using AssemblyCSharp.Scripts.Utilities;
	using UnityEngine;

	/// <summary>
	/// This class manages a "Team" of Ents.  Each member of a team shares the same
	/// genetic code and color scheme.  If a team goes extinct, the TeamManager is responsible 
	/// for selecting a new seed from the gene pool and restarting the colony.
	/// </summary>
	public class TeamManager
	{
		public string TeamName;
		public Material TeamMaterial;

		private static Dictionary<string, TeamManager> activeTeamDic = new Dictionary<string, TeamManager>();

		/// <summary>
		/// Determines if is team extinct the specified teamName.
		/// </summary>
		/// <returns><c>true</c> if is team extinct the specified teamName; otherwise, <c>false</c>.</returns>
		/// <param name="teamName">Team name.</param>
		public static bool TeamIsExtinct(string teamName)
		{
			int populationCount = GetTeamPopulationCount(teamName);
			return populationCount <= 0;
		}

		/// <summary>
		/// Gets the color of the team.
		/// </summary>
		/// <returns>The team color.</returns>
		/// <param name="teamName">Team name.</param>
		public static Color GetTeamColor(string teamName)
		{
			TeamManager manager;
			if (!activeTeamDic.TryGetValue(teamName, out manager))
			{
				LogUtility.LogErrorFormat(
					"Could not find team of name: {0}",
					teamName);
				return new Color();
			}

			return manager.TeamMaterial.color;
		}

		public static void ProcessExtinctionIfNeeded(string teamName)
		{
			if (!TeamIsExtinct(teamName))
			{
				return;
			}

			LogUtility.LogInfoFormat("{0} is extinct", teamName);

			lock (CommonHelperMethods.GlobalStateLock)
			{
				// Refresh everyone's DNA
				GenePoolManager.RefreshTeamDNA();
			}
		}

		/// <summary>
		/// Gets the team population count.
		/// </summary>
		/// <returns>The team population count.</returns>
		/// <param name="teamName">Team name.</param>
		public static int GetTeamPopulationCount(string teamName)
		{
			int result = 0;

			for (int x = 0; x < GameObjectGrid.GridWidth; ++x)
			for (int z = 0; z < GameObjectGrid.GridHeight; ++z)
			{
				GridPosition position = new GridPosition(x, z);
				if (!GameObjectGrid.PositionIsAlive(position))
				{
					continue;
				}

				GameObject obj = GameObjectGrid.GetObjectAt(position);
				if (ObjectIsOnTeam(obj, teamName))
				{
					result++;
				}
			}
			
			return result;
		}

		/// <summary>
		/// Objects the is on team.
		/// </summary>
		/// <returns><c>true</c>, if is on team was objected, <c>false</c> otherwise.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="teamName">Team name.</param>
		public static bool ObjectIsOnTeam(GameObject obj, string teamName)
		{
			EntBehaviorManager manager = obj.GetComponent<EntBehaviorManager> ();
			return CommonHelperMethods.StringsAreEqual(manager.TeamName, teamName);
		}

		/// <summary>
		/// Resets the team.
		/// </summary>
		/// <param name="teamName">Team name.</param>
		public static void ResetTeam(string teamName)
		{
			TeamManager manager;
			if (!activeTeamDic.TryGetValue(teamName, out manager))
			{
				LogUtility.LogErrorFormat(
					"Could not find team of name: {0}",
					teamName);
				return;
			}

			manager.ForcePlaceSeedEnt();
		}

		// Use this for initialization
		public TeamManager(Material teamMaterial)
		{
			this.TeamMaterial = teamMaterial;
			this.TeamName = teamMaterial.name;

			if (activeTeamDic.ContainsKey(this.TeamName))
			{
				LogUtility.LogErrorFormat(
					"Created more than one team with name: {0}",
					this.TeamName);
			}
			else
			{
				activeTeamDic.Add(this.TeamName, this);
			}

			GenePoolManager.RegisterTeam (this.TeamName);
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return string.Format ("[TeamManager TeamName:{0}]", this.TeamName);
		}

		private void ForcePlaceSeedEnt()
		{
			GridPosition seedPoint = GameObjectGrid.ChooseRandomPosition();

			EntBehaviorManager.TryCreateAndPlaceNewEnt (
				this.TeamMaterial.color,
				seedPoint,
				this.TeamName,
				true);
		}
	}
}
