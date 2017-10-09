using System;
using System.Collections.Generic;
using System.Linq;

namespace GameToolbox.Planner
{
	/// <summary>
	/// Represents a set of PlannerActions.
	/// </summary>
	[Serializable]
	public class PlannerActionSet : IEnumerable<PlannerAction>
	{
		private SortedDictionary<string, PlannerAction> _actionsByName = new SortedDictionary<string, PlannerAction>();
		private Dictionary<string, List<PlannerAction>> _actionsByAffectedSymbol = new Dictionary<string, List<PlannerAction>>();

		/// <summary>
		/// Constructor.
		/// </summary>
		public PlannerActionSet() { }

		/// <summary>
		/// Constructor. Initializes set to contain the given actions.
		/// </summary>
		/// <param name="actions"></param>
		public PlannerActionSet(params PlannerAction[] actions)
		{
			Add(actions);
		}

		/// <summary>
		/// Constructor. Initializes set to contain the given actions.
		/// </summary>
		/// <param name="actions"></param>
		public PlannerActionSet(IEnumerable<PlannerAction> actions)
		{
			Add(actions);
		}

		/// <summary>
		/// Gets the action with the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public PlannerAction this[string name]
		{
			get
			{
				if (name == null)
					return null;
				if (_actionsByName.ContainsKey(name))
				{
					try
					{
						return _actionsByName[name];
					}
					catch
					{ }
				}
				return null;
			}
		}

		/// <summary>
		/// Gets an IEnumerable of the actions which affect the state symbol with the given name.
		/// </summary>
		/// <param name="symbol"></param>
		/// <returns></returns>
		public IEnumerable<PlannerAction> Affecting(string symbol)
		{
			if (!_actionsByAffectedSymbol.ContainsKey(symbol))
				yield break;
			foreach (PlannerAction action in _actionsByAffectedSymbol[symbol])
				yield return action;
			yield break;
		}

		public void Add(Object action)
		{
			if (!(action is PlannerAction))
				return;
			Add(action as PlannerAction);
		}

		/// <summary>
		/// Actions with the same name as previously added actions will overwrite the old ones.
		/// </summary>
		/// <param name="actions">The actions to add.</param>
		public void Add(params Object[] actions)
		{
			if (!(actions is PlannerAction[]))
				return;
			Add(actions as PlannerAction[]);
		}

		/// <summary>
		/// Actions with the same name as previously added actions will overwrite the old ones.
		/// </summary>
		/// <param name="actions">The actions to add.</param>
		public void Add(params PlannerAction[] actions)
		{
			foreach (PlannerAction action in actions)
			{
				if (_actionsByName.ContainsKey(action.Name))
					Remove(action.Name);
				_actionsByName[action.Name] = action;
				foreach (string symbol in action.AffectedSymbols)
				{
					if (!_actionsByAffectedSymbol.ContainsKey(symbol))
						_actionsByAffectedSymbol[symbol] = new List<PlannerAction>();
					_actionsByAffectedSymbol[symbol].Add(action);
				}
			}
		}

		/// <summary>
		/// Actions with the same name as previously added actions will overwrite the old ones.
		/// </summary>
		/// <param name="actions">The actions to add.</param>
		public void Add(IEnumerable<PlannerAction> actions)
		{
			Add(actions.ToArray());
		}

		/// <summary>
		/// Removes the action with the given name from the set.
		/// </summary>
		/// <param name="action"></param>
		public void Remove(string action)
		{
			PlannerAction act = _actionsByName[action];
			foreach (string symbol in act.AffectedSymbols)
			{
				_actionsByAffectedSymbol[symbol].Remove(act);
			}
			_actionsByName.Remove(action);
		}

		#region IEnumerable<PlannerAction> Members

		/// <summary>
		/// Gets an IEnumerator which iterates through the set.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<PlannerAction> GetEnumerator()
		{
			return _actionsByName.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Gets an IEnumerator which iterates through the set.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
