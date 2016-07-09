namespace Poker.Utility.HandEvaluation
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Poker.Utility.Cards;

	/// <summary>
	/// This class represents a hand and an associated average
	/// </summary>
	public class HandStrengthWavePair
	{
		/// <summary>
		/// Initializes a new instance of the HandFuzzySetPair class.
		/// </summary>
		/// <param name="hand">A hand</param>
		/// <param name="strength">A strength value</param>
		internal HandStrengthWavePair(Hand hand, float strength)
			: this(hand, new StrengthWave(strength))
		{
		}

		/// <summary>
		/// Initializes a new instance of the HandAveragePair class.
		/// </summary>
		/// <param name="hand">A hand</param>
		/// <param name="strength">A strength</param>
		internal HandStrengthWavePair(Hand hand, StrengthWave strength)
		{
			if (hand == null || strength == null)
			{
				throw new ArgumentException("Recieved a null parameter");
			}

			this.Hand = hand;
			this.FuzzySet = strength;
		}

		/// <summary>
		/// Initializes a new instance of the HandAveragePair class.
		/// </summary>
		/// <param name="listToCompile">A list of strengths to compile into one</param>
		internal HandStrengthWavePair(List<HandStrengthWavePair> listToCompile)
		{
			if (listToCompile == null || listToCompile.Count == 0)
			{
				throw new ArgumentException("Invalid list parameter given");
			}

			// Take the hand from the first list item
			this.Hand = listToCompile[0].Hand;

			// Compile the averages together to get the strength
			List<StrengthWave> setList = new List<StrengthWave>();

			foreach (HandStrengthWavePair pair in listToCompile)
			{
				if (pair.Hand != this.Hand)
				{
					throw new ArgumentException("All pairs in given list should correspond to the same hand");
				}

				setList.Add(pair.FuzzySet);
			}

			this.FuzzySet = new StrengthWave(setList);
		}

		/// <summary>
		/// Gets the hand in the pair
		/// </summary>
		internal Hand Hand { get; private set; }

		/// <summary>
		/// Gets the strength in the pair
		/// </summary>
		public StrengthWave FuzzySet { get; private set; }

		/// <summary>
		/// An override for the ToString method
		/// </summary>
		/// <returns>A string representation of this object</returns>
		public override string ToString()
		{
			return this.Hand.ToString() + " : " + this.FuzzySet.Mean.ToString();
		}

		/// <summary>
		/// Combines a 2D list of HandAveragePair objects and combines them into a single list.
		/// This assumes that each list contains a no more than a single entry for each hand.
		/// </summary>
		/// <param name="listsIn">The 2D list in</param>
		/// <returns>A single list of compiled HandAveraePairs</returns>
		internal static List<HandStrengthWavePair> CombineLists(List<List<HandStrengthWavePair>> listsIn)
		{
			// Initialize a result list
			List<HandStrengthWavePair> resultsList = new List<HandStrengthWavePair>();

			// Initialize a list to contain all the fuzzy sets for a hand until we are ready to combine them
			Dictionary<Hand, List<HandStrengthWavePair>> handSetDictionary = new Dictionary<Hand, List<HandStrengthWavePair>>();

			// For each list in the 2D list in
			foreach (List<HandStrengthWavePair> handSetList in listsIn)
			{
				// For each item in the list
				foreach (HandStrengthWavePair handSetPair in handSetList)
				{
					// Try to find the hand in the dictionary
					if (handSetDictionary.ContainsKey(handSetPair.Hand))
					{
						// Add to the compile list for that hand
						handSetDictionary[handSetPair.Hand].Add(handSetPair);
					}
					else
					{
						// Add a new entry to the dictionary
						handSetDictionary.Add(handSetPair.Hand, new List<HandStrengthWavePair> () { handSetPair });
					}
				}
			}

			// Now that everythign is compiled by hand, walk through the list and compile the hand averages for each hand
			foreach (Hand key in handSetDictionary.Keys)
			{
				resultsList.Add(new HandStrengthWavePair(handSetDictionary[key]));
			}

			// Return the results list
			return resultsList;
		}
	}
}
