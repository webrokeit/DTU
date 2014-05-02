using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Graph;

namespace Archimedes.Logic {
    internal class ClauseNode : IClauseNode {
        public string Id { get; private set; }
		public bool Fact { get; set; }
        public IPoint Point { get; set; }

        public ClauseNode(string id) {
            Id = id;
        }
    }
}
