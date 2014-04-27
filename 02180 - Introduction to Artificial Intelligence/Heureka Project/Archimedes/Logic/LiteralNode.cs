using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Logic {
	internal class LiteralNode : ILiteralNode {
        public string Id { get; private set; }
        public bool Fact { get; set; }

        public LiteralNode(string id) {
            Id = id;
        }

        public bool Evaluate() {
            throw new NotImplementedException();
        }
    }
}
