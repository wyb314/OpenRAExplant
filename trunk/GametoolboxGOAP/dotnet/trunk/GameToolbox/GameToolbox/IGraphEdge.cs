using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameToolbox
{
	/// <summary>
	/// Represents an edge in a graph.
	/// </summary>
	public interface IGraphEdge<TNode, TEdge>
	{
		TEdge Value { get; set; }
		IGraphNode<TNode, TEdge> From { get; }
		IGraphNode<TNode, TEdge> To { get; }
	}
}
