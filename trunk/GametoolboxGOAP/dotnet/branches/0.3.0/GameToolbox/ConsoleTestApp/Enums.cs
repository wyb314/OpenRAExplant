using System;

namespace ConsoleTestApp
{
	[Flags]
	public enum Food
	{
		None = 0,
		Cake = 0x1,
		IceCream = 0x2,
		Spaghetti = 0x4,
		Steak = 0x8,
		Chili = 0x10
	}

	public enum Hunger
	{
		Starving = 0,
		Hungry,
		Peckish,
		NotHungry,
		Stuffed
	}

	public enum Location
	{
		Couch = 0,
		LivingRoom,
		Kitchen,
		Table,
		Car,
		Restaurant
	}
}
