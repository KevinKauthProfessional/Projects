

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
    public class FileIOManager : IDisposable
    {
        public const string DNAFileExtension = ".dna";
        private const string GenePoolDirectoryName = "GenePool";

        private static Dictionary<string, List<RootStatement>> statementDictionary = new Dictionary<string, List<RootStatement>>();

        private StreamWriter writer = null;
        private CustomFileReader reader = null;

        private FileIOManager(FileInfo fileOnDisk, bool read)
        {
            this.FileName = fileOnDisk.FullName;

            if (read)
            {
                if (!fileOnDisk.Exists)
                {
                    // Better than File.Create as the dispose will release system holds on the file that may linger
                    StreamWriter creationWriter = new StreamWriter(fileOnDisk.FullName);
                    creationWriter.Dispose();
                }

                this.reader = new CustomFileReader(fileOnDisk.FullName);
            }
            else
            {
                this.writer = new StreamWriter(fileOnDisk.FullName);
            }
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

        public bool EndOfStream
        {
            get
            {
                lock (CommonHelperMethods.GlobalFileIOLock)
                {
                    return this.reader.EndOfStream;
                }
            }
        }

        public int LineNumber
        {
            get
            {
                return this.reader.LineNumber;
            }
        }

        public string ReadLine()
        {
            lock (CommonHelperMethods.GlobalFileIOLock)
            {
                return this.reader.ReadLine();
            }
        }

        public string ReadNextContentLineAndTrim()
        {
            lock (CommonHelperMethods.GlobalFileIOLock)
            {
                return this.reader.ReadNextContentLineAndTrim();
            }
        }

        public void WriteLine(string line)
        {
            lock (CommonHelperMethods.GlobalFileIOLock)
            {
                 this.writer.WriteLine(line);
            }
        }

        public static FileIOManager OpenDiskFileForRead(string fileName)
        {
            lock (CommonHelperMethods.GlobalFileIOLock)
            {
                return OpenFile(fileName, true);
            }
        }

        public static FileIOManager OpenDiskFileForWrite(string fileName)
        {
            lock (CommonHelperMethods.GlobalFileIOLock)
            {
                return OpenFile(fileName, false);
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

        private static FileIOManager OpenFile(string fileName, bool read)
        {
            // Read and write to disk
            FileInfo fileOnDisk = new FileInfo(fileName);
            return new FileIOManager(fileOnDisk, read);
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
                if (this.reader != null)
                {
                    this.reader.Dispose();
                    this.reader = null;
                }

                if (this.writer != null)
                {
                    this.writer.Dispose();
                    this.writer = null;
                }
            }
        }
    }
}
