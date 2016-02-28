//------------------------------------------------------------------
// <copyright file="RightMethodCall.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.SerializationObjects
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	
	using AssemblyCSharp.Scripts.EntLogic.GeneticMemberRegistration;
	using AssemblyCSharp.Scripts.EntLogic.GeneticTypes;
	using AssemblyCSharp.Scripts.UnityGameObjects;
	using AssemblyCSharp.Scripts.Utilities;

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

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.RightMethodCall"/> class.
		/// </summary>
		/// <param name="signatureIn">Signature in.</param>
		public RightMethodCall(MethodSignature signatureIn)
		{
			this.signature = signatureIn;

			foreach (Type returnType in signatureIn.ParameterTypes) 
			{
				this.parameterList.Add(new RightStatement(returnType));
			}
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.RightMethodCall"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public RightMethodCall (CustomFileReader reader)
		{
			this.signature = new MethodSignature (reader);

			for (int i = 0; i < this.signature.ParameterTypes.Count; ++i)
			{
				string nextLine = reader.ReadNextContentLineAndTrim ();
				if (CommonHelperMethods.StringStartsWith(nextLine, RightStatement.Name))
				{
					this.parameterList.Add(new RightStatement(reader));
				}
			}
		}

		public Type ReturnType
		{
			get
			{
				if (this.signature != null)
				{
					return this.signature.ReturnType;
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
			return this.signature.ToString ();
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="tabDepth">Tab depth.</param>
		public virtual void WriteToDisk(StreamWriter writer, int tabDepth)
		{
			writer.WriteLine (CommonHelperMethods.PrePendTabs (RightMethodCall.Name, tabDepth));
			this.WriteToDiskProtected (writer, tabDepth + 1);
		}

		protected void WriteToDiskProtected(StreamWriter writer, int tabDepth)
		{
			if (this.signature != null) 
			{
				this.signature.WriteToDisk(writer, tabDepth);
			}

			if (this.parameterList != null) 
				foreach(RightStatement parameter in this.parameterList)
			{
				parameter.WriteToDisk(writer, tabDepth);
			}
		}

		/// <summary>
		/// Executes the method.
		/// </summary>
		/// <returns>The return value of the method.</returns>
		/// <param name="instance">The instance to execute against.</param>
		public virtual GeneticObject Execute(ref EntBehaviorManager instance)
		{
			return instance.ExecuteRightMethod (this.signature, this.EvaluateParameters (ref instance));
		}

		/// <summary>
		/// Evaluates the parameters.
		/// </summary>
		/// <returns>The parameters.</returns>
		/// <param name="instance">Instance.</param>
		protected IList<GeneticObject> EvaluateParameters(ref EntBehaviorManager instance)
		{
			List<GeneticObject> result = new List<GeneticObject> ();

			foreach (RightStatement rightStatement in this.parameterList) 
			{
				result.Add(rightStatement.Evaluate(ref instance));
			}

			return result;
		}
	}
}

