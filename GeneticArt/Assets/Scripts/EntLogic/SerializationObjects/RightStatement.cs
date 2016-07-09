//------------------------------------------------------------------
// <copyright file="RightStatement.cs">
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
        private int depth;

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.RightStatement"/> class.
		/// </summary>
		/// <param name="returnType">Return type.</param>
		public RightStatement(byte returnType, int depthIn)
		{
            this.depth = depthIn;
			double nextDouble = CommonHelperMethods.GetRandomDouble0To1();

            if (depthIn >= RootStatement.MaxDepth)
            {
                // Force use of literal
                nextDouble = 1.0;
            }

			if (nextDouble < 0.25) 
			{
				OperatorSignature signature;
				if (RegistrationManager.TrySelectOperatorAtRandom(returnType, out signature))
				{
					this.rightStatementOperation = new RightStatementOperation(signature, depthIn + 1);
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
					this.rightMethodCall = new RightMethodCall(signature, this.depth + 1);
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
		public RightStatement(FileIOManager reader, int depthIn)
		{
            this.depth = depthIn;

            byte nextByte = reader.ReadByte();
			if (nextByte == StatementTypeEnum.RightStatementOperation)
			{
				this.rightStatementOperation = new RightStatementOperation(reader, depthIn + 1);
			} 
			else if (nextByte == StatementTypeEnum.ReadOnlyVariable)
			{
				this.readOnlyVariable = new ReadOnlyVariable(reader);
			} 
			else if (nextByte == StatementTypeEnum.RightMethodCall) 
			{
				this.rightMethodCall = new RightMethodCall(reader, depthIn + 1);
			} 
			else if (nextByte == StatementTypeEnum.LiteralValue)
			{
				this.literalValue = new LiteralValue(reader);
			}
			else
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextByte,
					reader,
					new List<byte>()
                    {
                        StatementTypeEnum.RightStatementOperation,
                        StatementTypeEnum.ReadOnlyVariable,
                        StatementTypeEnum.RightMethodCall,
                        StatementTypeEnum.LiteralValue
                    });
			}
		}

		public byte ReturnType
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
		public void WriteToDisk(FileIOManager writer)
		{
            writer.WriteByte(StatementTypeEnum.RightStatement);

			if (this.rightStatementOperation != null) 
			{
				this.rightStatementOperation.WriteToDisk(writer);
			}

			if (this.readOnlyVariable != null) 
			{
				this.readOnlyVariable.WriteToDisk(writer);
			}

			if (this.rightMethodCall != null) 
			{
				this.rightMethodCall.WriteToDisk(writer);
			}

			if (this.literalValue != null) 
			{
				this.literalValue.WriteToDisk(writer);
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
					this.rightStatementOperation = new RightStatementOperation(signature, this.depth + 1);
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
					this.rightMethodCall = new RightMethodCall(signature, this.depth + 1);
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
		public byte Evaluate(ref EntBehaviorManager instance)
		{
			this.EnforceOneFieldIsNonNull();

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

