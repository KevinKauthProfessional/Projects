//------------------------------------------------------------------
// <copyright file="GameObjectGrid.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.GameLogic
{
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;

	using AssemblyCSharp.Scripts.UnityGameObjects;
	using AssemblyCSharp.Scripts.Utilities;
	using UnityEngine;

	public static class GameObjectGrid
	{
		public const string Name = "GameObjectGrid";
		public static int GridWidth;
		public static int GridHeight;
		public static float CellScale = 1.0f;

		private static int createdObjectCount = 0;
		private static GameObject[,] objectGrid = null;
		private static bool[,] aliveGrid = null;
		private static Color?[,] colorGrid = null;
		private static object GridChangeLock = new object();

		internal static void InitializeToMatchScreenResolution()
		{
			int height = Screen.height;
			int width = Screen.width;

			while (height > 100 || width > 100)
			{
				height = height / 2;
				width = width / 2;
			}

			if (objectGrid != null || 
			    colorGrid != null ||
			    aliveGrid != null)
			{
				LogUtility.LogError("Initializing more than once");
			}

			objectGrid = new GameObject[width, height];
			aliveGrid = new bool[width, height];
			colorGrid = new Color?[width, height];

			GridWidth = width;
			GridHeight = height;
		}

		internal static bool GridIsMonoChromatic()
		{
			if (EveryoneIsDead())
			{
				return true;
			}

			Color? color = null;
			lock (GridChangeLock)
			{
				for (int x = 0; x < GameObjectGrid.GridWidth; ++x)
				for (int z = 0; z < GameObjectGrid.GridHeight; ++z)
				{
					if (!aliveGrid[x, z])
					{
						continue;
					}

					Color? nextColor = colorGrid[x, z];

					if (nextColor == null)
					{
						LogUtility.LogError("Grid position should not be alive while having a null color");
					}

					if (color == null)
					{
						color = nextColor;
						continue;
					}
					else if (color != nextColor)
					{
						// Two different colors found
						return false;
					}
				}
				
				// Everything is the same color
				return true;
			}
		}

		internal static bool EveryoneIsDead()
		{
			lock (GridChangeLock)
			{
				for (int x = 0; x < GameObjectGrid.GridWidth; ++x)
				for (int z = 0; z < GameObjectGrid.GridHeight; ++z)
				if (aliveGrid[x, z])
				{
					// Someone is alive
					return false;
				}

				// Everyone is dead
				return true;
			}
		}

		internal static GridPosition WorldToGridPosition(Vector3 position)
		{
			float cellX = position.x / CellScale;
			float cellZ = position.z / CellScale;

			int snapX = Mathf.RoundToInt (cellX);
			int snapZ = Mathf.RoundToInt (cellZ);

			GridPosition result = new GridPosition (snapX, snapZ);
			return result;
		}

		internal static Vector3 GridPositionToWorld(GridPosition position)
		{
			// Origin is the center of cell 0,0
			float x = position.X * CellScale;
			float z = position.Z * CellScale;
			float y = 0.0f;

			return new Vector3 (x, y, z);
		}

		internal static GridPosition ChooseRandomPosition()
		{
			int randX = CommonHelperMethods.GetRandomPositiveInt0ToValue (GridWidth - 1);
			int randZ = CommonHelperMethods.GetRandomPositiveInt0ToValue (GridHeight - 1);

			return new GridPosition (randX, randZ);
		}

		internal static float GetHealthInRegion(GridPosition center, int regionSize)
		{
			int total = 0;
			int count = 0;
			
			for (int x = center.X - regionSize; x <= center.X + regionSize; ++x)
			for (int z = center.Z - regionSize; z <= center.Z + regionSize; ++z)
			{
				// Constructor wraps around world if needed
				GridPosition position = new GridPosition(x, z);
				GameObject ent = GetObjectAt(position);
				EntBehaviorManager manager = ent.GetComponent<EntBehaviorManager> ();
				total += manager.HealthValue;
				++count;
			}
			
			return (float)total / (float)count;
		}

		internal static bool TryMoveObject(
			string teamName,
			Color color,
			GridPosition start,
			GridDirection direction)
		{
			lock (GridChangeLock)
			{
				GridPosition destination = start.GetPositionInDirection (direction);

				if (TryReviveObjectAt(teamName, color, destination, false)) 
				{
					KillObjectAt(start);
					return true;
				}

				return false;
			}
		}

		internal static bool PositionIsAlive(GridPosition position)
		{
			lock (GridChangeLock)
			{
				return aliveGrid [position.X, position.Z];
			}
		}

		internal static GameObject GetObjectAt(GridPosition position)
		{
			lock (GridChangeLock)
			{
				GameObject entOut = objectGrid [position.X, position.Z];

				if (entOut == null) 
				{
					entOut = CreateFromPrefab();
					objectGrid[position.X, position.Z] = entOut;
					KillObjectAt(position);
				}

				return entOut;
			}
		}

		internal static void KillObject(GameObject ent)
		{
			lock (GridChangeLock)
			{
				GridPosition position = WorldToGridPosition (ent.transform.position);
				KillObjectAt (position);
			}
		}

		internal static void KillObjectAt(GridPosition position)
		{
			lock (GridChangeLock)
			{
				aliveGrid[position.X, position.Z] = false;

				if (StaticController.GlobalShowDeath)
				{
					GetObjectAt(position).SetActive(false);
				}
			}
		}

		internal static void KillAllObjects()
		{
			for (int x = 0; x < GameObjectGrid.GridWidth; ++x)
			for (int z = 0; z < GameObjectGrid.GridHeight; ++z)
			{
				GridPosition position = new GridPosition(x, z);
				GameObjectGrid.KillObjectAt(position);
			}
		}

		internal static bool TryReviveObjectAt(
			string teamName,
			Color color,
			GridPosition position,
			bool destroyOccupant)
		{
			lock (GridChangeLock)
			{
				if (PositionIsAlive(position))
				{
					if (destroyOccupant)
					{
						ReviveObject(position, color, teamName);
						return true;
					}
					
					return false;
				}

				// Position is dead
				ReviveObject(position, color, teamName);
				return true;
			}
		}

		private static GameObject CreateFromPrefab()
		{
			GameObject prefab = StaticController.GlobalCreationPrefab;

			if (prefab == null)
			{
				LogUtility.LogErrorFormat("Could not find prefab");
			}

			GameObject copy = GameObject.Instantiate(prefab);

			createdObjectCount++;
			if (createdObjectCount > GridWidth * GridHeight)
			{
				LogUtility.LogWarning("More objects than expected created. Leak probable.");
			}

			return copy;
		}

		private static void ReviveObject(GridPosition position, Color color, string teamName)
		{
			lock (GridChangeLock)
			{
				GameObject ent = GetObjectAt(position);
				ent.transform.position = GridPositionToWorld(position);
				aliveGrid[position.X, position.Z] = true;
				colorGrid[position.X, position.Z] = color;

				EntBehaviorManager manager = ent.GetComponent<EntBehaviorManager>();

				if (StaticController.GlobalColorBlendDistance > 1)
				{
					manager.Reset(teamName, GetColorInRegion(position, StaticController.GlobalColorBlendDistance));
				}
				else
				{
					manager.Reset(teamName, color);
				}
			}
		}

		private static Color GetColorInRegion(GridPosition center, int regionSize)
		{
			float redTotal = 0.0f;
			float greenTotal = 0.0f;
			float blueTotal = 0.0f;
			float alphaTotal = 0.0f;
			int count = 0;
			
			for (int x = center.X - regionSize; x <= center.X + regionSize; ++x)
			for (int z = center.Z - regionSize; z <= center.Z + regionSize; ++z)
			{
				// Constructor wraps around world if needed
				GridPosition position = new GridPosition(x, z);
				if (!PositionIsAlive(position))
				{
					continue;
				}
				
				Color? colorAtPosition = colorGrid[position.X, position.Z];
				redTotal += colorAtPosition.Value.r;
				blueTotal += colorAtPosition.Value.b;
				greenTotal += colorAtPosition.Value.g;
				alphaTotal += colorAtPosition.Value.a;
				++count;
			}
			
			float red = redTotal / (float)count;
			float blue = blueTotal / (float)count;
			float green = greenTotal / (float)count;
			float alpha = alphaTotal / (float)count;
			
			return new Color(red, green, blue, alpha);
		}
	}
}
