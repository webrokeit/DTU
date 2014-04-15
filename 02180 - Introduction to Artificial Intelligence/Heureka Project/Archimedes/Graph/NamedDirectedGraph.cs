using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Graph {
	public class NamedDirectedGraph<TNode, TEdge> : DirectedGraph<TNode, TEdge>, INamedDirectedGraph<TNode, TEdge> where TNode : INode where TEdge : IDirectedEdge<TNode>, INamed {
		private readonly IDictionary<string, ICollection<TEdge>> _edgeNames; 
		
		public ICollection<string> EdgeNames { get { return _edgeNames.Keys; }}

		public NamedDirectedGraph() : base() {
			_edgeNames = new Dictionary<string, ICollection<TEdge>>();
		} 

		public new INamedDirectedGraph<TNode, TEdge> AddNode(TNode node) {
			base.AddNode(node);
			return this;
		}

		public new INamedDirectedGraph<TNode, TEdge> AddEdge(TEdge edge) {
			base.AddEdge(edge);
			if (!_edgeNames.ContainsKey(edge.Name)) {
				_edgeNames[edge.Name] = new HashSet<TEdge>();
			}
			_edgeNames[edge.Name].Add(edge);
			return this;
		}

		public new INamedDirectedGraph<TNode, TEdge> RemoveNode(TNode node) {
			base.RemoveNode(node);
			return this;
		}

		public new INamedDirectedGraph<TNode, TEdge> RemoveEdge(TEdge edge) {
			base.RemoveEdge(edge);
			_edgeNames[edge.Name].Remove(edge);
			if (_edgeNames[edge.Name].Count < 1) {
				_edgeNames.Remove(edge.Name);
			}
			return this;
		}

		public TNode GetNodeByEdgeNames(params string[] edgeNames) {
			return GetNodesByEdgeNames(edgeNames).FirstOrDefault();
		}

		public ICollection<TNode> GetNodesByEdgeNames(params string[] edgeNames) {
			var nodeSet = new HashSet<TNode>();
			if (edgeNames == null || edgeNames.Length < 1) return nodeSet;

			foreach (var edge in _edgeNames[edgeNames[0]]) {
				nodeSet.Add(edge.From);
				nodeSet.Add(edge.To);
			}

			if (edgeNames.Length > 1) {
				for (var i = 1; i < edgeNames.Length && nodeSet.Count > 0; i++) {
					var newNodeSet = new HashSet<TNode>();
					foreach (var edge in _edgeNames[edgeNames[i]]) {
						newNodeSet.Add(edge.From);
						newNodeSet.Add(edge.To);
					}
					nodeSet.IntersectWith(newNodeSet);
				}
			}

			return nodeSet;
		}

		public ICollection<TEdge> GetEdgesByName(string edgeName) {
			return _edgeNames[edgeName];
		}
	}
}
