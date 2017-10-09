using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameToolbox
{
	public interface IPlannerStateSymbol : IEquatable<IPlannerStateSymbol>
	{
		string Name { get; set; }
		object Value { get; set; }

		bool Meets(IPlannerStateSymbolCondition condition);
		double DistanceFrom(IPlannerStateSymbolCondition condition);

		IPlannerStateSymbolCondition ToStateSymbolCondition();
		IPlannerStateSymbol Clone();
	}

	[Serializable]
	public class PlannerStateSymbol<T> : IPlannerStateSymbol
	{
		public T Value { get; set; }

		#region IPlannerStateSymbol Members

		public string Name { get; set; }

		object IPlannerStateSymbol.Value { get { return this.Value; } set { this.Value = (T)value; } }

		public bool Meets(IPlannerStateSymbolCondition condition)
		{
			if (!(condition.Value is T))
				return false;
			if (condition.Comparison == ComparisonOperator.None)
				throw new ArgumentException("Comparison is not set.");

			if (((condition.Comparison & ComparisonOperator.EqualTo) == ComparisonOperator.EqualTo)
				&& (Value.IsEqualTo((T)condition.Value)))
				return true;
			if (((condition.Comparison & ComparisonOperator.NotEqualTo) == ComparisonOperator.NotEqualTo)
				&& (!Value.IsEqualTo((T)condition.Value)))
				return true;
			if (((condition.Comparison & ComparisonOperator.GreaterThan) == ComparisonOperator.GreaterThan)
				&& (Value.IsGreaterThan((T)condition.Value)))
				return true;
			if (((condition.Comparison & ComparisonOperator.LessThan) == ComparisonOperator.LessThan)
				&& (Value.IsLessThan((T)condition.Value)))
				return true;
			return false;
		}

		public double DistanceFrom(IPlannerStateSymbolCondition condition)
		{
			if (Meets(condition))
				return 0;
			if (Extensions<T>.DistanceBetween == null)
				return 1;
			if ((condition.Comparison & ComparisonOperator.EqualTo) == ComparisonOperator.EqualTo)
				return Value.DistanceFrom((T)(condition.Value));
			if ((condition.Comparison & ComparisonOperator.NotEqualTo) == ComparisonOperator.NotEqualTo)
				return 1;
			if (Value.IsGreaterThan((T)condition.Value))
				return Value.DistanceFrom((T)(condition.Value.Decrement()));
			if (Value.IsLessThan((T)condition.Value))
				return Value.DistanceFrom((T)(condition.Value.Increment()));
			return 1;
		}

		public IPlannerStateSymbolCondition ToStateSymbolCondition()
		{
			return new PlannerStateSymbolCondition<T> { Name = Name, Value = Value, Comparison = ComparisonOperator.EqualTo };
		}

		public IPlannerStateSymbol Clone()
		{
			return new PlannerStateSymbol<T> { Name = Name, Value = Value };
		}

		#endregion

		#region IEquatable<IPlannerStateSymbol> Members

		public bool Equals(IPlannerStateSymbol other)
		{
			if (!(other.Value is T))
				return false;
			return ((Name == other.Name) && Value.IsEqualTo((T)other.Value));
		}

		#endregion
	}
}
