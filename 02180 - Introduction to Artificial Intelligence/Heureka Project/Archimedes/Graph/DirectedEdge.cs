using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Graph {
    public class DirectedEdge<TNode> : IDirectedEdge<TNode> where TNode : INode {
        public TNode From { get; private set; }
        public TNode To { get; private set; }

        private readonly int _hashCode;

        public DirectedEdge(TNode from, TNode to) {
            From = from;
            To = to;
            _hashCode = ToString().GetHashCode();
        }

        public override bool Equals(object obj) {
            if (obj == this) return true;
            var weObj = obj as DirectedEdge<TNode>;
            return weObj != null && From.Equals(weObj.From) && To.Equals(weObj.To);
        }

        public override int GetHashCode() {
            return _hashCode;
        }

        public override sealed string ToString() {
            return "{" + From.Id + " -> " + To.Id + "}";
        }
    }
}
