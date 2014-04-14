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

        void AddNode(TNode node);
        void AddEdge(TEdge edge);

        void RemoveNode(TNode node);
        void RemoveEdge(TEdge edge);

        IEnumerable<TNode> Adjacent(INode fromNode);
    }
}
