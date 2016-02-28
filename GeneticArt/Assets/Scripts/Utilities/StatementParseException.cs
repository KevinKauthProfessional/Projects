//------------------------------------------------------------------
// <copyright file="StatementParseException.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.Utilities
{
	using System;

	/// <summary>
	/// An exception representing the inability to parse a 
	/// string into a corresponding serialized object.
	/// </summary>
	public class StatementParseException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.StatementParseException"/> class.
		/// </summary>
		public StatementParseException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.StatementParseException"/> class.
		/// </summary>
		/// <param name="message">Message.</param>
		public StatementParseException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.StatementParseException"/> class.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="inner">Inner.</param>
		public StatementParseException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}

