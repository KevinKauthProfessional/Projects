//------------------------------------------------------------------
// <copyright file="Condition.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.SerializationObjects
{
	using System;

	using AssemblyCSharp.Scripts.EntLogic.GeneticTypes;
	using AssemblyCSharp.Scripts.UnityGameObjects;
	using Assets.Scripts.Utilities;

	/// <summary>
	/// This class represents a boolean condition statement.
	/// </summary>
	internal class Condition
	{
		public const string Name = "Condition";

		private RightStatement rightStatement;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.Condition"/> class.
		/// </summary>
		public Condition()
		{
			this.rightStatement = new RightStatement (typeof(GeneticBool));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.Condition"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public Condition (FileIOManager reader)
		{
			string nextLine = reader.ReadNextContentLineAndTrim ();
			if (CommonHelperMethods.StringStartsWith (nextLine, RightStatement.Name)) 
			{
				this.rightStatement = new RightStatement (reader);
			} 
			else 
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextLine,
					reader,
					RightStatement.Name);
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
		/// <param name="tabDepth">Tab depth.</param>
		public void WriteToDisk(FileIOManager writer, int tabDepth)
		{
			writer.WriteLine (CommonHelperMethods.PrePendTabs (Condition.Name, tabDepth));

			if (this.rightStatement != null) 
			{
				this.rightStatement.WriteToDisk(writer, tabDepth + 1);
			}
		}

		/// <summary>
		/// Will possibly mutate this section of logic.
		/// </summary>
		public void PossiblyMutate()
		{
			if (GeneticLogicRoot.RollMutateDice ()) 
			{
				this.rightStatement = new RightStatement(typeof(bool));
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
		public GeneticBool Evaluate(ref EntBehaviorManager instance)
		{
			GeneticObject evaluatedObject = this.rightStatement.Evaluate (ref instance);

			if (evaluatedObject is GeneticBool) 
			{
				return (GeneticBool)evaluatedObject;
			}

			throw new InvalidOperationException("Condition statement did not evaluate to a GeneticBool");
		}
	}
}

