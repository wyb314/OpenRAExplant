using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameToolbox;

namespace ConsoleTestApp
{
	public class BuyFoodAtRestaurant : PlannerAction
	{
		public BuyFoodAtRestaurant()
		{
			Prerequisites.Add(new PlannerStateSymbolCondition<Location>
				{
					Name = "Location",
					Value = Location.Restaurant,
					Comparison = ComparisonOperator.EqualTo
				});
			Prerequisites.Add(new PlannerStateSymbolCondition<double>
				{
					Name = "Money",
					Value = 14.99,
					Comparison = ComparisonOperator.GreaterThan | ComparisonOperator.EqualTo
				});
		}

		public override string DisplayName
		{
			get { return "Buy food at restaurant."; }
		}

		public override double Cost(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			return 14.99;
		}

		public override bool IsValid(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			return true;
		}

		public override void Execute(params IPlannerStateSymbol[] parameters)
		{
			Console.WriteLine("Mmm...");
		}

		public override PlannerState SymbolicExecute(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			var newState = currentState.Clone();
			if (newState.Contains("Money"))
				((PlannerStateSymbol<double>)newState["Money"]).Value -= 14.99;
			else
				newState.Add(new PlannerStateSymbol<double> { Name = "Money", Value = 0.0 });
			if (newState.Contains("Food"))
				((PlannerStateSymbol<Food>)newState["Food"]).Value |= Food.Steak | Food.IceCream;
			else
				newState.Add(new PlannerStateSymbol<Food> { Name = "Food", Value = Food.Steak | Food.IceCream });
			return newState;
		}

		public override PlannerState SymbolicRevert(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			var newState = currentState.Clone();
			if (newState.Contains("Money"))
				((PlannerStateSymbol<double>)newState["Money"]).Value += 14.99;
			else
				newState.Add(new PlannerStateSymbol<double> { Name = "Money", Value = 14.99 });

			if (newState.Contains("Food"))
			{
				((PlannerStateSymbol<Food>)newState["Food"]).Value &= ~Food.Steak;
				((PlannerStateSymbol<Food>)newState["Food"]).Value &= ~Food.IceCream;
			}
			else
				newState.Add(new PlannerStateSymbol<Food> { Name = "Food", Value = Food.None });
			return newState;
		}

		public override IEnumerable<string> OtherAffectedSymbols
		{
			get
			{
				yield return "Money";
				yield return "Food";
				yield break;
			}
		}
	}
}
