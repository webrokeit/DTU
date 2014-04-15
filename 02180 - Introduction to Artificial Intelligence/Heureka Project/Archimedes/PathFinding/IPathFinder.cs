using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Graph;
using Archimedes.Heuristics;

namespace Archimedes.PathFinding {
	public interface IPathFinder<TNode, TEdge> where TNode : INode where TEdge : IDirectedEdge<TNode> {
		IHeuristic Heuristic { get; }
		IList<TNode> ShortestPath(IDirectedGraph<TNode, TEdge> graph, TNode source, TNode goal);
		//IList<TNode> ShortestPath(IDirectedGraph<TNode, TEdge> graph, TNode source, ICollection<TNode> goals);
		//IList<TNode> ShortestPath(IDirectedGraph<TNode, TEdge> graph, ICollection<TNode> sources, TNode goal);
		//IList<TNode> ShortestPath(IDirectedGraph<TNode, TEdge> graph, ICollection<TNode> sources, ICollection<TNode> goals);
	}
}
