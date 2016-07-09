//------------------------------------------------------------------
// <copyright file="EntBehaviorManager.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.UnityGameObjects
{
    using System;
    using AssemblyCSharp.Scripts.GameLogic;
    using Assets.Scripts.EntLogic.GeneticMemberRegistration;
    using Assets.Scripts.Utilities;
    using System.Collections.Generic;
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

		public byte HealthValue
		{
			get
			{
				return this.Health;
			}
		}

		private GeneData genome;

		// Key read only variables
		private byte Health = InitialHealth;
		private byte AttackStrength = InitialAttackStrength;
		private byte GrowFoodStrength = InitialGrowFoodStrength;

		// Key read write variables
		private byte HelperInt1 = 0;
		private byte HelperInt2 = 0;
		private byte HelperBool1 = 0;
		private byte HelperBool2 = 0;
		private byte HelperGridDirection1 = (byte)GridDirectionEnum.North;
		private byte HelperGridDirection2 = (byte)GridDirectionEnum.North;

		/// <summary>
		/// Registers the genetic members.
		/// </summary>
		public static void RegisterGeneticMembers()
		{
            RegistrationManager.AddOperator(OperatorTypeEnum.And, GeneticTypeEnum.GeneticBool, GeneticTypeEnum.GeneticBool, GeneticTypeEnum.GeneticBool);
            RegistrationManager.AddOperator(OperatorTypeEnum.Equal, GeneticTypeEnum.GeneticBool, GeneticTypeEnum.GeneticBool, GeneticTypeEnum.GeneticBool);
            RegistrationManager.AddOperator(OperatorTypeEnum.Equal, GeneticTypeEnum.GeneticBool, GeneticTypeEnum.GeneticGridDirection, GeneticTypeEnum.GeneticGridDirection);
            RegistrationManager.AddOperator(OperatorTypeEnum.Equal, GeneticTypeEnum.GeneticBool, GeneticTypeEnum.GeneticInt, GeneticTypeEnum.GeneticInt);
            RegistrationManager.AddOperator(OperatorTypeEnum.Minus, GeneticTypeEnum.GeneticInt, GeneticTypeEnum.GeneticInt, GeneticTypeEnum.GeneticInt);
            RegistrationManager.AddOperator(OperatorTypeEnum.NotEqual, GeneticTypeEnum.GeneticBool, GeneticTypeEnum.GeneticBool, GeneticTypeEnum.GeneticBool);
            RegistrationManager.AddOperator(OperatorTypeEnum.NotEqual, GeneticTypeEnum.GeneticBool, GeneticTypeEnum.GeneticGridDirection, GeneticTypeEnum.GeneticGridDirection);
            RegistrationManager.AddOperator(OperatorTypeEnum.NotEqual, GeneticTypeEnum.GeneticBool, GeneticTypeEnum.GeneticInt, GeneticTypeEnum.GeneticInt);
            RegistrationManager.AddOperator(OperatorTypeEnum.Or, GeneticTypeEnum.GeneticBool, GeneticTypeEnum.GeneticBool, GeneticTypeEnum.GeneticBool);
            RegistrationManager.AddOperator(OperatorTypeEnum.Plus, GeneticTypeEnum.GeneticInt, GeneticTypeEnum.GeneticInt, GeneticTypeEnum.GeneticInt);

            RegistrationManager.AddReadOnlyVariable(EntVariableEnum.Health, GeneticTypeEnum.GeneticInt);
			RegistrationManager.AddReadOnlyVariable(EntVariableEnum.AttackStrength, GeneticTypeEnum.GeneticInt);
			RegistrationManager.AddReadOnlyVariable(EntVariableEnum.GrowFoodStrength, GeneticTypeEnum.GeneticInt);

			RegistrationManager.AddReadWriteVariable(EntVariableEnum.HelperInt1, GeneticTypeEnum.GeneticInt);
			RegistrationManager.AddReadWriteVariable(EntVariableEnum.HelperInt2, GeneticTypeEnum.GeneticInt);
			RegistrationManager.AddReadWriteVariable(EntVariableEnum.HelperBool1, GeneticTypeEnum.GeneticBool);
			RegistrationManager.AddReadWriteVariable(EntVariableEnum.HelperBool2, GeneticTypeEnum.GeneticBool);
			RegistrationManager.AddReadWriteVariable(EntVariableEnum.HelperGridDirection1, GeneticTypeEnum.GeneticGridDirection);
			RegistrationManager.AddReadWriteVariable(EntVariableEnum.HelperGridDirection2, GeneticTypeEnum.GeneticGridDirection);

			RegistrationManager.AddLeftMethod(EntMethodEnum.Reproduce, GeneticTypeEnum.Void, GeneticTypeEnum.GeneticGridDirection);
			RegistrationManager.AddLeftMethod(EntMethodEnum.Move, GeneticTypeEnum.Void, GeneticTypeEnum.GeneticGridDirection);
            RegistrationManager.AddLeftMethod(EntMethodEnum.Attack, GeneticTypeEnum.Void, GeneticTypeEnum.GeneticGridDirection);
            RegistrationManager.AddLeftMethod(EntMethodEnum.GrowFood, GeneticTypeEnum.Void);

			RegistrationManager.AddRightMethod(EntMethodEnum.DirectionIsOccupied, GeneticTypeEnum.GeneticBool, GeneticTypeEnum.GeneticGridDirection);
			RegistrationManager.AddRightMethod(EntMethodEnum.DirectionIsEnemy, GeneticTypeEnum.GeneticBool, GeneticTypeEnum.GeneticGridDirection);
            RegistrationManager.AddRightMethod(EntMethodEnum.DirectionIsFriend, GeneticTypeEnum.GeneticBool, GeneticTypeEnum.GeneticGridDirection);
        }

		/// <summary>
		/// Tries the create and place new ent.
		/// </summary>
		/// <returns><c>true</c>, if create and place new ent was tryed, <c>false</c> otherwise.</returns>
		/// <param name="template">Template.</param>
		/// <param name="position">Position.</param>
		/// <param name="teamName">Team name.</param>
		/// <param name="forcePlace">If set to <c>true</c> force place.</param>
		public static byte TryCreateAndPlaceNewEnt(
			Color color,
			GridPosition position,
			string teamName,
			bool forcePlace)
		{						
			if (GameObjectGrid.TryReviveObjectAt(teamName, color, position, forcePlace) == 0) 
			{
				return 0;
			}

			return 1;
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
            if (this.Genome.DNA.Execute(ref myself) == 0)
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
			this.Health = InitialHealth;
			this.AttackStrength = InitialAttackStrength;
			this.GrowFoodStrength = InitialGrowFoodStrength;
			
			// Key read write variables
			this.HelperInt1 = 0;
			this.HelperInt2 = 0;
			this.HelperBool1 = 0;
			this.HelperBool2 = 0;
			this.HelperGridDirection1 = (byte)GridDirectionEnum.North;
			this.HelperGridDirection2 = (byte)GridDirectionEnum.North;
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
		public byte ExecuteLeftMethod(MethodSignature signature, IList<byte> parameters)
		{
			if (signature.MethodId == EntMethodEnum.Reproduce)
			{
				return this.TryReproduce(parameters[0]);
			}

			if (signature.MethodId == EntMethodEnum.Move)
			{
				return this.TryMove(parameters[0]);
			}

			if (signature.MethodId == EntMethodEnum.Attack)
			{
				return this.TryAttack(parameters[0]);
			}

			if (signature.MethodId == EntMethodEnum.GrowFood)
			{
				return this.TryGrowFood();
			}

			LogUtility.LogErrorFormat(
				"No implementation found for left method: {0}",
				signature);
			return 0;
		}

		/// <summary>
		/// Executes the right method.
		/// </summary>
		/// <returns>The right method.</returns>
		/// <param name="signature">Signature.</param>
		/// <param name="parameters">Parameters.</param>
		public byte ExecuteRightMethod(
            MethodSignature signature,
            IList<byte> parameters)
		{
			if (signature.MethodId == EntMethodEnum.DirectionIsOccupied)
			{
				return this.DirectionIsOccupied(parameters[0]);
			}

			if (signature.MethodId == EntMethodEnum.DirectionIsEnemy)
			{
				return this.DirectionIsEnemy(parameters[0]);
			}

			if (signature.MethodId == EntMethodEnum.DirectionIsFriend)
			{
				return this.DirectionIsFriend(parameters[0]);
			}

			LogUtility.LogErrorFormat(
				"No implementation found for right method: {0}",
				signature);
			return 0;
		}

		/// <summary>
		/// Writes to variable.
		/// </summary>
		/// <param name="signature">Signature.</param>
		/// <param name="value">Value.</param>
		public void WriteToVariable(VariableSignature signature, byte value)
		{
			if (signature.VariableId == EntVariableEnum.HelperInt1)
			{
				this.HelperInt1 = value;
				return;
			}

			if (signature.VariableId == EntVariableEnum.HelperInt2)
			{
				this.HelperInt2 = value;
				return;
			}

			if (signature.VariableId == EntVariableEnum.HelperBool1)
			{
				this.HelperBool1 = value;
				return;
			}

			if (signature.VariableId == EntVariableEnum.HelperBool2)
			{
				this.HelperBool2 = value;
				return;
			}

			if (signature.VariableId == EntVariableEnum.HelperGridDirection1)
			{
				this.HelperGridDirection1 = value;
				return;
			}

			if (signature.VariableId == EntVariableEnum.HelperGridDirection2)
			{
				this.HelperGridDirection2 = value;
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
		public byte ReadFromVariable(VariableSignature signature)
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
			return 0;
		}

		/// <summary>
		/// Takes the damage.
		/// </summary>
		/// <param name="amount">Amount.</param>
		public void TakeDamage(byte amount)
		{            
            if (amount >= this.Health)
            {
                GameObjectGrid.KillObject(this.gameObject);
            }
            else
            {
                this.Health = (byte)(this.Health - amount);
            }
		}

		/// <summary>
		/// Adds the health.
		/// </summary>
		/// <param name="amount">Amount.</param>
		public byte TryAddHealth(byte amount)
		{
            int addResult = this.Health + amount;
            if (addResult > byte.MaxValue)
            {
                return 0;
            }

            this.Health = (byte)addResult;
            return 1;
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

		private byte TryReproduce(byte direction)
		{
			GridPosition myPosition = GameObjectGrid.WorldToGridPosition(this.transform.position);
			GridPosition childPosition = myPosition.GetPositionInDirection(direction);

			return GameObjectGrid.TryReviveObjectAt(
				this.TeamName,
				TeamManager.GetTeamColor(this.TeamName),
				childPosition,
				false);
		}

		private byte TryMove(byte direction)
		{
			return GameObjectGrid.TryMoveObject(
				this.TeamName,
				TeamManager.GetTeamColor(this.TeamName),
				GameObjectGrid.WorldToGridPosition(this.transform.position),
				direction);
		}

		private byte TryAttack(byte direction)
		{
			GridPosition myPosition = GameObjectGrid.WorldToGridPosition(this.transform.position);
			GridPosition victimPosition = myPosition.GetPositionInDirection(direction);

			if (!GameObjectGrid.PositionIsAlive(victimPosition))
			{
				return 0;
			}

			GameObject victim = GameObjectGrid.GetObjectAt(victimPosition);
			EntBehaviorManager victimManager = victim.GetComponent<EntBehaviorManager>();

			int damage = (this.AttackStrength * this.Health) / InitialHealth;
            if (damage > byte.MaxValue)
            {
                damage = byte.MaxValue;
            }

			victimManager.TakeDamage((byte)damage);

			// Will succeed if we killed the victim
			if (this.TryMove(direction) == 1)
			{
				this.TryAddHealth(victimManager.GrowFoodStrength);
			}

			if (this.AttackStrength < byte.MaxValue)
			{
				++this.AttackStrength;
			}
			
			if (this.GrowFoodStrength > byte.MinValue)
			{
				--this.GrowFoodStrength;
			}

			return 1;
		}

		private byte TryGrowFood()
		{
			int foodAmount = (this.GrowFoodStrength * this.Health) / InitialHealth;
            if (foodAmount > byte.MaxValue)
            {
                foodAmount = byte.MaxValue;
            }

			byte result = this.TryAddHealth((byte)foodAmount);

			if (result == 1)
			{
				if (this.GrowFoodStrength < byte.MaxValue)
				{
					++this.GrowFoodStrength;
				}

				if (this.AttackStrength > byte.MinValue)
				{
					--this.AttackStrength;
				}
			}

			return result;
		}

		private byte DirectionIsOccupied(byte direction)
		{
			GridPosition currentPosition = GameObjectGrid.WorldToGridPosition(this.transform.position);
			GridPosition testPosition = currentPosition.GetPositionInDirection(direction);

            if (GameObjectGrid.PositionIsAlive(testPosition))
            {
                return 1;
            }
            else
            {
                return 0;
            }
		}

		private byte DirectionIsEnemy(byte direction)
		{
			GridPosition currentPosition = GameObjectGrid.WorldToGridPosition(this.transform.position);
			GridPosition testPosition = currentPosition.GetPositionInDirection(direction);

			if (!GameObjectGrid.PositionIsAlive(testPosition))
			{
                return 0;
			}

			GameObject neighbor = GameObjectGrid.GetObjectAt(testPosition);
			EntBehaviorManager manager = neighbor.GetComponent<EntBehaviorManager> ();

			if (CommonHelperMethods.StringsAreEqual(manager.TeamName, this.TeamName)) 
			{
                return 0;
			}

            return 1;
		}

		private byte DirectionIsFriend(byte direction)
		{
			GridPosition currentPosition = GameObjectGrid.WorldToGridPosition(this.transform.position);
			GridPosition testPosition = currentPosition.GetPositionInDirection (direction);

			if (!GameObjectGrid.PositionIsAlive(testPosition))
			{
                return 0;
			}

			GameObject neighbor = GameObjectGrid.GetObjectAt(testPosition);			
			EntBehaviorManager manager = neighbor.GetComponent<EntBehaviorManager> ();

			if (CommonHelperMethods.StringsAreEqual(manager.TeamName, this.TeamName)) 
			{
                return 1;
			}

            return 0;
		}
	}
}
