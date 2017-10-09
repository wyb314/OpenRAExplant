using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameToolbox
{
	/// <summary>
	/// Represents an edge in a graph.
	/// </summary>
	[Serializable]
	public class GraphEdge<TNode, TEdge> : IGraphEdge<TNode, TEdge>
	{
		public TEdge Value { get; set; }
		public GraphNode<TNode, TEdge> From { get; internal set; }
		public GraphNode<TNode, TEdge> To { get; internal set; }

		#region IGraphEdge<TNode,TEdge> Members

		IGraphNode<TNode, TEdge> IGraphEdge<TNode, TEdge>.From
		{
			get { return From; }
		}

		IGraphNode<TNode, TEdge> IGraphEdge<TNode, TEdge>.To
		{
			get { return To; }
		}

		#endregion
	}
}
