using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Graph {
    public interface IDirectedEdge<out TNode> where TNode : INode {
        TNode From { get; }
        TNode To { get; }
    }
}
