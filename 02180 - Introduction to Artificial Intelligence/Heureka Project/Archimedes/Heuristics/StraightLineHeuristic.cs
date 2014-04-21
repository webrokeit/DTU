using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Graph;

namespace Archimedes.Heuristics {
	public class StraightLineHeuristic : IHeuristic {
		public int Evaluate(INode from, INode to) {
			return Point.DistanceBetween(((ICoordinateNode) from).Point, ((ICoordinateNode) to).Point);
		}
	}
}
