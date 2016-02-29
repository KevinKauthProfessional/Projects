//------------------------------------------------------------------
// <copyright file="RightStatementOperation.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.SerializationObjects
{
	using System;
	using System.Collections.Generic;

	using AssemblyCSharp.Scripts.EntLogic.GeneticMemberRegistration;
	using AssemblyCSharp.Scripts.EntLogic.GeneticTypes;
	using AssemblyCSharp.Scripts.UnityGameObjects;
	using Assets.Scripts.Utilities;

	/// <summary>
	/// This class represents a read only operation statement that returns a value 
	/// to the calling LeftStatement when executed.
	/// </summary>
	internal class RightStatementOperation
	{
		public const string Name = "RightStatementOperation";

		// All will be parsed and populated.
		private RightStatement leftHandSide;
		private OperatorSignature operatorSignature;
		private RightStatement rightHandSide;

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.RightStatementOperation"/> class.
		/// </summary>
		/// <param name="returnType">Return type.</param>
		public RightStatementOperation(OperatorSignature signature)
		{
			this.operatorSignature = signature;
			this.leftHandSide = new RightStatement (signature.LhsType);
			this.rightHandSide = new RightStatement (signature.RhsType);
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.RightStatementOperation`1"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public RightStatementOperation (FileIOManager reader)
		{
			// Parse operator
			this.operatorSignature = new OperatorSignature (reader);

			// Parse left hand side
			string nextLine = reader.ReadNextContentLineAndTrim ();
			if (CommonHelperMethods.StringStartsWith (nextLine, RightStatement.Name)) 
			{
				this.leftHandSide = new RightStatement (reader);
			}
			else
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextLine, 
					reader, 
					RightStatement.Name);
			}

			// Parse right hand side
			nextLine = reader.ReadNextContentLineAndTrim ();
			if (CommonHelperMethods.StringStartsWith (nextLine, RightStatement.Name)) 
			{
				this.rightHandSide = new RightStatement (reader);
			}
			else
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextLine, 
					reader, 
					RightStatement.Name);
			}
		}
	
		/// <summary>
		/// Gets the type of the return.
		/// </summary>
		/// <value>The type of the return.</value>
		public Type ReturnType
		{
			get
			{
				if (this.operatorSignature != null)
				{
					return this.operatorSignature.ReturnType;
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
			return this.operatorSignature.ToString ();
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="tabDepth">Tab depth.</param>
		public void WriteToDisk(FileIOManager writer, int tabDepth)
		{
			writer.WriteLine (CommonHelperMethods.PrePendTabs (RightStatementOperation.Name, tabDepth));

			if (this.operatorSignature != null) 
			{
				this.operatorSignature.WriteToDisk(writer, tabDepth + 1);
			}

			if (this.leftHandSide != null) 
			{
				this.leftHandSide.WriteToDisk(writer, tabDepth + 1);
			}

			if (this.rightHandSide != null) 
			{
				this.rightHandSide.WriteToDisk(writer, tabDepth + 1);
			}
		}

		/// <summary>
		/// Will possibly mutate this section of logic.
		/// </summary>
		public void PossiblyMutate()
		{
			if (GeneticLogicRoot.RollMutateDice ()) 
			{
				OperatorSignature signature;
				if (RegistrationManager.TrySelectOperatorAtRandom(this.ReturnType, out signature))
				{
					this.operatorSignature = signature;
					this.leftHandSide = new RightStatement(signature.LhsType);
					this.rightHandSide = new RightStatement(signature.RhsType);
					return;
				}
			}

			if (GeneticLogicRoot.RollMutateDice ()) 
			{
				this.rightHandSide = new RightStatement(this.operatorSignature.RhsType);
				return;
			}

			if (GeneticLogicRoot.RollMutateDice ()) 
			{
				this.leftHandSide = new RightStatement(this.operatorSignature.LhsType);
				return;
			}

			if (this.rightHandSide != null) 
			{
				this.rightHandSide.PossiblyMutate();
			}

			if (this.leftHandSide != null) 
			{
				this.leftHandSide.PossiblyMutate();
			}
		}

		/// <summary>
		/// Evaluate this statement against the specified instance.
		/// </summary>
		/// <param name="instance">Instance.</param>
		public GeneticObject Evaluate(ref EntBehaviorManager instance)
		{
			GeneticObject leftHandObject = this.leftHandSide.Evaluate (ref instance);
			GeneticObject rightHandObject = this.rightHandSide.Evaluate (ref instance);

			return GeneticObject.EvaluateOperator (this.operatorSignature, leftHandObject, rightHandObject);
		}
	}
}

