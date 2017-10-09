using System;
using System.Collections.Generic;

namespace GameToolbox.Planner
{
	[Serializable]
	public class PlannerState : ICollection<IPlannerStateSymbol>
	{
		private SortedDictionary<string, IPlannerStateSymbol> _state = new SortedDictionary<string, IPlannerStateSymbol>();

		public IPlannerStateSymbol this[string name]
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
		public bool Contains(IPlannerStateSymbol symbol)
		{
			return _state.ContainsKey(symbol.Name) && (_state[symbol.Name].Value == symbol.Value);
		}

		/// <summary>
		/// Returns the number of symbols in this state which either do not exist or have different values in
		/// the other state.
		/// </summary>
		/// <param name="otherState">The state to determine the distance to.</param>
		/// <returns>An int representing the number of states different.</returns>
		public double DistanceFrom(PlannerState otherState)
		{
			double dist = 0;
			foreach (var symbol in otherState)
			{
				if (!_state.ContainsKey(symbol.Name))
					dist += 1;
				else
					dist += symbol.DistanceFrom(_state[symbol.Name].ToStateSymbolCondition());
			}
			return dist;
		}

		/// <summary>
		/// Returns the number of symbols in this state which do not meet the conditions in the state condition.
		/// </summary>
		/// <param name="condition">The condition to check whether this state meets.</param>
		/// <returns>An int representing the number of conditions not met.</returns>
		public double Distance(PlannerStateCondition condition)
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
		/// Returns a list of the names of the symbols in this state which either do not exist or have different
		/// values in the other state.
		/// </summary>
		/// <param name="otherState">The state to find the difference between.</param>
		/// <returns>An IEnumerable of the names of the symbols which are different.</returns>
		public IEnumerable<string> UnmetConditions(PlannerState otherState)
		{
			foreach (var symbol in otherState)
			{
				if (!_state.ContainsKey(symbol.Name))
					yield return symbol.Name;
				else if (!symbol.Equals(_state[symbol.Name]))
					yield return symbol.Name;
			}
			yield break;
		}

		/// <summary>
		/// Returns a list of the names of symbols in this state which do not meet the condition.
		/// </summary>
		/// <param name="condition">The state condition to compare against.</param>
		/// <returns>An IEnumerable of the names of the symbols which are different.</returns>
		public IEnumerable<string> UnmetConditions(PlannerStateCondition condition)
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
			if (obj is PlannerState)
			{
				if (DistanceFrom((PlannerState)obj) == 0)
					return true;
			}
			return false;
		}

		public bool Meets(PlannerStateCondition condition)
		{
			if (Distance(condition) == 0)
				return true;
			return false;
		}

		/// <summary>
		/// Compiler was complaining that Equals was overridden but not GetHashCode.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Returns a planner state containing all symbols from the left state, plus all symbols from the right which are not in the left.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static PlannerState operator +(PlannerState left, PlannerState right)
		{
			PlannerState result = new PlannerState();

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

		public static implicit operator PlannerStateCondition(PlannerState state)
		{
			var res = new PlannerStateCondition();
			foreach (var symbol in state)
				res.Add(symbol.ToStateSymbolCondition());
			return res;
		}

		public PlannerState Clone()
		{
			var clone = new PlannerState();
			foreach (var symbol in _state)
				clone.Add(symbol.Value.Clone());
			return clone;
		}

		#region ICollection<IPlannerStateSymbol> Members

		public void Add(IPlannerStateSymbol item)
		{
			_state[item.Name] = item;
		}

		public void Clear()
		{
			_state.Clear();
		}

		public void CopyTo(IPlannerStateSymbol[] array, int arrayIndex)
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

		public bool Remove(IPlannerStateSymbol item)
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

		public IEnumerator<IPlannerStateSymbol> GetEnumerator()
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
