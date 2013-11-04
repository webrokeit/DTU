using System;

/*
 * C# version of Edge, found in Sedgewick & Wayne, Algorithms (4th ED, 2011)
 * Written by:
 * 	Andreas Kjeldsen (s092638),
 * 	Morten Eskesen (s133304)
*/
namespace MFMSTProject.MFMST {
    public class Edge : IComparable<Edge> {
        public int Id { get; private set; }
        public int Vertex1 { get; private set; }
        public int Vertex2 { get; private set; }
        public int Weight { get; private set; }

        public Edge(int id, int vertex1, int vertex2, int weight) {
            if (vertex1 == vertex2) throw new ArgumentException("The vertices cannot be the same");
            Id = id;
            // The -1 is to make the vertex id a zero based index
            if (vertex1 < vertex2) {
                Vertex1 = vertex1-1;
                Vertex2 = vertex2-1;
            } else {
                Vertex1 = vertex2-1;
                Vertex2 = vertex1-1;
            }
            Weight = weight;
        }

        public int OtherVertex(int vertex) {
            return vertex == Vertex1 ? Vertex2 : vertex == Vertex2 ? Vertex1 : -1;
        }

        public int CompareTo(Edge other) {
            return Weight < other.Weight ? -1 : Weight > other.Weight ? 1 : 0;
        }

        public override string ToString() {
            return string.Format("Edge {0:d}: {4}{1:d}, {2:d}{5}; weight: {3:d}", Id, Vertex1+1, Vertex2+1, Weight, "{", "}");
        }
    }
}
