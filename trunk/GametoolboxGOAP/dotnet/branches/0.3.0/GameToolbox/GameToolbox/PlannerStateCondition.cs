using System;
using System.Collections.Generic;
using System.Text;

namespace GameToolbox.Planner
{
	[Serializable]
	public class PlannerStateCondition : ICollection<IPlannerStateSymbolCondition>
	{
		private SortedDictionary<string, IPlannerStateSymbolCondition> _state = new SortedDictionary<string, IPlannerStateSymbolCondition>();

		/// <summary>
		/// Returns the number of symbol conditions in this state which do not meet the conditions in the given state condition.
		/// </summary>
		/// <param name="condition">The condition to check whether this state meets.</param>
		/// <returns>An int representing the number of conditions not met.</returns>
		internal double DistanceFrom(PlannerStateCondition condition)
		{
			double dist = 0;
			foreach (var symbolCondition in condition)
			{
				if (!_state.ContainsKey(symbolCondition.Name))
					dist += 1;
				else
					dist += _state[symbolCondition.Name].DistanceFrom(symbolCondition);
			}
			return dist;
		}

		/// <summary>
		/// Returns a list of the names of symbols in the given state condition which are not met by this state condition.
		/// </summary>
		/// <param name="condition">The state condition to compare against.</param>
		/// <returns>An IEnumerable of the names of the symbols which are different.</returns>
		internal IEnumerable<string> UnmetConditions(PlannerStateCondition condition)
		{
			foreach (var symbolCondition in condition)
			{
				if (!_state.ContainsKey(symbolCondition.Name))
					yield return symbolCondition.Name;
				else if (!_state[symbolCondition.Name].Meets(symbolCondition))
					yield return symbolCondition.Name;
			}
			yield break;
		}

		/// <summary>
		/// Returns true if this state is at least a subset of the state it is being compared to.
		/// </summary>
		/// <param name="obj">The state to compare to.</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is PlannerStateCondition)
			{
				if (DistanceFrom((PlannerStateCondition)obj) == 0)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Returns a planner state condition containing all symbol conditions from the left state,
		/// plus all symbol conditions from the right which are not in the left.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static PlannerStateCondition operator +(PlannerStateCondition left, PlannerStateCondition right)
		{
			PlannerStateCondition result = new PlannerStateCondition();

			foreach (var symbol in right)
			{
				if (!left.Contains(symbol.Name))
					result.Add(symbol);
			}
			foreach (var symbol in left)
			{
				result.Add(symbol);
			}

			return result;
		}

		/// <summary>
		/// Converts this PlannerStateCondition to a PlannerState. This loses the information of the comparisons
		/// in the symbol conditions, effectively changing all of them to ComparisonOperator.EqualTo.
		/// </summary>
		/// <param name="condition"></param>
		/// <returns></returns>
		public static explicit operator PlannerState(PlannerStateCondition condition)
		{
			var res = new PlannerState();
			foreach (var symbol in condition)
			{
				res.Add(symbol.ToStateSymbol());
			}
			return res;
		}

		public PlannerStateCondition Clone()
		{
			var clone = new PlannerStateCondition();
			foreach (var symbol in _state)
				clone.Add(symbol.Value.Clone());
			return clone;
		}

		public IPlannerStateSymbolCondition this[string name]
		{
			get
			{
				return _state[name];
			}
		}

		/// <summary>
		/// Checks if this state contains a symbol with the given symbol name.
		/// </summary>
		/// <param name="symbol">The name of the symbol to find.</param>
		/// <returns>True if a matching symbol exists in this state, otherwise false.</returns>
		public bool Contains(string symbol)
		{
			return _state.ContainsKey(symbol);
		}

		/// <summary>
		/// Checks if this state contains a symbol with the same name and value as the given symbol.
		/// </summary>
		/// <param name="symbol">The symbol to look for.</param>
		/// <returns>True if a matching symbol exists in this state, otherwise false.</returns>
		public bool Contains(IPlannerStateSymbolCondition symbol)
		{
			return _state.ContainsKey(symbol.Name) && (_state[symbol.Name].Value == symbol.Value);
		}

		/// <summary>
		/// Compiler was complaining that Equals was overridden but not GetHashCode.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			StringBuilder builder = new StringBuilder();
			foreach (var condition in this)
			{
				builder.Append(condition.Name);
				builder.Append(condition.Comparison.ToString());
				builder.Append(condition.Value.ToString());
			}
			return builder.ToString().GetHashCode();
		}

		#region ICollection<IPlannerStateConditionalSymbol> Members

		public void Add(IPlannerStateSymbolCondition item)
		{
			_state[item.Name] = item;
		}

		public void Clear()
		{
			_state.Clear();
		}

		public void CopyTo(IPlannerStateSymbolCondition[] array, int arrayIndex)
		{
			if (array == null)
				throw new ArgumentNullException();
			if (arrayIndex < 0)
				throw new ArgumentOutOfRangeException();
			if (array.Rank > 1 || arrayIndex >= array.Length || arrayIndex > array.Length - _state.Count)
				throw new ArgumentException();
			foreach (var symbol in _state.Values)
			{
				array[arrayIndex] = symbol;
				arrayIndex++;
			}
		}

		public int Count
		{
			get { return _state.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(IPlannerStateSymbolCondition item)
		{
			if (_state.ContainsKey(item.Name))
			{
				try
				{
					_state.Remove(item.Name);
					return true;
				}
				catch
				{
					return false;
				}
			}
			return false;
		}

		#endregion

		#region IEnumerable<IPlannerStateSymbol> Members

		public IEnumerator<IPlannerStateSymbolCondition> GetEnumerator()
		{
			return _state.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
