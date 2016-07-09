//------------------------------------------------------------------
// <copyright file="GeneticLogicRoot.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.EntLogic.SerializationObjects
{
	using System;
	using System.Collections.Generic;

	using AssemblyCSharp.Scripts.UnityGameObjects;
	using Assets.Scripts.Utilities;
	
	/// <summary>
	/// This class represents the root note in a data structure representing 
	/// dynamically compiled genetic logic.
	/// </summary>
	public class GeneticLogicRoot
	{
		private const int MaxRootStatementCount = 125;

		private List<RootStatement> rootStatementList = new List<RootStatement>();
		
		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.GeneticLogicRoot"/> class.
		/// </summary>
		/// <param name="file">File path.</param>
		public GeneticLogicRoot(string filePath)
		{
			lock (CommonHelperMethods.GlobalFileIOLock)
			{
                if (FileIOManager.DiskUsePermitted)
                using (FileIOManager reader = FileIOManager.OpenDiskFileForRead(filePath))
                while (!reader.EndOfStream)
                {
                    // Fast forward to the next RootStatement start
                    string nextLine = reader.ReadNextContentLineAndTrim();
                    if (CommonHelperMethods.StringStartsWith(nextLine, RootStatement.Name))
                    {
                        RootStatement nextRootStatement = null;

                        try
                        {
                            nextRootStatement = new RootStatement(reader);
                        }
                        catch (StatementParseException ex)
                        {
                            // Throw this section of the logic on disk out.
                            // This is expected to happen due to version updates or human
                            // error when handling files representing genetic logic.
                            LogUtility.LogWarningFormat(
                                "Line {0} of file {1} did not parse correctly.  The corresponsing root statement will be thrown out.  Message: {2} CallStack: {3}",
                                reader.LineNumber,
                                filePath,
                                ex.Message,
                                ex.StackTrace);

                            nextRootStatement = null;
                        }

                        if (nextRootStatement != null)
                        {
                            // Root statement was successfully parsed.
                            this.rootStatementList.Add(nextRootStatement);
                        }
                    }
                }
                else
                {
                    // Disk use not permitted
                    this.rootStatementList.AddRange(FileIOManager.ReadNonDiskFile(filePath));
                }
			}
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="fileName">File name.</param>
		public void WriteToDisk(string fileName)
		{
			lock (CommonHelperMethods.GlobalFileIOLock) 
			{
                if (FileIOManager.DiskUsePermitted)
                    using (FileIOManager fileManager = FileIOManager.OpenDiskFileForWrite(fileName))
                        foreach (RootStatement rootStatement in this.rootStatementList)
                        {
                            rootStatement.WriteToDisk(fileManager, 0);
                        }
                else
                {
                    FileIOManager.WriteNonDiskFile(fileName, this.rootStatementList);
                }
			}
		}

		/// <summary>
		/// Will possibly mutate this section of logic.
		/// </summary>
		public void PossiblyMutate()
		{
			double nextDouble = CommonHelperMethods.GetRandomDouble0To1();

			double deleteChance = (double)this.rootStatementList.Count / (double)MaxRootStatementCount;
			if (nextDouble <= deleteChance) 
			{
				// Pick random statement to delete
				int deleteIndex = CommonHelperMethods.GetRandomPositiveInt0ToValue(this.rootStatementList.Count - 1);
				this.rootStatementList.RemoveAt(deleteIndex);
				return;
			}

			nextDouble = CommonHelperMethods.GetRandomDouble0To1();
			double addChance = 1.0 - deleteChance;
			if (nextDouble <= addChance) 
			{
				// Pick random place to insert new statement
				int insertIndex = CommonHelperMethods.GetRandomPositiveInt0ToValue(this.rootStatementList.Count - 1);

				try
				{
					this.rootStatementList.Insert(insertIndex, new RootStatement());
				}
				catch(ArgumentOutOfRangeException)
				{
					throw new InvalidOperationException(string.Format(
						"Index {0} was out of range inserting into collection of size {1}",
						insertIndex,
						this.rootStatementList.Count));
				}

				return;
			}

			if (this.rootStatementList.Count > 0) 
			{
				// Pick random statement to modify
				int modifyIndex = CommonHelperMethods.GetRandomPositiveInt0ToValue (this.rootStatementList.Count - 1);
				this.rootStatementList [modifyIndex].PossiblyMutate();
			}
		}

		/// <summary>
		/// Executes the genetic logic represented by this data structure 
		/// against the given instance.
		/// </summary>
		/// <param name="instance">The instance to execute against.</param>
		public bool Execute(ref EntBehaviorManager instance)
		{
			bool halt = false;

			foreach (RootStatement rootStatement in this.rootStatementList) 
			{
				halt = rootStatement.Execute(ref instance);

				if (halt)
				{
					// Halts before all code is executed is expected.
					// Triggered by execution of LeftMethodCall.
                    break;
				}
			}

            return halt;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return string.Format ("[GeneticLogicRoot StatementCount={0}]", this.rootStatementList.Count);
		}

		internal static bool RollMutateDice()
		{
			double nextDouble = CommonHelperMethods.GetRandomDouble0To1();
			return nextDouble <= StaticController.GlobalMutationChance;
		}
	}
}

