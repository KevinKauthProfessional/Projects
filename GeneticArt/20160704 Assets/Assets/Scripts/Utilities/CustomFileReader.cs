//------------------------------------------------------------------
// <copyright file="CustomFileReader.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace Assets.Scripts.Utilities
{
	using System;
    using System.IO;

	/// <summary>
	/// A class wrapping a StreamReader object with common helper functionality added.
	/// </summary>
	internal class CustomFileReader : IDisposable
	{
		private bool isDisposed = false;
		private int lineNumber = 0;
		private StreamReader reader;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Scripts.EntLogic.Utilities.CustomFileReader"/> class.
		/// </summary>
		/// <param name="fileName">File name.</param>
		public CustomFileReader (string fileName)
		{
			this.FileName = fileName;
            this.reader = new StreamReader(fileName);
		}

		/// <summary>
		/// Gets the name of the file.
		/// </summary>
		/// <value>The name of the file.</value>
		public string FileName { get; private set; }

		/// <summary>
		/// Gets the line number.
		/// </summary>
		/// <value>The line number.</value>
		public int LineNumber 
		{
			get
			{
				return this.lineNumber;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="AssemblyCSharp.Scripts.EntLogic.Utilities.CustomFileReader"/> is at end
		/// of stream.
		/// </summary>
		/// <value><c>true</c> if end of stream; otherwise, <c>false</c>.</value>
		public bool EndOfStream
		{
			get
			{
				return this.reader.EndOfStream;
			}
		}

		/// <summary>
		/// Reads the next line.
		/// </summary>
		/// <returns>The next line. Returns null if at end of stream.</returns>
		public string ReadLine()
		{
			this.lineNumber++;
			return this.reader.ReadLine ();
		}

		/// <summary>
		/// Reads the next line with non whitespace characters and trims the result.
		/// </summary>
		/// <returns>The nest line of the stream after being trimmed.  
		/// Returns string.Empty if at end of stream.</returns>
		public string ReadNextContentLineAndTrim()
		{
			string nextLine = string.Empty;

			while (string.IsNullOrEmpty(nextLine)) 
			{
				if (this.EndOfStream)
				{
					return string.Empty;
				}

				nextLine = this.reader.ReadLine ();			
				nextLine = nextLine.Trim ();
			}

			return nextLine;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.Utilities.CustomFileReader"/>. The <see cref="Dispose"/> method leaves
		/// the <see cref="AssemblyCSharp.Scripts.EntLogic.Utilities.CustomFileReader"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.Utilities.CustomFileReader"/> so the garbage collector can reclaim the
		/// memory that the <see cref="AssemblyCSharp.Scripts.EntLogic.Utilities.CustomFileReader"/> was occupying.</remarks>
		public void Dispose()
		{
			this.Dispose (true);
			GC.SuppressFinalize (this);
		}

		/// <summary>
		/// Dispose the specified disposing.
		/// </summary>
		/// <param name="disposing">If set to <c>true</c> disposing.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (this.isDisposed) 
			{
				return;
			}

			if (disposing) 
			{
				this.reader.Dispose();
			}

			this.isDisposed = true;
		}
	}
}

