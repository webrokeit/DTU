using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Graph;

namespace Archimedes.Logic {
    internal interface ILogicNode : ICoordinateNode {
		bool Fact { get; set; }
        new IPoint Point { get; set; }
    }
}
