using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Graph;

namespace Archimedes.Logic {
    internal interface ILogicNode : INode {
        bool Fact { get; }
        bool Evaluate();
    }
}
