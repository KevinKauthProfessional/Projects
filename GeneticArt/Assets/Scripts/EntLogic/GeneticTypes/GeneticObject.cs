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
	public class GeneticObject
	{
		protected object value;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.GeneticTypes.GeneticObject"/> class.
		/// </summary>
		/// <param name="valueIn">Value in.</param>
		public GeneticObject (object valueIn)
		{
			this.value = valueIn;
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public object Value 
		{ 
			get
			{
				return this.value;
			}

			set
			{
				this.value = value;
			}
		}

		public static void RegisterOperatorsForAllTypes()
		{
			GeneticBool.RegisterOperators ();
			GeneticInt.RegisterOperators ();
		}

		/// <summary>
		/// Parses a string as a GeneticObject Type.
		/// </summary>
		/// <returns>The type.</returns>
		/// <param name="typeString">Type string.</param>
		public static Type ParseType(string typeString)
		{
			if (CommonHelperMethods.StringStartsWith (typeString, typeof(GeneticInt).ToString ())) 
			{
				return typeof(GeneticInt);
			} 
			else if (CommonHelperMethods.StringStartsWith (typeString, typeof(GeneticBool).ToString ())) 
			{
				return typeof(GeneticBool);
			} 
			else if (CommonHelperMethods.StringStartsWith (typeString, typeof(GeneticGridDirection).ToString ()))
			{
				return typeof(GeneticGridDirection);
			}
			else if (CommonHelperMethods.StringStartsWith(typeString, typeof(void).ToString()))
			{
				return typeof(void);
			}

			throw new StatementParseException(string.Format("Type: {0} is not a supported Genetic type", typeString));
		}

		/// <summary>
		/// Creates the type at random.
		/// </summary>
		/// <returns>The type at random.</returns>
		/// <param name="type">Type.</param>
		public static GeneticObject CreateTypeAtRandom(Type type)
		{
			if (type == typeof(GeneticInt)) 
			{
				return GeneticInt.CreateAtRandom();
			}
			
			if (type == typeof(GeneticBool)) 
			{
				return GeneticBool.CreateAtRandom();
			}

			if (type == typeof(GeneticGridDirection)) 
			{
				return GeneticGridDirection.CreateAtRandom();
			}
			
			throw new StatementParseException(string.Format("Type: {0} is missing implementation support as a Genetic type", type));
		}

		/// <summary>
		/// Parses the value.
		/// </summary>
		/// <returns>The value.</returns>
		/// <param name="type">Type.</param>
		/// <param name="valueString">Value string.</param>
		public static GeneticObject ParseValue(Type type, string valueString)
		{
			if (type == typeof(GeneticInt)) 
			{
				return GeneticInt.FromString (valueString);
			}
			
			if (type == typeof(GeneticBool)) 
			{
				return GeneticBool.FromString(valueString);
			}

			if (type == typeof(GeneticGridDirection)) 
			{
				return GeneticGridDirection.FromString(valueString);
			}

			throw new StatementParseException(string.Format("Type: {0} is missing implementation support as a Genetic type", type));
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

