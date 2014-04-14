using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Graph {
    public interface IDirectedGraph<TNode, TEdge> where TNode : INode where TEdge : IDirectedEdge<TNode> {
        ICollection<TNode> Nodes { get; }
        int NodeCount { get; }

        ICollection<TEdge> Edges { get; }
        int EdgeCount { get; }

        TNode this[string nodeId] { get; }
        TEdge this[TNode to, TNode from] { get; }

		IDirectedGraph<TNode, TEdge> AddNode(TNode node);
		IDirectedGraph<TNode, TEdge> AddEdge(TEdge edge);

		IDirectedGraph<TNode, TEdge> RemoveNode(TNode node);
		IDirectedGraph<TNode, TEdge> RemoveEdge(TEdge edge);

        IEnumerable<TNode> Outgoing(INode fromNode);
		IEnumerable<TNode> Incoming(INode toNode);
    }
}
