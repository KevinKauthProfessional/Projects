//------------------------------------------------------------------
// <copyright file="LeftStatement.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.SerializationObjects
{
	using System;
	using System.Collections.Generic;

	using AssemblyCSharp.Scripts.EntLogic.GeneticMemberRegistration;
	using AssemblyCSharp.Scripts.UnityGameObjects;
	using Assets.Scripts.Utilities;

	/// <summary>
	/// This class represents a statement that results in a write operation.
	/// Can be either a LeftMethodCall or an Assignment into a writable variable.
	/// </summary>
	internal class LeftStatement
	{
		public const string Name = "LeftStatement";

		// One or the other
		private LeftMethodCall leftMethodCall = null;
		private Assignment assignment = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.LeftStatement"/> class.
		/// </summary>
		public LeftStatement()
		{
			double nextDouble = CommonHelperMethods.GetRandomDouble0To1();

			if (nextDouble < 0.5) 
			{
				MethodSignature signature = RegistrationManager.SelectLeftMethodAtRandom ();
				this.leftMethodCall = new LeftMethodCall(signature);
			}
			else
			{
				this.assignment = new Assignment();
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.LeftStatement"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public LeftStatement (FileIOManager reader)
		{
			string nextLine = reader.ReadNextContentLineAndTrim ();
			if (CommonHelperMethods.StringStartsWith(nextLine, LeftMethodCall.Name))
			{
				this.leftMethodCall = new LeftMethodCall(reader);
			}
			else if (CommonHelperMethods.StringStartsWith(nextLine, Assignment.Name))
			{
				this.assignment = new Assignment(reader);
			}
			else
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextLine,
					reader,
					new List<string>() { LeftMethodCall.Name, Assignment.Name });
			}
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			if (this.leftMethodCall != null) 
			{
				return this.leftMethodCall.ToString();
			}

			if (this.assignment != null) 
			{
				return this.assignment.ToString();
			}

			return "Null LeftStatement";
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="tabDepth">Tab depth.</param>
		public void WriteToDisk(FileIOManager writer, int tabDepth)
		{
			writer.WriteLine (CommonHelperMethods.PrePendTabs (LeftStatement.Name, tabDepth));

			if (this.leftMethodCall != null) 
			{
				this.leftMethodCall.WriteToDisk(writer, tabDepth + 1);
			}

			if (this.assignment != null) 
			{
				this.assignment.WriteToDisk(writer, tabDepth + 1);
			}
		}

		/// <summary>
		/// Will possibly mutate this section of logic.
		/// </summary>
		public void PossiblyMutate()
		{
			if (GeneticLogicRoot.RollMutateDice ()) 
			{
				MethodSignature signature = RegistrationManager.SelectLeftMethodAtRandom();
				this.leftMethodCall = new LeftMethodCall(signature);
				this.assignment = null;
				return;
			}

			if (GeneticLogicRoot.RollMutateDice ()) 
			{
				this.leftMethodCall = null;
				this.assignment = new Assignment();
				return;
			}

			// Note: Method calls and variables are possibly replaced, but not themselves mutated.
			if (this.assignment != null) 
			{
				this.assignment.PossiblyMutate();
			}
		}

		/// <summary>
		/// Evaluates this statement.
		/// </summary>
		/// <param name="instance">The instance to evaluate against.</param>
		public bool Execute(ref EntBehaviorManager instance)
		{
			if (this.leftMethodCall != null &&
				this.assignment != null) 
			{
				throw new InvalidOperationException("Only one field should be not null but both are not null.");
			}

			if (this.leftMethodCall != null) 
			{
				// End of logic for the frame if successful!
				return this.leftMethodCall.Execute(ref instance);
			}

			if (this.assignment != null) 
			{
				this.assignment.Execute(ref instance);
				return false;
			}

			throw new InvalidOperationException("One field should be not null but they are both null.");
		}
	}
}

