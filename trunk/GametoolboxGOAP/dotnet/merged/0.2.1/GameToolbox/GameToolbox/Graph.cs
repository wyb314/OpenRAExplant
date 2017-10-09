using System;
using System.Collections.Generic;

namespace GameToolbox
{
	/// <summary>
	/// Data structure representing a graph with nodes and edges.
	/// </summary>
	/// <typeparam name="TNode">The type of data stored at the graph nodes.</typeparam>
	/// <typeparam name="TEdge">The type of data stored at the graph edges.</typeparam>
	[Serializable]
	public class Graph<TNode, TEdge>
	{
		private Dictionary<int, GraphNode<TNode, TEdge>> _nodes = new Dictionary<int, GraphNode<TNode, TEdge>>();
		private int _nextID = 0;

		public int NodeCount { get { return _nodes.Count; } }
		public int EdgeCount { get; private set; }

		/// <summary>
		/// Gets the node with the given node ID.
		/// </summary>
		/// <param name="id">The id of the graph node to retrieve.</param>
		/// <returns>The graph node in the graph with the given ID, or null if there is none.</returns>
		public GraphNode<TNode, TEdge> this[int id]
		{
			get
			{
				if (!_nodes.ContainsKey(id))
					return null;
				return _nodes[id];
			}
		}

		/// <summary>
		/// Gets the nodes in the graph, in no particular order.
		/// </summary>
		public IEnumerable<GraphNode<TNode, TEdge>> Nodes
		{
			get
			{
				foreach (GraphNode<TNode, TEdge> node in _nodes.Values)
				{
					yield return node;
				}
				yield break;
			}
		}

		/// <summary>
		/// Gets the node with the given ID.
		/// </summary>
		/// <param name="id">The id of the graph node to retrieve.</param>
		/// <returns>The graph node in the graph with the given ID, or null if there is none.</returns>
		public GraphNode<TNode, TEdge> GetNode(int id)
		{
			return this[id];
		}

		/// <summary>
		/// Adds a node to the graph with the given data.
		/// </summary>
		/// <param name="value">The data for the new node.</param>
		/// <returns>The id of the graph node just added.</returns>
		public int AddNode(TNode value)
		{
			int id = _nextID;
			_nextID++;
			_nodes[id] = new GraphNode<TNode, TEdge> { Value = value, ID = id };
			return id;
		}

		/// <summary>
		/// Removes the node with the given id from the graph, if such a node exists.
		/// </summary>
		/// <param name="id">The id of the node to remove from the graph.</param>
		/// <returns>True if a node was removed from the graph, otherwise false.</returns>
		public bool RemoveNode(int id)
		{
			if (!_nodes.ContainsKey(id))
				return false;

			GraphNode<TNode, TEdge> removed = _nodes[id];

			_nodes.Remove(id);

			foreach (GraphEdge<TNode, TEdge> edge in removed.EdgesOut)
			{
				edge.To._edgesIn.Remove(edge);
				EdgeCount--;
			}

			foreach (GraphEdge<TNode, TEdge> edge in removed.EdgesIn)
			{
				edge.From._edgesOut.Remove(edge);
				EdgeCount--;
			}

			return true;
		}

		/// <summary>
		/// Adds an edge to the graph from one node to another, with the given data.
		/// </summary>
		/// <param name="fromID">The id of the node at which the new egde will originate.</param>
		/// <param name="toID">The id of the node at which the new edge will terminate.</param>
		/// <param name="value">The data that will be stored with the new edge.</param>
		/// <returns>True if the edge is added, otherwise false.</returns>
		public bool AddEdge(int fromID, int toID, TEdge value)
		{
			if (!_nodes.ContainsKey(fromID) || !_nodes.ContainsKey(toID))
				return false;

			GraphEdge<TNode, TEdge> newEdge = new GraphEdge<TNode, TEdge> { From = _nodes[fromID], To = _nodes[toID], Value = value };

			newEdge.From._edgesOut.Add(newEdge);
			newEdge.To._edgesIn.Add(newEdge);
			EdgeCount++;

			return true;
		}

		/// <summary>
		/// Removes the given edge from the graph.
		/// </summary>
		/// <param name="edge">The edge to remove.</param>
		/// <returns>True if an edge is removed, otherwise false.</returns>
		public bool RemoveEdge(GraphEdge<TNode, TEdge> edge)
		{
			if (edge.From == null || edge.To == null || !_nodes.ContainsKey(edge.From.ID) || !_nodes.ContainsKey(edge.To.ID)
				|| _nodes[edge.From.ID] != edge.From || _nodes[edge.To.ID] != edge.To)
				return false;

			edge.From._edgesOut.Remove(edge);
			edge.To._edgesIn.Remove(edge);
			EdgeCount--;

			return true;
		}
	}
}
