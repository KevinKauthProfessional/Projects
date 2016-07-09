//------------------------------------------------------------------
// <copyright file="CommonHelperMethods.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace Assets.Scripts.Utilities
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using AssemblyCSharp.Scripts.EntLogic.GeneticTypes;
	using AssemblyCSharp.Scripts.UnityGameObjects;

	/// <summary>
	/// Common helper methods.
	/// </summary>
	public static class CommonHelperMethods
	{
		public static readonly object GlobalFileIOLock = new object();
		public static readonly object GlobalStateLock = new object();

		// Remember to vary the seed value if we ever want to instantiate more than once.
		private static System.Random globalRand = new System.Random();
		private static bool GameIsInitialized = InitializeGame();

		public static double GetRandomDouble0To1()
		{
			return globalRand.NextDouble();
		}

        public static byte GetRandomByte()
        {
            byte[] buffer = new byte[1];
            globalRand.NextBytes(buffer);
            return buffer[0];
        }

		public static bool InitializeGame()
		{
			if (!GameIsInitialized) 
			{
				EntBehaviorManager.RegisterGeneticMembers();
				GeneticObject.RegisterOperatorsForAllTypes();
			}

			return true;
		}

		public static bool StringsAreEqual(string lhs, string rhs)
		{
			return string.Compare (lhs, rhs, StringComparison.OrdinalIgnoreCase) == 0;
		}

        public static bool StringContainsSubString(string largeString, string subString)
        {
            return largeString.IndexOf(subString, StringComparison.OrdinalIgnoreCase) >= 0;
        }

		public static DateTime ConvertFromUnixTimestamp(double timestamp)
		{
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return origin.AddSeconds(timestamp);
		}
		
		public static double ConvertToUnixTimestamp(DateTime date)
		{
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			TimeSpan diff = date.ToUniversalTime() - origin;
			return Math.Floor(diff.TotalSeconds);
		}

		public static int GetRandomPositiveInt0ToValue(int maxValue)
		{
			if (maxValue <= 0) 
			{
				return 0;
			}

			int result = (int)Math.Round (GetRandomDouble0To1() * maxValue);

			if (result < 0) 
			{
				return 0;
			}

			if (result > maxValue) 
			{
				return maxValue;
			}

			return result;
		}

		public static string PrePendTabs(string text, int tabCount)
		{
			if (text == null) 
			{
				text = string.Empty;
			}

			for (int i = 0; i < tabCount; ++i)
			{
				text = "  " + text;
			}

			return text;
		}

		/// <summary>
		/// A helper method determining whether a string starts with a value.
		/// </summary>
		/// <returns><c>true</c>, if the string starts with the value, <c>false</c> otherwise.</returns>
		/// <param name="original">The string to test against.</param>
		/// <param name="possibleStartingValue">Possible starting value of the string to test with.</param>
		public static bool StringStartsWith(string original, string possibleStartingValue)
		{
			if (string.IsNullOrEmpty (original) || 
			    string.IsNullOrEmpty (possibleStartingValue)) 
			{
				return false;
			}

			return original.IndexOf (possibleStartingValue, StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static void ThrowStatementParseException(
			string value,
			FileIOManager reader,
			string acceptableValue)
		{
			ThrowStatementParseException (value, reader, new List<string> () { acceptableValue });
		}

		public static void ThrowStatementParseException(
			string value,
			FileIOManager reader,
			IList<string> acceptableValueList)
		{
			string errorMessage = string.Format (
				"LineNumber:{0} File:\"{1}\" Value:\"{2}\" AcceptableValues:",
				reader.LineNumber,
				reader.FileName,
				value); 

			foreach (string acceptableValue in acceptableValueList) 
			{
				errorMessage += string.Format("\"{0}\" ", acceptableValue);
			}

			throw new StatementParseException(errorMessage);
		}
	}
}

