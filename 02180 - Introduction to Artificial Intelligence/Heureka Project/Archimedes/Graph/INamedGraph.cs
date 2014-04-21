using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Graph {
	public interface INamedGraph<TNode, TEdge> where TNode : INode where TEdge : INamed {
		ICollection<string> EdgeNames { get; }
		TNode GetNodeByEdgeNames(params string[] edgeName);
		ICollection<TNode> GetNodesByEdgeNames(params string[] edgeName);
		ICollection<TEdge> GetEdgesByName(string edgeName);
	}
}
