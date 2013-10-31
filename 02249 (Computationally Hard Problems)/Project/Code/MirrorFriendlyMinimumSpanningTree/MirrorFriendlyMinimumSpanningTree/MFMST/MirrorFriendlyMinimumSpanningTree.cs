using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MFMSTProject.Util;

namespace MFMSTProject.MFMST {
    public class MirrorFriendlyMinimumSpanningTree : IEnumerable<Edge> {
        private IndexMinPriorityQueue<int> _priorityQueue = null;
        private readonly EdgeWeightedGraph _graph = null;
        private int _threshold = 0;
        private List<HashSet<int>> _breakers = null;
        private readonly int[] _baseDistTo = null;

        public Edge[] EdgeTo { get; private set; }
        public int[] DistTo { get; private set; }
        public bool[] Marked { get; private set; }

        public bool IsMst { get { return this.All(edge => edge != null); } }

        public int Weight {
            get {
                return this.Sum(edge => edge.Weight);
            }
        }

        public int MirrorWeight {
            get {
                return this.Sum(edge => MirrorEdge(edge).Weight);
            }
        }

        public MirrorFriendlyMinimumSpanningTree(EdgeWeightedGraph graph) {
            _graph = graph;
            _baseDistTo = new int[_graph.Vertices];
            for (var i = 0; i < _baseDistTo.Length; i++) _baseDistTo[i] = int.MaxValue;
            Reset();
        }

        private Edge[] CloneEdgeTo() {
            var edgeToClone = new Edge[EdgeTo.Length];
            Array.Copy(EdgeTo, edgeToClone, EdgeTo.Length);
            return edgeToClone;
        }

        public void Run() {
            MakeMst(new HashSet<int>());
            _threshold = Math.Max(Weight, MirrorWeight);

            _breakers = new List<HashSet<int>>(); 
            var reqEdges = GetRequiredEdgeIds();
            var edgeToRes = CloneEdgeTo();
            var edgeToClone = edgeToRes.Where(edge => edge != null && !reqEdges.Contains(edge.Id)).OrderBy(edge => edge.Weight).ToArray();

            foreach (var excludes in PermutateMstEdges(edgeToClone).Where(excludes => excludes.Count > 0)) {
                //Console.WriteLine("Trying perm: " + string.Join(", ", excludes));
                Reset();
                MakeMst(excludes);
                if (IsMst) {
                    if (Math.Max(Weight, MirrorWeight) < _threshold) {
                        _threshold = Math.Max(Weight, MirrorWeight);
                        edgeToRes = CloneEdgeTo();
                    } 
                } else {
                    _breakers.Add(new HashSet<int>(excludes));
                }
            }

            EdgeTo = edgeToRes;
        }

        private void MakeMst(ISet<int> excludes) {
            while (!_priorityQueue.IsEmpty) {
                Visit(_graph, _priorityQueue.DeleteMinimum(), excludes);
            }
        }

        private void Visit(EdgeWeightedGraph graph, int vertex, ISet<int> excludes) {
            Marked[vertex] = true;
            foreach (var edge in graph.Edges[vertex].Where(edge => !excludes.Contains(edge.Id))) {
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

        private ISet<int> GetRequiredEdgeIds() {
            var set = new HashSet<int>();
            var required = new int[_graph.Vertices];
            for(var i = 0; i < _graph.EdgeCount; i++) {
                var edge = _graph[i];
                required[edge.Vertex1]++;
                required[edge.Vertex2]++;
            }
            for (var i = 0; i < required.Length; i++) {
                if (required[i] > 1) continue;
                set.Add(required[i]);
                //Console.WriteLine("Edge was required: " + EdgeTo[i]);
            }
            return set;
        }

        private const int Sizeofint = sizeof (int);
        public void Reset() {
            EdgeTo = new Edge[_graph.Vertices];
            DistTo = new int[_graph.Vertices];
            Marked = new bool[_graph.Vertices];
            _priorityQueue = new IndexMinPriorityQueue<int>(_graph.Vertices);

            // Copy the int.maxValue to DistTo by copying the bytes - *should* be faster
            Buffer.BlockCopy(_baseDistTo, Sizeofint, DistTo, Sizeofint, Sizeofint * (_graph.Vertices - 1));
            _priorityQueue.Insert(0, 0); 
        }

        public Edge MirrorEdge(Edge edge) {
            return _graph[_graph.EdgeCount - edge.Id];
        }

        public IEnumerator<Edge> GetEnumerator() {
            for (var i = 1; i < EdgeTo.Length; i++) {
                yield return EdgeTo[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private IEnumerable<ISet<int>> PermutateMstEdges(Edge[] edges) {
            return PermutateMstEdges(edges, 0, new HashSet<int>());
        }

        private IEnumerable<ISet<int>> PermutateMstEdges(Edge[] edges, int index, ISet<int> set) {
            if (index >= edges.Length) {
                yield return set;
            } else {
                foreach (var perm in PermutateMstEdges(edges, index + 1, set).Where(perm => perm.Count > 0)) {
                    yield return perm;
                }
                set.Add(edges[index].Id);
                if (!_breakers.Any(breakset => breakset.IsSubsetOf(set))) {
                    foreach (var perm in PermutateMstEdges(edges, index + 1, set).Where(perm => perm.Count > 0)) {
                        yield return perm;
                    }
                }
                set.Remove(edges[index].Id);
            }
        }

        public void PrintSolution() {
            foreach (var edge in this.OrderBy(edge => edge.Id)) {
                Console.WriteLine(edge + ", mirrored weight: " + MirrorEdge(edge).Weight);
            }
        }

        private static MirrorFriendlyMinimumSpanningTree _yesNoMfmst = null;
        public static bool MirrorFriendlyMinimumSpanningTreeWithWeight(EdgeWeightedGraph graph, int weight) {
            if (_yesNoMfmst == null || _yesNoMfmst._graph != graph) {
                _yesNoMfmst = new MirrorFriendlyMinimumSpanningTree(graph);
            } else {
                _yesNoMfmst.Reset();
            }
            _yesNoMfmst.Run();
            return _yesNoMfmst.Weight == weight;
        }
    }
}
