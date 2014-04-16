using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Graph {
    public class Node : INode {
        public string Id { get; private set; }

        public Node(string id) {
            Id = id;
        }

        public override bool Equals(object obj) {
            if (obj == this) return true;
            var cObj = obj as Node;
            return cObj != null && Id.Equals(cObj.Id);
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }

        public override sealed string ToString() {
            return "Node(" + Id + ")";
        }
    }
}
