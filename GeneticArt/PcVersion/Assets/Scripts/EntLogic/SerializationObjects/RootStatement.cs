//------------------------------------------------------------------
// <copyright file="RootStatement.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.SerializationObjects
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	using AssemblyCSharp.Scripts.UnityGameObjects;
	using AssemblyCSharp.Scripts.Utilities;

	/// <summary>
	/// This class represents a root level statement in a method controlled by genetic logic.
	/// The statement can be either a ConditionalLeftStatement or a LeftStatement.
	/// </summary>
	internal class RootStatement
	{
		public const string Name = "RootStatement";

		// Only one will be populated.
		private ConditionalLeftStatement conditionalLeftStatement = null;
		private LeftStatement leftStatement = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.RootStatement"/> class.
		/// </summary>
		public RootStatement()
		{
			int value = CommonHelperMethods.GetRandomPositiveInt0ToValue (1);

			if (value == 0)
			{
				this.conditionalLeftStatement = new ConditionalLeftStatement ();
			}
			else
			{
				this.leftStatement = new LeftStatement();
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.RootStatement"/> class.
		/// </summary>
		/// <param name="reader">A StreamReader pointed to line where this RootStatement begins.</param>
		public RootStatement(CustomFileReader reader)
		{
			string nextLine = reader.ReadNextContentLineAndTrim ();
			if (CommonHelperMethods.StringStartsWith (nextLine, ConditionalLeftStatement.Name)) 
			{
				this.conditionalLeftStatement = new ConditionalLeftStatement (reader);
			} 
			else if (CommonHelperMethods.StringStartsWith (nextLine, LeftStatement.Name)) 
			{
				this.leftStatement = new LeftStatement (reader);
			} 
			else 
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextLine,
					reader,
					new List<string>() { ConditionalLeftStatement.Name, LeftStatement.Name });
			}
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="tabDepth">Tab depth.</param>
		public void WriteToDisk(StreamWriter writer, int tabDepth)
		{
			writer.WriteLine (CommonHelperMethods.PrePendTabs (RootStatement.Name, tabDepth));

			if (this.conditionalLeftStatement != null)
			{
				this.conditionalLeftStatement.WriteToDisk (writer, tabDepth + 1);
			} 

			if (this.leftStatement != null)
			{
				this.leftStatement.WriteToDisk (writer, tabDepth + 1);
			}
		}

		public void PossiblyMutate()
		{
			if (GeneticLogicRoot.RollMutateDice()) 
			{
				this.conditionalLeftStatement = new ConditionalLeftStatement();
				this.leftStatement = null;
				return;
			}

			if (GeneticLogicRoot.RollMutateDice()) 
			{
				this.conditionalLeftStatement = null;
				this.leftStatement = new LeftStatement();
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

		public bool Execute(ref EntBehaviorManager instance)
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

