using Archimedes.Graph;

namespace Archimedes.Logic {
    internal interface ILogicNode : INode {
		bool Fact { get; set; }
        int LiteralCount { get; }
    }
}
