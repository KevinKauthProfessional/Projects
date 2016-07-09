//------------------------------------------------------------------
// <copyright file="VariableSignature.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace Assets.Scripts.EntLogic.GeneticMemberRegistration
{
    using System;

    using AssemblyCSharp.Scripts.UnityGameObjects;
    using Assets.Scripts.Utilities;

    /// <summary>
    /// Variable signature.
    /// </summary>
    public class VariableSignature
    {
        private byte variableType;
        private byte variableId;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.Attributes.VariableSignature"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="variableTypeIn">Variable type in.</param>
        public VariableSignature(byte variableIdIn, byte variableTypeIn)
        {
            this.VariableId = variableIdIn;
            this.variableType = variableTypeIn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.Attributes.VariableSignature"/> class.
        /// </summary>
        /// <param name="reader">Reader.</param>
        public VariableSignature(FileIOManager reader)
        {
            this.variableType = reader.ReadByte();
            this.VariableId = reader.ReadByte();
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public byte VariableId
        {
            get
            {
                return this.variableId;
            }

            private set
            {
                this.variableId = value;
            }
        }

        /// <summary>
        /// Gets the type of the variable.
        /// </summary>
        /// <value>The type of the variable.</value>
        public byte VariableType
        {
            get
            {
                return this.variableType;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "[VariableSignature: VariableId={0}, VariableType={1}]",
                this.VariableId,
                this.VariableType);
        }

        /// <summary>
        /// Writes to disk.
        /// </summary>
        /// <param name="writer">Writer.</param>
        public void WriteToDisk(FileIOManager writer)
        {
            writer.WriteByte(this.VariableType);
            writer.WriteByte(this.VariableId);
        }
    }
}

