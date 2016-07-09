//------------------------------------------------------------------
// <copyright file="LeftStatement.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace Assets.Scripts.EntLogic.SerializationObjects
{
    using Assets.Scripts.EntLogic.GeneticMemberRegistration;
    using AssemblyCSharp.Scripts.UnityGameObjects;
    using Assets.Scripts.Utilities;
    using System;
    using System.Collections.Generic;

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

        private int depth;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.LeftStatement"/> class.
		/// </summary>
		public LeftStatement(int depthIn)
		{
            this.depth = depthIn;

			double nextDouble = CommonHelperMethods.GetRandomDouble0To1();

			if (nextDouble < 0.5) 
			{
				MethodSignature signature = RegistrationManager.SelectLeftMethodAtRandom ();
				this.leftMethodCall = new LeftMethodCall(signature, depthIn + 1);
			}
			else
			{
				this.assignment = new Assignment(depthIn + 1);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.LeftStatement"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public LeftStatement(FileIOManager reader, int depthIn)
		{
            this.depth = depthIn;

            byte nextByte = reader.ReadByte();
			if (nextByte == StatementTypeEnum.LeftMethodCall)
			{
				this.leftMethodCall = new LeftMethodCall(reader, depthIn + 1);
			}
			else if (nextByte == StatementTypeEnum.Assignment)
			{
				this.assignment = new Assignment(reader, depthIn + 1);
			}
			else
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextByte,
					reader,
					new List<byte>() { StatementTypeEnum.LeftMethodCall, StatementTypeEnum.Assignment });
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
		public void WriteToDisk(FileIOManager writer)
		{
            writer.WriteByte(StatementTypeEnum.LeftStatement);

			if (this.leftMethodCall != null) 
			{
				this.leftMethodCall.WriteToDisk(writer);
			}

			if (this.assignment != null) 
			{
				this.assignment.WriteToDisk(writer);
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
				this.leftMethodCall = new LeftMethodCall(signature, this.depth + 1);
				this.assignment = null;
				return;
			}

			if (GeneticLogicRoot.RollMutateDice ()) 
			{
				this.leftMethodCall = null;
				this.assignment = new Assignment(this.depth + 1);
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
		public byte Execute(ref EntBehaviorManager instance)
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
				return 0;
			}

			throw new InvalidOperationException("One field should be not null but they are both null.");
		}
	}
}

