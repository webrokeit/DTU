using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Graph {
	public interface IWeightedNamedDirectedEdge<out TNode> :  IWeighted, INamed, IDirectedEdge<TNode> where TNode : class, INode {}
}
