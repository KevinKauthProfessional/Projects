//------------------------------------------------------------------
// <copyright file="LiteralValue.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace Assets.Scripts.EntLogic.SerializationObjects
{
    using Assets.Scripts.Utilities;
    using Assets.Scripts.EntLogic.GeneticMemberRegistration;
    using System;
    using AssemblyCSharp.Scripts.GameLogic;

    /// <summary>
    /// This class represents a literal value.
    /// </summary>
    public class LiteralValue
	{
		public const string Name = "LiteralValue";

		private byte type;
		private byte value;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.LiteralValue"/> class.
		/// </summary>
		/// <param name="returnType">Return type.</param>
		public LiteralValue(byte returnType)
		{
			this.type = returnType;
            byte randomByte = CommonHelperMethods.GetRandomByte(); ;

            switch (returnType)
            {
                case GeneticTypeEnum.GeneticBool:
                    this.value = (byte)(randomByte & 1);
                    break;

                case GeneticTypeEnum.GeneticGridDirection:
                    this.value = (byte)CommonHelperMethods.GetRandomPositiveInt0ToValue(
                        GridDirectionEnum.Count - 1);
                    break;

                case GeneticTypeEnum.GeneticInt:
                    this.value = randomByte;
                    break;

                default:
                    throw new NotImplementedException(
                        string.Format("No support for object type: {0}", returnType));
            }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.LiteralValue"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public LiteralValue (FileIOManager reader)
		{
            this.type = reader.ReadByte();
            this.value = reader.ReadByte();
		}

		/// <summary>
		/// Gets the type of the variable.
		/// </summary>
		/// <value>The type of the variable.</value>
		public byte VariableType
		{
			get
			{
				return this.type;
			}
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public byte Value
		{
			get
			{
				return this.value;
			}
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return this.Value.ToString();
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		public void WriteToDisk(FileIOManager writer)
		{
            writer.WriteByte(StatementTypeEnum.LiteralValue);
            writer.WriteByte(this.type);
            writer.WriteByte(this.value);
		}
	}
}

