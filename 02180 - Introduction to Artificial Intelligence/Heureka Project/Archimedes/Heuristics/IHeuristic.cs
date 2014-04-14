using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Graph;

namespace Archimedes.Heuristics {
	public interface IHeuristic {
		int Evaluate(INode from, INode to);
	}
}
