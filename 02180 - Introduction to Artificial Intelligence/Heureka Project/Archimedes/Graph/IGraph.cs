using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Graph {
    public interface IGraph<TNode, TEdge> where TNode : INode where TEdge : IEdge<TNode> {
        ICollection<TNode> Nodes { get; }
        int NodeCount { get; }

        ICollection<TEdge> Edges { get; }
        int EdgeCount { get; }

        TNode this[string nodeId] { get; }
        TEdge this[TNode node1, TNode node2] { get; }

		IGraph<TNode, TEdge> AddNode(TNode node);
		IGraph<TNode, TEdge> AddEdge(TEdge edge);

		IGraph<TNode, TEdge> RemoveNode(TNode node);
		IGraph<TNode, TEdge> RemoveEdge(TEdge edge);

        IEnumerable<TNode> Adjacent(INode node);
    }
}
