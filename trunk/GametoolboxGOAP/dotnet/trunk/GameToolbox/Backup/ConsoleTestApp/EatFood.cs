using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameToolbox;

namespace ConsoleTestApp
{
	public class EatIceCream : PlannerAction
	{
		public override IEnumerable<string> OtherAffectedSymbols
		{
			get
			{
				yield return "Food";
				yield break;
			}
		}

		public override PlannerState SymbolicExecute(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			var newState = currentState.Clone();
			if (newState.Contains("Food"))
				((PlannerStateSymbol<Food>)newState["Food"]).Value &= ~Food.IceCream;
			else
				newState.Add(new PlannerStateSymbol<Food> { Name = "Food", Value = Food.None });
			return newState;
		}

		public override PlannerState SymbolicRevert(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			var newState = currentState.Clone();
			if (newState.Contains("Food"))
				((PlannerStateSymbol<Food>)newState["Food"]).Value |= Food.IceCream;
			else
				newState.Add(new PlannerStateSymbol<Food> { Name = "Food", Value = Food.IceCream });
			return newState;
		}

		public override string DisplayName
		{
			get { return "Eat ice cream."; }
		}

		public override double Cost(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			return 0.1;
		}

		public override bool IsValid(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			if (currentState.Contains("Food"))
				return (((PlannerStateSymbol<Food>)currentState["Food"]).Value & Food.IceCream) == Food.IceCream;
			return false;
		}

		public override void Execute(params IPlannerStateSymbol[] parameters)
		{
			Console.WriteLine("Om nom nom nom."); ;
		}
	}

	public class EatCake : PlannerAction
	{
		public override IEnumerable<string> OtherAffectedSymbols
		{
			get
			{
				yield return "Food";
				yield return "Hunger";
				yield break;
			}
		}

		public override PlannerState SymbolicExecute(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			var newState = currentState.Clone();
			if (newState.Contains("Food"))
				((PlannerStateSymbol<Food>)newState["Food"]).Value &= ~Food.Cake;
			else
				newState.Add(new PlannerStateSymbol<Food> { Name = "Food", Value = Food.None });

			if (newState.Contains("Hunger"))
				((PlannerStateSymbol<Hunger>)newState["Hunger"]).Value =
					Algorithms.Min(((PlannerStateSymbol<Hunger>)newState["Hunger"]).Value + 1, Hunger.Stuffed);
			else
				newState.Add(new PlannerStateSymbol<Hunger> { Name = "Hunger", Value = Hunger.Stuffed });
			return newState;
		}

		public override PlannerState SymbolicRevert(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			var newState = currentState.Clone();
			if (newState.Contains("Food"))
				((PlannerStateSymbol<Food>)newState["Food"]).Value |= Food.Cake;
			else
				newState.Add(new PlannerStateSymbol<Food> { Name = "Food", Value = Food.Cake });

			if (newState.Contains("Hunger"))
				((PlannerStateSymbol<Hunger>)newState["Hunger"]).Value =
					Algorithms.Max(((PlannerStateSymbol<Hunger>)newState["Hunger"]).Value - 1, Hunger.Starving);
			else
				newState.Add(new PlannerStateSymbol<Hunger> { Name = "Hunger", Value = Hunger.Starving });
			return newState;
		}

		public override string DisplayName
		{
			get { return "Eat cake."; }
		}

		public override double Cost(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			return 0.1;
		}

		public override bool IsValid(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			if(currentState.Contains("Food"))
				return (((PlannerStateSymbol<Food>)currentState["Food"]).Value & Food.Cake) == Food.Cake;
			return false;
		}

		public override void Execute(params IPlannerStateSymbol[] parameters)
		{
			Console.WriteLine("Om nom nom nom."); ;
		}
	}

	public class EatSteak : PlannerAction
	{
		public override IEnumerable<string> OtherAffectedSymbols
		{
			get
			{
				yield return "Food";
				yield return "Hunger";
				yield break;
			}
		}

		public override PlannerState SymbolicExecute(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			var newState = currentState.Clone();
			if (newState.Contains("Food"))
				((PlannerStateSymbol<Food>)newState["Food"]).Value &= ~Food.Steak;
			else
				newState.Add(new PlannerStateSymbol<Food> { Name = "Food", Value = Food.None });

			if (newState.Contains("Hunger"))
				((PlannerStateSymbol<Hunger>)newState["Hunger"]).Value =
					Algorithms.Min(((PlannerStateSymbol<Hunger>)newState["Hunger"]).Value + 2, Hunger.Stuffed);
			else
				newState.Add(new PlannerStateSymbol<Hunger> { Name = "Hunger", Value = Hunger.Stuffed });
			return newState;
		}

		public override PlannerState SymbolicRevert(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			var newState = currentState.Clone();
			if (newState.Contains("Food"))
				((PlannerStateSymbol<Food>)newState["Food"]).Value |= Food.Steak;
			else
				newState.Add(new PlannerStateSymbol<Food> { Name = "Food", Value = Food.Steak });

			if (newState.Contains("Hunger"))
				((PlannerStateSymbol<Hunger>)newState["Hunger"]).Value =
					Algorithms.Max(((PlannerStateSymbol<Hunger>)newState["Hunger"]).Value - 2, Hunger.Starving);
			else
				newState.Add(new PlannerStateSymbol<Hunger> { Name = "Hunger", Value = Hunger.Starving });
			return newState;
		}

