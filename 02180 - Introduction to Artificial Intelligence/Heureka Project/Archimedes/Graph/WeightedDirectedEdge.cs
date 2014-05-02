using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Graph {
    public class WeightedDirectedEdge<TNode> : IWeightedDirectedEdge<TNode> where TNode : class, INode {
        public TNode From { get; private set; }
        public TNode To { get; private set; }
        public int Weight { get; private set; }

        private readonly int _hashCode;

        public WeightedDirectedEdge(TNode from, TNode to, int weight) {
            if (from == null) throw new ArgumentNullException("from");
            if (to == null) throw new ArgumentNullException("to");
            From = from;
            To = to;
            Weight = weight;
            _hashCode = ToString().GetHashCode();
        }

        public override bool Equals(object obj) {
            if (obj == this) return true;
            var weObj = obj as WeightedDirectedEdge<TNode>;
            return weObj != null && Weight == weObj.Weight && From.Equals(weObj.From) && To.Equals(weObj.To);
        }

        public override int GetHashCode() {
            return _hashCode;
        }

        public override sealed string ToString() {
            return "{" + From.Id + " -> " + To.Id + " : " + Weight + "}";
        }
    }
}
