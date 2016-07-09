namespace Poker.Utility.HandEvaluation
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Poker.Utility.Cards;

	/// <summary>
	/// This class handles the back and forth conversion of hands to strength ratings and 
	/// back again within the context of a specific list if community cards.
	/// </summary>
	public static class CommunityCardListContext
	{
		/// <summary>
		/// Precalculated ranks of preflop hands.
		/// Suited below diagonal, unsuited above diagonal
		/// </summary>
		private static int[,] PreFlopLookUpTable = new int[13, 13]
        { //   2,   3,   4,   5,   6,   7,   8,   9,  10,   J,   Q,   K,   A
            { 87, 169, 168, 166, 167, 165, 159, 149, 135, 121, 105,  86,  59}, // 2
            {163,  66, 164, 161, 162, 160, 157, 144, 131, 116,  98,  80,  53}, // 3
            {158, 150,  48, 153, 154, 151, 148, 140, 125, 111,  93,  74,  49}, // 4
            {155, 146, 136,  27, 145, 141, 137, 130, 122, 107,  89,  69,  41}, // 5
            {156, 147, 138, 128,  17, 133, 127, 120, 112, 102,  81,  62,  42}, // 6
            {152, 143, 134, 124, 115,   9, 117, 109, 101,  92,  77,  58,  36}, // 7
            {142, 139, 129, 119, 110, 100,   7,  99,  91,  79,  68,  51,  32}, // 8
            {132, 126, 123, 113, 103,  94,  83,   6,  78,  70,  56,  40,  25}, // 9
            {118, 114, 108, 106,  96,  84,  73,  64,   5,  57,  47,  33,  19}, // 10
            {104,  97,  95,  90,  85,  75,  65,  55,  45,   4,  39,  26,  15}, // J
            { 88,  82,  76,  72,  67,  61,  52,  43,  34,  28,   3,  23,  14}, // Q
            { 71,  63,  60,  54,  50,  44,  37,  29,  22,  20,  16,   2,  12}, // K
            { 46,  38,  35,  30,  31,  24,  21,  18,  13,  11,  10,   8,   1}  // A
          //   2,   3,   4,   5,   6,   7,   8,   9,  10,   J,   Q,   K,   A
        };

		/// <summary>
		/// Converts a hand to its corresponding strength rating in this context
		/// </summary>
		/// <param name="hand">A hand</param>
		/// <returns>The strength of the hand given this context</returns>
		public static HandStrengthWavePair ToStrengthFromHand(Hand hand, IReadOnlyList<Card> communityCardList)
		{
			if (communityCardList.Count == 0)
			{
				return new HandStrengthWavePair(hand, CalculatePreFlopStrength(hand));
			}
			else if (communityCardList.Count == 3)
			{
				return new HandStrengthWavePair(hand, CalculateFlopOrTurnStrength(hand, communityCardList));
			}
			else if (communityCardList.Count == 4)
			{
				return new HandStrengthWavePair(hand, CalculateFlopOrTurnStrength(hand, communityCardList));
			}
			else if (communityCardList.Count == 5)
			{
				return new HandStrengthWavePair(hand, CalculateRiverStrength(hand, communityCardList));
			}

			throw new InvalidOperationException("Bad number of community cards recieved.");
		}

		/// <summary>
		/// Compiles a sorted list of hands by strength for pre-flop hands
		/// </summary>
		private static StrengthWave CalculatePreFlopStrength(Hand hand)
		{
			// If pocket pair, or suited
			float strength = 0.0f;
			if (hand.IsPocketPair || hand.IsSuited)
			{
				// Lookup with lower card first for below diagonal values
				int rank = PreFlopLookUpTable[(int)hand.HigherCard.Value, (int)hand.LowerCard.Value];
				strength = (float)rank / 169.0f;
			}

			// If unsuited non pocket pair
			else
			{
				// Lookup with higher card first for above diagonal values
				int rank = PreFlopLookUpTable[(int)hand.LowerCard.Value, (int)hand.HigherCard.Value];
				strength = (float)rank / 169.0f;
			}

			return new StrengthWave(1.0f - strength);
		}

		private static StrengthWave CalculateFlopOrTurnStrength(Hand hand, IReadOnlyList<Card> communityCardList)
		{
			List<CardSuit> flushSuits;
			List<CardSuit> nonFlushSuits;
			DetermineFlushAndNonFlushSuits(communityCardList, out flushSuits, out nonFlushSuits);

			List<Card> takenList = new List<Card>(communityCardList);
			takenList.Add(hand.Card2);
			takenList.Add(hand.Card1);

			List<StrengthWave> compilationList = new List<StrengthWave>();

			// If at least one suit with no flush possibility
			if (nonFlushSuits.Count >= 1)
			{
				// For each card value ("None" is 13)
				for (int valueInt = 0; valueInt < 13; ++valueInt)
				{
					IReadOnlyList<Card> notTakenList =
						DetermineNonFlushNonTakenCards(valueInt, flushSuits, takenList);

					// If at least one not taken
					if (notTakenList.Count > 0)
					{
						// Calculate strength once
						StrengthWave strength = NextStreetHelper(hand, communityCardList, notTakenList[0]);

						// Add result into running list for each not taken card
						foreach(Card card in notTakenList) compilationList.Add(strength);
					}
				}
			}

			// For each represented suit that a flush is possible in
			foreach (CardSuit suit in flushSuits)
			{
				List<Card> notTakenList = new List<Card>();

				// For each card value ("None" is 13)
				for (int valueInt = 0; valueInt < 13; ++valueInt)
				{
					CardValue value = (CardValue)valueInt;

					// Get the card
					Card candidateCard = Card.ToCardFromEnums(value, suit);

					// If not taken
					if (!Card.CardListContainsCard(takenList, candidateCard))
					{
						notTakenList.Add(candidateCard);
					}
				}

				// Calculate and add strength once for each not taken card
				foreach (Card card in notTakenList)
				{
					compilationList.Add(NextStreetHelper(hand, communityCardList, card));
				}
			}

			return new StrengthWave(compilationList);
		}

		private static StrengthWave CalculateRiverStrength(Hand playerHand, IReadOnlyList<Card> communityCardList)
		{
			// For each suit, determine if flush possible or not
			List<CardSuit> flushSuits;
			List<CardSuit> nonFlushSuits;

			// Set initial counts
			int betterCountTotal = 0;
			int notBetterCountTotal = 0;

			// Separate flush and non flush sensitive suits
			List<Card> flushEvaluationList = new List<Card>(communityCardList);
			DetermineFlushAndNonFlushSuits(flushEvaluationList, out flushSuits, out nonFlushSuits);

			// Compile the list of taken cards
			List<Card> takenList = new List<Card>(communityCardList);
			takenList.Add(playerHand.Card2);
			takenList.Add(playerHand.Card1);

			// Evaluate the player's hand
			EvaluatedHand playerEvaluatedHand = new EvaluatedHand(communityCardList, playerHand);

			// If at least one suit with no flush possibility
			if (nonFlushSuits.Count >= 1)
			{
				// For each card value ("None" is 13)
				for (int valueInt = 0; valueInt < 13; ++valueInt)
				{
					// Get the list of card of a non flush suit, of this card value, that is also not taken
					IReadOnlyList<Card> notTakenList =
						DetermineNonFlushNonTakenCards(valueInt, flushSuits, takenList);

					// If at least one
					if (notTakenList.Count > 0)
					{
						int betterCount;
						int notBetterCount;

						// Only calculate once
						CalculateRiverStrength(
							playerHand,
							playerEvaluatedHand,
							communityCardList,
							notTakenList[0],
							out betterCount,
							out notBetterCount);

						// Add to totals
						betterCountTotal += (betterCount * notTakenList.Count);
						notBetterCountTotal += (notBetterCount * notTakenList.Count);
					}
				}
			}

			// For each represented suit that a flush is possible in
			foreach (CardSuit suit in flushSuits)
			{
				// Get the list of card in this suit that are not taken
				IReadOnlyList<Card> notTakenList =
					DetermineFlushNonTakenCards(suit, takenList);

				// For each of them
				foreach (Card card in notTakenList)
				{
					int betterCount;
					int notBetterCount;

					// Only calculate once
					CalculateRiverStrength(
						playerHand,
						playerEvaluatedHand,
						communityCardList,
						card,
						out betterCount,
						out notBetterCount);

					// Add to totals
					betterCountTotal += betterCount;
					notBetterCountTotal += notBetterCount;
				}
			}

			float strengthValue = 
				1.0f - ((float)betterCountTotal / (float)(betterCountTotal + notBetterCountTotal));
			return new StrengthWave(strengthValue);
		}

		private static void CalculateRiverStrength(
			Hand playerHand,
			EvaluatedHand playerEvaluatedHand,
			IReadOnlyList<Card> communityCardList,
			Card opponentfirstCard,
			out int betterCount,
			out int notBetterCount)
		{
			// For each suit, determine if flush possible or not
			List<CardSuit> flushSuits;
			List<CardSuit> nonFlushSuits;

			// Set initial counts
			betterCount = 0;
			notBetterCount = 0;

			List<Card> flushEvaluationList = new List<Card>(communityCardList);
			flushEvaluationList.Add(opponentfirstCard);
			DetermineFlushAndNonFlushSuits(flushEvaluationList, out flushSuits, out nonFlushSuits);

			List<Card> takenList = new List<Card>(communityCardList);
			takenList.Add(playerHand.Card2);
			takenList.Add(playerHand.Card1);
			takenList.Add(opponentfirstCard);

			// If at least one suit with no flush possibility
			if (nonFlushSuits.Count >= 1)
			{
				// For each card value ("None" is 13)
				for (int valueInt = 0; valueInt < 13; ++valueInt)
				{
					// Get the list of card of a non flush suit, of this card value, that is also not taken
					IReadOnlyList<Card> notTakenList =
						DetermineNonFlushNonTakenCards(valueInt, flushSuits, takenList);

					// If at least one
					if (notTakenList.Count > 0)
					{
						// Make opponent hand once
						Hand opponentHand = new Hand(notTakenList[0], opponentfirstCard);

						// Evaluate against CC list
						EvaluatedHand evaluatedOpponentHand = new EvaluatedHand(communityCardList, opponentHand);

						if (evaluatedOpponentHand > playerEvaluatedHand)
						{
							betterCount += notTakenList.Count;
						}
						else
						{
							notBetterCount += notTakenList.Count;
						}
					}
				}
			}

			// For each represented suit that a flush is possible in
			foreach (CardSuit suit in flushSuits)
			{
				// Get the list of card in this suit that are not taken
				IReadOnlyList<Card> notTakenList =
					DetermineFlushNonTakenCards(suit, takenList);

				// For each of them
				foreach (Card card in notTakenList)
				{
					// Make opponent hand
					Hand opponentHand = new Hand(notTakenList[0], opponentfirstCard);

					// Evaluate against CC list
					EvaluatedHand evaluatedOpponentHand = new EvaluatedHand(communityCardList, opponentHand);

					if (evaluatedOpponentHand > playerEvaluatedHand)
					{
						betterCount++;
					}
					else
					{
						notBetterCount++;
					}
				}
			}
		}

		private static StrengthWave NextStreetHelper(Hand hand, IReadOnlyList<Card> communityCardList, Card addedCard)
		{
			List<Card> newCCList = new List<Card>(communityCardList);
			newCCList.Add(addedCard);

			if (newCCList.Count == 4)
			{
				return CalculateFlopOrTurnStrength(hand, newCCList);
			}
			else if (newCCList.Count == 5)
			{
				return CalculateRiverStrength(hand, newCCList);
			}

			throw new InvalidOperationException("Bad number of community cards.");
		}

		private static IReadOnlyList<Card> DetermineFlushNonTakenCards(
			CardSuit flushSuit,
			IReadOnlyList<Card> takenList)
		{
			List<Card> notTakenList = new List<Card>();

			// For each card value ("None" is 13)
			for (int valueInt = 0; valueInt < 13; ++valueInt)
			{
				CardValue value = (CardValue)valueInt;

				// Get the card
				Card candidateCard = Card.ToCardFromEnums(value, flushSuit);

				// If not taken
				if (!Card.CardListContainsCard(takenList, candidateCard))
				{
					notTakenList.Add(candidateCard);
				}
			}

			return notTakenList;
		}

		private static IReadOnlyList<Card> DetermineNonFlushNonTakenCards(
			int valueInt,
			IReadOnlyList<CardSuit> flushSuits,
			IReadOnlyList<Card> takenList)
		{
			CardValue value = (CardValue)valueInt;

			// Find the cards of this value that are not taken
			List<Card> notTakenList = new List<Card>();
			for (int suitInt = 0; suitInt < 4; ++suitInt)
			{
				// "None" is 4
				CardSuit suit = (CardSuit)suitInt;

				// Skip flush possible suits
				if (flushSuits.Contains(suit)) continue;

				// Get the card
				Card candidateCard = Card.ToCardFromEnums(value, suit);

				// If not taken
				if (!Card.CardListContainsCard(takenList, candidateCard))
				{
					notTakenList.Add(candidateCard);
				}
			}

			return notTakenList;
		}

		private static void DetermineFlushAndNonFlushSuits(
			IReadOnlyList<Card> communityCardList,
			out List<CardSuit> flushSuitsOut,
			out List<CardSuit> nonFlushSuitsOut)
		{
			flushSuitsOut = new List<CardSuit>();
			nonFlushSuitsOut = new List<CardSuit>();

			int heartCount = 0;
			int spadeCount = 0;
			int diamondCount = 0;
			int clubCount = 0;

			foreach (Card card in communityCardList)
			{
				switch (card.Suit)
				{
					case CardSuit.Club:
						clubCount++;
						break;
					case CardSuit.Diamond:
						diamondCount++;
						break;
					case CardSuit.Heart:
						heartCount++;
						break;
					case CardSuit.Spade:
						spadeCount++;
						break;
				}
			}

			// Determine the number of cards needed for a flush possibility
			int countRequiredForFlushPossibility;
			if (communityCardList.Count == 3)
			{
				// 1 if flop
				countRequiredForFlushPossibility = 1;
			}
			else if (communityCardList.Count == 4)
			{
				// 2 if Turn
				countRequiredForFlushPossibility = 2;
			}
			else if (communityCardList.Count == 5)
			{
				// 3 if River, 0 opponent cards chosen
				countRequiredForFlushPossibility = 3;
			}
			else if (communityCardList.Count == 6)
			{
				// 4 if River, 1 opponent cards chosen
				countRequiredForFlushPossibility = 4;
			}
			else
			{
				// Error
				throw new InvalidOperationException("Bad community card list count.");
			}

			if (spadeCount >= countRequiredForFlushPossibility)
			{
				flushSuitsOut.Add(CardSuit.Spade);
			}
			else
			{
				nonFlushSuitsOut.Add(CardSuit.Spade);
			}

			if (heartCount >= countRequiredForFlushPossibility)
			{
				flushSuitsOut.Add(CardSuit.Heart);
			}
			else
			{
				nonFlushSuitsOut.Add(CardSuit.Heart);
			}

			if (diamondCount >= countRequiredForFlushPossibility)
			{
				flushSuitsOut.Add(CardSuit.Diamond);
			}
			else
			{
				nonFlushSuitsOut.Add(CardSuit.Diamond);
			}

			if (clubCount >= countRequiredForFlushPossibility)
			{
				flushSuitsOut.Add(CardSuit.Club);
			}
			else
			{
				nonFlushSuitsOut.Add(CardSuit.Club);
			}
		}
	}
}
