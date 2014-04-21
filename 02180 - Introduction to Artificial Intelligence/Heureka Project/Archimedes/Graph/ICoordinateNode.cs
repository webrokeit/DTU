using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Graph {
    public interface ICoordinateNode : INode {
        IPoint Point { get; }
    }
}
