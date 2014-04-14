using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Graph;

namespace Heureka.PathFinding {
    public class PathFinder<TEdge> where TEdge : IDirectedEdge<ICoordinateNode>, IWeighted, INamed {
        public IDirectedGraph<ICoordinateNode, TEdge> Graph { get; private set; }

        private PathFinder() { }

        public static PathFinder<T> Create<T>(IDirectedGraph<ICoordinateNode, T> graph)
            where T : IDirectedEdge<ICoordinateNode>, IWeighted, INamed {
            var pf = new PathFinder<T> {Graph = graph};
            return pf;
        }
    }
}
