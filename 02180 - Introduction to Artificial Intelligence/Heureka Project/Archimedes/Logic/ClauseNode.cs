using System.Linq;

namespace Archimedes.Logic {
    internal class ClauseNode : IClauseNode {
        public string Id { get; private set; }
		public bool Fact { get; set; }
        public int LiteralCount { get; private set; }

        public ClauseNode(string id) {
            Id = id;
            LiteralCount = 1 + id.Count(c => c == '&' || c == '|');
        }
    }
}
