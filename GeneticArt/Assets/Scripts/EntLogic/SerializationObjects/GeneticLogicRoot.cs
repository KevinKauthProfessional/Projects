//------------------------------------------------------------------
// <copyright file="GeneticLogicRoot.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace Assets.Scripts.EntLogic.SerializationObjects
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
		private const int MaxRootStatementCount = 25;

        //private List<RootStatement> rootStatementList = new List<RootStatement>();
        private string filePath;
		
		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="AssemblyCSharp.Scripts.EntLogic.SerializationObjects.GeneticLogicRoot"/> class.
		/// </summary>
		/// <param name="file">File path.</param>
		public GeneticLogicRoot(string filePathIn)
		{
            this.filePath = filePathIn;
		}

		/// <summary>
		/// Writes to disk.
		/// </summary>
		/// <param name="fileName">File name.</param>
		private void WriteToDisk(string fileName, List<RootStatement> statementList)
		{
			lock (CommonHelperMethods.GlobalFileIOLock) 
			{
                using (FileIOManager fileManager = new FileIOManager(fileName))
                foreach (RootStatement rootStatement in statementList)
                {
                    rootStatement.WriteToDisk(fileManager);
                }
			}
		}

		/// <summary>
		/// Will possibly mutate this section of logic.
		/// </summary>
		public void PossiblyMutateAndWriteToDisk(string childFileName)
		{
            List<RootStatement> mutatedDna = this.GetMutatedDNA();
            this.WriteToDisk(childFileName, mutatedDna);
		}

        private List<RootStatement> GetMutatedDNA()
        {
            List<RootStatement> rootStatementList = this.ParseRootStatementList();

            double nextDouble = CommonHelperMethods.GetRandomDouble0To1();

            double deleteChance = (double)rootStatementList.Count / (double)MaxRootStatementCount;
            if (nextDouble <= deleteChance)
            {
                // Pick random statement to delete
                int deleteIndex = CommonHelperMethods.GetRandomPositiveInt0ToValue(rootStatementList.Count - 1);
                rootStatementList.RemoveAt(deleteIndex);
                return rootStatementList;
            }

            nextDouble = CommonHelperMethods.GetRandomDouble0To1();
            double addChance = 1.0 - deleteChance;
            if (nextDouble <= addChance)
            {
                // Pick random place to insert new statement
                int insertIndex = CommonHelperMethods.GetRandomPositiveInt0ToValue(rootStatementList.Count - 1);

                try
                {
                    rootStatementList.Insert(insertIndex, new RootStatement());
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new InvalidOperationException(string.Format(
                        "Index {0} was out of range inserting into collection of size {1}",
                        insertIndex,
                        rootStatementList.Count));
                }

                return rootStatementList;
            }

            if (rootStatementList.Count > 0)
            {
                // Pick random statement to modify
                int modifyIndex = CommonHelperMethods.GetRandomPositiveInt0ToValue(rootStatementList.Count - 1);
                rootStatementList[modifyIndex].PossiblyMutate();
            }

            return rootStatementList;
        }

		/// <summary>
		/// Executes the genetic logic represented by this data structure 
		/// against the given instance.
		/// </summary>
		/// <param name="instance">The instance to execute against.</param>
		public byte Execute(ref EntBehaviorManager instance)
		{
			byte halt = 0;

			foreach (RootStatement rootStatement in this.ParseRootStatementList()) 
			{
				halt = rootStatement.Execute(ref instance);

				if (halt == 1)
				{
					// Halts before all code is executed is expected.
					// Triggered by execution of LeftMethodCall.
                    break;
				}
			}

            return halt;
		}

		internal static bool RollMutateDice()
		{
			double nextDouble = CommonHelperMethods.GetRandomDouble0To1();
			return nextDouble <= StaticController.GlobalMutationChance;
		}

        private List<RootStatement> ParseRootStatementList()
        {
            List<RootStatement> resultList = new List<RootStatement>();
            lock (CommonHelperMethods.GlobalFileIOLock)
            {
                using (FileIOManager reader = new FileIOManager(filePath))
                    while (!reader.EndOfStream)
                    {
                        byte nextByte = reader.ReadByte();
                        if (nextByte != StatementTypeEnum.RootStatement)
                        {
                            // Fast forward to the next RootStatement start
                            continue;
                        }

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
                                "File {0} did not parse correctly.  The corresponsing root statement will be thrown out.  Message: {1} CallStack: {2}",
                                filePath,
                                ex.Message,
                                ex.StackTrace);

                            nextRootStatement = null;
                        }

                        if (nextRootStatement != null)
                        {
                            // Root statement was successfully parsed.
                            resultList.Add(nextRootStatement);
                        }
                    }
            }

            return resultList;
        }
	}
}

