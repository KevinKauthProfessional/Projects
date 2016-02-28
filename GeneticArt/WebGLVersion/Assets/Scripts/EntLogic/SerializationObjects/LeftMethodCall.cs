//------------------------------------------------------------------
// <copyright file="LeftMethodCall.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.SerializationObjects
{
	using System;
	using System.IO;
	
	using AssemblyCSharp.Scripts.EntLogic.GeneticMemberRegistration;
	using AssemblyCSharp.Scripts.EntLogic.GeneticTypes;
	using AssemblyCSharp.Scripts.UnityGameObjects;
	using AssemblyCSharp.Scripts.Utilities;

	/// <summary>
	/// This class represents a method call that conducts significant write actions on the game state.
	/// For example, "Move" or "Attack neighbor".  It has a return type of void and will end the execution
	/// of the genetic logic for that frame (adds implicit return statement).
	/// </summary>
	internal class LeftMethodCall : RightMethodCall
	{
		new public const string Name = "LeftMethodCall";

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.LeftMethodCall"/> class.
		/// </summary>
		/// <param name="signature">Signature.</param>
		public LeftMethodCall(MethodSignature signature) : 
			base(signature)
		{
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.LeftMethodCall"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public LeftMethodCall (CustomFileReader reader) :
			base(reader)
		{
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
		public override void WriteToDisk(StreamWriter writer, int tabDepth)
		{
			writer.WriteLine (CommonHelperMethods.PrePendTabs (LeftMethodCall.Name, tabDepth));
			this.WriteToDiskProtected (writer, tabDepth + 1);
		}

		/// <summary>
		/// Executes the LeftMethod.
		/// </summary>
		/// <param name="instance">The instance to execute against.</param>
		new public bool Execute(ref EntBehaviorManager instance)
		{
			return instance.ExecuteLeftMethod(this.signature, this.EvaluateParameters(ref instance));
		}
	}
}

