namespace Poker.Utility.HandEvaluation
{
	/// <summary>
	/// Order important for two plus two lookup algorithm.
	/// </summary>
	public enum HandCategoryEnum
	{
		PreFlop_Or_Bad,
		HighCard,
		Pair,
		TwoPair,
		ThreeOfAKind,
		Straight,
		Flush,
		FullHouse,
		FourOfAKind,
		StraightFlush
	}
}
