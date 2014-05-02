using System.Linq;

namespace Archimedes.Logic {
	internal class LiteralNode : ILiteralNode {
        public string Id { get; private set; }
        public bool Fact { get; set; }
        public int LiteralCount { get; private set; }

        public LiteralNode(string id) {
            Id = id;
            LiteralCount = 1 + id.Count(c => c == '&' || c == '|');
        }
	}
}
