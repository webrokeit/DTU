using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Graph {
    public interface IWeightedDirectedEdge<out TNode> : IWeighted, IDirectedEdge<TNode> where TNode : class, INode {
    }
}
