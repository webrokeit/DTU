using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Graph;

namespace Archimedes.Logic {
	internal class LiteralNode : ILiteralNode {
        public string Id { get; private set; }
        public bool Fact { get; set; }
        public IPoint Point { get; set; }

        public LiteralNode(string id) {
            Id = id;
        }
	}
}
