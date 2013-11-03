using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MFMSTProject.Util;

/*
 * C# version of Prims eager MST algorithm, found in Sedgewick & Wayne, Algorithms (4th ED, 2011)
 * Written by:
 * 	Andreas Kjeldsen (s092638),
 * 	Morten Eskesen (s133304)
*/
namespace MFMSTProject.MST {
    public class MinimumSpanningTree : IEnumerable<Edge> {
        private IndexMinPriorityQueue<int> _priorityQueue = null;
        private readonly EdgeWeightedGraph _graph = null;
        private int _weight = int.MinValue;

        public Edge[] EdgeTo { get; private set; }
        public int[] DistTo { get; private set; }
        public bool[] Marked { get; private set; }

        public int Weight {
            get {
                if (_weight == int.MinValue && EdgeTo.Count(edge => edge != null) == _graph.Vertices-1) {
                    _weight = this.Sum(edge => edge.Weight);
                }
                return _weight;
            }
        }

        public MinimumSpanningTree(EdgeWeightedGraph graph) {
            _graph = graph;
            Reset();
        }

        public void Run() {
            while (!_priorityQueue.IsEmpty) {
                Visit(_graph, _priorityQueue.DeleteMinimum());  
            }
        }

        private void Visit(EdgeWeightedGraph graph, int vertex) {
            Marked[vertex] = true;
            foreach (var edge in graph[vertex]) {
                var other = edge.OtherVertex(vertex);
                if (Marked[other] || edge.Weight >= DistTo[other]) continue;
                EdgeTo[other] = edge;
                DistTo[other] = edge.Weight;
                if (_priorityQueue.Contains(other)) {
                    _priorityQueue.Change(other, DistTo[other]);
                } else {
                    _priorityQueue.Insert(other, DistTo[other]);
                }
            }
        }

        public void Reset() {
            EdgeTo = new Edge[_graph.Vertices];
            DistTo = new int[_graph.Vertices];
            Marked = new bool[_graph.Vertices];
            _priorityQueue = new IndexMinPriorityQueue<int>(_graph.Vertices);

            for (var i = 0; i < DistTo.Length; i++) {
                DistTo[i] = int.MaxValue;
            }

            DistTo[0] = 0;
            _priorityQueue.Insert(0, 0);
            _weight = int.MinValue;
        }

        public IEnumerator<Edge> GetEnumerator() {
            for (var i = 1; i < EdgeTo.Length; i++) {
                yield return EdgeTo[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
