//------------------------------------------------------------------
// <copyright file="ReadOnlyVariable.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.SerializationObjects
{
	using System;

	using AssemblyCSharp.Scripts.EntLogic.GeneticMemberRegistration;
	using AssemblyCSharp.Scripts.EntLogic.GeneticTypes;
	using AssemblyCSharp.Scripts.UnityGameObjects;
	using Assets.Scripts.Utilities;

	/// <summary>
	/// This class represents a variable belonging to an object with genetic logic
	/// that is read only to the genetic logic in that object.
	/// </summary>
	internal class ReadOnlyVariable
	{
		public const string Name = "ReadOnlyVariable";

		protected VariableSignature signature = null;

		public ReadOnlyVariable(VariableSignature signatureIn)
		{
			this.signature = signatureIn;
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.ReadOnlyVariable"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public ReadOnlyVariable (FileIOManager reader)
		{
			this.signature = new VariableSignature (reader);
		}

		public Type VariableType
		{
			get
			{
				if (this.signature != null)
				{
					return this.signature.VariableType;
				}

				return null;
			}
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return this.signature.VariableId.ToString();
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="tabDepth">Tab depth.</param>
		public virtual void WriteToDisk(FileIOManager writer, int tabDepth)
		{
			writer.WriteLine (CommonHelperMethods.PrePendTabs (ReadOnlyVariable.Name, tabDepth));
			this.WriteToDiskProtected (writer, tabDepth + 1);
		}

		/// <summary>
		/// Writes to disk protected.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="tabDepth">Tab depth.</param>
		protected void WriteToDiskProtected(FileIOManager writer, int tabDepth)
		{
			if (this.signature != null) 
			{
				this.signature.WriteToDisk(writer, tabDepth);
			}
		}

		/// <summary>
		/// Evaluates the variable's value.
		/// </summary>
		/// <param name="instance">The instance to evaluate against.</param>
		public virtual GeneticObject Evaluate(ref EntBehaviorManager instance)
		{
			return instance.ReadFromVariable (this.signature);
		}
	}
}

