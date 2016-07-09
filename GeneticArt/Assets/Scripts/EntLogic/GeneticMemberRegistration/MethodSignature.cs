//------------------------------------------------------------------
// <copyright file="MethodSignature.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace Assets.Scripts.EntLogic.GeneticMemberRegistration
{
	using System;
	using System.Collections.Generic;

	using AssemblyCSharp.Scripts.UnityGameObjects;
	using Assets.Scripts.Utilities;
    using Assets.Scripts.EntLogic.SerializationObjects;

    /// <summary>
    /// A container for metadata related to a method.
    /// </summary>
    public class MethodSignature
	{
		private byte returnType;
		private List<byte> parameterTypes = new List<byte>();
		private byte methodId;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.Attributes.MethodSignature"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="returnTypeIn">Return type in.</param>
		/// <param name="parameterTypesIn">Parameter types in.</param>
		public MethodSignature(
            byte methodIdIn,
            byte returnTypeIn,
            params byte[] parameterTypesIn)
		{
			this.MethodId = methodIdIn;
			this.returnType = returnTypeIn;
			this.parameterTypes = new List<byte> (parameterTypesIn);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodSignatureBinary"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public MethodSignature(FileIOManager reader)
		{
            this.MethodId = reader.ReadByte();
            this.returnType = reader.ReadByte();

            int numberOfParameters = (int)reader.ReadByte();
			
			for (int i = 0; i < numberOfParameters; ++i)
			{
				this.parameterTypes.Add(reader.ReadByte());
			}
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public byte MethodId 
		{ 
			get
			{
				return this.methodId;
			}

			private set
			{
				this.methodId = value;
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
				return this.returnType;
			}
		}

		/// <summary>
		/// Gets the parameter types.
		/// </summary>
		/// <value>The parameter types.</value>
		public IList<byte> ParameterTypes
		{
			get
			{
				return new List<byte>(this.parameterTypes);
			}
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return string.Format (
				"[MethodSignature: MethodId={0}, ReturnType={1}, ParameterTypes={2}]",
				this.MethodId,
				this.ReturnType,
				this.ParameterTypes);
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		public void WriteToDisk(FileIOManager writer)
		{
            writer.WriteByte(StatementTypeEnum.MethodSignature);
            writer.WriteByte(this.methodId);
            writer.WriteByte(this.returnType);
            writer.WriteByte((byte)this.parameterTypes.Count);
			foreach (byte parameterType in this.parameterTypes)
			{
                writer.WriteByte(parameterType);
			}
		}
	}
}

