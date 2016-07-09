//------------------------------------------------------------------
// <copyright file="LeftMethodCall.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace Assets.Scripts.EntLogic.SerializationObjects
{
    using AssemblyCSharp.Scripts.UnityGameObjects;
    using Assets.Scripts.EntLogic.GeneticMemberRegistration;
    using Assets.Scripts.Utilities;

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
		public LeftMethodCall(MethodSignature signature, int depthIn) : 
			base(signature, depthIn)
		{
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.LeftMethodCall"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public LeftMethodCall (FileIOManager reader, int depthIn) :
			base(reader, depthIn)
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
		public override void WriteToDisk(FileIOManager writer)
		{
            writer.WriteByte(StatementTypeEnum.LeftMethodCall);
			this.WriteToDiskProtected(writer);
		}

		/// <summary>
		/// Executes the LeftMethod.
		/// </summary>
		/// <param name="instance">The instance to execute against.</param>
		new public byte Execute(ref EntBehaviorManager instance)
		{
			return instance.ExecuteLeftMethod(this.signature, this.EvaluateParameters(ref instance));
		}
	}
}

