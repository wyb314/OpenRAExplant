namespace GameToolbox.DataStructures
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
