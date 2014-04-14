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
        private readonly Dictionary<string, ICollection<string>> _outgoing;
	    private readonly Dictionary<string, ICollection<string>> _incoming; 

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
            _outgoing = new Dictionary<string, ICollection<string>>();
	        _incoming = new Dictionary<string, ICollection<string>>();
        }

        public TNode this[string node] {
            get { return _nodes[node]; }
        }

        public TEdge this[TNode from, TNode to] {
            get { return _edges[EdgeKey(from, to)]; }
        }

		public IDirectedGraph<TNode, TEdge> AddNode(TNode node) {
			if (!_nodes.ContainsKey(node.Id)) {
				_nodes.Add(node.Id, node);
				_outgoing.Add(node.Id, new HashSet<string>());
				_incoming.Add(node.Id, new HashSet<string>());
			}
			return this;
		}

		public IDirectedGraph<TNode, TEdge> AddEdge(TEdge edge) {
			_edges.Add(EdgeKey(edge), edge);
			_outgoing[edge.From.Id].Add(edge.To.Id);
			_incoming[edge.To.Id].Add(edge.From.Id);
			return this;
        }

		public IDirectedGraph<TNode, TEdge> RemoveNode(TNode node) {
			_nodes.Remove(node.Id);

			foreach (var edge in _outgoing[node.Id].ToList().Select(outgoingNode => this[node, this[outgoingNode]])) {
				RemoveEdge(edge);
			}
			_outgoing.Remove(node.Id);

			foreach (var edge in _incoming[node.Id].ToList().Select(incomingNode => this[this[incomingNode], node])) {
				RemoveEdge(edge);
			}
			_incoming.Remove(node.Id);

			return this;
        }

		public IDirectedGraph<TNode, TEdge> RemoveEdge(TEdge edge) {
			_edges.Remove(EdgeKey(edge));
			_outgoing[edge.From.Id].Remove(edge.To.Id);
			_incoming[edge.To.Id].Remove(edge.From.Id);
			return this;
        }

        public IEnumerable<TNode> Outgoing(INode fromNode) {
            if(fromNode == null) yield break;
            var outgoingNodes = _outgoing.GetOrDefault(fromNode.ToString(), null);
            if(outgoingNodes == null) yield break;
            foreach (var node in outgoingNodes) {
                yield return _nodes[node];
            }
        }
		public IEnumerable<TNode> Incoming(INode toNode) {
			if (toNode == null) yield break;
			var incomingNodes = _incoming.GetOrDefault(toNode.ToString(), null);
			if (incomingNodes == null) yield break;
			foreach (var node in incomingNodes) {
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
