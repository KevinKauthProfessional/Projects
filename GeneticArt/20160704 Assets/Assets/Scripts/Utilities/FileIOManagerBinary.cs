

namespace Assets.Scripts.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using AssemblyCSharp.Scripts.EntLogic.SerializationObjects;
    using Assets.Scripts.Utilities;    
    using UnityEngine;

    /// <summary>
    /// Replaces StreamReader / StreamWriter in order to handle build targets with no File IO allowed.
    /// </summary>
    public class FileIOManagerBinary : IDisposable
    {
        public const string DNAFileExtension = ".dna";
        private const string GenePoolDirectoryName = "GenePool";

        private static Dictionary<string, List<RootStatement>> statementDictionary = new Dictionary<string, List<RootStatement>>();

        private FileStream stream;

        private FileIOManagerBinary(string fileName, bool read)
        {
            this.FileName = fileName;
            this.stream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public static bool DiskUsePermitted
        {
            get
            {
                //return Application.platform != RuntimePlatform.WebGLPlayer;
                return false;
            }
        }

        public string FileName { get; private set; }

        public byte ReadByte()
        {
            lock (CommonHelperMethods.GlobalFileIOLock)
            {
                return (byte)this.stream.ReadByte();
            }
        }

        public void WriteByte(byte value)
        {
            lock (CommonHelperMethods.GlobalFileIOLock)
            {
                this.stream.WriteByte(value);
            }
        }

        public static List<RootStatement> ReadNonDiskFile(string fileName)
        {
            List<RootStatement> result;
            if (statementDictionary.TryGetValue(fileName, out result))
            {
                return new List<RootStatement>(result);
            }

            return new List<RootStatement>();
        }

        public static void WriteNonDiskFile(string fileName, List<RootStatement> statementList)
        {
            if (!statementDictionary.ContainsKey(fileName))
            {
                statementDictionary.Add(fileName, statementList);
            }
            else
            {
                statementDictionary[fileName] = new List<RootStatement>(statementList);
            }
        }

        /// <summary>
        /// Generates the new DNA file path.
        /// </summary>
        /// <returns>The new DNA file path.</returns>
        /// <param name="winLossCount">Win loss count.</param>
        public static string GenerateNewDNAFilePath(int winLossCount)
        {
            lock (CommonHelperMethods.GlobalFileIOLock)
            {
                if (winLossCount <= 0)
                {
                    LogUtility.LogError("Creating DNA file with non positive win loss count");
                }

                string fileName = string.Format(
                    "{0}_{1}{2}",
                    winLossCount,
                    Guid.NewGuid(),
                    DNAFileExtension);

                if (!DiskUsePermitted)
                {
                    return fileName;
                }

                string path = Path.Combine(GetGenePoolDirectoryInfo().FullName, fileName);
                return path;
            }
        }

        public static void Delete(string fileName)
        {
            lock (CommonHelperMethods.GlobalFileIOLock)
            {
                if (!DiskUsePermitted)
                {
                    statementDictionary.Remove(fileName);
                }
                else
                {
                    File.Delete(fileName);
                }
            }
        }

        public static void Move(string oldFileName, string newFileName)
        {
            lock (CommonHelperMethods.GlobalFileIOLock)
            {
                if (!DiskUsePermitted)
                {
                    List<RootStatement> statementList;
                    if (statementDictionary.TryGetValue(oldFileName, out statementList))
                    {
                        statementDictionary.Remove(oldFileName);
                        statementDictionary.Add(newFileName, statementList);
                    }
                }
                else
                {
                    File.Move(oldFileName, newFileName);
                }
            }
        }

        public static List<string> GetFileNames()
        {
            lock (CommonHelperMethods.GlobalFileIOLock)
            {
                List<string> resultList = new List<string>();

                if (!DiskUsePermitted)
                {
                    resultList.AddRange(statementDictionary.Keys.ToList());
                }
                else
                {
                    DirectoryInfo genePoolDirectory = GetGenePoolDirectoryInfo();
                    foreach (FileInfo file in genePoolDirectory.GetFiles())
                    {
                        resultList.Add(file.FullName);
                    }
                }

                return resultList;
            }
        }

        /// <summary>
        /// Gets the gene pool directory info.
        /// </summary>
        /// <returns>The gene pool directory info.</returns>
        private static DirectoryInfo GetGenePoolDirectoryInfo()
        {
            string path = Path.Combine(Application.persistentDataPath, GenePoolDirectoryName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return new DirectoryInfo(path);
        }

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            lock (CommonHelperMethods.GlobalFileIOLock)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) 
            {
                // free managed resources
                if (this.stream != null)
                {
                    this.stream.Dispose();
                    this.stream = null;
                }
            }
        }
    }
}
