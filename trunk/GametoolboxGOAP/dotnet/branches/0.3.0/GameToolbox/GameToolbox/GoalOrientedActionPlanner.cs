using System;
using System.Collections.Generic;
using System.Linq;
using GameToolbox.Algorithms;
using GameToolbox.DataStructures;

namespace GameToolbox.Planner
{
	public static class GoalOrientedActionPlanner
	{
		private static AStar<PlannerStateCondition, PlannerActionInstance> _aStar = new AStar<PlannerStateCondition, PlannerActionInstance>();

		public static void Initialize()
		{
			_aStar.Cost = (state1, actionInstance, state2) =>
				{
					return actionInstance.Action.Cost((PlannerState)state2, actionInstance.Parameters.ToArray());
				};
			_aStar.ExpectedCost = (state1, state2) =>
				{
					return state1.DistanceFrom(state2);
				};
			_aStar.CloseEnough = (state1, state2) =>
				{
					return state2.Equals(state1);
				};
		}

		/// <summary>
		/// Finds a plan to get from the current state to the goal state. The plan is the series of actions to be taken.
		/// </summary>
		/// <param name="currentState">The current planner state.</param>
		/// <param name="goalState">The goal state. This can be conditional.</param>
		/// <param name="availableActions">The available actions for planning.</param>
		/// <returns>The plan, as a series of actions to be taken.</returns>
		public static IEnumerable<PlannerActionInstance> Plan(PlannerState currentState, PlannerStateCondition goalState,
			PlannerActionSet availableActions)
		{
			return Plan(currentState, goalState, availableActions, null);
		}

		/// <summary>
		/// Finds a plan to get from the current state to the goal state. The plan is the series of actions to be taken.
		/// </summary>
		/// <param name="currentState">The current planner state.</param>
		/// <param name="goalState">The goal state. This can be conditional.</param>
		/// <param name="availableActions">The available actions for planning.</param>
		/// <param name="bailEarly">A function which can cut the planning short by returning true when the algorithm should be halted.</param>
		/// <returns>The plan, as a series of actions to be taken.</returns>
		public static IEnumerable<PlannerActionInstance> Plan(PlannerState currentState, PlannerStateCondition goalState,
			PlannerActionSet availableActions, Func<bool> bailEarly)
		{
			PlannerStateCondition currentStateCondition = currentState;

			var startNode = new PlannerGraphNode
				{
					ID = 1,
					IDs = new Dictionary<PlannerStateCondition, int> { { goalState, 1 } },
					AvailableActions = availableActions,
					GoalState = currentStateCondition,
					Value = goalState
				};
			var targetNode = new PlannerGraphNode
				{
					ID = 0,
					AvailableActions = availableActions,
					GoalState = currentStateCondition,
					Value = currentStateCondition
				};

			var plan = 
				(from e in _aStar.Search(startNode, targetNode, bailEarly).Reverse()
				 select e.Value).ToList();

			return plan;
		}

		/// <summary>
		/// Special graph node which contains most of the implementation of the goal-oriented action planner.
		/// For GOAP we can't use a regular graph because of the combinatorial explosion which would be involved
		/// if we tried to fully specify it.
		/// </summary>
		private class PlannerGraphNode : IGraphNode<PlannerStateCondition, PlannerActionInstance>
		{
			private PlannerActionSet _availableActions;
			private PlannerStateCondition _goalState;
			private PlannerStateCondition _value;

			public PlannerGraphNode()
			{
				Blacklist = new HashSet<PlannerActionInstance>();
			}

			/// <summary>
			/// Dictionary of node IDs, so that if we reach a node which is the same as one already reached,
			/// we can be sure it will have the same ID. This is important for the A* algorithm.
			/// </summary>
			public Dictionary<PlannerStateCondition, int> IDs { get; set; }

			/// <summary>
			/// Set of actions which have already been taken in this plan and did not get it any closer to
			/// the goal state.
			/// </summary>
			public HashSet<PlannerActionInstance> Blacklist { get; set; }

