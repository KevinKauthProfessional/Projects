//------------------------------------------------------------------
// <copyright file="OperatorSignature.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.GeneticMemberRegistration
{
	using System;

	using AssemblyCSharp.Scripts.EntLogic.GeneticTypes;
	using Assets.Scripts.Utilities;

	/// <summary>
	/// Operator signature.
	/// </summary>
	public class OperatorSignature
	{
		private Type lhsType;
		private Type rhsType;
		private Type returnType;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.Attributes.OperatorSignature"/> class.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="lhsTypeIn">Lhs type in.</param>
		/// <param name="rhsTypeIn">Rhs type in.</param>
		/// <param name="returnTypeIn">Return type in.</param>
		public OperatorSignature (string value, Type returnTypeIn , Type lhsTypeIn, Type rhsTypeIn)
		{
			this.lhsType = lhsTypeIn;
			this.rhsType = rhsTypeIn;
			this.returnType = returnTypeIn;
			this.Value = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.Attributes.OperatorSignature"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public OperatorSignature (FileIOManager reader)
		{
			string nextLine = reader.ReadNextContentLineAndTrim ();
			this.Value = nextLine;

			nextLine = reader.ReadNextContentLineAndTrim ();
			this.returnType = GeneticObject.ParseType (nextLine);

			nextLine = reader.ReadNextContentLineAndTrim ();
			this.lhsType = GeneticObject.ParseType (nextLine);

			nextLine = reader.ReadNextContentLineAndTrim ();
			this.rhsType = GeneticObject.ParseType (nextLine);
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public string Value { get; private set; }

		/// <summary>
		/// Gets the type of the rhs.
		/// </summary>
		/// <value>The type of the rhs.</value>
		public Type RhsType
		{
			get
			{
				return this.rhsType;
			}
		}

		/// <summary>
		/// Gets the type of the lhs.
		/// </summary>
		/// <value>The type of the lhs.</value>
		public Type LhsType
		{
			get
			{
				return this.lhsType;
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
				return this.returnType;
			}
		}

		public override string ToString ()
		{
			return string.Format ("[OperatorSignature: Value={0}, RhsType={1}, LhsType={2}, ReturnType={3}]", Value, RhsType, LhsType, ReturnType);
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="tabDepth">Tab depth.</param>
		public void WriteToDisk(FileIOManager writer, int tabDepth)
		{
			writer.WriteLine(CommonHelperMethods.PrePendTabs(this.Value, tabDepth));
			writer.WriteLine(CommonHelperMethods.PrePendTabs(this.returnType.ToString(), tabDepth));
			writer.WriteLine(CommonHelperMethods.PrePendTabs(this.lhsType.ToString(), tabDepth));
			writer.WriteLine(CommonHelperMethods.PrePendTabs(this.rhsType.ToString(), tabDepth));
		}
	}
}

