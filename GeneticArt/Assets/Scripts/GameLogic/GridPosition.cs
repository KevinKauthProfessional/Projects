//------------------------------------------------------------------
// <copyright file="GridPosition.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.GameLogic
{
	using System;

	public class GridPosition
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.GridPosition"/> class.
		/// </summary>
		/// <param name="X">X.</param>
		/// <param name="Z">Z.</param>
		public GridPosition (int x, int z)
		{
			// Wrap around the world
			if (x < 0)
			{
				x += GameObjectGrid.GridWidth;
			}

			if (z < 0)
			{
				z += GameObjectGrid.GridHeight;
			}
			
			if (x >= GameObjectGrid.GridWidth)
			{
				x -= GameObjectGrid.GridWidth;
			}
			
			if (z >= GameObjectGrid.GridHeight)
			{
				z -= GameObjectGrid.GridHeight;
			}

			this.X = x;
			this.Z = z;
		}

		/// <summary>
		/// Gets the x.
		/// </summary>
		/// <value>The x.</value>
		public int X { get; private set; }

		/// <summary>
		/// Gets the z.
		/// </summary>
		/// <value>The z.</value>
		public int Z { get; private set; }

		/// <summary>
		/// Gets the position in a given direction.
		/// </summary>
		/// <returns>The position in direction.</returns>
		/// <param name="direction">Direction.</param>
		public GridPosition GetPositionInDirection(byte direction)
		{
			switch (direction) 
			{
			case (byte)GridDirectionEnum.East:
				return new GridPosition(this.X - 1, this.Z);

			case (byte)GridDirectionEnum.North:
				return new GridPosition(this.X, this.Z + 1);

			case (byte)GridDirectionEnum.South:
				return new GridPosition(this.X, this.Z - 1);

			case (byte)GridDirectionEnum.West:
				return new GridPosition(this.X + 1, this.Z);
			}

			throw new InvalidOperationException (string.Format (
				"Direction {0} not recognized",
				direction));
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return string.Format ("[GridPosition: X={0}, Z={1}]", X, Z);
		}
	}
}

