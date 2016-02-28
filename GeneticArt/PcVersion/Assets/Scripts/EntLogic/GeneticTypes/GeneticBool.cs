//------------------------------------------------------------------
// <copyright file="GeneticBool.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.GeneticTypes
{
	using System;
	using AssemblyCSharp.Scripts.EntLogic.GeneticMemberRegistration;
	using AssemblyCSharp.Scripts.Utilities;
	
	/// <summary>
	/// Genetic bool.
	/// </summary>
	public class GeneticBool : GeneticObject
	{
		public static readonly GeneticBool False = new GeneticBool(false);
		public static readonly GeneticBool True = new GeneticBool(true);

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.GeneticTypes.GeneticBool"/> class.
		/// </summary>
		/// <param name="value">The value.</param>
		private GeneticBool (bool value) :
			base(value)
		{
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		new public bool Value
		{
			get
			{
				return (bool)this.value;
			}

			set
			{
				this.value = value;
			}
		}

		/// <summary>
		/// Creates a new GeneticBool instance from a string value.
		/// </summary>
		/// <returns>The new GeneticBool instance.</returns>
		/// <param name="valueString">Value string.</param>
		public static GeneticBool FromString(string valueString)
		{
			bool valueBool;
			if (!bool.TryParse (valueString, out valueBool)) 
			{
				throw new InvalidOperationException(valueString + "could not be parsed as bool");
			}

			return new GeneticBool (valueBool);
		}
		
		/// <summary>
		/// Creates a GeneticBool at random.
		/// </summary>
		/// <returns>The randomly created GeneticBool.</returns>
		public static GeneticBool CreateAtRandom()
		{
			double nextDouble = CommonHelperMethods.GetRandomDouble0To1();
			
			if (nextDouble < 0.5) 
			{
				return new GeneticBool(true);
			}
			
			return new GeneticBool(false);
		}

		/// <summary>
		/// Registers the operators.
		/// </summary>
		public static void RegisterOperators()
		{
			RegistrationManager.AddOperator ("<", typeof(GeneticBool), typeof(GeneticInt), typeof(GeneticInt));
			RegistrationManager.AddOperator (">", typeof(GeneticBool), typeof(GeneticInt), typeof(GeneticInt));
			RegistrationManager.AddOperator ("==", typeof(GeneticBool), typeof(GeneticInt), typeof(GeneticInt));
			RegistrationManager.AddOperator ("==", typeof(GeneticBool), typeof(GeneticBool), typeof(GeneticBool));
			RegistrationManager.AddOperator ("!=", typeof(GeneticBool), typeof(GeneticInt), typeof(GeneticInt));
			RegistrationManager.AddOperator ("!=", typeof(GeneticBool), typeof(GeneticBool), typeof(GeneticBool));
			RegistrationManager.AddOperator ("&&", typeof(GeneticBool), typeof(GeneticBool), typeof(GeneticBool));
			RegistrationManager.AddOperator ("||", typeof(GeneticBool), typeof(GeneticBool), typeof(GeneticBool));
		}
		
		/// <summary>
		/// Evaluates the operator.
		/// </summary>
		/// <returns>The operator.</returns>
		/// <param name="signature">Signature.</param>
		/// <param name="lhs">Lhs.</param>
		/// <param name="rhs">Rhs.</param>
		new public static GeneticBool EvaluateOperator(OperatorSignature signature, GeneticObject lhs, GeneticObject rhs)
		{
			GeneticObject.ValidateOperatorCall (signature, typeof(GeneticBool), lhs, rhs);
			
			switch (signature.Value) 
			{
			case "<":
				return GeneticBool.OperatorLessThan((GeneticInt)lhs, (GeneticInt)rhs);
				
			case ">":
				return GeneticBool.OperatorGreaterThan((GeneticInt)lhs, (GeneticInt)rhs);
				
			case "==":
				return GeneticBool.OperatorEquals(lhs, rhs);
				
			case "!=":
				return GeneticBool.OperatorNotEquals(lhs, rhs);

			case "&&":
				return GeneticBool.OperatorAnd((GeneticBool)lhs, (GeneticBool)rhs);

			case "||":
				return GeneticBool.OperatorAnd((GeneticBool)lhs, (GeneticBool)rhs);
			}

			throw new InvalidOperationException (string.Format(
				"Did not recognize operator with signature {0} with return type {1}",
				signature.Value,
				signature.ReturnType));
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			bool valueBool = (bool)this.Value;
			return valueBool.ToString();
		}

		private static GeneticBool OperatorLessThan(GeneticInt lhs, GeneticInt rhs)
		{
			return new GeneticBool (lhs.Value < rhs.Value);
		}
	
		private static GeneticBool OperatorGreaterThan(GeneticInt lhs, GeneticInt rhs)
		{
			return new GeneticBool (lhs.Value > rhs.Value);
		}

		private static GeneticBool OperatorEquals(GeneticObject lhs, GeneticObject rhs)
		{
			if (lhs is GeneticInt && rhs is GeneticInt) 
			{
				return GeneticBool.OperatorEquals((GeneticInt)lhs, (GeneticInt)rhs);
			}

			if (lhs is GeneticBool && rhs is GeneticBool) 
			{
				return GeneticBool.OperatorEquals((GeneticBool)lhs, (GeneticBool)rhs);
			}

			throw new InvalidOperationException ("Could not cast arguemnts as expected");
		}

		private static GeneticBool OperatorEquals(GeneticInt lhs, GeneticInt rhs)
		{
			return new GeneticBool (lhs.Value == rhs.Value);
		}

		private static GeneticBool OperatorEquals(GeneticBool lhs, GeneticBool rhs)
		{
			return new GeneticBool (lhs.Value == rhs.Value);
		}

		private static GeneticBool OperatorNotEquals(GeneticObject lhs, GeneticObject rhs)
		{
			if (lhs is GeneticInt && rhs is GeneticInt) 
			{
				return GeneticBool.OperatorNotEquals((GeneticInt)lhs, (GeneticInt)rhs);
			}
			
			if (lhs is GeneticBool && rhs is GeneticBool) 
			{
				return GeneticBool.OperatorNotEquals((GeneticBool)lhs, (GeneticBool)rhs);
			}
			
			throw new InvalidOperationException ("Could not cast arguemnts as expected");
		}

		private static GeneticBool OperatorNotEquals(GeneticInt lhs, GeneticInt rhs)
		{
			return new GeneticBool (lhs.Value != rhs.Value);
		}

		private static GeneticBool OperatorNotEquals(GeneticBool lhs, GeneticBool rhs)
		{
			return new GeneticBool (lhs.Value != rhs.Value);
		}

		private static GeneticBool OperatorAnd(GeneticBool lhs, GeneticBool rhs)
		{
			return new GeneticBool (lhs.Value && rhs.Value);
		}

		private static GeneticBool OperatorOr(GeneticBool lhs, GeneticBool rhs)
		{
			return new GeneticBool (lhs.Value || rhs.Value);
		}
	}
}

