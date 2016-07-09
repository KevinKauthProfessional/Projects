

namespace Assets.Scripts.Utilities
{
    using Assets.Scripts.EntLogic.SerializationObjects;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Replaces StreamReader / StreamWriter in order to handle build targets with no File IO allowed.
    /// </summary>
    public class FileIOManager : IDisposable
    {
        public const string DNAFileExtension = ".dna";
        private const string GenePoolDirectoryName = "GenePool";

        private static Dictionary<string, List<byte>> fileCacheDictionary =
            new Dictionary<string, List<byte>>();
        private List<byte> fileCache;
        private int readPosition;

        private FileStream stream;
        private static string diskOutputDirectory;

        public FileIOManager(string fileName)
        {
            this.FileName = fileName;

            if (DiskUsePermitted)
            {
                this.stream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            else
            {
                if (!fileCacheDictionary.ContainsKey(fileName))
                {
                    fileCacheDictionary.Add(fileName, new List<byte>());
                }

                this.fileCache = fileCacheDictionary[fileName];
            }
        }

        private static bool DiskUsePermitted
        {
            get
            {
                //return Application.platform != RuntimePlatform.WebGLPlayer;
                return false;
            }
        }

        private static string DiskOutputDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(diskOutputDirectory))
                {
                    diskOutputDirectory = Path.Combine(Application.persistentDataPath, GenePoolDirectoryName);
                    Console.WriteLine("Gene file output directory: {0}", diskOutputDirectory);
                }

                return diskOutputDirectory;
            }
        }

        public bool EndOfStream
        {
            get
            {
                if (DiskUsePermitted)
                {
                    return this.stream.Length == this.stream.Position;
                }
                else
                {
                    return this.readPosition == this.fileCache.Count;
                }
            }
        }

        public string FileName { get; private set; }

        public byte ReadByte()
        {
            lock (CommonHelperMethods.GlobalFileIOLock)
            {
                if (DiskUsePermitted)
                {
                    return (byte)this.stream.ReadByte();
                }
                else
                {
                    byte result = this.fileCache[this.readPosition];
                    this.readPosition++;
                    return result;
                }
            }
        }

        public void WriteByte(byte value)
        {
            lock (CommonHelperMethods.GlobalFileIOLock)
            {
                if (DiskUsePermitted)
                {
                    this.stream.WriteByte(value);
                }
                else
                {
                    this.fileCache.Add(value);
                }
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
                if (DiskUsePermitted)
                {
                    File.Delete(fileName);
                }
                else
                {
                    fileCacheDictionary.Remove(fileName);
                }
            }
        }

        public static void Move(string oldFileName, string newFileName)
        {
            lock (CommonHelperMethods.GlobalFileIOLock)
            {
                if (DiskUsePermitted)
                {
                    File.Move(oldFileName, newFileName);
                }
                else
                {
                    List<byte> cacheData;
                    if (fileCacheDictionary.TryGetValue(oldFileName, out cacheData))
                    {
                        fileCacheDictionary.Remove(oldFileName);
                        fileCacheDictionary.Add(newFileName, cacheData);
                    }
                }
            }
        }

        public static List<string> GetFileNames()
        {
            lock (CommonHelperMethods.GlobalFileIOLock)
            {
                List<string> resultList = new List<string>();

                if (DiskUsePermitted)
                {
                    DirectoryInfo genePoolDirectory = GetGenePoolDirectoryInfo();
                    foreach (FileInfo file in genePoolDirectory.GetFiles())
                    {
                        resultList.Add(file.FullName);
                    }
                }
                else
                {
                    resultList.AddRange(fileCacheDictionary.Keys.ToList());
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
            string path = DiskOutputDirectory;
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
