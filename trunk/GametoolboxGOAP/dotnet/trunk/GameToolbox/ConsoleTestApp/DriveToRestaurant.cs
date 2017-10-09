using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameToolbox;

namespace ConsoleTestApp
{
	public class DriveToRestaurant : PlannerAction
	{
		public DriveToRestaurant()
		{
			Prerequisites.Add(new PlannerStateSymbolCondition<Location>
				{
					Name = "Location",
					Value = Location.Car,
					Comparison = ComparisonOperator.EqualTo
				});

			Effects.Add(new PlannerStateSymbol<Location>
				{
					Name = "Location",
					Value = Location.Restaurant
				});
		}

		public override string DisplayName
		{
			get { return "Drive to restaurant."; }
		}

		public override double Cost(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			return 0.2;
		}

		public override bool IsValid(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			return true;
		}

		public override void Execute(params IPlannerStateSymbol[] parameters)
		{
			Console.WriteLine("Vroom vroom.");
		}
	}
}
