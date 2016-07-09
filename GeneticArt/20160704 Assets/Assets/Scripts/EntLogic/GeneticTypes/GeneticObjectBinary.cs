//------------------------------------------------------------------
// <copyright file="GeneticObject.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.GeneticTypes
{
	using System;
	using System.Collections.Generic;

	using AssemblyCSharp.Scripts.EntLogic.GeneticMemberRegistration;
	using Assets.Scripts.Utilities;

	/// <summary>
	/// This is the base class of a family of wrapper objects 
	/// that make wrapped objects friendly to the Genetic Logic system.
	/// </summary>
	public class GeneticObjectBinary
	{
		private byte valueByte;
        private byte typeByte;

		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticObjectBinary"/> class.
		/// </summary>
		/// <param name="valueIn">Value in.</param>
		public GeneticObjectBinary (byte typeByteIn, byte valueIn)
		{
			this.valueByte = valueIn;
            this.typeByte = typeByteIn;
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public byte Value 
		{ 
			get
			{
				return this.valueByte;
			}

			set
			{
				this.valueByte = value;
			}
		}

		public static void RegisterOperatorsForAllTypes()
		{
			GeneticBool.RegisterOperators ();
			GeneticInt.RegisterOperators ();
		}

		/// <summary>
		/// Evaluates the operator.
		/// </summary>
		/// <returns>The operator.</returns>
		/// <param name="signature">Signature.</param>
		/// <param name="lhs">Lhs.</param>
		/// <param name="rhs">Rhs.</param>
		public static GeneticObject EvaluateOperator(OperatorSignature signature, GeneticObject lhs, GeneticObject rhs)
		{
			if (signature.ReturnType == typeof(GeneticInt)) 
			{
				return GeneticInt.EvaluateOperator (signature, lhs, rhs);
			}
					
			if (signature.ReturnType == typeof(GeneticBool)) 
			{
				return GeneticBool.EvaluateOperator(signature, lhs, rhs);
			}

			throw new InvalidOperationException(string.Format(
				"{0} is missing implementation support for an operator {1}",
				signature.ReturnType,
				signature.Value));
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return string.Format ("[GeneticObject: Value={0}]", Value);
		}

		protected static void ValidateOperatorCall(
			OperatorSignature signature,
			Type expectedReturnType,
			GeneticObject lhs,
			GeneticObject rhs)
		{
			if (signature.ReturnType != expectedReturnType) 
			{
				throw new InvalidOperationException(string.Format(
					"Expecting type {0} got type {1}",
					expectedReturnType,
					signature.ReturnType));
			}
		}
	}
}

