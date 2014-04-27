using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Logic {
    internal class ClauseNode : IClauseNode {
        public string Id { get; private set; }
		public bool Fact { get { return false; } set { throw new NotSupportedException (); } }

        public ClauseNode(string id) {
            Id = id;
        }

        public bool Evaluate() {
            throw new NotImplementedException();
        }
    }
}
