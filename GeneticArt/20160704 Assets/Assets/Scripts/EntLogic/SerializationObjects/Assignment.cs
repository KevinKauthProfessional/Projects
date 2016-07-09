//------------------------------------------------------------------
// <copyright file="Assignment.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.SerializationObjects
{
	using System;

	using AssemblyCSharp.Scripts.EntLogic.GeneticMemberRegistration;
	using AssemblyCSharp.Scripts.UnityGameObjects;
	using Assets.Scripts.Utilities;
	
	/// <summary>
	/// A class representing the assignment operation.
	/// </summary>
	internal class Assignment
	{
		public const string Name = "Assignment";

		// Both parsed and populated
		private ReadWriteVariable readWriteVariable = null;
		private RightStatement rightStatement = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.Assignment"/> class.
		/// </summary>
		public Assignment()
		{
			VariableSignature signature = RegistrationManager.SelectReadWriteVariableAtRandom ();

			this.readWriteVariable = new ReadWriteVariable (signature);
			this.rightStatement = new RightStatement (signature.VariableType);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.Assignment"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public Assignment (FileIOManager reader)
		{
			string nextLine = reader.ReadNextContentLineAndTrim ();
			if (CommonHelperMethods.StringStartsWith (nextLine, ReadWriteVariable.Name)) 
			{
				this.readWriteVariable = new ReadWriteVariable(reader);
			} 
			else 
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextLine,
					reader,
					ReadWriteVariable.Name);
			}

			nextLine = reader.ReadNextContentLineAndTrim ();
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
			return string.Format ("{0} = {1}", this.readWriteVariable, this.rightStatement);
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="tabDepth">Tab depth.</param>
		public void WriteToDisk(FileIOManager writer, int tabDepth)
		{
			writer.WriteLine (CommonHelperMethods.PrePendTabs (Assignment.Name, tabDepth));

			if (this.readWriteVariable != null) 
			{
				this.readWriteVariable.WriteToDisk(writer, tabDepth + 1);
			}

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
			if (GeneticLogicRoot.RollMutateDice())  
			{
				VariableSignature signature = RegistrationManager.SelectReadWriteVariableAtRandom();
				this.readWriteVariable = new ReadWriteVariable(signature);
				this.rightStatement = new RightStatement(signature.VariableType);
				return;
			}

			if (this.rightStatement != null) 
			{
				this.rightStatement.PossiblyMutate ();
			}
		}

		/// <summary>
		/// Executes the assignment.
		/// </summary>
		/// <param name="instance">The instance to evaluate against.</param>
		public void Execute(ref EntBehaviorManager instance)
		{
			this.readWriteVariable.WriteToVariable (ref instance, this.rightStatement);
		}
	}
}

