//------------------------------------------------------------------
// <copyright file="GeneData.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.GameLogic
{
	using System.IO;
	using AssemblyCSharp.Scripts.EntLogic.SerializationObjects;
	using AssemblyCSharp.Scripts.Utilities;

	public class GeneData
	{
		private GeneticLogicRoot dna;
		private FileInfo parentDNAFile;

		public GeneData (string teamName, FileInfo parentDNAFile)
		{
			this.TeamName = teamName;
			this.ParentDNAFile = parentDNAFile;
			this.DNA = new GeneticLogicRoot (parentDNAFile);
			this.DNA.PossiblyMutate ();
		}

		public string TeamName { get; private set; }

		public FileInfo ParentDNAFile 
		{ 
			get
			{
				return this.parentDNAFile;
			}

			private set
			{
				this.parentDNAFile = value;
			}
		}

		public GeneticLogicRoot DNA 
		{
			get
			{
				return this.dna;
			}

			private set
			{
				this.dna = value;
			}
		}
	}
}

