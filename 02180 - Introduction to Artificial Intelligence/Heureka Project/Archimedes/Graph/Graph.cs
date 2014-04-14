using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Extensions;

namespace Archimedes.Graph {
    public class Graph<TNode, TEdge> : IGraph<TNode, TEdge> where TNode : INode where TEdge : IEdge<TNode> {
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

        public Graph() {
            _nodes = new Dictionary<string, TNode>();
            _edges = new Dictionary<string, TEdge>();
            _adjacent = new Dictionary<string, ICollection<string>>();
        }

        public TNode this[string node] {
            get { return _nodes[node]; }
        }

        public TEdge this[TNode node1, TNode node2] {
            get { return _edges[EdgeKey(node1, node2)]; }
        }

		public IGraph<TNode, TEdge> AddNode(TNode node) {
			_nodes.Add(node.Id, node);
			return this;
        }

		public IGraph<TNode, TEdge> AddEdge(TEdge edge) {
			_edges.Add(EdgeKey(edge), edge);
			return this;
        }

		public IGraph<TNode, TEdge> RemoveNode(TNode node) {
            foreach (var adjacent in Adjacent(node)) {
                RemoveEdge(this[node, adjacent]);
            }
			_nodes.Remove(node.Id);
			return this;
        }

		public IGraph<TNode, TEdge> RemoveEdge(TEdge edge) {
            _edges.Remove(EdgeKey(edge));
			return this;
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
            return EdgeKey(edge.Node1, edge.Node2);
        }

        private static string EdgeKey(TNode node1, TNode node2) {
	        return String.Compare(node1.Id, node2.Id, StringComparison.Ordinal) <= 0 ? node1.Id + " <-> " + node2.Id : node2.Id + " <-> " + node1.Id;
        }
    }
}
