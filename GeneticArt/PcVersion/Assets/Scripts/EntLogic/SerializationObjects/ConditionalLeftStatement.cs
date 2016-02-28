//------------------------------------------------------------------
// <copyright file="ConditionalLeftStatement.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.SerializationObjects
{
	using System;
	using System.IO;

	using AssemblyCSharp.Scripts.EntLogic.GeneticTypes;
	using AssemblyCSharp.Scripts.UnityGameObjects;
	using AssemblyCSharp.Scripts.Utilities;

	/// <summary>
	/// This class represents a "conditional left statement".  That means a Condition
	/// which if true will result in the execution of a LeftStatement at run time.
	/// </summary>
	internal class ConditionalLeftStatement
	{
		public const string Name = "ConditionalLeftStatement";

		// Both parsed and populated
		private Condition condition;
		private LeftStatement leftStatement;

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.ConditionalLeftStatement"/> class.
		/// </summary>
		public ConditionalLeftStatement()
		{
			this.condition = new Condition ();
			this.leftStatement = new LeftStatement ();
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.ConditionalLeftStatement"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public ConditionalLeftStatement (CustomFileReader reader)
		{
			// Condition block parse
			string nextLine = reader.ReadNextContentLineAndTrim ();
			if (CommonHelperMethods.StringStartsWith (nextLine, Condition.Name)) 
			{
				this.condition = new Condition(reader);
			} 
			else
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextLine,
					reader,
					Condition.Name);
			}

			// LeftStatement block parse
			nextLine = reader.ReadNextContentLineAndTrim ();
			if (CommonHelperMethods.StringStartsWith (nextLine, LeftStatement.Name)) 
			{
				this.leftStatement = new LeftStatement (reader);
			} 
			else 
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextLine,
					reader,
					LeftStatement.Name);
			}
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return string.Format ("if {0} then {1}", this.condition, this.leftStatement);
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="tabDepth">Tab depth.</param>
		public void WriteToDisk(StreamWriter writer, int tabDepth)
		{
			writer.WriteLine (CommonHelperMethods.PrePendTabs (ConditionalLeftStatement.Name, tabDepth));

			if (this.condition != null) 
			{
				this.condition.WriteToDisk(writer, tabDepth + 1);
			}

			if (this.leftStatement != null) 
			{
				this.leftStatement.WriteToDisk(writer, tabDepth + 1);
			}
		}

		/// <summary>
		/// Will possibly mutate this section of logic.
		/// </summary>
		public void PossiblyMutate()
		{
			if (GeneticLogicRoot.RollMutateDice ()) 
			{
				this.condition = new Condition();
				this.leftStatement = new LeftStatement();
				return;
			}

			if (this.condition != null) 
			{
				this.condition.PossiblyMutate();
			}

			if (this.leftStatement != null) 
			{
				this.leftStatement.PossiblyMutate();
			}
		}

		/// <summary>
		/// Evaluates the Condition, and if true, executes the LeftStatement.
		/// </summary>
		/// <param name="instance">The instance to execute against.</param>
		public bool Execute(ref EntBehaviorManager instance)
		{
			GeneticBool condition = this.condition.Evaluate (ref instance);

			if ((bool)condition.Value) 
			{
				return this.leftStatement.Execute (ref instance);
			}

			return false;
		}
	}
}

