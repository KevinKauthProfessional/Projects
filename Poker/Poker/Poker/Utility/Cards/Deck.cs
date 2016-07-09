namespace Poker.Utility.Cards
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class Deck
	{
		private const int ShuffleIterations = 10000;
		List<Card> cardList = new List<Card>();

		public Deck()
		{
			// Populate
			for (int value = 0; value < 13; ++value)
				for (int suit = 0; suit < 4; ++suit)
				{
					this.cardList.Add(Card.ToCardFromEnums((CardValue)value, (CardSuit)suit));
				}

			// Shuffle
			Random rand = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
			for (int i = 0; i < ShuffleIterations; ++i)
			{
				int toIndex = (int)Math.Floor((rand.NextDouble() * (this.cardList.Count() - 1)) + 0.5);
				int fromIndex = (int)Math.Floor((rand.NextDouble() * (this.cardList.Count() - 1)) + 0.5);

				Card toCard = this.cardList[toIndex];
				Card fromCard = this.cardList[fromIndex];

				this.cardList.RemoveAt(toIndex);
				this.cardList.Insert(toIndex, fromCard);

				this.cardList.RemoveAt(fromIndex);
				this.cardList.Insert(fromIndex, toCard);
			}
		}

		public Card NextCard
		{
			get
			{
				if (cardList.Count == 0)
				{
					throw new InvalidOperationException("Deck is empty");
				}

				Card result = cardList[0];
				this.cardList.RemoveAt(0);
				return result;
			}
		}
	}
}