			/// <summary>
			/// The available actions for planning.
			/// </summary>
			public PlannerActionSet AvailableActions
			{
				get { return _availableActions; }
				set
				{
					_availableActions = value;
					CheckStuffAndGenEdges();
				}
			}

			/// <summary>
			/// The plan's goal state.
			/// </summary>
			public PlannerStateCondition GoalState
			{
				get { return _goalState; }
				set
				{
					_goalState = value;
					CheckStuffAndGenEdges();
				}
			}

			/// <summary>
			/// Checks whether the given action is valid. Also returns any extra symbols which must be tacked on to
			/// the current planner state if this action is taken.
			/// </summary>
			/// <param name="action">The action to validate.</param>
			/// <param name="extraSymbols">Any extra symbols this action will add to the current planner state.</param>
			/// <returns>True if the action is valid, otherwise false.</returns>
			private bool ValidAction(PlannerActionInstance actionInstance, out PlannerStateCondition prerequisites)
			{
				prerequisites = new PlannerStateCondition();

				if (Blacklist.Contains(actionInstance))
					return false;

				PlannerStateCondition prereq, effects, temp;
				temp = actionInstance.Action.SymbolicRevert((PlannerState)Value, actionInstance.Parameters.ToArray());
				foreach (var symbol in temp)
				{
					if (Value.Contains(symbol.Name))
						symbol.Comparison = Value[symbol.Name].Comparison;
				}
				foreach (var symbol in temp.UnmetConditions(actionInstance.Action.Prerequisites))
				{
					if ((temp.Contains(symbol)) && (actionInstance.Action.Prerequisites.Contains(symbol))
						&& (!actionInstance.Action.Prerequisites[symbol].Meets(temp[symbol])))
						return false;
				}
				if (Value.DistanceFrom(temp) != 0)
					prereq = temp + actionInstance.Action.Prerequisites;
				else
					prereq = actionInstance.Action.Prerequisites;

				temp = actionInstance.Action.SymbolicExecute((PlannerState)prereq, actionInstance.Parameters.ToArray());
				foreach (var symbol in temp)
				{
					if (prereq.Contains(symbol.Name))
						symbol.Comparison = prereq[symbol.Name].Comparison;
				}
				if (prereq.DistanceFrom(temp) != 0)
					effects = temp + actionInstance.Action.Effects;
				else
					effects = actionInstance.Action.Effects;

				if (Value.DistanceFrom(prereq) == 0)
				{
					foreach (var symbol in effects)
					{
						if ((Value.Contains(symbol.Name)) && (!symbol.Meets(Value[symbol.Name])))
							return false;
						if (!prereq.Contains(symbol.Name))
							prerequisites.Add(GoalState[symbol.Name]);
					}
					if (prerequisites.Count == 0)
						return false;
				}
				else
					prerequisites = prereq;

				if (!(actionInstance.Action.IsValid((PlannerState)(prerequisites + Value), actionInstance.Parameters.ToArray())))
					return false;

				foreach (var symbol in Value.UnmetConditions(effects))
				{
					if ((Value.Contains(symbol)) && (effects.Contains(symbol)) && (!effects[symbol].Meets(Value[symbol])))
						return false;
				}

				return true;
			}

			/// <summary>
			/// Checks if there is enough information to generate the outgoing edges for this node. If so, generates them.
			/// </summary>
			private void CheckStuffAndGenEdges()
			{
				if (_availableActions != null && _goalState != null && Value != null)
					GenEdgesOut();
			}

