using System.Collections.Generic;

namespace GameToolbox.DataStructures
{
	/// <summary>
	/// Represents a node in a graph.
	/// </summary>
	public interface IGraphNode<TNode, TEdge>
	{
		int ID { get; }
		TNode Value { get; set; }
		IEnumerable<IGraphEdge<TNode, TEdge>> EdgesIn { get; }
		IEnumerable<IGraphEdge<TNode, TEdge>> EdgesOut { get; }
	}
}
