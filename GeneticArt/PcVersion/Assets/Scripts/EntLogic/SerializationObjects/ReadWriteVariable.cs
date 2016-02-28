//------------------------------------------------------------------
// <copyright file="ReadWriteVariable.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.SerializationObjects
{
	using System;
	using System.IO;

	using AssemblyCSharp.Scripts.EntLogic.GeneticMemberRegistration;
	using AssemblyCSharp.Scripts.EntLogic.GeneticTypes;
	using AssemblyCSharp.Scripts.UnityGameObjects;
	using AssemblyCSharp.Scripts.Utilities;

	/// <summary>
	/// This class is used to represent members of an object with genetic logic that 
	/// can be read AND written to by the genetic logic.  Due to the unpredictable
	/// nature of how the genetic logic will use these variables, they should only 
	/// be depended on by the code sections that are governed by genetic logic.
	/// </summary>
	internal class ReadWriteVariable : ReadOnlyVariable
	{
		new public const string Name = "ReadWriteVariable";

		public ReadWriteVariable(VariableSignature signature) :
			base(signature)
		{
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.KeyGeneticReadWriteVariable`1"/> class.
		/// </summary>
		/// <param name="value">The value of the variable.</param>
		public ReadWriteVariable (CustomFileReader reader) :
			base(reader)
		{
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="tabDepth">Tab depth.</param>
		public override void WriteToDisk (StreamWriter writer, int tabDepth)
		{
			writer.WriteLine (CommonHelperMethods.PrePendTabs (ReadWriteVariable.Name, tabDepth));
			this.WriteToDiskProtected (writer, tabDepth + 1);
		}

		/// <summary>
		/// Writes to this variable.
		/// </summary>
		/// <param name="instance">The instance to execute against.</param>
		/// <param name="payload">The value to write.</param>
		public void WriteToVariable(ref EntBehaviorManager instance, RightStatement payload)
		{
			GeneticObject value = payload.Evaluate (ref instance);
			instance.WriteToVariable (this.signature, value);
		}
	}
}

