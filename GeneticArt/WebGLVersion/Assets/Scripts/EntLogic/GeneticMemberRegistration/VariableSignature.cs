//------------------------------------------------------------------
// <copyright file="VariableSignature.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.GeneticMemberRegistration
{
	using System;
	using System.IO;

	using AssemblyCSharp.Scripts.EntLogic.GeneticTypes;
	using AssemblyCSharp.Scripts.UnityGameObjects;
	using AssemblyCSharp.Scripts.Utilities;

	/// <summary>
	/// Variable signature.
	/// </summary>
	public class VariableSignature
	{
		private Type variableType;
		private EntVariableEnum variableId;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.Attributes.VariableSignature"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="variableTypeIn">Variable type in.</param>
		public VariableSignature (EntVariableEnum variableIdIn, Type variableTypeIn)
		{
			this.VariableId = variableIdIn;
			this.variableType = variableTypeIn;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.Attributes.VariableSignature"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public VariableSignature (CustomFileReader reader)
		{
			string nextLine = reader.ReadNextContentLineAndTrim ();
			this.variableType = GeneticObject.ParseType (nextLine);
			
			this.VariableId = (EntVariableEnum)Enum.Parse(typeof(EntVariableEnum), reader.ReadNextContentLineAndTrim());
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public EntVariableEnum VariableId 
		{ 
			get
			{
				return this.variableId;
			}

			private set
			{
				this.variableId = value;
			}
		}

		/// <summary>
		/// Gets the type of the variable.
		/// </summary>
		/// <value>The type of the variable.</value>
		public Type VariableType
		{
			get
			{
				return this.variableType;
			}
		}

		public override string ToString ()
		{
			return string.Format (
				"[VariableSignature: VariableId={0}, VariableType={1}]",
				this.VariableId, 
				this.VariableType);
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="tabDepth">Tab depth.</param>
		public void WriteToDisk(StreamWriter writer, int tabDepth)
		{
			writer.WriteLine(CommonHelperMethods.PrePendTabs(this.VariableType.ToString(), tabDepth));
			writer.WriteLine(CommonHelperMethods.PrePendTabs(this.VariableId.ToString(), tabDepth));
		}
	}
}

