//------------------------------------------------------------------
// <copyright file="RootStatement.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace Assets.Scripts.EntLogic.SerializationObjects
{
	using System;
	using System.Collections.Generic;

	using AssemblyCSharp.Scripts.UnityGameObjects;
	using Assets.Scripts.Utilities;

	/// <summary>
	/// This class represents a root level statement in a method controlled by genetic logic.
	/// The statement can be either a ConditionalLeftStatement or a LeftStatement.
	/// </summary>
	public class RootStatement
	{
        public const int MaxDepth = 5;
		public const string Name = "RootStatement";

		// Only one will be populated.
		private ConditionalLeftStatement conditionalLeftStatement = null;
		private LeftStatement leftStatement = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.RootStatement"/> class.
		/// </summary>
		public RootStatement()
		{
			int value = CommonHelperMethods.GetRandomPositiveInt0ToValue(1);

			if (value == 0)
			{
				this.conditionalLeftStatement = new ConditionalLeftStatement(1);
			}
			else
			{
				this.leftStatement = new LeftStatement(1);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.RootStatement"/> class.
		/// </summary>
		/// <param name="reader">A StreamReader pointed to line where this RootStatement begins.</param>
		public RootStatement(FileIOManager reader)
		{
			byte nextByte = reader.ReadByte();
			if (nextByte == StatementTypeEnum.ConditionalLeftStatement)
			{
				this.conditionalLeftStatement = new ConditionalLeftStatement(reader, 1);
			} 
			else if (nextByte == StatementTypeEnum.LeftStatement) 
			{
				this.leftStatement = new LeftStatement(reader, 1);
			} 
			else 
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextByte,
					reader,
					new List<byte>() {
                        StatementTypeEnum.ConditionalLeftStatement,
                        StatementTypeEnum.LeftStatement
                    });
			}
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		public void WriteToDisk(FileIOManager writer)
		{
            writer.WriteByte(StatementTypeEnum.RootStatement);

			if (this.conditionalLeftStatement != null)
			{
				this.conditionalLeftStatement.WriteToDisk(writer);
			} 

			if (this.leftStatement != null)
			{
				this.leftStatement.WriteToDisk (writer);
			}
		}

		public void PossiblyMutate()
		{
			if (GeneticLogicRoot.RollMutateDice()) 
			{
				this.conditionalLeftStatement = new ConditionalLeftStatement(1);
				this.leftStatement = null;
				return;
			}

			if (GeneticLogicRoot.RollMutateDice()) 
			{
				this.conditionalLeftStatement = null;
				this.leftStatement = new LeftStatement(1);
				return;
			}

			if (this.conditionalLeftStatement != null) 
			{
				this.conditionalLeftStatement.PossiblyMutate();
			}

			if (this.leftStatement != null) 
			{
				this.leftStatement.PossiblyMutate();
			}
		}

		public byte Execute(ref EntBehaviorManager instance)
		{
			// Enforce parsing behavior we expect.
			if (this.conditionalLeftStatement != null &&
				this.leftStatement != null) 
			{
				throw new InvalidOperationException("Only one field should be non null");
			}

			if (this.conditionalLeftStatement != null) 
			{
				return this.conditionalLeftStatement.Execute(ref instance);
			} 
			else if (this.leftStatement != null)
			{
				return this.leftStatement.Execute(ref instance);
			} 
			else 
			{
				// Enforce correct parsing behavior.
				throw new InvalidOperationException("No field was populated during parsing and this should have been culled");
			}
		}
	}
}

