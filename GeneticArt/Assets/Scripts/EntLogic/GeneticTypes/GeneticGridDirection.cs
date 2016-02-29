//------------------------------------------------------------------
// <copyright file="GeneticGridDirection.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.GeneticTypes
{
	using System;
	using AssemblyCSharp.Scripts.GameLogic;
    using Assets.Scripts.Utilities;

	/// <summary>
	/// Genetic grid direction.
	/// </summary>
	public class GeneticGridDirection : GeneticObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.GeneticTypes.GeneticGridDirection"/> class.
		/// </summary>
		/// <param name="value">Value.</param>
		public GeneticGridDirection (GridDirection value) :
			base(value)
		{
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		new public GridDirection Value
		{
			get
			{
				return (GridDirection)this.value;
			}

			set
			{
				this.value = value;
			}
		}

		/// <summary>
		/// Converts a string into a new GeneticGridDirection instance.
		/// </summary>
		/// <returns>The new GeneticGridDirection instance.</returns>
		/// <param name="valueString">Value string.</param>
		public static GeneticGridDirection FromString(string valueString)
		{
			GridDirection direction = (GridDirection)Enum.Parse (typeof(GridDirection), valueString);
			return new GeneticGridDirection(direction);
		}

		/// <summary>
		/// Creates at random.
		/// </summary>
		/// <returns>The randomly created instance.</returns>
		public static GeneticGridDirection CreateAtRandom()
		{
			int randInt = CommonHelperMethods.GetRandomPositiveInt0ToValue (3);
			GridDirection direction = (GridDirection)randInt;
			return new GeneticGridDirection(direction);
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return string.Format ("{0}", this.Value);
		}
	}
}

