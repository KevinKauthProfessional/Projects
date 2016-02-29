//------------------------------------------------------------------
// <copyright file="RightStatement.cs">
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
	/// This class represents a statement that is read only returning a value of a given type
	/// that is then delivered to a LeftStatement of some kind.
	/// </summary>
	internal class RightStatement
	{
		public const string Name = "RightStatement";

		// Only one will be populated.
		private RightStatementOperation rightStatementOperation = null;
		private ReadOnlyVariable readOnlyVariable = null;
		private RightMethodCall rightMethodCall = null;
		private LiteralValue literalValue = null;

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.RightStatement"/> class.
		/// </summary>
		/// <param name="returnType">Return type.</param>
		public RightStatement(Type returnType)
		{
			if (returnType == null)
			{
				LogUtility.LogError("Cannot create right statement with null return type");
			}

			double nextDouble = CommonHelperMethods.GetRandomDouble0To1();

			if (nextDouble < 0.25) 
			{
				OperatorSignature signature;
				if (RegistrationManager.TrySelectOperatorAtRandom(returnType, out signature))
				{
					this.rightStatementOperation = new RightStatementOperation(signature);
					return;
				}
			}

			if (nextDouble < 0.5) 
			{
				VariableSignature signature;
				if (RegistrationManager.TrySelectReadOnlyVariableAtRandom(returnType, out signature))
				{
					this.readOnlyVariable = new ReadOnlyVariable(signature);
					return;
				}
			} 

			if (nextDouble < 0.75) 
			{
				MethodSignature signature;
				if (RegistrationManager.TrySelectRightMethodAtRandom(returnType, out signature))
				{
					this.rightMethodCall = new RightMethodCall(signature);
					return;
				}
			} 

			// Every GeneticType should support generation of a random literal value
			this.literalValue = new LiteralValue(returnType);
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.RightStatement"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public RightStatement (FileIOManager reader)
		{
			string nextLine = reader.ReadNextContentLineAndTrim (); 
			if (CommonHelperMethods.StringStartsWith (nextLine, RightStatementOperation.Name))
			{
				this.rightStatementOperation = new RightStatementOperation (reader);
			} 
			else if (CommonHelperMethods.StringStartsWith (nextLine, ReadOnlyVariable.Name))
			{
				this.readOnlyVariable = new ReadOnlyVariable (reader);
			} 
			else if (CommonHelperMethods.StringStartsWith (nextLine, RightMethodCall.Name)) 
			{
				this.rightMethodCall = new RightMethodCall (reader);
			} 
			else if (CommonHelperMethods.StringStartsWith (nextLine, LiteralValue.Name))
			{
				this.literalValue = new LiteralValue(reader);
			}
			else
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextLine,
					reader,
					new List<string>() { RightStatementOperation.Name, ReadOnlyVariable.Name, RightMethodCall.Name });
			}
		}

		public Type ReturnType
		{
			get
			{
				if (this.rightStatementOperation != null)
				{
					return this.rightStatementOperation.ReturnType;
				}
				else if (this.rightMethodCall != null)
				{
					return this.rightMethodCall.ReturnType;
				}
				else if (this.literalValue != null)
				{
					return this.literalValue.VariableType;
				}
				else if (this.readOnlyVariable != null)
				{
					return this.readOnlyVariable.VariableType;
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
			if (this.rightStatementOperation != null)
			{
				return this.rightStatementOperation.ToString();
			}
			else if (this.rightMethodCall != null)
			{
				return this.rightMethodCall.ToString();
			}
			else if (this.literalValue != null)
			{
				return this.literalValue.ToString();
			}
			else if (this.readOnlyVariable != null)
			{
				return this.readOnlyVariable.ToString();
			}

			return "Null RightStatement";
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="tabDepth">Tab depth.</param>
		public void WriteToDisk(FileIOManager writer, int tabDepth)
		{
			writer.WriteLine (CommonHelperMethods.PrePendTabs (RightStatement.Name, tabDepth));

			if (this.rightStatementOperation != null) 
			{
				this.rightStatementOperation.WriteToDisk(writer, tabDepth + 1);
			}

			if (this.readOnlyVariable != null) 
			{
				this.readOnlyVariable.WriteToDisk(writer, tabDepth + 1);
			}

			if (this.rightMethodCall != null) 
			{
				this.rightMethodCall.WriteToDisk(writer, tabDepth + 1);
			}

			if (this.literalValue != null) 
			{
				this.literalValue.WriteToDisk(writer, tabDepth + 1);
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
					this.rightStatementOperation = new RightStatementOperation(signature);
					this.readOnlyVariable = null;
					this.rightMethodCall = null;
					this.literalValue = null;
					return;
				}
			}

			if (GeneticLogicRoot.RollMutateDice ()) 
			{
				VariableSignature signature;
				if (RegistrationManager.TrySelectReadOnlyVariableAtRandom(this.ReturnType, out signature))
				{
					this.rightStatementOperation = null;
					this.readOnlyVariable = new ReadOnlyVariable(signature);
					this.rightMethodCall = null;
					this.literalValue = null;
					return;
				}
			}

			if (GeneticLogicRoot.RollMutateDice ()) 
			{
				MethodSignature signature;
				if (RegistrationManager.TrySelectRightMethodAtRandom(this.ReturnType, out signature))
				{
					this.rightStatementOperation = null;
					this.readOnlyVariable = null;
					this.rightMethodCall = new RightMethodCall(signature);
					this.literalValue = null;
					return;
				}
			}

			if (GeneticLogicRoot.RollMutateDice ()) 
			{
				// Create the new literal value first so ReturnType get doesn't reutrn null
				this.literalValue = new LiteralValue(this.ReturnType);
				this.rightStatementOperation = null;
				this.readOnlyVariable = null;
				this.rightMethodCall = null;
				return;
			}

			if (this.rightStatementOperation != null) 
			{
				this.rightStatementOperation.PossiblyMutate();
			}
		}

		/// <summary>
		/// Evaluate the specified instance.
		/// </summary>
		/// <param name="instance">Instance.</param>
		public GeneticObject Evaluate(ref EntBehaviorManager instance)
		{
			this.EnforceOneFieldIsNonNull ();

			if (this.rightStatementOperation != null) 
			{
				return this.rightStatementOperation.Evaluate(ref instance);
			}

			if (this.readOnlyVariable != null) 
			{
				return this.readOnlyVariable.Evaluate(ref instance);
			}

			if (this.rightMethodCall != null) 
			{
				return this.rightMethodCall.Execute(ref instance);
			}

			if (this.literalValue != null)
			{
				return this.literalValue.Value;
			}

			throw new InvalidOperationException ("Should never get here");
		}

		private void EnforceOneFieldIsNonNull()
		{
			int nonNullFieldCount = 0;
			if (this.rightStatementOperation != null) 
			{
				nonNullFieldCount++;
			}

			if (this.readOnlyVariable != null) 
			{
				nonNullFieldCount++;
			}

			if (this.rightMethodCall != null) 
			{
				nonNullFieldCount++;
			}

			if (this.literalValue != null)
			{
				nonNullFieldCount++;
			}

			if (nonNullFieldCount != 1) 
			{
				throw new InvalidOperationException("Expecting exactly one non null field");
			}
		}
	}
}

