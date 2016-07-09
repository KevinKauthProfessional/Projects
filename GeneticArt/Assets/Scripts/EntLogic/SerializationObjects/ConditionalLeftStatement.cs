//------------------------------------------------------------------
// <copyright file="ConditionalLeftStatement.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace Assets.Scripts.EntLogic.SerializationObjects
{
    using AssemblyCSharp.Scripts.UnityGameObjects;
    using Assets.Scripts.Utilities;

    /// <summary>
    /// This class represents a "conditional left statement".  That means a Condition
    /// which if true will result in the execution of a LeftStatement at run time.
    /// </summary>
    internal class ConditionalLeftStatement
	{
		public const string Name = "ConditionalLeftStatement";

		// Both parsed and populated
		private Condition condition;
		private LeftStatement leftStatement;
        private int depth;

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.ConditionalLeftStatement"/> class.
		/// </summary>
		public ConditionalLeftStatement(int depthIn)
		{
            this.depth = depthIn;
			this.condition = new Condition(depthIn + 1);
			this.leftStatement = new LeftStatement(depthIn + 1);
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.ConditionalLeftStatement"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public ConditionalLeftStatement (FileIOManager reader, int depthIn)
		{
            this.depth = depthIn;

			// Condition block parse
			byte nextByte = reader.ReadByte();
			if (nextByte == StatementTypeEnum.Condition) 
			{
				this.condition = new Condition(reader, depthIn + 1);
			} 
			else
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextByte,
					reader,
					StatementTypeEnum.Condition);
			}

			// LeftStatement block parse
			nextByte = reader.ReadByte();
			if (nextByte == StatementTypeEnum.LeftStatement) 
			{
				this.leftStatement = new LeftStatement(reader, depthIn + 1);
			} 
			else 
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextByte,
					reader,
					StatementTypeEnum.LeftStatement);
			}
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return string.Format ("if {0} then {1}", this.condition, this.leftStatement);
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		public void WriteToDisk(FileIOManager writer)
		{
            writer.WriteByte(StatementTypeEnum.ConditionalLeftStatement);

			if (this.condition != null) 
			{
				this.condition.WriteToDisk(writer);
			}

			if (this.leftStatement != null) 
			{
				this.leftStatement.WriteToDisk(writer);
			}
		}

		/// <summary>
		/// Will possibly mutate this section of logic.
		/// </summary>
		public void PossiblyMutate()
		{
			if (GeneticLogicRoot.RollMutateDice ()) 
			{
				this.condition = new Condition(this.depth + 1);
				this.leftStatement = new LeftStatement(this.depth + 1);
				return;
			}

			if (this.condition != null) 
			{
				this.condition.PossiblyMutate();
			}

			if (this.leftStatement != null) 
			{
				this.leftStatement.PossiblyMutate();
			}
		}

		/// <summary>
		/// Evaluates the Condition, and if true, executes the LeftStatement.
		/// </summary>
		/// <param name="instance">The instance to execute against.</param>
		public byte Execute(ref EntBehaviorManager instance)
		{
			byte condition = this.condition.Evaluate(ref instance);

			if (condition == 1) 
			{
				return this.leftStatement.Execute(ref instance);
			}

			return condition;
		}
	}
}

