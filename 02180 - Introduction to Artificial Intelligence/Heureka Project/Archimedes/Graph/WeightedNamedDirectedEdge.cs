using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Graph {
	public class WeightedNamedDirectedEdge<TNode> : IWeightedNamedDirectedEdge<TNode> where TNode : class, INode {
        public TNode From { get; private set; }
        public TNode To { get; private set; }
        public int Weight { get; private set; }
        public string Name { get; private set; }

        private readonly int _hashCode;

        public WeightedNamedDirectedEdge(string name, TNode from, TNode to, int weight) {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (from == null) throw new ArgumentNullException("from");
            if (to == null) throw new ArgumentNullException("to");
            Name = name;
            From = from;
            To = to;
            Weight = weight;
            _hashCode = ToString().GetHashCode();
        }

        public override bool Equals(object obj) {
            if (obj == this) return true;
            var weObj = obj as WeightedNamedDirectedEdge<TNode>;
            return weObj != null && Weight == weObj.Weight && From.Equals(weObj.From) && To.Equals(weObj.To);
        }

        public override int GetHashCode() {
            return _hashCode;
        }

        public override sealed string ToString() {
            return "{" + From + " -> " + To + "} : " + Name;
        }
    }
}
