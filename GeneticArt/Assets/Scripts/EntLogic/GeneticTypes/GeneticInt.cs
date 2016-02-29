//------------------------------------------------------------------
// <copyright file="GeneticInt.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.GeneticTypes
{
	using System;
	using AssemblyCSharp.Scripts.EntLogic.GeneticMemberRegistration;
    using Assets.Scripts.Utilities;

	/// <summary>
	/// Genetic int.
	/// </summary>
	public class GeneticInt : GeneticObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.GeneticTypes.GeneticInt"/> class.
		/// </summary>
		/// <param name="value">Value.</param>
		public GeneticInt (int value) :
			base(value)
		{
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		new public int Value
		{
			get
			{
				return (int)this.value;
			}

			set
			{
				this.value = value;
			}
		}

		/// <summary>
		/// Creates a new GeneticInt instance from a string value.
		/// </summary>
		/// <returns>The new GeneticInt instance.</returns>
		/// <param name="valueString">Value string.</param>
		public static GeneticInt FromString(string valueString)
		{
			int valueInt;
			if (!int.TryParse (valueString, out valueInt)) 
			{
				throw new InvalidOperationException(valueString + " could not be parsed as an int");
			}

			return new GeneticInt (valueInt);
		}

		/// <summary>
		/// Creates a GeneticInt at random.
		/// </summary>
		/// <returns>The randomly created GeneticInt.</returns>
		public static GeneticInt CreateAtRandom()
		{
			double nextDouble = CommonHelperMethods.GetRandomDouble0To1();
			int literal = (int)(StaticController.GlobalMaxInt * nextDouble);

			if (nextDouble < 0.5) 
			{
				return new GeneticInt(literal);
			}

			return new GeneticInt (-literal);
		}

		/// <summary>
		/// Registers the operators.
		/// </summary>
		public static void RegisterOperators()
		{
			RegistrationManager.AddOperator ("+", typeof(GeneticInt), typeof(GeneticInt), typeof(GeneticInt));
			RegistrationManager.AddOperator("-", typeof(GeneticInt), typeof(GeneticInt), typeof(GeneticInt));
			RegistrationManager.AddOperator("*", typeof(GeneticInt), typeof(GeneticInt), typeof(GeneticInt));
			RegistrationManager.AddOperator("/", typeof(GeneticInt), typeof(GeneticInt), typeof(GeneticInt));
		}

		/// <summary>
		/// Evaluates the operator.
		/// </summary>
		/// <returns>The operator.</returns>
		/// <param name="signature">Signature.</param>
		/// <param name="lhs">Lhs.</param>
		/// <param name="rhs">Rhs.</param>
		new public static GeneticInt EvaluateOperator(OperatorSignature signature, GeneticObject lhs, GeneticObject rhs)
		{
			GeneticObject.ValidateOperatorCall (signature, typeof(GeneticInt), lhs, rhs);

			switch (signature.Value) 
			{
			case "+":
				return GeneticInt.OperatorPlus((GeneticInt)lhs, (GeneticInt)rhs);

			case "-":
				return GeneticInt.OperatorMinus((GeneticInt)lhs, (GeneticInt)rhs);
			
			case "*":
				return GeneticInt.OperatorMultiply((GeneticInt)lhs, (GeneticInt)rhs);

			case "/":
				return GeneticInt.OperatorDivide((GeneticInt)lhs, (GeneticInt)rhs);
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
			int valueInt = (int)this.Value;
			return valueInt.ToString();
		}

		private static GeneticInt OperatorPlus(GeneticInt lhs, GeneticInt rhs)
		{
			return new GeneticInt (lhs.Value + rhs.Value);
		}

        private static GeneticInt OperatorMinus(GeneticInt lhs, GeneticInt rhs)
        {
			return new GeneticInt (lhs.Value - rhs.Value);
		}

		private static GeneticInt OperatorMultiply(GeneticInt lhs, GeneticInt rhs)
		{
			return new GeneticInt (lhs.Value * rhs.Value);
		}

		private static GeneticInt OperatorDivide(GeneticInt lhs, GeneticInt rhs)
		{
			if (rhs.Value == 0)
			{
				return new GeneticInt(0);
			}

			return new GeneticInt (lhs.Value / rhs.Value);
		}
	}
}

