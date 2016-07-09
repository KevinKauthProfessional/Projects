//------------------------------------------------------------------
// <copyright file="Condition.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace Assets.Scripts.EntLogic.SerializationObjects
{
    using Assets.Scripts.EntLogic.GeneticMemberRegistration;
    using AssemblyCSharp.Scripts.UnityGameObjects;
    using Assets.Scripts.Utilities;
    using System;

    /// <summary>
    /// This class represents a boolean condition statement.
    /// </summary>
    internal class Condition
	{
		public const string Name = "Condition";

		private RightStatement rightStatement;
        private int depth;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.Condition"/> class.
		/// </summary>
		public Condition(int depthIn)
		{
            this.depth = depthIn;
			this.rightStatement = new RightStatement(GeneticTypeEnum.GeneticBool, depthIn + 1);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.Condition"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public Condition (FileIOManager reader, int depthIn)
		{
            this.depth = depthIn;

            byte nextByte = reader.ReadByte();
        
			if (nextByte == StatementTypeEnum.RightStatement) 
			{
				this.rightStatement = new RightStatement(reader, depthIn + 1);
			} 
			else 
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextByte,
					reader,
					StatementTypeEnum.RightStatement);
			}
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return this.rightStatement.ToString();
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		public void WriteToDisk(FileIOManager writer)
		{
            writer.WriteByte(StatementTypeEnum.Condition);

			if (this.rightStatement != null) 
			{
				this.rightStatement.WriteToDisk(writer);
			}
		}

		/// <summary>
		/// Will possibly mutate this section of logic.
		/// </summary>
		public void PossiblyMutate()
		{
			if (GeneticLogicRoot.RollMutateDice ()) 
			{
				this.rightStatement = new RightStatement(GeneticTypeEnum.GeneticBool, this.depth + 1);
				return;
			}

			if (this.rightStatement != null) 
			{
				this.rightStatement.PossiblyMutate();
			}
		}

		/// <summary>
		/// Evaluates the condition.
		/// </summary>
		/// <param name="instance">The instance to evaluate against.</param>
		public byte Evaluate(ref EntBehaviorManager instance)
		{
			return this.rightStatement.Evaluate(ref instance);
		}
	}
}

