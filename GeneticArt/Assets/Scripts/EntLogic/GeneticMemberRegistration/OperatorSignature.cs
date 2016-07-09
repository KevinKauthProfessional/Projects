//------------------------------------------------------------------
// <copyright file="OperatorSignature.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace Assets.Scripts.EntLogic.GeneticMemberRegistration
{
    using Assets.Scripts.Utilities;
    using Assets.Scripts.EntLogic.SerializationObjects;

    /// <summary>
    /// Operator signature.
    /// </summary>
    public class OperatorSignature
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.Attributes.OperatorSignature"/> class.
		/// </summary>
		/// <param name="operatorType">Value.</param>
		/// <param name="lhsTypeIn">Lhs type in.</param>
		/// <param name="rhsTypeIn">Rhs type in.</param>
		/// <param name="returnTypeIn">Return type in.</param>
		public OperatorSignature(byte operatorType, byte returnTypeIn, byte lhsTypeIn, byte rhsTypeIn)
		{
			this.LhsType = lhsTypeIn;
			this.RhsType = rhsTypeIn;
			this.ReturnType = returnTypeIn;
			this.OperatorType = operatorType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.Attributes.OperatorSignature"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public OperatorSignature (FileIOManager reader)
		{
            this.OperatorType = reader.ReadByte();
			this.ReturnType = reader.ReadByte();
			this.LhsType = reader.ReadByte();
			this.RhsType = reader.ReadByte();
        }

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public byte OperatorType { get; private set; }

		/// <summary>
		/// Gets the type of the rhs.
		/// </summary>
		/// <value>The type of the rhs.</value>
		public byte RhsType { get; private set; }

		/// <summary>
		/// Gets the type of the lhs.
		/// </summary>
		/// <value>The type of the lhs.</value>
		public byte LhsType { get; private set; }

		/// <summary>
		/// Gets the type of the return.
		/// </summary>
		/// <value>The type of the return.</value>
		public byte ReturnType { get; private set; }

		public override string ToString ()
		{
			return string.Format ("[OperatorSignature: Value={0}, RhsType={1}, LhsType={2}, ReturnType={3}]", OperatorType, RhsType, LhsType, ReturnType);
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="tabDepth">Tab depth.</param>
		public void WriteToDisk(FileIOManager writer)
		{
            writer.WriteByte(this.OperatorType);
            writer.WriteByte(this.ReturnType);
            writer.WriteByte(this.LhsType);
            writer.WriteByte(this.RhsType);
		}
	}
}

