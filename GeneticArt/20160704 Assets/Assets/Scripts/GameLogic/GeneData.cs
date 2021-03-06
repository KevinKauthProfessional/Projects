//------------------------------------------------------------------
// <copyright file="GeneData.cs">
//     Copyright (c) Kevin Joshua Kauth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------
namespace AssemblyCSharp.Scripts.GameLogic
{
	using AssemblyCSharp.Scripts.EntLogic.SerializationObjects;
	using Assets.Scripts.Utilities;

	public class GeneData
	{
		private GeneticLogicRoot dna;

		public GeneData (string teamName, string parentDNAFilePath)
		{
			this.TeamName = teamName;
            this.ParentDNAFilePath = parentDNAFilePath;
            this.DNA = new GeneticLogicRoot(parentDNAFilePath);
			this.DNA.PossiblyMutate ();
		}

		public string TeamName { get; private set; }

        public string ParentDNAFilePath { get; private set; }

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

