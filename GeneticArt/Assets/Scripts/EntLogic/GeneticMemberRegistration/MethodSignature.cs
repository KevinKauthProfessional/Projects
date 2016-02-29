//------------------------------------------------------------------
// <copyright file="MethodSignature.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.GeneticMemberRegistration
{
	using System;
	using System.Collections.Generic;

	using AssemblyCSharp.Scripts.EntLogic.GeneticTypes;
	using AssemblyCSharp.Scripts.UnityGameObjects;
	using Assets.Scripts.Utilities;

	/// <summary>
	/// A container for metadata related to a method.
	/// </summary>
	public class MethodSignature
	{
		private Type returnType;
		private List<Type> parameterTypes = new List<Type>();
		private EntMethodEnum methodId;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.Attributes.MethodSignature"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="returnTypeIn">Return type in.</param>
		/// <param name="parameterTypesIn">Parameter types in.</param>
		public MethodSignature (EntMethodEnum methodIdIn, Type returnTypeIn, params Type[] parameterTypesIn)
		{
			this.MethodId = methodIdIn;
			this.returnType = returnTypeIn;
			this.parameterTypes = new List<Type> (parameterTypesIn);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.Attributes.MethodSignature"/> class.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public MethodSignature(FileIOManager reader)
		{
			string nextLine = reader.ReadNextContentLineAndTrim ();
			this.MethodId = (EntMethodEnum)Enum.Parse(typeof(EntMethodEnum), nextLine);

			nextLine = reader.ReadNextContentLineAndTrim ();
			this.returnType = GeneticObject.ParseType(nextLine);
			
			int numberOfParameters;
			nextLine = reader.ReadNextContentLineAndTrim ();
			if (!int.TryParse(nextLine, out numberOfParameters))
			{
				CommonHelperMethods.ThrowStatementParseException(
					nextLine,
					reader,
					"An integer representing the number of parameters");
			}
			
			for (int i = 0; i < numberOfParameters; ++i)
			{
				nextLine = reader.ReadNextContentLineAndTrim ();
				Type parsedType = GeneticObject.ParseType(nextLine);
				this.parameterTypes.Add(parsedType);
			}
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public EntMethodEnum MethodId 
		{ 
			get
			{
				return this.methodId;
			}

			private set
			{
				this.methodId = value;
			}
		}

		/// <summary>
		/// Gets the type of the return.
		/// </summary>
		/// <value>The type of the return.</value>
		public Type ReturnType 
		{
			get
			{
				return this.returnType;
			}
		}

		/// <summary>
		/// Gets the parameter types.
		/// </summary>
		/// <value>The parameter types.</value>
		public IList<Type> ParameterTypes
		{
			get
			{
				return new List<Type>(this.parameterTypes);
			}
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return string.Format (
				"[MethodSignature: MethodId={0}, ReturnType={1}, ParameterTypes={2}]",
				this.MethodId,
				this.ReturnType,
				this.ParameterTypes);
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="tabDepth">Tab depth.</param>
		public void WriteToDisk(FileIOManager writer, int tabDepth)
		{
			writer.WriteLine(CommonHelperMethods.PrePendTabs(this.MethodId.ToString(), tabDepth));

			if (this.ReturnType != null)
			{
				writer.WriteLine(CommonHelperMethods.PrePendTabs(this.ReturnType.ToString(), tabDepth));
			}

			
			if (this.parameterTypes != null) 
			{
				writer.WriteLine(CommonHelperMethods.PrePendTabs(this.parameterTypes.Count.ToString(), tabDepth));
				foreach (Type parameterType in this.parameterTypes)
				{
					writer.WriteLine(CommonHelperMethods.PrePendTabs(parameterType.ToString(), tabDepth + 1));
				}
			}
		}
	}
}

