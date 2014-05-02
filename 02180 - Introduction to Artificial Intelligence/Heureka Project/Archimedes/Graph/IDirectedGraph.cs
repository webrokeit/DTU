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
        IDirectedGraph<TNode, TEdge> RemoveAllEdgesForNode(TNode node);

        bool HasNode(string nodeId);
        bool HasEdge(TNode to, TNode from);
        TNode GetNode(string nodeId);
        TEdge GetEdge(TNode to, TNode from);

        IEnumerable<TNode> Outgoing(INode fromNode);
		IEnumerable<TNode> Incoming(INode toNode);
        int OutDegree(INode node);
        int InDegree(INode node);
        IEnumerable<TNode> NodesOfDegree(int degree);
    }
}
