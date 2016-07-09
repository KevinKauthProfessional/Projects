using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{
	public enum PokerAction
	{
		PostSmallBlind,
		PostBigBlind,
		Check,
		Call,
		Raise,
		Fold,
		Deal
	}
}
