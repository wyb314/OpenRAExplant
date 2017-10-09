using System;
using System.Collections.Generic;
using GameToolbox.DataStructures;

namespace GameToolbox.Algorithms
{
	/// <summary>
	/// Performs an A* search on the given graph. A* is Dijkstra's algorithm for finding the shortest path
	/// to a target node in a weighted directional graph, with the added optimization of using a heuristic
	/// function (ExpectedCost) to estimate expected cost from the current node in the search to the target
	/// node. With a heuristic function that always returns the same value, A* is the same as Dijkstra's
	/// algorithm. The better the heuristic function, the more it optimizes the search.
	/// 
	/// The worst case for A* is if the target node is not reachable from the start node (this will take
	/// maximum running time to find no result). If the target node is reachable from the start node, then
	/// the running time of A* is equal to that of Dijkstra's algorithm in the worst case (where the heuristic
	/// does not give an accurate estimate), and unless the size of the priority queue is limited, it is
	/// guaranteed (mathematically proven) to give an optimal solution.
	/// 
	/// Setting the maximum number of items allowed in the priority queue can save memory and possibly
	/// processing time, at the expense of being guaranteed the optimal shortest path to the target.
	/// 
	/// This is a class so that the dictionaries used can be instantiated once and used for multiple searches,
	/// and to give a more convenient place to specify the various functions needed by the algorithm than the
	/// parameter list of a function.
	/// </summary>
	/// <typeparam name="TNode">The type of data stored at nodes in the graph.</typeparam>
	/// <typeparam name="TEdge">The type of data stored at edges in the graph.</typeparam>
	public class AStar<TNode, TEdge>
	{
		private Dictionary<int, IGraphEdge<TNode, TEdge>> _shortestPathTree = new Dictionary<int, IGraphEdge<TNode, TEdge>>();
		private Dictionary<int, IGraphEdge<TNode, TEdge>> _searchFrontier = new Dictionary<int, IGraphEdge<TNode, TEdge>>();
		private Dictionary<int, double> _cumulativeCosts = new Dictionary<int, double>();
		private Dictionary<int, double> _cumulativeHeuristicCosts = new Dictionary<int, double>();
		private Dictionary<int, PriorityQueue<IGraphNode<TNode, TEdge>>.Item> _priorityQueueItems = new Dictionary<int, PriorityQueue<IGraphNode<TNode, TEdge>>.Item>();
		private PriorityQueue<IGraphNode<TNode, TEdge>> _priorityQueue;

		/// <summary>
		/// Returns the cost of moving from the first node along the edge to the second node. This function is required.
		/// </summary>
		public Func<TNode, TEdge, TNode, double> Cost { get; set; }

		/// <summary>
		/// Returns the expected cost of moving from the first node to the target node, using some
		/// heuristic. This function is required.
		/// </summary>
		public Func<TNode, TNode, double> ExpectedCost { get; set; }

