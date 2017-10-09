using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleTestApp
{
	public enum TarotCardType { MinorArcana, MajorArcana };
	public enum MinorArcanaSuit { Wands, Cups, Pentacles, Swords };
	public enum MinorArcanaRank
	{
		Ace = 1, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10,
		Page = 11, Knight = 12, Queen = 13, King = 14
	};
	public enum MajorArcanaValue
	{
		TheFool = 0, TheMagician = 1, TheHighPriestess = 2, TheEmpress = 3, TheEmperor = 4, TheHierophant = 5, TheLovers = 6,
		TheChariot = 7, Strength = 8, TheHermit = 9, WheelOfFortune = 10, Justice = 11, TheHangingMan = 12, Death = 13, Temperance = 14,
		TheDevil = 15, TheTower = 16, TheStar = 17, TheMoon = 18, TheSun = 19, Judgement = 20, TheWorld = 21
	};

	public class TarotCard
	{
		private TarotCardValue _value;

		public TarotCardType CardType { get; private set; }
		public TarotCardValue Value
		{
			get { return _value; }
			set
			{
				if (value is MinorArcana)
					CardType = TarotCardType.MinorArcana;
				else
					CardType = TarotCardType.MajorArcana;
				_value = value;
			}
		}
	}

	public abstract class TarotCardValue
	{
	}

	public class MinorArcana : TarotCardValue
	{
		public MinorArcanaSuit Suit { get; set; }
		public MinorArcanaRank Rank { get; set; }
	}

	public class MajorArcana : TarotCardValue
	{
		public MajorArcanaValue Value { get; set; }
	}
}
