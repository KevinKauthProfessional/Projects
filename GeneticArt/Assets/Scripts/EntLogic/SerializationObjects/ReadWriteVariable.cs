//------------------------------------------------------------------
// <copyright file="ReadWriteVariable.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace Assets.Scripts.EntLogic.SerializationObjects
{
    using Assets.Scripts.EntLogic.GeneticMemberRegistration;
    using AssemblyCSharp.Scripts.UnityGameObjects;
    using Assets.Scripts.Utilities;

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
		public ReadWriteVariable (FileIOManager reader) :
			base(reader)
		{
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		public override void WriteToDisk(FileIOManager writer)
		{
            writer.WriteByte(StatementTypeEnum.ReadWriteVariable);
			this.WriteToDiskProtected(writer);
		}

		/// <summary>
		/// Writes to this variable.
		/// </summary>
		/// <param name="instance">The instance to execute against.</param>
		/// <param name="payload">The value to write.</param>
		public void WriteToVariable(ref EntBehaviorManager instance, RightStatement payload)
		{
			byte value = payload.Evaluate(ref instance);
			instance.WriteToVariable(this.signature, value);
		}
	}
}

