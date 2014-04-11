using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Extensions;

namespace Archimedes.Graph {
    public class DirectedGraph<TNode, TEdge> : IDirectedGraph<TNode, TEdge> where TNode : INode where TEdge : IDirectedEdge<TNode> {
        private readonly Dictionary<string, TNode> _nodes;
        private readonly Dictionary<string, TEdge> _edges;
        private readonly Dictionary<string, ICollection<string>> _adjacent; 

        public ICollection<TNode> Nodes {
            get { return _nodes.Values; }
        }

        public int NodeCount {
            get { return _nodes.Count; }
        }

        public ICollection<TEdge> Edges {
            get { return _edges.Values; }
        }

        public int EdgeCount {
            get { return _edges.Count; }
        }

        public DirectedGraph() {
            _nodes = new Dictionary<string, TNode>();
            _edges = new Dictionary<string, TEdge>();
            _adjacent = new Dictionary<string, ICollection<string>>();
        }

        public TNode this[string node] {
            get { return _nodes[node]; }
        }

        public TEdge this[TNode from, TNode to] {
            get { return _edges[EdgeKey(from, to)]; }
        } 

        public void AddNode(TNode node) {
            _nodes.Add(node.Id, node);
        }

        public void AddEdge(TEdge edge) {
            _edges.Add(EdgeKey(edge), edge);
        }

        public void RemoveNode(TNode node) {
            foreach (var adjacent in Adjacent(node)) {
                RemoveEdge(this[node, adjacent]);
            }
            _nodes.Remove(node.Id);
        }

        public void RemoveEdge(TEdge edge) {
            _edges.Remove(EdgeKey(edge));
        }

        public IEnumerable<TNode> Adjacent(INode fromNode) {
            if(fromNode == null) yield break;
            var adjacentNodes = _adjacent.GetOrDefault(fromNode.ToString(), null);
            if(adjacentNodes == null) yield break;
            foreach (var node in adjacentNodes) {
                yield return _nodes[node];
            }
        }

        private static string EdgeKey(TEdge edge) {
            return EdgeKey(edge.To, edge.From);
        }

        private static string EdgeKey(TNode from, TNode to) {
            return from.Id + " -> " + to.Id;
        }
    }
}
