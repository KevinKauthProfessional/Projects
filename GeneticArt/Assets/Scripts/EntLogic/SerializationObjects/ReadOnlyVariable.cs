//------------------------------------------------------------------
// <copyright file="ReadOnlyVariable.cs">
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

		public byte VariableType
		{
			get
			{
				if (this.signature != null)
				{
					return this.signature.VariableType;
				}

				return 0;
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
		public virtual void WriteToDisk(FileIOManager writer)
		{
            writer.WriteByte(StatementTypeEnum.ReadOnlyVariable);
			this.WriteToDiskProtected(writer);
		}

		/// <summary>
		/// Writes to disk protected.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="tabDepth">Tab depth.</param>
		protected void WriteToDiskProtected(FileIOManager writer)
		{
			if (this.signature != null) 
			{
				this.signature.WriteToDisk(writer);
			}
		}

		/// <summary>
		/// Evaluates the variable's value.
		/// </summary>
		/// <param name="instance">The instance to evaluate against.</param>
		public virtual byte Evaluate(ref EntBehaviorManager instance)
		{
			return instance.ReadFromVariable (this.signature);
		}
	}
}

