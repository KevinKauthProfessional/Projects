//------------------------------------------------------------------
// <copyright file="EntBehaviorManager.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.UnityGameObjects
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	using AssemblyCSharp.Scripts.EntLogic.GeneticMemberRegistration;
	using AssemblyCSharp.Scripts.EntLogic.GeneticTypes;
	using AssemblyCSharp.Scripts.EntLogic.SerializationObjects;
	using AssemblyCSharp.Scripts.GameLogic;
	using Assets.Scripts.Utilities;
	using UnityEngine;

	/// <summary>
	/// Ent behavior manager.
	/// </summary>
	public class EntBehaviorManager : MonoBehaviour 
	{
		private const int InitialHealth = 100;
		private const int InitialAttackStrength = 30;
		private const int InitialGrowFoodStrength = 10;

		/// <summary>
		/// Gets or sets the name of the team.
		/// </summary>
		/// <value>The name of the team.</value>
		public string TeamName { get; set; }

		/// <summary>
		/// Gets or sets the genome.
		/// </summary>
		/// <value>The genome.</value>
		public GeneData Genome 
		{ 
			get
			{
				return this.genome;
			}
		}

		public int HealthValue
		{
			get
			{
				return this.Health.Value;
			}
		}

		private GeneData genome;

		// Key read only variables
		private GeneticInt Health = new GeneticInt(InitialHealth);
		private GeneticInt AttackStrength = new GeneticInt(InitialAttackStrength);
		private GeneticInt GrowFoodStrength = new GeneticInt(InitialGrowFoodStrength);

		// Key read write variables
		private GeneticInt HelperInt1 = new GeneticInt(0);
		private GeneticInt HelperInt2 = new GeneticInt(0);
		private GeneticBool HelperBool1 = GeneticBool.False;
		private GeneticBool HelperBool2 = GeneticBool.False;
		private GeneticGridDirection HelperGridDirection1 = new GeneticGridDirection(GridDirection.North);
		private GeneticGridDirection HelperGridDirection2 = new GeneticGridDirection(GridDirection.North);

		/// <summary>
		/// Registers the genetic members.
		/// </summary>
		public static void RegisterGeneticMembers()
		{
			RegistrationManager.AddReadOnlyVariable (EntVariableEnum.Health, typeof(GeneticInt));
			RegistrationManager.AddReadOnlyVariable (EntVariableEnum.AttackStrength, typeof(GeneticInt));
			RegistrationManager.AddReadOnlyVariable (EntVariableEnum.GrowFoodStrength, typeof(GeneticInt));

			RegistrationManager.AddReadWriteVariable(EntVariableEnum.HelperInt1, typeof(GeneticInt));
			RegistrationManager.AddReadWriteVariable(EntVariableEnum.HelperInt2, typeof(GeneticInt));
			RegistrationManager.AddReadWriteVariable(EntVariableEnum.HelperBool1, typeof(GeneticBool));
			RegistrationManager.AddReadWriteVariable(EntVariableEnum.HelperBool2, typeof(GeneticBool));
			RegistrationManager.AddReadWriteVariable(EntVariableEnum.HelperGridDirection1, typeof(GeneticGridDirection));
			RegistrationManager.AddReadWriteVariable(EntVariableEnum.HelperGridDirection2, typeof(GeneticGridDirection));

			RegistrationManager.AddLeftMethod (EntMethodEnum.Reproduce, typeof(void), typeof(GeneticGridDirection));
			RegistrationManager.AddLeftMethod (EntMethodEnum.Move, typeof(void), typeof(GeneticGridDirection));
			RegistrationManager.AddLeftMethod (EntMethodEnum.Attack, typeof(void), typeof(GeneticGridDirection));
			RegistrationManager.AddLeftMethod (EntMethodEnum.GrowFood, typeof(void));

			RegistrationManager.AddRightMethod (EntMethodEnum.DirectionIsOccupied, typeof(GeneticBool), typeof(GeneticGridDirection));
			RegistrationManager.AddRightMethod (EntMethodEnum.DirectionIsEnemy, typeof(GeneticBool), typeof(GeneticGridDirection));
			RegistrationManager.AddRightMethod (EntMethodEnum.DirectionIsFriend, typeof(GeneticBool), typeof(GeneticGridDirection));
		}

		/// <summary>
		/// Tries the create and place new ent.
		/// </summary>
		/// <returns><c>true</c>, if create and place new ent was tryed, <c>false</c> otherwise.</returns>
		/// <param name="template">Template.</param>
		/// <param name="position">Position.</param>
		/// <param name="teamName">Team name.</param>
		/// <param name="forcePlace">If set to <c>true</c> force place.</param>
		public static bool TryCreateAndPlaceNewEnt(
			Color color,
			GridPosition position,
			string teamName,
			bool forcePlace)
		{						
			if (!GameObjectGrid.TryReviveObjectAt(teamName, color, position, forcePlace)) 
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		public void Start () 
		{
		}
		
		/// <summary>
		/// Update this instance.  Called once per frame.
		/// </summary>
		public void Update () 
		{
			if (!this.IsActiveOnGrid())
			{
				return;
			}

			var myself = this;
            if (!this.Genome.DNA.Execute(ref myself))
            {
                // If no action taken, kill ent
                this.TakeDamage(this.HealthValue);
            }
            else
            {
                this.TakeDamage(StaticController.GlobalEnviornmentDamagePerTurn);
            }

			if (!StaticController.GlobalForceFlat)
			{
				float healthPercent =  
					GameObjectGrid.GetHealthInRegion(GameObjectGrid.WorldToGridPosition(this.transform.position), StaticController.GlobalHeightBlendDistance) / 
					(float)StaticController.GlobalMaxInt;

				float yScale = healthPercent * StaticController.GlobalMaxHeight;

				this.transform.localScale = 
					new Vector3(
						this.transform.localScale.x,
						yScale,
						this.transform.localScale.z);
			}
		}

		public void FixedUpdate()
		{
		}

		/// <summary>
		/// Reset the specified teamName and material.
		/// </summary>
		/// <param name="teamName">Team name.</param>
		/// <param name="material">Material.</param>
		public void Reset(string teamName, Color color)
		{
			MeshRenderer[] renderComponents = this.gameObject.GetComponents<MeshRenderer>();
			if (renderComponents.Length != 1)
			{
				LogUtility.LogErrorFormat("Object had {0} render components", renderComponents.Length);
			}

			this.transform.localScale = 
				new Vector3(
					StaticController.GlobalShapeHorizontalScale,
					this.transform.localScale.y,
					StaticController.GlobalShapeHorizontalScale);

			renderComponents[0].material.color = color;

			this.gameObject.SetActive(true);

			this.TeamName = teamName;

			this.genome = GenePoolManager.GetData(teamName);
			
			// Key read only variables
			this.Health.Value = InitialHealth;
			this.AttackStrength.Value = InitialAttackStrength;
			this.GrowFoodStrength.Value = InitialGrowFoodStrength;
			
			// Key read write variables
			this.HelperInt1.Value = 0;
			this.HelperInt2.Value = 0;
			this.HelperBool1 = GeneticBool.False;
			this.HelperBool2 = GeneticBool.False;
			this.HelperGridDirection1.Value = GridDirection.North;
			this.HelperGridDirection2.Value = GridDirection.North;
		}

		/// <summary>
		/// Determines whether this instance is active on grid.
		/// </summary>
		/// <returns><c>true</c> if this instance is active on grid; otherwise, <c>false</c>.</returns>
		public bool IsActiveOnGrid()
		{
			GridPosition position = GameObjectGrid.WorldToGridPosition(this.transform.position);

			if (!GameObjectGrid.PositionIsAlive(position))
			{
				return false;
			}

			GameObject objectInThisSpot= GameObjectGrid.GetObjectAt(position);
			if (objectInThisSpot.GetInstanceID() == this.gameObject.GetInstanceID())
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Executes the left method.
		/// </summary>
		/// <param name="signature">Signature.</param>
		/// <param name="parameters">Parameters.</param>
		public bool ExecuteLeftMethod(MethodSignature signature, IList<GeneticObject> parameters)
		{
			if (signature.MethodId == EntMethodEnum.Reproduce)
			{
				return this.TryReproduce(parameters[0] as GeneticGridDirection);
			}

			if (signature.MethodId == EntMethodEnum.Move)
			{
				return this.TryMove(parameters[0] as GeneticGridDirection);
			}

			if (signature.MethodId == EntMethodEnum.Attack)
			{
				return this.TryAttack(parameters[0] as GeneticGridDirection);
			}

			if (signature.MethodId == EntMethodEnum.GrowFood)
			{
				return this.TryGrowFood();
			}

			LogUtility.LogErrorFormat(
				"No implementation found for left method: {0}",
				signature);
			return false;
		}

		/// <summary>
		/// Executes the right method.
		/// </summary>
		/// <returns>The right method.</returns>
		/// <param name="signature">Signature.</param>
		/// <param name="parameters">Parameters.</param>
		public GeneticObject ExecuteRightMethod(MethodSignature signature, IList<GeneticObject> parameters)
		{
			if (signature.MethodId == EntMethodEnum.DirectionIsOccupied)
			{
				return this.DirectionIsOccupied(parameters[0] as GeneticGridDirection);
			}

			if (signature.MethodId == EntMethodEnum.DirectionIsEnemy)
			{
				return this.DirectionIsEnemy(parameters[0] as GeneticGridDirection);
			}

			if (signature.MethodId == EntMethodEnum.DirectionIsFriend)
			{
				return this.DirectionIsFriend(parameters[0] as GeneticGridDirection);
			}

			LogUtility.LogErrorFormat(
				"No implementation found for right method: {0}",
				signature);
			return null;
		}

		/// <summary>
		/// Writes to variable.
		/// </summary>
		/// <param name="signature">Signature.</param>
		/// <param name="value">Value.</param>
		public void WriteToVariable(VariableSignature signature, GeneticObject value)
		{
			if (signature.VariableId == EntVariableEnum.HelperInt1)
			{
				this.HelperInt1 = value as GeneticInt;
				return;
			}

			if (signature.VariableId == EntVariableEnum.HelperInt2)
			{
				this.HelperInt2 = value as GeneticInt;
				return;
			}

			if (signature.VariableId == EntVariableEnum.HelperBool1)
			{
				this.HelperBool1 = value as GeneticBool;
				return;
			}

			if (signature.VariableId == EntVariableEnum.HelperBool2)
			{
				this.HelperBool2 = value as GeneticBool;
				return;
			}

			if (signature.VariableId == EntVariableEnum.HelperGridDirection1)
			{
				this.HelperGridDirection1 = value as GeneticGridDirection;
				return;
			}

			if (signature.VariableId == EntVariableEnum.HelperGridDirection2)
			{
				this.HelperGridDirection2 = value as GeneticGridDirection;
				return;
			}

			LogUtility.LogErrorFormat(
				"No implementation found for writable variable: {0}",
				signature);
		}

		/// <summary>
		/// Reads from variable.
		/// </summary>
		/// <returns>The from variable.</returns>
		/// <param name="signature">Signature.</param>
		public GeneticObject ReadFromVariable(VariableSignature signature)
		{
			if (signature.VariableId == EntVariableEnum.Health)
			{
				return this.Health;
			}

			if (signature.VariableId == EntVariableEnum.AttackStrength)
			{
				return this.AttackStrength;
			}

			if (signature.VariableId == EntVariableEnum.GrowFoodStrength)
			{
				return this.GrowFoodStrength;
			}

			if (signature.VariableId == EntVariableEnum.HelperInt1)
			{
				return this.HelperInt1;
			}
			
			if (signature.VariableId == EntVariableEnum.HelperInt2)
			{
				return this.HelperInt2;
			}
			
			if (signature.VariableId == EntVariableEnum.HelperBool1)
			{
				return this.HelperBool1;
			}
			
			if (signature.VariableId == EntVariableEnum.HelperBool2)
			{
				return this.HelperBool2;
			}
			
			if (signature.VariableId == EntVariableEnum.HelperGridDirection1)
			{
				return this.HelperGridDirection1;
			}
			
			if (signature.VariableId == EntVariableEnum.HelperGridDirection2)
			{
				return this.HelperGridDirection2;
			}

			LogUtility.LogErrorFormat(
				"No implementation found for read only variable: {0}",
				signature);
			return null;
		}

		/// <summary>
		/// Takes the damage.
		/// </summary>
		/// <param name="amount">Amount.</param>
		public void TakeDamage(int amount)
		{
			if (amount <= 0)
			{
				return;
			}

			try
			{
				this.Health.Value = checked(this.Health.Value - amount);
			}
			catch (OverflowException)
			{
				this.Health.Value = int.MinValue;
			}

			if (this.Health.Value <= 0) 
			{
				GameObjectGrid.KillObject(this.gameObject);
			}
		}

		/// <summary>
		/// Adds the health.
		/// </summary>
		/// <param name="amount">Amount.</param>
		public bool TryAddHealth(int amount)
		{
			if (amount <= 0)
			{
				return false;
			}

			try
			{
				this.Health.Value = checked(this.Health.Value + amount);
			}
			catch (OverflowException)
			{
				this.Health.Value = int.MaxValue;
				return false;
			}

			if (this.Health.Value >= StaticController.GlobalMaxInt)
			{
				this.Health.Value = StaticController.GlobalMaxInt;
				return false;
			}

			return true;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return string.Format(
				"[Ent: TeamName={0} {1}]",
				this.TeamName,
				GameObjectGrid.WorldToGridPosition(this.transform.position));
		}

		private bool TryReproduce(GeneticGridDirection direction)
		{
			GridPosition myPosition = GameObjectGrid.WorldToGridPosition(this.transform.position);
			GridPosition childPosition = myPosition.GetPositionInDirection (direction.Value);

			return GameObjectGrid.TryReviveObjectAt(
				this.TeamName,
				TeamManager.GetTeamColor(this.TeamName),
				childPosition,
				false);
		}

		private bool TryMove(GeneticGridDirection direction)
		{
			return GameObjectGrid.TryMoveObject(
				this.TeamName,
				TeamManager.GetTeamColor(this.TeamName),
				GameObjectGrid.WorldToGridPosition(this.transform.position),
				direction.Value);
		}

		private bool TryAttack(GeneticGridDirection direction)
		{
			GridPosition myPosition = GameObjectGrid.WorldToGridPosition(this.transform.position);
			GridPosition victimPosition = myPosition.GetPositionInDirection (direction.Value);

			if (!GameObjectGrid.PositionIsAlive(victimPosition))
			{
				return false;
			}

			GameObject victim = GameObjectGrid.GetObjectAt(victimPosition);
			EntBehaviorManager victimManager = victim.GetComponent<EntBehaviorManager>();

			int damage = (this.AttackStrength.Value * this.Health.Value) / InitialHealth;
			victimManager.TakeDamage(damage);

			// Will succeed if we killed the victim
			if (this.TryMove(direction))
			{
				this.TryAddHealth(victimManager.GrowFoodStrength.Value);
			}

			if (this.AttackStrength.Value < StaticController.GlobalMaxInt)
			{
				++this.AttackStrength.Value;
			}
			
			if (this.GrowFoodStrength.Value > 1)
			{
				--this.GrowFoodStrength.Value;
			}

			return true;
		}

		private bool TryGrowFood()
		{
			int foodAmount = (this.GrowFoodStrength.Value * this.Health.Value) / InitialHealth;

			bool result = this.TryAddHealth (foodAmount);

			if (result)
			{
				if (this.GrowFoodStrength.Value < StaticController.GlobalMaxInt)
				{
					++this.GrowFoodStrength.Value;
				}

				if (this.AttackStrength.Value > 1)
				{
					--this.AttackStrength.Value;
				}
			}

			return result;
		}

		private GeneticBool DirectionIsOccupied(GeneticGridDirection direction)
		{
			GridPosition currentPosition = GameObjectGrid.WorldToGridPosition(this.transform.position);
			GridPosition testPosition = currentPosition.GetPositionInDirection (direction.Value);

			if (GameObjectGrid.PositionIsAlive(testPosition)) 
			{
				return GeneticBool.True;
			}

			return GeneticBool.False;
		}

		private GeneticBool DirectionIsEnemy(GeneticGridDirection direction)
		{
			GridPosition currentPosition = GameObjectGrid.WorldToGridPosition(this.transform.position);
			GridPosition testPosition = currentPosition.GetPositionInDirection (direction.Value);

			if (!GameObjectGrid.PositionIsAlive(testPosition))
			{
				return GeneticBool.False;
			}

			GameObject neighbor = GameObjectGrid.GetObjectAt(testPosition);
			EntBehaviorManager manager = neighbor.GetComponent<EntBehaviorManager> ();

			if (CommonHelperMethods.StringsAreEqual (manager.TeamName, this.TeamName)) 
			{
				return GeneticBool.False;
			}

			return GeneticBool.True;
		}

		private GeneticBool DirectionIsFriend(GeneticGridDirection direction)
		{
			GridPosition currentPosition = GameObjectGrid.WorldToGridPosition(this.transform.position);
			GridPosition testPosition = currentPosition.GetPositionInDirection (direction.Value);

			if (!GameObjectGrid.PositionIsAlive(testPosition))
			{
				return GeneticBool.False;
			}

			GameObject neighbor = GameObjectGrid.GetObjectAt(testPosition);			
			EntBehaviorManager manager = neighbor.GetComponent<EntBehaviorManager> ();

			if (CommonHelperMethods.StringsAreEqual (manager.TeamName, this.TeamName)) 
			{
				return GeneticBool.True;
			}
			
			return GeneticBool.False;
		}
	}
}
