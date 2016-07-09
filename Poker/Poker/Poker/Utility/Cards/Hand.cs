namespace Poker.Utility.Cards
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class Hand
	{
		private Card card1;
		private Card card2;

		public Hand(Card card1In, Card card2In)
		{
			this.card1 = card1In;
			this.card2 = card2In;
		}

		public Card Card1 { get { return card1; } }
		public Card Card2 { get { return card2; } }

		public bool IsPocketPair
		{
			get
			{
				return card1.Value == card2.Value;
			}
		}

		public bool IsSuited
		{
			get
			{
				return card1.Suit == card2.Suit;
			}
		}

		public Card HigherCard
		{
			get
			{
				if (this.card1.Value > this.card2.Value)
				{
					return this.card1;
				}

				return this.card2;
			}
		}

		public Card LowerCard
		{
			get
			{
				if (this.card1.Value < this.card2.Value)
				{
					return this.card1;
				}

				return this.card2;
			}
		}

		public static bool operator == (Hand lhs, Hand rhs)
		{
			if ((object)lhs == null && (object)rhs == null)
			{
				// Both null
				return true;
			}
			
			if ((object)lhs == null || (object)rhs == null)
			{
				// One is null
				return false;
			}

			if (lhs.HigherCard == rhs.HigherCard && lhs.LowerCard == rhs.LowerCard)
			{
				// Cards are equal
				return true;
			}

			// Otherwise no
			return false;
		}

		public static bool operator != (Hand lhs, Hand rhs)
		{
			return !(lhs == rhs);
		}

		/// <summary>
		/// Based on first two suits in Card.CardStrings array.
		/// </summary>
		/// <param name="handIn"></param>
		/// <returns></returns>
		public static Hand ToSuitAgnosicHand(Hand handIn)
		{
			Card resultCard1;
			Card resultCard2;
			if (handIn.IsSuited)
			{
				resultCard1 = Card.ToCardFromEnums(handIn.card1.Value, CardSuit.Club);
				resultCard2 = Card.ToCardFromEnums(handIn.card2.Value, CardSuit.Club);
			}
			else
			{
				resultCard1 = Card.ToCardFromEnums(handIn.HigherCard.Value, CardSuit.Club);
				resultCard2 = Card.ToCardFromEnums(handIn.LowerCard.Value, CardSuit.Spade);
			}

			return new Hand(resultCard1, resultCard2);
		}

		public override bool Equals(object obj)
		{
			Hand hand = obj as Hand;
			if (hand == null)
			{
				return false;
			}

			return this == hand;
		}

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

		public override string ToString()
		{
			return card1.ToString() + card2.ToString();
		}
	}
}
