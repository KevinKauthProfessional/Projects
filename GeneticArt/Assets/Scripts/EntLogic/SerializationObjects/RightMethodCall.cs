//------------------------------------------------------------------
// <copyright file="RightMethodCall.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace Assets.Scripts.EntLogic.SerializationObjects
{
    using AssemblyCSharp.Scripts.UnityGameObjects;
    using Assets.Scripts.Utilities;
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.EntLogic.GeneticMemberRegistration;

    /// <summary>
    /// This class represents calling a method that is read only on game state
    /// and returns some information.  It is useful for giving the genetic logic
    /// access to basic calculations that it can't screw up via mutation.
    /// </summary>
    internal class RightMethodCall
	{
		public const string Name = "RightMethodCall";

		protected MethodSignature signature;
		protected List<RightStatement> parameterList = new List<RightStatement> ();
        protected int depth;

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.RightMethodCall"/> class.
		/// </summary>
		/// <param name="signatureIn">Signature in.</param>
		public RightMethodCall(MethodSignature signatureIn, int depthIn)
		{
            this.depth = depthIn;
			this.signature = signatureIn;

			foreach (byte returnType in signatureIn.ParameterTypes) 
			{
				this.parameterList.Add(new RightStatement(returnType, depthIn + 1));
			}
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.RightMethodCall"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public RightMethodCall(FileIOManager reader, int depthIn)
		{
            this.depth = depthIn;

            byte nextByte = reader.ReadByte();
            if (nextByte != StatementTypeEnum.MethodSignature)
            {
                CommonHelperMethods.ThrowStatementParseException(nextByte, reader, StatementTypeEnum.MethodSignature);
            }

            this.signature = new MethodSignature(reader);
            
			for (int i = 0; i < this.signature.ParameterTypes.Count; ++i)
			{
				nextByte = reader.ReadByte();
				if (nextByte == StatementTypeEnum.RightStatement)
				{
					this.parameterList.Add(new RightStatement(reader, depthIn + 1));
				}
			}
		}

		public byte ReturnType
		{
			get
			{
				if (this.signature != null)
				{
					return this.signature.ReturnType;
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
			return this.signature.ToString ();
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		public virtual void WriteToDisk(FileIOManager writer)
		{
            writer.WriteByte(StatementTypeEnum.RightMethodCall);
			this.WriteToDiskProtected(writer);
		}

		protected void WriteToDiskProtected(FileIOManager writer)
		{
			if (this.signature != null) 
			{
				this.signature.WriteToDisk(writer);
			}

			if (this.parameterList != null) 
			foreach(RightStatement parameter in this.parameterList)
			{
				parameter.WriteToDisk(writer);
			}
		}

		/// <summary>
		/// Executes the method.
		/// </summary>
		/// <returns>The return value of the method.</returns>
		/// <param name="instance">The instance to execute against.</param>
		public virtual byte Execute(ref EntBehaviorManager instance)
		{
			return instance.ExecuteRightMethod(this.signature, this.EvaluateParameters(ref instance));
		}

		/// <summary>
		/// Evaluates the parameters.
		/// </summary>
		/// <returns>The parameters.</returns>
		/// <param name="instance">Instance.</param>
		protected IList<byte> EvaluateParameters(ref EntBehaviorManager instance)
		{
			List<byte> result = new List<byte>();

			foreach (RightStatement rightStatement in this.parameterList) 
			{
				result.Add(rightStatement.Evaluate(ref instance));
			}

			return result;
		}
	}
}

