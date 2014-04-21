using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Graph {
	public interface IEdge<out TNode> where TNode : INode {
        TNode Node1 { get; }
        TNode Node2 { get; }
    }
}
