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

        void AddNode(TNode node);
        void AddEdge(TEdge edge);

        void RemoveNode(TNode node);
        void RemoveEdge(TEdge edge);

        IEnumerable<TNode> Adjacent(INode node);
    }
}
