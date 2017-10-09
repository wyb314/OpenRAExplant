using System;
using System.Collections.Generic;
using System.Linq;
using GameToolbox.Planner;

namespace ConsoleTestApp
{
	public class GoTo : PlannerAction
	{
		public override string DisplayName
		{
			get { return "Go to... "; }
		}

		public override PlannerState SymbolicExecute(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			PlannerState newState = currentState.Clone();
			IPlannerStateSymbol newLocation = parameters.FirstOrDefault();
			if (newLocation != null)
			{
				if (newState.Contains("Location"))
					newState["Location"].Value = newLocation.Value;
				else
					newState.Add(newLocation);
			}
			return newState;
		}

		public override double Cost(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			return 0.1;
		}

		public override bool IsValid(PlannerState currentState, params IPlannerStateSymbol[] parameters)
		{
			return true;
		}

		public override void Execute(params IPlannerStateSymbol[] parameters)
		{
			Console.WriteLine("On my way...");
		}

		public override IEnumerable<string> ParameterSymbols
		{
			get
			{
				yield return "Location";
				yield break;
			}
		}
	}
}
