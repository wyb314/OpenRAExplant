using System;
using GameToolbox.Planner;

namespace ConsoleTestApp
{
	public class GetOffAss : PlannerAction
	{
		public GetOffAss()
		{
			Effects.Add(new PlannerStateSymbol<Location>
				{
					Name = "Location",
					Value = Location.Car
				});
		}

		public override string DisplayName
		{
			get { return "Get off ass."; }
		}

		public override double Cost(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			return 0.5;
		}

		public override bool IsValid(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			return true;
		}

		public override void Execute(params IPlannerStateSymbol[] parameters)
		{
			Console.WriteLine("Ugh, better get up...");
		}
	}
}
