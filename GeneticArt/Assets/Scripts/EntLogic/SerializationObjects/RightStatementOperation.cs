//------------------------------------------------------------------
// <copyright file="RightStatementOperation.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace Assets.Scripts.EntLogic.SerializationObjects
{
    using Assets.Scripts.EntLogic.GeneticMemberRegistration;
    using AssemblyCSharp.Scripts.UnityGameObjects;
    using Assets.Scripts.Utilities;
    using System;

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

        private int depth;

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.RightStatementOperation"/> class.
		/// </summary>
		/// <param name="returnType">Return type.</param>
		public RightStatementOperation(OperatorSignature signature, int depthIn)
		{
            this.depth = depthIn;

			this.operatorSignature = signature;
			this.leftHandSide = new RightStatement(signature.LhsType, depthIn + 1);
			this.rightHandSide = new RightStatement(signature.RhsType, depthIn + 1);
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.RightStatementOperation`1"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public RightStatementOperation (FileIOManager reader, int depthIn)
		{
            this.depth = depthIn;

			// Parse operator
			this.operatorSignature = new OperatorSignature (reader);

            // Parse left hand side
            byte nextByte = reader.ReadByte();
			if (nextByte == StatementTypeEnum.RightStatement) 
			{
				this.leftHandSide = new RightStatement(reader, depthIn + 1);
			}
			else
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextByte, 
					reader, 
					StatementTypeEnum.RightStatement);
			}

            // Parse right hand side
            nextByte = reader.ReadByte();
			if (nextByte == StatementTypeEnum.RightStatement) 
			{
				this.rightHandSide = new RightStatement(reader, depthIn + 1);
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
		/// Gets the type of the return.
		/// </summary>
		/// <value>The type of the return.</value>
		public byte ReturnType
		{
			get
			{
				if (this.operatorSignature != null)
				{
					return this.operatorSignature.ReturnType;
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
			return this.operatorSignature.ToString ();
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		public void WriteToDisk(FileIOManager writer)
		{
            writer.WriteByte(StatementTypeEnum.RightStatementOperation);

			if (this.operatorSignature != null) 
			{
				this.operatorSignature.WriteToDisk(writer);
			}

			if (this.leftHandSide != null) 
			{
				this.leftHandSide.WriteToDisk(writer);
			}

			if (this.rightHandSide != null) 
			{
				this.rightHandSide.WriteToDisk(writer);
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
					this.leftHandSide = new RightStatement(signature.LhsType, this.depth + 1);
					this.rightHandSide = new RightStatement(signature.RhsType, this.depth + 1);
					return;
				}
			}

			if (GeneticLogicRoot.RollMutateDice ()) 
			{
				this.rightHandSide = new RightStatement(this.operatorSignature.RhsType, this.depth + 1);
				return;
			}

			if (GeneticLogicRoot.RollMutateDice ()) 
			{
				this.leftHandSide = new RightStatement(this.operatorSignature.LhsType, this.depth + 1);
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
		public byte Evaluate(ref EntBehaviorManager instance)
		{
			byte leftHandObject = this.leftHandSide.Evaluate(ref instance);
			byte rightHandObject = this.rightHandSide.Evaluate(ref instance);

            switch (this.operatorSignature.OperatorType)
            {
                case OperatorTypeEnum.And:
                    return this.EvaluateAnd(leftHandObject, rightHandObject);

                case OperatorTypeEnum.Equal:
                    return this.EvaluateEqual(leftHandObject, rightHandObject);

                case OperatorTypeEnum.Minus:
                    return this.EvaluateMinus(leftHandObject, rightHandObject);

                case OperatorTypeEnum.NotEqual:
                    return this.EvaluateNotEqual(leftHandObject, rightHandObject);

                case OperatorTypeEnum.Or:
                    return this.EvaluateOr(leftHandObject, rightHandObject);

                case OperatorTypeEnum.Plus:
                    return this.EvaluatePlus(leftHandObject, rightHandObject);

                default:
                    throw new NotImplementedException(string.Format(
                        "Unrecognized operator type: {0}", 
                        this.operatorSignature.OperatorType));
            }
		}

        private byte EvaluatePlus(byte lhs, byte rhs)
        {
            int result = lhs + rhs;

            if (result > byte.MaxValue)
            {
                return byte.MaxValue;
            }

            return (byte)(result);
        }

        private byte EvaluateMinus(byte lhs, byte rhs)
        {
            if (rhs > lhs)
            {
                return byte.MinValue;
            }

            return (byte)(lhs - rhs);
        }

        private byte EvaluateAnd(byte lhs, byte rhs)
        {
            return (byte)(lhs & rhs);
        }

        private byte EvaluateOr(byte lhs, byte rhs)
        {
            return (byte)(lhs | rhs);
        }

        private byte EvaluateEqual(byte lhs, byte rhs)
        {
            if (lhs == rhs)
            {
                return 1;
            }

            return 0;
        }

        private byte EvaluateNotEqual(byte lhs, byte rhs)
        {
            if (lhs == rhs)
            {
                return 0;
            }

            return 1;
        }
	}
}