		/// <summary>
		/// Returns true if the first node is close enough to the target node to return. This function is optional.
		/// </summary>
		public Func<TNode, TNode, bool> CloseEnough { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public AStar()
		{
			_priorityQueue = new PriorityQueue<IGraphNode<TNode, TEdge>>((value1, value2) =>
				{
					bool value1CostExists = _cumulativeHeuristicCosts.ContainsKey(value1.ID);
					bool value2CostExists = _cumulativeHeuristicCosts.ContainsKey(value2.ID);

					if (!value1CostExists && value2CostExists)
						return -1;
					if (value1CostExists && !value2CostExists)
						return 1;
					if (_cumulativeHeuristicCosts[value1.ID] < _cumulativeHeuristicCosts[value2.ID])
						return 1;
					if (_cumulativeHeuristicCosts[value1.ID] == _cumulativeHeuristicCosts[value2.ID])
						return 0;
					return -1;
				});
		}

		/// <summary>
		/// Performs an A* search on the given graph. A* is Dijkstra's algorithm for finding the shortest path
		/// to a target node in a weighted directional graph, with the added optimization of using a heuristic
		/// function (ExpectedCost in AStarFunctions) to estimate expected cost from the current node in the
		/// search to the target node. With a heuristic function that always returns the same value, A* is the
		/// same as Dijkstra's algorithm. The better the heuristic function, the more it optimizes the search.
		/// 
		/// The worst case for A* is if the target node is not reachable from the start node (this will take
		/// maximum running time to find no result). If the target node is reachable from the start node, then
		/// the running time of A* is equal to that of Dijkstra's algorithm in the worst case (where the heuristic
		/// does not give an accurate estimate), and unless the size of the priority queue is limited, it is
		/// guaranteed (mathematically proven) to give the optimal solution.
		/// 
		/// Setting the maximum number of items allowed in the priority queue can save memory and possibly
		/// processing time, at the expense of being guaranteed the optimal shortest path to the target.
		/// </summary>
		/// <param name="startNode">The node to start searching from.</param>
		/// <param name="targetNode">The node to search for.</param>
		/// <returns>A path from the start node to the target node, comprised of all traversed edges.</returns>
		public IEnumerable<PathEdge<TNode, TEdge>> Search(IGraphNode<TNode, TEdge> startNode, IGraphNode<TNode, TEdge> targetNode)
		{
			return Search(startNode, targetNode, 0, null);
		}

		/// <summary>
		/// Performs an A* search on the given graph. A* is Dijkstra's algorithm for finding the shortest path
		/// to a target node in a weighted directional graph, with the added optimization of using a heuristic
		/// function (ExpectedCost in AStarFunctions) to estimate expected cost from the current node in the
		/// search to the target node. With a heuristic function that always returns the same value, A* is the
		/// same as Dijkstra's algorithm. The better the heuristic function, the more it optimizes the search.
		/// 
		/// The worst case for A* is if the target node is not reachable from the start node (this will take
		/// maximum running time to find no result). If the target node is reachable from the start node, then
		/// the running time of A* is equal to that of Dijkstra's algorithm in the worst case (where the heuristic
		/// does not give an accurate estimate), and unless the size of the priority queue is limited, it is
		/// guaranteed (mathematically proven) to give the optimal solution.
		/// 
		/// Setting the maximum number of items allowed in the priority queue can save memory and possibly
		/// processing time, at the expense of being guaranteed the optimal shortest path to the target.
		/// </summary>
		/// <param name="startNode">The node to start searching from.</param>
		/// <param name="targetNode">The node to search for.</param>
		/// <param name="maxPriorityQueueItems">The maximum number of items to keep in the priority queue. A value
		/// of 0 indicates no limit.</param>
		/// <returns>A path from the start node to the target node, comprised of all traversed edges.</returns>
		public IEnumerable<PathEdge<TNode, TEdge>> Search(IGraphNode<TNode, TEdge> startNode, IGraphNode<TNode, TEdge> targetNode,
			int maxPriorityQueueItems)
		{
			return Search(startNode, targetNode, maxPriorityQueueItems, null);
		}

		/// <summary>
		/// Performs an A* search on the given graph. A* is Dijkstra's algorithm for finding the shortest path
		/// to a target node in a weighted directional graph, with the added optimization of using a heuristic
		/// function (ExpectedCost in AStarFunctions) to estimate expected cost from the current node in the
		/// search to the target node. With a heuristic function that always returns the same value, A* is the
		/// same as Dijkstra's algorithm. The better the heuristic function, the more it optimizes the search.
		/// 
		/// The worst case for A* is if the target node is not reachable from the start node (this will take
		/// maximum running time to find no result). If the target node is reachable from the start node, then
		/// the running time of A* is equal to that of Dijkstra's algorithm in the worst case (where the heuristic
		/// does not give an accurate estimate), and unless the size of the priority queue is limited, it is
		/// guaranteed (mathematically proven) to give the optimal solution.
		/// 
		/// Setting the maximum number of items allowed in the priority queue can save memory and possibly
		/// processing time, at the expense of being guaranteed the optimal shortest path to the target.
		/// </summary>
		/// <param name="startNode">The node to start searching from.</param>
		/// <param name="targetNode">The node to search for.</param>
		/// <param name="bailEarly">Should return true if the search should be halted now.</param>
		/// <returns>A path from the start node to the target node, comprised of all traversed edges.</returns>
		public IEnumerable<PathEdge<TNode, TEdge>> Search(IGraphNode<TNode, TEdge> startNode, IGraphNode<TNode, TEdge> targetNode,
			Func<bool> bailEarly)
		{
			return Search(startNode, targetNode, 0, bailEarly);
		}

		/// <summary>
		/// Performs an A* search on the given graph. A* is Dijkstra's algorithm for finding the shortest path
		/// to a target node in a weighted directional graph, with the added optimization of using a heuristic
		/// function (ExpectedCost in AStarFunctions) to estimate expected cost from the current node in the
		/// search to the target node. With a heuristic function that always returns the same value, A* is the
		/// same as Dijkstra's algorithm. The better the heuristic function, the more it optimizes the search.
		/// 
		/// The worst case for A* is if the target node is not reachable from the start node (this will take
		/// maximum running time to find no result). If the target node is reachable from the start node, then
		/// the running time of A* is equal to that of Dijkstra's algorithm in the worst case (where the heuristic
		/// does not give an accurate estimate), and unless the size of the priority queue is limited, it is
		/// guaranteed (mathematically proven) to give the optimal solution.
		/// 
		/// Setting the maximum number of items allowed in the priority queue can save memory and possibly
		/// processing time, at the expense of being guaranteed the optimal shortest path to the target.
		/// </summary>
		/// <param name="startNode">The node to start searching from.</param>
		/// <param name="targetNode">The node to search for.</param>
		/// <param name="maxPriorityQueueItems">The maximum number of items to keep in the priority queue. A value
		/// of 0 indicates no limit.</param>
		/// <param name="bailEarly">Should return true if the search should be halted now.</param>
		/// <returns>A path from the start node to the target node, comprised of all traversed edges.</returns>
		public IEnumerable<PathEdge<TNode, TEdge>> Search(IGraphNode<TNode, TEdge> startNode, IGraphNode<TNode, TEdge> targetNode,
			int maxPriorityQueueItems, Func<bool> bailEarly)
		{
			if (Cost == null || ExpectedCost == null)
				yield break;
			IGraphNode<TNode, TEdge> currentNode;
			double currentCumulativeCost;
			double currentHeuristicCost;

			_shortestPathTree.Clear();
			_searchFrontier.Clear();
			_cumulativeCosts.Clear();
			_cumulativeHeuristicCosts.Clear();
			_priorityQueue.Clear();
			_priorityQueueItems.Clear();

			_priorityQueue.MaxItems = maxPriorityQueueItems;

			_cumulativeCosts[startNode.ID] = 0;
			_priorityQueueItems[startNode.ID] = _priorityQueue.Enqueue(startNode);

			while (_priorityQueue.Count > 0)
			{
				if (bailEarly != null && bailEarly())
					yield break;

				currentNode = _priorityQueue.Dequeue();

				if (currentNode.ID != startNode.ID)
					_shortestPathTree[currentNode.ID] = _searchFrontier[currentNode.ID];

				if (currentNode.ID == targetNode.ID)
					break;
				if (CloseEnough != null && CloseEnough(currentNode.Value, targetNode.Value))
				{
					targetNode = currentNode;
					break;
				}

				foreach (IGraphEdge<TNode, TEdge> edge in currentNode.EdgesOut)
				{
					if (bailEarly != null && bailEarly())
						yield break;
					currentCumulativeCost = _cumulativeCosts[currentNode.ID] + Cost(edge.From.Value, edge.Value, edge.To.Value);
					currentHeuristicCost = ExpectedCost(edge.To.Value, targetNode.Value);

					if (!_searchFrontier.ContainsKey(edge.To.ID))
					{
						_cumulativeHeuristicCosts[edge.To.ID] = currentCumulativeCost + currentHeuristicCost;
						_cumulativeCosts[edge.To.ID] = currentCumulativeCost;

						_priorityQueueItems[edge.To.ID] = _priorityQueue.Enqueue(edge.To);

						_searchFrontier[edge.To.ID] = edge;
					}
					else if ((currentCumulativeCost < _cumulativeCosts[edge.To.ID])
						&& (!_shortestPathTree.ContainsKey(edge.To.ID)))
					{
						_cumulativeHeuristicCosts[edge.To.ID] = currentCumulativeCost + currentHeuristicCost;
						_cumulativeCosts[edge.To.ID] = currentCumulativeCost;

						_priorityQueue.RecalculatePriority(_priorityQueueItems[edge.To.ID]);

						_searchFrontier[edge.To.ID] = edge;
					}
				}
			}

			var path = PathToTarget(startNode, targetNode);

			for (int i = path.Count - 1; i >= 0; i--)
			{
				yield return new PathEdge<TNode, TEdge> { Value = path[i].Value, FromNode = path[i].From.Value, ToNode = path[i].To.Value };
			}

			yield break;
		}

		/// <summary>
		/// Assembles a Path generic object with the node and edge values traversed, using the shortest path
		/// tree generated by the A* algorithm.
		/// </summary>
		/// <typeparam name="TNode">The type of data stored at nodes in the graph.</typeparam>
		/// <typeparam name="TEdge">The type of data stored at edges in the graph.</typeparam>
		/// <param name="graph">The graph.</param>
		/// <param name="startNodeID">The ID of the start node in the graph.</param>
		/// <param name="targetNodeID">The ID of the target node in the graph.</param>
		/// <param name="shortestPathTree">The shortest path tree generated by the A* algorithm.</param>
		/// <returns>A Path data structure containing the values traversed in the shortest path.</returns>
		private List<IGraphEdge<TNode, TEdge>> PathToTarget(IGraphNode<TNode, TEdge> startNode, IGraphNode<TNode, TEdge> targetNode)
		{
			var path = new List<IGraphEdge<TNode, TEdge>>();

			if (_shortestPathTree.Count == 0)
				return path;

			IGraphNode<TNode, TEdge> currentNode = targetNode;

			//go backwards from the target node to the start node in order to assemble the path
			while (currentNode != startNode)
			{
				if (!_shortestPathTree.ContainsKey(currentNode.ID))
					return new List<IGraphEdge<TNode, TEdge>>();
				path.Add(_shortestPathTree[currentNode.ID]);
				currentNode = _shortestPathTree[currentNode.ID].From;
			}

			return path;
		}
	}

	[Serializable]
	public class PathEdge<TNode, TEdge>
	{
		public TEdge Value { get; set; }
		public TNode FromNode { get; set; }
		public TNode ToNode { get; set; }
	}
}
