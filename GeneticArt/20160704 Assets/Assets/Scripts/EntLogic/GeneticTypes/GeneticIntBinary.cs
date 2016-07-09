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
	public class GeneticIntBinary : GeneticObjectBinary
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticIntBinary"/> class.
		/// </summary>
		/// <param name="value">Value.</param>
		public GeneticIntBinary (byte value) :
			base(value)
		{
		}
	}
}

