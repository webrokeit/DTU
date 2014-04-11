using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Graph {
    public class CoordinateNode : ICoordinateNode {
        public string Id { get; private set; }
        public IPoint Point { get; private set; }
        
        public CoordinateNode(IPoint point) {
            Point = point;
            Id = "Node{" + Point.X + ", " + Point.Y + "}";
        }
    }
}
