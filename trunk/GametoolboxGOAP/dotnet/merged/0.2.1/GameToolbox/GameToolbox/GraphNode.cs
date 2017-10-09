using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameToolbox
{
	/// <summary>
	/// Represents a node in a graph.
	/// </summary>
	[Serializable]
	public class GraphNode<TNode, TEdge> : IGraphNode<TNode, TEdge>
	{
		public int ID { get; internal set; }
		public TNode Value { get; set; }
		internal List<GraphEdge<TNode, TEdge>> _edgesIn = new List<GraphEdge<TNode, TEdge>>();
		public IEnumerable<GraphEdge<TNode, TEdge>> EdgesIn { get { return _edgesIn; } }
		internal List<GraphEdge<TNode, TEdge>> _edgesOut = new List<GraphEdge<TNode, TEdge>>();
		public IEnumerable<GraphEdge<TNode, TEdge>> EdgesOut { get { return _edgesOut; } }

		#region IGraphNode<TNode,TEdge> Members

		IEnumerable<IGraphEdge<TNode, TEdge>> IGraphNode<TNode, TEdge>.EdgesIn
		{
			get
			{
				foreach (GraphEdge<TNode, TEdge> edge in _edgesIn)
				{
					yield return edge;
				}

				yield break;
			}
		}

		IEnumerable<IGraphEdge<TNode, TEdge>> IGraphNode<TNode, TEdge>.EdgesOut
		{
			get
			{
				foreach (GraphEdge<TNode, TEdge> edge in _edgesOut)
				{
					yield return edge;
				}

				yield break;
			}
		}

		#endregion
	}
}
