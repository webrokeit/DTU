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

		public override bool Equals(object obj) {
			if (obj == this) return true;
			var cObj = obj as CoordinateNode;
			return cObj != null && Id.Equals(cObj.Id);
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		public override sealed string ToString() {
			return Id;
		}
    }
}
