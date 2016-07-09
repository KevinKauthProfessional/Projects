namespace Poker.Utility.HandEvaluation
{
    using System;
    using System.Collections.Generic;
    using System.IO;

	using Poker.Utility.Cards;

    public class EvaluatedHand
    {
        /// <summary>
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

        private static int[] HR = new int[32487834];
        private static bool isInitialized = false;

        private HandCategoryEnum handCategory;
        private int rankWithinCategory;
        private List<Card> cardList = new List<Card>();

        private EvaluatedHand(HandCategoryEnum category, int rankWithin)
        {
            this.handCategory = category;
            this.rankWithinCategory = rankWithin;
        }

		/// <summary>
		/// Initializes a new instance of the EvaluatedHand class.
		/// </summary>
		/// <param name="communityCardList">The list of community cards.</param>
		/// <param name="playerHand">The player's hand.</param>
        public EvaluatedHand(IReadOnlyList<Card> communityCardList, Hand playerHand)
        {
			List<Card> cardListInput = new List<Card>();
			cardListInput.AddRange(communityCardList);
			cardListInput.Add(playerHand.Card1);
			cardListInput.Add(playerHand.Card2);

            // Initialize the class if need be
            if (!isInitialized)
            {
                _Initialize();
            }

            // Populate internal card list
            foreach (Card card in cardListInput)
            {
                // Add only non null cards
                if (card.TwoPlusTwoValue > 0)
                {
                    this.cardList.Add(card);
                }
            }

            // Make internal list correct for testing
            if (this.cardList.Count != 2 && this.cardList.Count != 5 && this.cardList.Count != 6 && this.cardList.Count != 7)
            {
                throw new ArgumentException("Need 2, 5, 6, or 7 valid cards to be evaluated");
            }

            // Evaluate using either 5 card or 6/7 card method
            int twoPlusTwoValue;
            if (this.cardList.Count == 2)
            {
                this._ProcessPreFlopHand();
            }
            else
            {
                if (this.cardList.Count == 5)
                {
                    twoPlusTwoValue = _GetHandTwoPlusTwoValue5Card(cardList);
                }
                else if (this.cardList.Count == 6)
                {
                    // Add one null card to the end
                    this.cardList.Add(new Card(string.Empty));
                    twoPlusTwoValue = _GetHandTwoPlusTwoValue(cardList);

                    // Remove the null card
                    this.cardList.RemoveAt(6);
                }
                else if (this.cardList.Count == 7)
                {
                    twoPlusTwoValue = _GetHandTwoPlusTwoValue(cardList);
                }
                else
                {
                    throw new ArgumentException("Something went wrong");
                }

                int handCategoryInt = twoPlusTwoValue >> 12;
                this.handCategory = (HandCategoryEnum)handCategoryInt;
                this.rankWithinCategory = twoPlusTwoValue & 0x00000FFF;
            }
        }

        public HandCategoryEnum HandCategory
        { 
            get { return this.handCategory; } 
        }

        public int RankWithinCategory
        {
            get { return this.rankWithinCategory; }
        }

        public List<Card> CardList
        {
            get { return this.cardList; }
        }

		public static bool operator >(EvaluatedHand lhs, EvaluatedHand rhs)
		{
			// If lhs hand category is higher
			if (lhs.HandCategory > rhs.HandCategory)
			{
				return true;
			}

			// If hand categories are equal but lhs has a higher rank within category
			else if (lhs.handCategory == rhs.handCategory && lhs.rankWithinCategory > rhs.rankWithinCategory)
			{
				return true;
			}

			return false;
		}

		public static bool operator ==(EvaluatedHand lhs, EvaluatedHand rhs)
		{
			// Null checks
			if ((object)lhs == null && (object)rhs == null)
			{
				// Both null
				return true;
			}
			else if ((object)lhs == null || (object)rhs == null)
			{
				// One is null
				return false;
			}

			if (lhs.handCategory == rhs.handCategory &&
				lhs.rankWithinCategory == rhs.rankWithinCategory)
			{
				return true;
			}

			return false;
		}

		public static bool operator !=(EvaluatedHand lhs, EvaluatedHand rhs)
		{
			return !(lhs == rhs);
		}

		public static bool operator <(EvaluatedHand lhs, EvaluatedHand rhs)
		{
			if (lhs == rhs)
			{
				return false;
			}

			return !(lhs > rhs);
		}

		public override bool Equals(object obj)
		{
			try
			{
				EvaluatedHand otherObject = obj as EvaluatedHand;
				return this == otherObject;
			}
			catch (InvalidCastException)
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException("Avoid using hash codes for class EvaluatedHand.");
		}

		public override string ToString()
		{
			return string.Format(
				"Category: {0} Rank: {1}",
				this.handCategory.ToString(),
				this.rankWithinCategory.ToString());
		}

        private static void _Initialize()
        {
            // Initialize lookup table from precomputed file
            using (BinaryReader reader =
                new BinaryReader(File.OpenRead(Paths.EvaluatedHandDatFilePath)))
            {
                int numBytes = sizeof(int) * 32487834;
                byte[] byteArray = reader.ReadBytes(numBytes);
                Buffer.BlockCopy(byteArray, 0, HR, 0, numBytes);
            }

            isInitialized = true;
        }

        private void _ProcessPreFlopHand()
        {
            // Sort cards by value
            this.cardList = Card.SortCardListByValue(this.cardList);
            Card highCard = cardList[0];
            Card lowCard = cardList[1];

            // Do lookup
            if (highCard.Suit == lowCard.Suit)
            {
                this.rankWithinCategory = PreFlopLookUpTable[(int)lowCard.Value, (int)highCard.Value];
            }
            else
            {
                this.rankWithinCategory = PreFlopLookUpTable[(int)highCard.Value, (int)lowCard.Value];
            }
        }

        /// <summary>
        /// Can accept one card with 0 two plus two value and be ok if it is last
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        private static int _GetHandTwoPlusTwoValue(List<Card> cardList)
        {
            if (cardList.Count < 7)
            {
                throw new ArgumentException("Need 7 cards for this lookup, make 7th card have 0 value for 6 card lookup");
            }

            int p = HR[53 + cardList[0].TwoPlusTwoValue];
            p = HR[p + cardList[1].TwoPlusTwoValue];
            p = HR[p + cardList[2].TwoPlusTwoValue];
            p = HR[p + cardList[3].TwoPlusTwoValue];
            p = HR[p + cardList[4].TwoPlusTwoValue];
            p = HR[p + cardList[5].TwoPlusTwoValue];
            p = HR[p + cardList[6].TwoPlusTwoValue];
            return p;
        }

        private static int _GetHandTwoPlusTwoValue5Card(List<Card> cardList)
        {
            if (cardList.Count != 5)
            {
                throw new ArgumentException("This is to evaluate 5 card hands only");
            }

            int p = HR[53 + cardList[0].TwoPlusTwoValue];
            p = HR[p + cardList[1].TwoPlusTwoValue];
            p = HR[p + cardList[2].TwoPlusTwoValue];
            p = HR[p + cardList[3].TwoPlusTwoValue];
            p = HR[p + cardList[4].TwoPlusTwoValue];
            p = HR[p];
            return p;
        }
    }
}
