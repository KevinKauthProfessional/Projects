//------------------------------------------------------------------
// <copyright file="Assignment.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace Assets.Scripts.EntLogic.SerializationObjects
{
    using Assets.Scripts.EntLogic.GeneticMemberRegistration;
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
        private int depth;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.Assignment"/> class.
		/// </summary>
		public Assignment(int depthIn)
		{
            this.depth = depthIn;
			VariableSignature signature = RegistrationManager.SelectReadWriteVariableAtRandom ();

			this.readWriteVariable = new ReadWriteVariable(signature);
			this.rightStatement = new RightStatement(signature.VariableType, depthIn + 1);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.Assignment"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public Assignment (FileIOManager reader, int depthIn)
		{
            this.depth = depthIn;

            byte nextByte = reader.ReadByte();
			if (nextByte == StatementTypeEnum.ReadWriteVariable) 
			{
				this.readWriteVariable = new ReadWriteVariable(reader);
			} 
			else 
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextByte,
					reader,
					StatementTypeEnum.ReadWriteVariable);
			}

            nextByte = reader.ReadByte();
			if (nextByte == StatementTypeEnum.RightStatement) 
			{
				this.rightStatement = new RightStatement(reader, depthIn + 1);
			} 
			else 
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextByte,
					reader,
					StatementTypeEnum.RightStatement);
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
		public void WriteToDisk(FileIOManager writer)
		{
            writer.WriteByte(StatementTypeEnum.Assignment);

			if (this.readWriteVariable != null) 
			{
				this.readWriteVariable.WriteToDisk(writer);
			}

			if (this.rightStatement != null) 
			{
				this.rightStatement.WriteToDisk(writer);
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
				this.rightStatement = new RightStatement(signature.VariableType, this.depth + 1);
				return;
			}

			if (this.rightStatement != null) 
			{
				this.rightStatement.PossiblyMutate();
			}
		}

		/// <summary>
		/// Executes the assignment.
		/// </summary>
		/// <param name="instance">The instance to evaluate against.</param>
		public void Execute(ref EntBehaviorManager instance)
		{
			this.readWriteVariable.WriteToVariable(ref instance, this.rightStatement);
		}
	}
}

