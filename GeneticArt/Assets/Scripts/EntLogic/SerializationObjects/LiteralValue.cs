//------------------------------------------------------------------
// <copyright file="LiteralValue.cs">
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
	/// This class represents a literal value.
	/// </summary>
	public class LiteralValue
	{
		public const string Name = "LiteralValue";

		private Type type = null;
		private GeneticObject value = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.LiteralValue"/> class.
		/// </summary>
		/// <param name="returnType">Return type.</param>
		public LiteralValue(Type returnType)
		{
			if (returnType == null)
			{
				LogUtility.LogError("Cannot create literal value with null return type");
			}

			this.type = returnType;
			this.value = GeneticObject.CreateTypeAtRandom (this.type);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.LiteralValue"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public LiteralValue (FileIOManager reader)
		{
			string nextLine = reader.ReadNextContentLineAndTrim ();
			this.type = GeneticObject.ParseType(nextLine);

			nextLine = reader.ReadNextContentLineAndTrim ();
			this.value = GeneticObject.ParseValue (this.type, nextLine);
		}

		/// <summary>
		/// Gets the type of the variable.
		/// </summary>
		/// <value>The type of the variable.</value>
		public Type VariableType
		{
			get
			{
				return this.type;
			}
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public GeneticObject Value
		{
			get
			{
				return this.value;
			}
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return this.Value.ToString();
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="tabDepth">Tab depth.</param>
		public void WriteToDisk(FileIOManager writer, int tabDepth)
		{
			writer.WriteLine (CommonHelperMethods.PrePendTabs (Name, tabDepth));

			if (this.type != null) 
			{
				writer.WriteLine(CommonHelperMethods.PrePendTabs(this.type.ToString(), tabDepth + 1));
			}

			if (this.value != null) 
			{
				writer.WriteLine(CommonHelperMethods.PrePendTabs(this.value.ToString(), tabDepth + 1));
			}
		}
	}
}

