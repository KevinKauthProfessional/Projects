//------------------------------------------------------------------
// <copyright file="LogUtility.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace Assets.Scripts.Utilities
{
	using System;
	using UnityEngine;

// To ignore unreachable code warnings
#pragma warning disable

	/// <summary>
	/// Log utility.
	/// </summary>
	public static class LogUtility
	{
		private const bool IgnoreInfo = true;
		private const bool IgnoreWarnings = false;
		private const bool IgnoreErrors = false;

		private const bool LogToUnityConsole = true;

		private const bool ThrowOnErrors = true;
		private const bool ThrowOnWarnings = true;

		public static void LogInfo(string value)
		{
			if (IgnoreInfo) 
			{
				return;
			}

			string line = string.Format ("{0} Info: {1}", DateTime.Now, value);

			if (LogToUnityConsole) 
			{
				Debug.Log (line);
			}
		}

		public static void LogInfoFormat(string value, params object[] parameters)
		{
			LogInfo(string.Format(value, parameters));
		}

		public static void LogWarning(string value)
		{
			if (IgnoreWarnings) 
			{
				return;
			}

			string line = string.Format ("{0} Warning: {1}", DateTime.Now, value);
			
			if (LogToUnityConsole) 
			{
				Debug.LogWarning (line);
			}

			if (ThrowOnWarnings) 
			{
				throw new InvalidOperationException(line);
			}
		}

		public static void LogWarningFormat(string value, params object[] parameters)
		{
			LogWarning (string.Format (value, parameters));
		}

		public static void LogError(string value)
		{
			if (IgnoreErrors) 
			{
				return;
			}

			string line = string.Format ("{0} Error: {1}", DateTime.Now, value);
			
			if (LogToUnityConsole) 
			{
				Debug.LogError (line);
			}

			if (ThrowOnErrors) 
			{
				throw new InvalidOperationException(line);
			}
		}

		public static void LogErrorFormat(string value, params object[] parameters)
		{
			LogError(string.Format(value, parameters));
		}
	}

#pragma warning restore
}

