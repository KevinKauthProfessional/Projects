//------------------------------------------------------------------
// <copyright file="RegistrationManager.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.GeneticMemberRegistration
{
	using System;
	using System.Collections.Generic;

	using AssemblyCSharp.Scripts.UnityGameObjects;
    using Assets.Scripts.Utilities;
	
	/// <summary>
	/// This class maintains the collections of method and variable signatures exposed to the Genetic Logic system.
	/// </summary>
	public static class RegistrationManager
	{
		private static List<MethodSignature> leftMethodList = new List<MethodSignature>();
		private static List<MethodSignature> rightMethodList = new List<MethodSignature>();

		private static List<VariableSignature> readOnlyVariableList = new List<VariableSignature>();
		private static List<VariableSignature> readWriteVariableList = new List<VariableSignature>();

		private static List<OperatorSignature> operatorList = new List<OperatorSignature>();

		/// <summary>
		/// Selects a read write variable at random.
		/// </summary>
		/// <returns>A random read write variable.</returns>
		public static VariableSignature SelectReadWriteVariableAtRandom()
		{
			if (readWriteVariableList == null || readWriteVariableList.Count == 0) 
			{
				throw new InvalidOperationException("readWriteVariableList is null or empty");
			}

			int index = CommonHelperMethods.GetRandomPositiveInt0ToValue (readWriteVariableList.Count - 1);
			return readWriteVariableList [index];                                            
		}

		/// <summary>
		/// Selects the left method at random.
		/// </summary>
		/// <returns>The left method at random.</returns>
		public static MethodSignature SelectLeftMethodAtRandom()
		{
			if (leftMethodList == null || leftMethodList.Count == 0) 
			{
				throw new InvalidOperationException("leftMethodList is null or empty");
			}
			
			int index = CommonHelperMethods.GetRandomPositiveInt0ToValue (leftMethodList.Count - 1);
			return leftMethodList [index];   
		}

		/// <summary>
		/// Tries the select right method at random.
		/// </summary>
		/// <returns><c>true</c>, if select right method at random was tryed, <c>false</c> otherwise.</returns>
		/// <param name="returnType">Return type.</param>
		/// <param name="signatureOut">Signature out.</param>
		public static bool TrySelectRightMethodAtRandom(
			Type returnType,
			out MethodSignature signatureOut)
		{
			if (rightMethodList == null) 
			{
				throw new InvalidOperationException("rightMethodList is null");
			}

			List<MethodSignature> candidates = new List<MethodSignature> ();
			foreach (MethodSignature signature in rightMethodList) 
			{
				if (signature.ReturnType == returnType)
				{
					candidates.Add(signature);
				}
			}

			if (candidates.Count == 0) 
			{
				signatureOut = null;
				return false;
			}

			int index = CommonHelperMethods.GetRandomPositiveInt0ToValue (candidates.Count - 1);
			signatureOut = candidates [index];
			return true;
		}

		/// <summary>
		/// Tries the select read only variable at random.
		/// </summary>
		/// <returns><c>true</c>, if select read only variable at random was tryed, <c>false</c> otherwise.</returns>
		/// <param name="returnType">Return type.</param>
		/// <param name="signatureOut">Signature out.</param>
		public static bool TrySelectReadOnlyVariableAtRandom(
			Type returnType,
			out VariableSignature signatureOut)
		{
			if (readOnlyVariableList == null) 
			{
				throw new InvalidOperationException("readOnlyVariableList is null");
			}

			List<VariableSignature> candidates = new List<VariableSignature> ();
			foreach (VariableSignature signature in readOnlyVariableList) 
			{
				if (signature.VariableType == returnType)
				{
					candidates.Add(signature);
				}
			}

			foreach (VariableSignature signature in readWriteVariableList) 
			{
				if (signature.VariableType == returnType)
				{
					candidates.Add(signature);
				}
			}

			if (candidates.Count == 0) 
			{
				signatureOut = null;
				return false;
			}

			int index = CommonHelperMethods.GetRandomPositiveInt0ToValue (candidates.Count - 1);
			signatureOut = candidates [index];
			return true;
		}

		/// <summary>
		/// Selects an operator at random.
		/// </summary>
		/// <returns>The operator.</returns>
		/// <param name="returnType">Return type of the operator.</param>
		public static bool TrySelectOperatorAtRandom(
			Type returnType,
			out OperatorSignature signatureOut)
		{
			if (operatorList == null) 
			{
				throw new InvalidOperationException("operatorList is null");
			}
			
			List<OperatorSignature> candidates = new List<OperatorSignature> ();
			foreach (OperatorSignature signature in operatorList) 
			{
				if (signature.ReturnType == returnType)
				{
					candidates.Add(signature);
				}
			}
			
			if (candidates.Count == 0) 
			{
				signatureOut = null;
				return false;
			}
			
			int index = CommonHelperMethods.GetRandomPositiveInt0ToValue (candidates.Count - 1);
			signatureOut = candidates [index];
			return true;
		}

		/// <summary>
		/// Adds the operator.
		/// </summary>
		/// <param name="signature">Signature.</param>
		public static void AddOperator(string value, Type returnTypeIn, Type lhsTypeIn, Type rhsTypeIn)
		{
			OperatorSignature signature = new OperatorSignature (value, returnTypeIn , lhsTypeIn, rhsTypeIn);
			operatorList.Add (signature);
		}

		/// <summary>
		/// Adds a left method.
		/// </summary>
		/// <param name="signature">Signature.</param>
		public static void AddLeftMethod(EntMethodEnum methodId, Type returnTypeIn, params Type[] parameterTypesIn)
		{
			MethodSignature signature = new MethodSignature(methodId, returnTypeIn, parameterTypesIn);
			leftMethodList.Add (signature);
		}

		/// <summary>
		/// Adds a right method.
		/// </summary>
		/// <param name="signature">Signature.</param>
		public static void AddRightMethod(EntMethodEnum methodId, Type returnTypeIn, params Type[] parameterTypesIn)
		{
			MethodSignature signature = new MethodSignature(methodId, returnTypeIn, parameterTypesIn);
			rightMethodList.Add(signature);
		}

		/// <summary>
		/// Adds a read only variable.
		/// </summary>
		/// <param name="signature">Signature.</param>
		public static void AddReadOnlyVariable(EntVariableEnum variableId, Type variableType)
		{
			VariableSignature signature = new VariableSignature(variableId, variableType);
			readOnlyVariableList.Add (signature);
		}

		/// <summary>
		/// Adds a read write variable.
		/// </summary>
		/// <param name="signature">Signature.</param>
		public static void AddReadWriteVariable(EntVariableEnum variableId, Type variableType)
		{
			VariableSignature signature = new VariableSignature(variableId, variableType);
			readWriteVariableList.Add (signature);
		}
	}
}

