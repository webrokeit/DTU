using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Logic {
    internal class LiteralNode : ILogicNode {
        public string Id { get; private set; }
        public bool Fact { get; private set; }

        public LiteralNode(string id) {
            Id = id;
        }

        public LiteralNode(string id, bool fact) : this(id) {
            Fact = fact;
        }

        public bool Evaluate() {
            throw new NotImplementedException();
        }
    }
}
