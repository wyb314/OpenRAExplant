using System;

namespace GameToolbox.Planner
{
	[Flags]
	public enum ComparisonOperator { None = 0, LessThan = 1, EqualTo = 2, GreaterThan = 4, NotEqualTo = 5 };

	public interface IPlannerStateSymbolCondition : IEquatable<IPlannerStateSymbolCondition>
	{
		string Name { get; set; }
		ComparisonOperator Comparison { get; set; }
		object Value { get; set; }

		bool Meets(IPlannerStateSymbolCondition condition);
		double DistanceFrom(IPlannerStateSymbolCondition symbolCondition);

		IPlannerStateSymbol ToStateSymbol();
		IPlannerStateSymbolCondition Clone();
	}

	[Serializable]
	public class PlannerStateSymbolCondition<T> : IPlannerStateSymbolCondition
	{
		public T Value { get; set; }

		#region IPlannerStateConditionalSymbol Members

		public string Name { get; set; }

		public ComparisonOperator Comparison { get; set; }

		object IPlannerStateSymbolCondition.Value { get { return this.Value; } set { this.Value = (T)value; } }

		/// <summary>
		/// Verifies that values in this condition constitute a subset of the values allowed by the given condition.
		/// </summary>
		/// <param name="condition">The condition to check against.</param>
		/// <returns></returns>
		public bool Meets(IPlannerStateSymbolCondition condition)
		{
			if (!(condition.Value is T))
				return false;
			if ((Comparison == ComparisonOperator.None) || (condition.Comparison == ComparisonOperator.None))
				throw new ArgumentException("Comparison is not set.");

			var noncondition1 = ComparisonOperator.EqualTo | ComparisonOperator.NotEqualTo;
			var noncondition2 = ComparisonOperator.GreaterThan | ComparisonOperator.EqualTo | ComparisonOperator.LessThan;
			if ((condition.Comparison == noncondition1) || (condition.Comparison == noncondition2) || (condition.Comparison == (noncondition1 | noncondition2)))
				return true;

			if (Value.IsEqualTo((T)condition.Value))	//Value is equal to the value of the other condition.
			{
				if (((Comparison & ComparisonOperator.EqualTo) != ComparisonOperator.EqualTo)
					|| ((condition.Comparison & ComparisonOperator.EqualTo) != ComparisonOperator.EqualTo))
					return false;	//if either of these conditions does not include equal values, the condition isn't met
				if (((Comparison & ComparisonOperator.GreaterThan) == ComparisonOperator.GreaterThan)
					&& !(((condition.Comparison & ComparisonOperator.GreaterThan) == ComparisonOperator.GreaterThan)
					|| ((condition.Comparison & ComparisonOperator.NotEqualTo) == ComparisonOperator.NotEqualTo)))
					return false;	//any range allowed in this condition must be allowed in the other or it is not met
				if (((Comparison & ComparisonOperator.LessThan) == ComparisonOperator.LessThan)
					&& !(((condition.Comparison & ComparisonOperator.LessThan) == ComparisonOperator.LessThan)
					|| ((condition.Comparison & ComparisonOperator.NotEqualTo) == ComparisonOperator.NotEqualTo)))
					return false;
				if (((Comparison & ComparisonOperator.NotEqualTo) == ComparisonOperator.NotEqualTo)
					&& !(((condition.Comparison & ComparisonOperator.NotEqualTo) == ComparisonOperator.NotEqualTo)
					|| ((condition.Comparison & (ComparisonOperator.GreaterThan | ComparisonOperator.LessThan)) == (ComparisonOperator.GreaterThan | ComparisonOperator.LessThan))))
					return false;
				return true;
			}
			else if (condition.Comparison == ComparisonOperator.EqualTo)
				return false;
			else if (Comparison == ComparisonOperator.EqualTo)
			{	//Value is not equal to the value of the other condition, and this condition only allows for values equal to Value
				if ((condition.Comparison & ComparisonOperator.NotEqualTo) == ComparisonOperator.NotEqualTo)
					return true;	//if the other condition allows for this to be not equal to its value, the condition is met.
				if (Value.IsGreaterThan((T)condition.Value))
				{	//Value is greater than the value of the other condition
					if ((condition.Comparison & ComparisonOperator.GreaterThan) == ComparisonOperator.GreaterThan)
						return true;	//if the other condition allows that, it is met.
					return false;
				}
				else if (Value.IsLessThan((T)condition.Value))
				{	//Value is less than the value of the other condition
					if ((condition.Comparison & ComparisonOperator.LessThan) == ComparisonOperator.LessThan)
						return true;	//if the other condition allows that, it is met.
					return false;
				}
				return false; //What happened here?? This should be unreachable...
			}
			else
			{	//this Value is not equal to that of the other condition, and this Comparison is not as simple as just "EqualTo"
				if ((Comparison & ComparisonOperator.NotEqualTo) == ComparisonOperator.NotEqualTo)
					return false;	//since they're not equal, the condition can't cover all these possibilities
				else if (((Comparison & ComparisonOperator.GreaterThan) == ComparisonOperator.GreaterThan)
					&& ((Comparison & ComparisonOperator.LessThan) == ComparisonOperator.LessThan))
					return false;	//another way of specifying NotEqualTo
				else if ((Comparison & ComparisonOperator.GreaterThan) == ComparisonOperator.GreaterThan)
				{
					if ((Value.IsGreaterThan((T)condition.Value))
						&& (((condition.Comparison & ComparisonOperator.GreaterThan) == ComparisonOperator.GreaterThan)
						|| ((condition.Comparison & ComparisonOperator.NotEqualTo) == ComparisonOperator.NotEqualTo)))
						return true;
					return false;
				}
				else if ((Comparison & ComparisonOperator.LessThan) == ComparisonOperator.LessThan)
				{
					if ((Value.IsLessThan((T)condition.Value))
						&& (((condition.Comparison & ComparisonOperator.LessThan) == ComparisonOperator.LessThan)
						|| ((condition.Comparison & ComparisonOperator.NotEqualTo) == ComparisonOperator.NotEqualTo)))
						return true;
					return false;
				}
				return false;
			}
		}

		public double DistanceFrom(IPlannerStateSymbolCondition condition)
		{
			if (Meets(condition))
				return 0;
			if (Extensions<T>.DistanceBetween == null)
				return 1;
			if (Comparison == ComparisonOperator.EqualTo)
			{
				if ((condition.Comparison & ComparisonOperator.EqualTo) == ComparisonOperator.EqualTo)
					return Value.DistanceFrom((T)(condition.Value));
				if ((condition.Comparison & ComparisonOperator.NotEqualTo) == ComparisonOperator.NotEqualTo)
					return 1;
				if (Value.IsGreaterThan((T)condition.Value))
					return Value.DistanceFrom((T)(condition.Value.Decrement()));
				if (Value.IsLessThan((T)condition.Value))
					return Value.DistanceFrom((T)(condition.Value.Increment()));
			}
			return 1;
		}

		public IPlannerStateSymbol ToStateSymbol()
		{
			return new PlannerStateSymbol<T> { Name = Name, Value = Value };
		}

		public IPlannerStateSymbolCondition Clone()
		{
			return new PlannerStateSymbolCondition<T> { Name = Name, Value = Value, Comparison = Comparison };
		}

		#endregion

		#region IEquatable<IPlannerStateSymbolCondition> Members

		public bool Equals(IPlannerStateSymbolCondition other)
		{
			if (!(other.Value is T))
				return false;
			return ((Name == other.Name) && (Comparison == other.Comparison) && Value.IsEqualTo((T)other.Value));
		}

		#endregion
	}
}
