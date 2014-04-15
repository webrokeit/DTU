using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Graph {
	public interface INamedDirectedGraph<TNode, TEdge> : IDirectedGraph<TNode, TEdge>, INamedGraph<TNode, TEdge> where TNode : INode where TEdge : IDirectedEdge<TNode>, INamed {
		new INamedDirectedGraph<TNode, TEdge> AddNode(TNode node);
		new INamedDirectedGraph<TNode, TEdge> AddEdge(TEdge edge);

		new INamedDirectedGraph<TNode, TEdge> RemoveNode(TNode node);
		new INamedDirectedGraph<TNode, TEdge> RemoveEdge(TEdge edge);
	}
}