		public override string DisplayName
		{
			get { return "Eat steak."; }
		}

		public override double Cost(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			return 0.1;
		}

		public override bool IsValid(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			if (currentState.Contains("Food"))
				return (((PlannerStateSymbol<Food>)currentState["Food"]).Value & Food.Steak) == Food.Steak;
			return false;
		}

		public override void Execute(params IPlannerStateSymbol[] parameters)
		{
			Console.WriteLine("Om nom nom nom."); ;
		}
	}

	public class EatChili : PlannerAction
	{
		public override IEnumerable<string> OtherAffectedSymbols
		{
			get
			{
				yield return "Food";
				yield return "Hunger";
				yield break;
			}
		}

		public override PlannerState SymbolicExecute(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			var newState = currentState.Clone();
			if (newState.Contains("Food"))
				((PlannerStateSymbol<Food>)newState["Food"]).Value &= ~Food.Chili;
			else
				newState.Add(new PlannerStateSymbol<Food> { Name = "Food", Value = Food.None });

			if (newState.Contains("Hunger"))
				((PlannerStateSymbol<Hunger>)newState["Hunger"]).Value =
					Algorithms.Min(((PlannerStateSymbol<Hunger>)newState["Hunger"]).Value + 1, Hunger.Stuffed);
			else
				newState.Add(new PlannerStateSymbol<Hunger> { Name = "Hunger", Value = Hunger.Stuffed });
			return newState;
		}

		public override PlannerState SymbolicRevert(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			var newState = currentState.Clone();
			if (newState.Contains("Food"))
				((PlannerStateSymbol<Food>)newState["Food"]).Value |= Food.Chili;
			else
				newState.Add(new PlannerStateSymbol<Food> { Name = "Food", Value = Food.Chili });

			if (newState.Contains("Hunger"))
				((PlannerStateSymbol<Hunger>)newState["Hunger"]).Value =
					Algorithms.Max(((PlannerStateSymbol<Hunger>)newState["Hunger"]).Value - 1, Hunger.Starving);
			else
				newState.Add(new PlannerStateSymbol<Hunger> { Name = "Hunger", Value = Hunger.Starving });
			return newState;
		}

		public override string DisplayName
		{
			get { return "Eat chili."; }
		}

		public override double Cost(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			return 0.1;
		}

		public override bool IsValid(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			if (currentState.Contains("Food"))
				return (((PlannerStateSymbol<Food>)currentState["Food"]).Value & Food.Chili) == Food.Chili;
			return false;
		}

		public override void Execute(params IPlannerStateSymbol[] parameters)
		{
			Console.WriteLine("Om nom nom nom."); ;
		}
	}

	public class EatSpaghetti : PlannerAction
	{
		public override IEnumerable<string> OtherAffectedSymbols
		{
			get
			{
				yield return "Food";
				yield return "Hunger";
				yield break;
			}
		}

		public override PlannerState SymbolicExecute(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			var newState = currentState.Clone();
			if (newState.Contains("Food"))
				((PlannerStateSymbol<Food>)newState["Food"]).Value &= ~Food.Spaghetti;
			else
				newState.Add(new PlannerStateSymbol<Food> { Name = "Food", Value = Food.None });

			if (newState.Contains("Hunger"))
				((PlannerStateSymbol<Hunger>)newState["Hunger"]).Value =
					Algorithms.Min(((PlannerStateSymbol<Hunger>)newState["Hunger"]).Value + 2, Hunger.Stuffed);
			else
				newState.Add(new PlannerStateSymbol<Hunger> { Name = "Hunger", Value = Hunger.Stuffed });
			return newState;
		}

		public override PlannerState SymbolicRevert(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			var newState = currentState.Clone();
			if (newState.Contains("Food"))
				((PlannerStateSymbol<Food>)newState["Food"]).Value |= Food.Spaghetti;
			else
				newState.Add(new PlannerStateSymbol<Food> { Name = "Food", Value = Food.Spaghetti });

			if (newState.Contains("Hunger"))
				((PlannerStateSymbol<Hunger>)newState["Hunger"]).Value =
					Algorithms.Max(((PlannerStateSymbol<Hunger>)newState["Hunger"]).Value - 2, Hunger.Starving);
			else
				newState.Add(new PlannerStateSymbol<Hunger> { Name = "Hunger", Value = Hunger.Starving });
			return newState;
		}

		public override string DisplayName
		{
			get { return "Eat spaghetti."; }
		}

		public override double Cost(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			return 0.1;
		}

		public override bool IsValid(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			if (currentState.Contains("Food"))
				return (((PlannerStateSymbol<Food>)currentState["Food"]).Value & Food.Spaghetti) == Food.Spaghetti;
			return false;
		}

		public override void Execute(params IPlannerStateSymbol[] parameters)
		{
			Console.WriteLine("Om nom nom nom."); ;
		}
	}
}