			/// <summary>
			/// Generates the outgoing edges for this node.
			/// </summary>
			private void GenEdgesOut()
			{
				var res = new List<IGraphEdge<PlannerStateCondition, PlannerActionInstance>>();

				//From.Value is the current state in the plan
				foreach (var symbol in _goalState.UnmetConditions(Value))
				{
					foreach (var action in _availableActions.Affecting(symbol))
					{
						PlannerStateCondition prerequisites;	//filled in by ValidAction function
						PlannerActionInstance actionInstance = new PlannerActionInstance { Action = action };
						bool parameterMissing = false;
						foreach (var parameter in action.ParameterSymbols)
						{
							if (Value.Contains(parameter))
								actionInstance.Parameters.Add((IPlannerStateSymbol)Value[parameter].ToStateSymbol());
							else
							{
								parameterMissing = true;
								break;
							}
						}
						if (parameterMissing)
							continue; //if a needed parameter is not available for this action, this action is not valid
						if ((res.All((edge) => { return edge.Value.Action != action; })) && (ValidAction(actionInstance, out prerequisites)))
						{
							var newEdge = new PlannerGraphEdge
								{
									IDs = IDs,
									Blacklist = new HashSet<PlannerActionInstance>(Blacklist),
									From = this,
									Prerequisites = prerequisites,
									AvailableActions = AvailableActions,
									GoalState = GoalState,
									Value = actionInstance
								};
							var newNodeValue = prerequisites + Value;
							if (_goalState.DistanceFrom(newNodeValue) >= _goalState.DistanceFrom(Value))
								newEdge.Blacklist.Add(actionInstance);
							if (IDs.ContainsKey(newNodeValue))
								newEdge.NextID = IDs[newNodeValue];
							else
							{
								IDs[newNodeValue] = IDs.Count + 1;
								newEdge.NextID = IDs[newNodeValue];
							}
							res.Add(newEdge);
						}
					}
				}

				EdgesOut = res;
			}

			#region IGraphNode<Dictionary<string,IPlannerStateSymbol>,PlannerAction> Members

			/// <summary>
			/// This node's ID.
			/// </summary>
			public int ID { get; set; }

			/// <summary>
			/// The current node's planner state value.
			/// </summary>
			public PlannerStateCondition Value
			{
				get { return _value; }
				set
				{
					_value = value;
					CheckStuffAndGenEdges();
				}
			}

			/// <summary>
			/// Edges which point toward this node. Not implemented because it is not needed for GOAP.
			/// </summary>
			public IEnumerable<IGraphEdge<PlannerStateCondition, PlannerActionInstance>> EdgesIn
			{
				get { throw new InvalidOperationException(); }
			}

			/// <summary>
			/// The edges which point outward from this node. Generated when the necessary information is set.
			/// </summary>
			public IEnumerable<IGraphEdge<PlannerStateCondition, PlannerActionInstance>> EdgesOut { get; private set; }

			#endregion
		}

		/// <summary>
		/// Special graph edge which contains some of the implementation of the goal-oriented action planner.
		/// For GOAP we can't use a regular graph because of the combinatorial explosion which would be involved
		/// if we tried to fully specify it.
		/// </summary>
		private class PlannerGraphEdge : IGraphEdge<PlannerStateCondition, PlannerActionInstance>
		{
			private PlannerGraphNode _to;

			public Dictionary<PlannerStateCondition, int> IDs { get; set; }
			public HashSet<PlannerActionInstance> Blacklist { get; set; }
			public int NextID { get; set; }
			public PlannerActionSet AvailableActions { get; set; }
			public PlannerStateCondition GoalState { get; set; }
			public PlannerStateCondition Prerequisites { get; set; }

			#region IGraphEdge<Dictionary<string,IPlannerStateSymbol>,PlannerAction> Members

			public PlannerActionInstance Value { get; set; }

			public IGraphNode<PlannerStateCondition, PlannerActionInstance> From { get; set; }

			public IGraphNode<PlannerStateCondition, PlannerActionInstance> To
			{
				get
				{
					if ((Value == null) || (From == null))
						return null;
					if (_to == null)
					{
						_to = new PlannerGraphNode
							{
								ID = NextID,
								IDs = IDs,
								Blacklist = Blacklist,
								AvailableActions = AvailableActions,
								GoalState = GoalState,
								Value = Prerequisites + From.Value
							};
					}
					return _to;
				}
			}

			#endregion
		}
	}
}
