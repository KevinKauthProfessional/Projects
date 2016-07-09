// http://www.codingthewheel.com/archives/poker-hand-evaluator-roundup
// http://archives1.twoplustwo.com/showflat.php?Number=8513906
// http://www.jazbo.com/poker/huholdem.html
namespace Poker.Utility.Cards
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
    using System.Windows.Media.Imaging;
	using System.Text;
	using System.Threading.Tasks;

    public class Card
    {
		internal static string[] CardStrings = new string[52]
		{
			"2c", "3c", "4c", "5c", "6c", "7c", "8c", "9c", "Tc", "Jc", "Qc", "Kc", "Ac",
			"2s", "3s", "4s", "5s", "6s", "7s", "8s", "9s", "Ts", "Js", "Qs", "Ks", "As",
			"2h", "3h", "4h", "5h", "6h", "7h",	"8h", "9h", "Th", "Jh", "Qh", "Kh", "Ah",
			"2d", "3d", "4d", "5d", "6d", "7d", "8d", "9d", "Td", "Jd", "Qd", "Kd", "Ad"
		};

		private static List<string> CardStringsList = CardStrings.ToList();

        private CardSuit suit;
        private CardValue value;
        private int cactusKevValue;
        private int twoPlusTwoValue;
		private BitmapImage cardImage;

        public Card(CardSuit suit, CardValue value)
        {
            string cardString = string.Empty;
            switch (value)
            {
                case CardValue.Ace:
                    cardString += 'A';
                    break;

                case CardValue.Eight:
                    cardString += "8";
                    break;

                case CardValue.Five:
                    cardString += "5";
                    break;

                case CardValue.Four:
                    cardString += "4";
                    break;

                case CardValue.Jack:
                    cardString += "J";
                    break;

                case CardValue.King:
                    cardString += "K";
                    break;

                case CardValue.Nine:
                    cardString += "9";
                    break;

                case CardValue.Queen:
                    cardString += "Q";
                    break;

                case CardValue.Seven:
                    cardString += "7";
                    break;

                case CardValue.Six:
                    cardString += "6";
                    break;

                case CardValue.Ten:
                    cardString += "T";
                    break;

                case CardValue.Three:
                    cardString += "3";
                    break;

                case CardValue.Two:
                    cardString += "2";
                    break;

                default:
                    throw new NotImplementedException(string.Format("Card value {0} not supported", value));
            }

            switch (suit)
            {
                case CardSuit.Club:
                    cardString += 'c';
                    break;

                case CardSuit.Diamond:
                    cardString += 'd';
                    break;

                case CardSuit.Heart:
                    cardString += 'h';
                    break;

                case CardSuit.Spade:
                    cardString += 's';
                    break;

                default:
                    throw new NotImplementedException(string.Format("Suit {0} not supported", suit));
            }

            Card proxyCard = new Card(cardString);
            this.suit = proxyCard.suit;
            this.twoPlusTwoValue = proxyCard.twoPlusTwoValue;
            this.value = proxyCard.value;
            this.cardImage = proxyCard.cardImage;
            this.cactusKevValue = proxyCard.cactusKevValue;
        }

        public Card(string cardString)
        {
            // Handle "None" card
            if (string.IsNullOrEmpty(cardString))
            {
				this.Suit = CardSuit.None;
				this.Value = CardValue.None;
            }
            else
            {
                char valueChar = cardString[0];
                char suitChar = cardString[1];

                if (valueChar == '2')
                {
					this.Value = CardValue.Two;
                    this.cactusKevValue = 2;
                    this.cactusKevValue |= 0x00010000;
                    this.cactusKevValue |= 0x00000000;
                }
                else if (valueChar == '3')
                {
					this.Value = CardValue.Three;
                    this.cactusKevValue = 3;
                    this.cactusKevValue |= 0x00020000;
                    this.cactusKevValue |= 0x00000100;
                }
                else if (valueChar == '4')
                {
					this.Value = CardValue.Four;
                    this.cactusKevValue = 5;
                    this.cactusKevValue |= 0x00040000;
                    this.cactusKevValue |= 0x00000200;
                }
                else if (valueChar == '5')
                {
					this.Value = CardValue.Five;
                    this.cactusKevValue = 7;
                    this.cactusKevValue |= 0x00080000;
                    this.cactusKevValue |= 0x00000300;
                }
                else if (valueChar == '6')
                {
					this.Value = CardValue.Six;
                    this.cactusKevValue = 11;
                    this.cactusKevValue |= 0x00100000;
                    this.cactusKevValue |= 0x00000400;
                }
                else if (valueChar == '7')
                {
					this.Value = CardValue.Seven;
                    this.cactusKevValue = 13;
                    this.cactusKevValue |= 0x00200000;
                    this.cactusKevValue |= 0x00000500;
                }
                else if (valueChar == '8')
                {
					this.Value = CardValue.Eight;
                    this.cactusKevValue = 17;
                    this.cactusKevValue |= 0x00400000;
                    this.cactusKevValue |= 0x00000600;
                }
                else if (valueChar == '9')
                {
					this.Value = CardValue.Nine;
                    this.cactusKevValue = 19;
                    this.cactusKevValue |= 0x00800000;
                    this.cactusKevValue |= 0x00000700;
                }
                else if (valueChar == 'T')
                {
					this.Value = CardValue.Ten;
                    this.cactusKevValue = 23;
                    this.cactusKevValue |= 0x01000000;
                    this.cactusKevValue |= 0x00000800;
                }
                else if (valueChar == 'J')
                {
					this.Value = CardValue.Jack;
                    this.cactusKevValue = 29;
                    this.cactusKevValue |= 0x02000000;
                    this.cactusKevValue |= 0x00000900;
                }
                else if (valueChar == 'Q')
                {
					this.Value = CardValue.Queen;
                    this.cactusKevValue = 31;
                    this.cactusKevValue |= 0x04000000;
                    this.cactusKevValue |= 0x00000A00;
                }
                else if (valueChar == 'K')
                {
					this.Value = CardValue.King;
                    this.cactusKevValue = 37;
                    this.cactusKevValue |= 0x08000000;
                    this.cactusKevValue |= 0x00000B00;
                }
                else if (valueChar == 'A')
                {
					this.Value = CardValue.Ace;
                    this.cactusKevValue = 41;
                    this.cactusKevValue |= 0x10000000;
                    this.cactusKevValue |= 0x00000C00;
                }
                else
                {
                    throw new ArgumentException("Unexpected value char: " + valueChar);
                }

                if (suitChar == 'c')
                {
					this.Suit = CardSuit.Club;
                    this.cactusKevValue |= 0x00008000;
                }
                else if (suitChar == 's')
                {
					this.Suit = CardSuit.Spade;
                    this.cactusKevValue |= 0x00001000;
                }
                else if (suitChar == 'h')
                {
					this.Suit = CardSuit.Heart;
                    this.cactusKevValue |= 0x00002000;
                }
                else if (suitChar == 'd')
                {
					this.Suit = CardSuit.Diamond;
                    this.cactusKevValue |= 0x00004000;
                }
                else
                {
                    throw new ArgumentException("Unexpected suit char: " + suitChar);
                }
            }

            // Calculate two plus two value
			if (this.Value != CardValue.None && this.Suit != CardSuit.None)
            {
                this.twoPlusTwoValue = ((int)this.Value * 4 + (int)this.Suit) + 1;
            }
            else
            {
                this.twoPlusTwoValue = 0;
            }
        }

        public CardSuit Suit
        {
            get { return this.suit; }
            private set { this.suit = value; }
        }

        public CardValue Value
        {
            get { return this.value; }
            private set { this.value = value; }
        }

        public int CactusKevValue
        {
            get
            {
                return this.cactusKevValue;
            }
        }

        public int TwoPlusTwoValue
        {
            get
            {
                return this.twoPlusTwoValue;
            }
        }

		public BitmapImage CardImage
		{
			get
			{
				if (this.cardImage == null)
				{
					this.cardImage = new BitmapImage(new Uri(Paths.PathForCardName(this.ToString())));
				}

				return this.cardImage;
			}
		}

        public static bool AreCardListsEqual(List<Card> list1, List<Card> list2)
        {
            if (list1 == null && list2 == null)
            {
                return true;
            }

            if (list1 == null)
            {
                return false;
            }

            if (list2 == null)
            {
                return false;
            }

            if (list1.Count != list2.Count)
            {
                return false;
            }

            for (int i = 0; i < list1.Count; ++i)
            {
                if (!CardListContainsCard(list2, list1[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CardListContainsCard(IReadOnlyList<Card> cardList, Card cardIn)
        {
			if (cardIn == null)
			{
				throw new ArgumentNullException("cardIn parameter is null");
			}

            foreach (Card card in cardList)
            {
				if (card == null) continue;
                if (card.Suit == cardIn.Suit && card.Value == cardIn.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public static Card ToCardFromEnums(CardValue value, CardSuit suit)
        {
            string cardString = string.Empty;
            if (value == CardValue.Ace)
            {
                cardString += "A";
            }
            else if (value == CardValue.Eight)
            {
                cardString += "8";
            }
            else if (value == CardValue.Five)
            {
                cardString += "5";
            }
			else if (value == CardValue.Four)
            {
                cardString += "4";
            }
			else if (value == CardValue.Jack)
            {
                cardString += "J";
            }
			else if (value == CardValue.King)
            {
                cardString += "K";
            }
			else if (value == CardValue.Nine)
            {
                cardString += "9";
            }
			else if (value == CardValue.None)
            {
            }
			else if (value == CardValue.Queen)
            {
                cardString += "Q";
            }
			else if (value == CardValue.Seven)
            {
                cardString += "7";
            }
			else if (value == CardValue.Six)
            {
                cardString += "6";
            }
			else if (value == CardValue.Ten)
            {
                cardString += "T";
            }
			else if (value == CardValue.Three)
            {
                cardString += "3";
            }
			else if (value == CardValue.Two)
            {
                cardString += "2";
            }
            else
            {
                throw new ArgumentException("Bad card value recieved");
            }

			if (suit == CardSuit.Club)
            {
                cardString += "c";
            }
			else if (suit == CardSuit.Diamond)
            {
                cardString += "d";
            }
			else if (suit == CardSuit.Heart)
            {
                cardString += "h";
            }
			else if (suit == CardSuit.Spade)
            {
                cardString += "s";
            }
			else if (suit == CardSuit.None)
            {
            }
            else
            {
                throw new ArgumentException("Bad suit recieved");
            }

            return new Card(cardString);
        }

        public static string ToStringFromCardList(List<Card> cardList)
        {
            string output = string.Empty;
            foreach (Card card in cardList)
            {
                output += card.ToString();
            }

            return output;
        }

		public static List<Card> ToCardListFromString(string cardListString)
		{
			List<Card> resultList = new List<Card>();
			for (int i = 0; i < cardListString.Length - 1; i += 2)
			{
				string cardString = cardListString[i].ToString() + cardListString[i + 1].ToString();
				resultList.Add(new Card(cardString));
			}

			return resultList;
		}

		public static long ToLongHashFromCardList(List<Card> cardList)
		{
			// Generalize by value order and suit number to reduce search space
			List<Card> generalizedList = new List<Card>();
			generalizedList.AddRange(cardList);

			generalizedList = Card.SortCardListByValue(generalizedList);

			long result = 0;

			for (int i = 0; i < cardList.Count; ++i)
			{
				result *= 53;
				result += CardStringsList.IndexOf(cardList[i].ToString()) + 1;
			}

			return result;
		}

		public static List<Card> ToCardListFromLongHash(long hash)
		{
			List<Card> result = new List<Card>();
			while (hash > 0)
			{
				int index = (int)(hash % 53);
				hash -= index;
				hash /= 53;
				result.Add(new Card(CardStrings[index - 1]));
			}

			return result;
		}

		/// <summary>
		/// This method should sort the given card list by value.  Value = None is the lowest
		/// Ace is the highest.  Ties dont matter.
		/// </summary>
		/// <param name="unsortedList">An unsorted card list</param>
		/// <returns>A sorted card list</returns>
		public static List<Card> SortCardListByValue(List<Card> unsortedList)
        {
			List<Card> unsortedCopy = new List<Card>();
			unsortedCopy.AddRange(unsortedList);

            List<Card> resultList = new List<Card>();

			for (int i = 0; i < unsortedCopy.Count; )
            {
                // Find the highest card
				Card highestCard = unsortedCopy[0];
				foreach (Card card in unsortedCopy)
                {
                    if (card.Value > highestCard.Value)
                    {
                        highestCard = card;
                    }
                    else if (card.Value == highestCard.Value && card.Suit > highestCard.Suit)
                    {
                        highestCard = card;
                    }
                }

                // Remove it from unsorted list, add it to sorted list
				unsortedCopy.Remove(highestCard);
                resultList.Add(highestCard);
            }

            return resultList;
        }

		/// <summary>
		/// Given a string representing a list of cards, will 
		/// generalize the suits in the string.
		/// </summary>
		/// <param name="cardListString"></param>
		/// <returns></returns>
		public static string GeneralizeSuitsInString(string cardListString)
		{
			// Walk through each character in the string
			bool xTaken = false;
			bool yTaken = false;
			bool zTaken = false;
			bool wTaken = false;

			for (int i = 0; i < cardListString.Length; ++i)
			{
				if (cardListString[i] == 'x') xTaken = true;
				else if (cardListString[i] == 'y') yTaken = true;
				else if (cardListString[i] == 'z') zTaken = true;
				else if (cardListString[i] == 'w') wTaken = true;
				else if (cardListString[i] == 'c' ||
					cardListString[i] == 's' ||
					cardListString[i] == 'h' ||
					cardListString[i] == 'd')
				{
					if (!xTaken) cardListString = cardListString.Replace(cardListString[i], 'x');
					else if (!yTaken) cardListString = cardListString.Replace(cardListString[i], 'y');
					else if (!zTaken) cardListString = cardListString.Replace(cardListString[i], 'z');
					else if (!wTaken) cardListString = cardListString.Replace(cardListString[i], 'w');
					else throw new InvalidOperationException("Replacement of too many characters");
					return GeneralizeSuitsInString(cardListString);
				}
			}

			// Replace back with normal characters
			for (int i = 0; i < cardListString.Length; ++i)
			{
				cardListString = cardListString.Replace('x', 'c');
				cardListString = cardListString.Replace('y', 's');
				cardListString = cardListString.Replace('z', 'h');
				cardListString = cardListString.Replace('w', 'd');
			}

			return cardListString;
		}

		public override bool Equals(object obj)
		{
			Card card = obj as Card;
			if (card != null)
			{
				return this == card;
			}

			return false;
		}

		/// <summary>
		/// An override for the GetHashCode method
		/// </summary>
		/// <returns>A hash code for this object</returns>
		public override int GetHashCode()
		{
			return CardStringsList.IndexOf(this.ToString()) + 1;
		}

		public static bool operator ==(Card card1, Card card2)
		{
			if ((object)card1 == null || (object)card2 == null)
			{
				if ((object)card1 == (object)card2)
				{
					return true;
				}

				return false;
			}

			if (card1.value == card2.value &&
				card1.suit == card2.suit)
			{
				return true;
			}

			return false;
		}

		public static bool operator !=(Card card1, Card card2)
		{
			return !(card1 == card2);
		}

		public override string ToString()
        {
            string output = string.Empty;

			if (this.Value == CardValue.Ace)
            {
                output += "A";
            }
			else if (this.Value == CardValue.Eight)
            {
                output += "8";
            }
			else if (this.Value == CardValue.Five)
            {
                output += "5";
            }
			else if (this.Value == CardValue.Four)
            {
                output += "4";
            }
			else if (this.Value == CardValue.Jack)
            {
                output += "J";
            }
			else if (this.Value == CardValue.King)
            {
                output += "K";
            }
			else if (this.Value == CardValue.Nine)
            {
                output += "9";
            }
			else if (this.Value == CardValue.Queen)
            {
                output += "Q";
            }
			else if (this.Value == CardValue.Seven)
            {
                output += "7";
            }
			else if (this.Value == CardValue.Six)
            {
                output += "6";
            }
			else if (this.Value == CardValue.Ten)
            {
                output += "T";
            }
			else if (this.Value == CardValue.Three)
            {
                output += "3";
            }
			else if (this.Value == CardValue.Two)
            {
                output += "2";
            }
            else
            {
                throw new ArgumentException("Unexpected card value recieved");
            }

			if (this.Suit == CardSuit.Club)
            {
                output += "c";
            }
			else if (this.Suit == CardSuit.Diamond)
            {
                output += "d";
            }
			else if (this.Suit == CardSuit.Heart)
            {
                output += "h";
            }
			else if (this.Suit == CardSuit.Spade)
            {
                output += "s";
            }
            else
            {
                throw new ArgumentException("Unexpected card suit recieved");
            }

            return output;
        }

		internal static bool AreHandsStatisticallyEqual(List<Card> communityCards, Tuple<Card, Card> hand1, Tuple<Card, Card> hand2)
		{
			// Only works if all three parameters are raw or previously generalized all at the same time
			List<Card> sortedCommunityCardList = new List<Card>();
			sortedCommunityCardList.AddRange(communityCards);
			sortedCommunityCardList = Card.SortCardListByValue(sortedCommunityCardList);

			List<Card> sortedHand1 = new List<Card>();
			sortedHand1.Add(hand1.Item1);
			sortedHand1.Add(hand1.Item2);
			sortedHand1 = Card.SortCardListByValue(sortedHand1);

			List<Card> sortedHand2 = new List<Card>();
			sortedHand2.Add(hand2.Item1);
			sortedHand2.Add(hand2.Item2);
			sortedHand2 = Card.SortCardListByValue(sortedHand2);

			// If values aren't equal at this point, the hands are not equal
			if (sortedHand1[0].Value != sortedHand2[0].Value ||
				sortedHand1[1].Value != sortedHand2[1].Value)
			{
				return false;
			}

			// We know the values are the same, now lets check suits
			List<Card> generalizedSuitListHand1 = new List<Card>();
			generalizedSuitListHand1.AddRange(sortedCommunityCardList);
			generalizedSuitListHand1.AddRange(sortedHand1);
			generalizedSuitListHand1 = Card.GeneralizeSuitsInList(generalizedSuitListHand1);

			List<Card> generalizedSuitListHand2 = new List<Card>();
			generalizedSuitListHand2.AddRange(sortedCommunityCardList);
			generalizedSuitListHand2.AddRange(sortedHand2);
			generalizedSuitListHand2 = Card.GeneralizeSuitsInList(generalizedSuitListHand2);

			Tuple<Card, Card> cleanHand1 = 
				new Tuple<Card, Card>(
					generalizedSuitListHand1[generalizedSuitListHand1.Count - 2],
					generalizedSuitListHand1[generalizedSuitListHand1.Count - 1]);

			Tuple<Card, Card> cleanHand2 =
				new Tuple<Card, Card>(
					generalizedSuitListHand2[generalizedSuitListHand2.Count - 2],
					generalizedSuitListHand2[generalizedSuitListHand2.Count - 1]);

			// If the hands match suits at this point, we are done
			if (cleanHand1.Item1.suit == cleanHand2.Item1.suit &&
				cleanHand1.Item2.Suit == cleanHand2.Item2.Suit)
			{
				return true;
			}

			if (communityCards.Count == 0)
			{
				// Preflop always false here
				return false;
			}

			// Hand 1 critical suit count
			int hand1NumClubs = Card.CountSuitInList(generalizedSuitListHand1, CardSuit.Club);
			int hand1NumSpades = Card.CountSuitInList(generalizedSuitListHand1, CardSuit.Spade);
			int hand1NumHearts = Card.CountSuitInList(generalizedSuitListHand1, CardSuit.Heart);

			int hand1Suits0ToGo = 0;
			int hand1Suits1ToGo = 0;
			int hand1Suits2ToGo = 0;

			if (hand1NumClubs == 5)
			{
				hand1Suits0ToGo++;
			}
			else if (hand1NumClubs == 4)
			{
				hand1Suits1ToGo++;
			}
			else if (hand1NumClubs == 3)
			{
				hand1Suits2ToGo++;
			}

			if (hand1NumSpades == 5)
			{
				hand1Suits0ToGo++;
			}
			else if (hand1NumSpades == 4)
			{
				hand1Suits1ToGo++;
			}
			else if (hand1NumSpades == 3)
			{
				hand1Suits2ToGo++;
			}

			if (hand1NumHearts == 5)
			{
				hand1Suits0ToGo++;
			}
			else if (hand1NumHearts == 4)
			{
				hand1Suits1ToGo++;
			}
			else if (hand1NumHearts == 3)
			{
				hand1Suits2ToGo++;
			}

			// Hand two critical suit count
			int hand2NumClubs = Card.CountSuitInList(generalizedSuitListHand2, CardSuit.Club);
			int hand2NumSpades = Card.CountSuitInList(generalizedSuitListHand2, CardSuit.Spade);
			int hand2NumHearts = Card.CountSuitInList(generalizedSuitListHand2, CardSuit.Heart);

			int hand2Suits0ToGo = 0;
			int hand2Suits1ToGo = 0;
			int hand2Suits2ToGo = 0;

			if (hand2NumClubs == 5)
			{
				hand2Suits0ToGo++;
			}
			else if (hand2NumClubs == 4)
			{
				hand2Suits1ToGo++;
			}
			else if (hand2NumClubs == 3)
			{
				hand2Suits2ToGo++;
			}

			if (hand2NumSpades == 5)
			{
				hand2Suits0ToGo++;
			}
			else if (hand2NumSpades == 4)
			{
				hand2Suits1ToGo++;
			}
			else if (hand2NumSpades == 3)
			{
				hand2Suits2ToGo++;
			}

			if (hand2NumHearts == 5)
			{
				hand2Suits0ToGo++;
			}
			else if (hand2NumHearts == 4)
			{
				hand2Suits1ToGo++;
			}
			else if (hand2NumHearts == 3)
			{
				hand2Suits2ToGo++;
			}

			int numberCardsToGo = 5 - communityCards.Count;

			
			if (numberCardsToGo == 0)
			{
				// If river, only count 0 cards to go
				if (hand1Suits0ToGo == hand2Suits0ToGo)
				{
					return true;
				}
			}
			else if (numberCardsToGo == 1)
			{
				// If turn only count 0 to go and 1 to go
				if (hand1Suits0ToGo == hand2Suits0ToGo &&
					hand1Suits1ToGo == hand2Suits1ToGo)
				{
					return true;
				}
			}
			else if (numberCardsToGo == 2)
			{
				// If flop count all three
				if (hand1Suits0ToGo == hand2Suits0ToGo &&
					hand1Suits1ToGo == hand2Suits1ToGo &&
					hand1Suits2ToGo == hand2Suits2ToGo)
				{
					return true;
				}
			}


			return false;
		}

		private static int CountSuitInList(List<Card> cardList, CardSuit suit)
		{
			int result = 0;
			foreach (Card card in cardList)
			{
				if (card.Suit == suit)
				{
					result++;
				}
			}

			return result;
		}

		internal static List<Card> GeneralizeSuitsInList(List<Card> cardListIn)
		{
			string cardString = Card.ToStringFromCardList(cardListIn);
			cardString = GeneralizeSuitsInString(cardString);
			return Card.ToCardListFromString(cardString);
		}

		public static bool AreHandsEqual(Tuple<Card, Card> hand1, Tuple<Card, Card> hand2)
		{
			if (hand1.Item1.Equals(hand2.Item1) && hand1.Item2.Equals(hand2.Item2) ||
				hand1.Item2.Equals(hand2.Item1) && hand1.Item1.Equals(hand2.Item2))
			{
				return true;
			}

			return false;
		}

		private static int NumberOfSuitsInList(List<Card> cardList)
		{
			bool heartsFound = false;
			bool spadesFound = false;
			bool diamondsFound = false;
			bool clubsFound = false;
			foreach (Card card in cardList)
			{
				if (card.suit == CardSuit.Club)
				{
					clubsFound = true;
				}
				else if (card.suit == CardSuit.Diamond)
				{
					diamondsFound = true;
				}
				else if (card.suit == CardSuit.Heart)
				{
					heartsFound = true;
				}
				else if (card.suit == CardSuit.Spade)
				{
					spadesFound = true;
				}
			}

			int count = 0;
			if (heartsFound) count++;
			if (spadesFound) count++;
			if (clubsFound) count++;
			if (diamondsFound) count++;

			return count;
		}
    }
}
