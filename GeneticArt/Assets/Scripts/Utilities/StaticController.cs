//------------------------------------------------------------------
// <copyright file="StaticController.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace Assets.Scripts.Utilities
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	using AssemblyCSharp.Scripts.GameLogic;
	using AssemblyCSharp.Scripts.UnityGameObjects;
	using UnityEngine;

	public class StaticController : MonoBehaviour 
	{
		public int MaxInt;
		public float MaxHeight;
		public bool ForceFlat;
		public float MutationChance;
		public int FramesPerIteration;
		public int EnviornmentDamagePerTurn;
		public int ColorBlendDistance;
		public int HeightBlendDistance;
		public float ShapeHorizontalScale;
		public bool ShowDeath;
		public GameObject CreationPrefab;
		public Material[] TeamMaterials;

		public static float GlobalMaxHeight { get; private set;}
		public static int GlobalMaxInt { get; private set; }
		public static bool GlobalForceFlat { get; private set; }
		public static float GlobalMutationChance { get; private set; }
		public static int GlobalEnviornmentDamagePerTurn { get; private set; }
		public static int GlobalColorBlendDistance { get; private set; }
		public static bool GlobalShowDeath { get; private set; }
		public static int GlobalHeightBlendDistance { get; private set; }
		public static GameObject GlobalCreationPrefab { get; private set; }
		public static float GlobalShapeHorizontalScale { get; private set; }

		private int frameCounter = 0;
		private IList<TeamManager> teamManagerList = new List<TeamManager>();

		// Use this for initialization
		void Start () 
		{
			GlobalEnviornmentDamagePerTurn = this.EnviornmentDamagePerTurn;
			GlobalMutationChance = this.MutationChance;
			GlobalMaxInt = this.MaxInt;
			GlobalMaxHeight = this.MaxHeight;
			GlobalForceFlat = this.ForceFlat;
			GlobalColorBlendDistance = this.ColorBlendDistance;
			GlobalShowDeath = this.ShowDeath;
			GlobalHeightBlendDistance = this.HeightBlendDistance;
			GlobalCreationPrefab = this.CreationPrefab;
			GlobalShapeHorizontalScale = this.ShapeHorizontalScale;

			GameObjectGrid.InitializeToMatchScreenResolution();
			CameraController.ResetMainCamera();

			// Important to come after prefab set
			foreach (Material material in this.TeamMaterials)
			{
				this.teamManagerList.Add(new TeamManager(material));
			}
		}
		
		// Update is called once per frame
		void Update () 
		{
			this.frameCounter++;
		}

		void FixedUpdate () 
		{
			bool forceEarlyRefresh = 
				GameObjectGrid.EveryoneIsDead() ||
				GameObjectGrid.GridIsMonoChromatic();

			if (frameCounter >= this.FramesPerIteration || forceEarlyRefresh)
			{
				lock (CommonHelperMethods.GlobalStateLock)
				{
					// Everyone wins
					GenePoolManager.RefreshTeamDNA();
				}

				this.frameCounter = 0;
			}
		}
	}
}
