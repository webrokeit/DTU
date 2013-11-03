﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MFMSTProject.Util;

/*
 * C# version of EWG, found in Sedgewick & Wayne, Algorithms (4th ED, 2011)
 * Written by:
 * 	Andreas Kjeldsen (s092638),
 * 	Morten Eskesen (s133304)
*/
namespace MFMSTProject.MFMST {
    public class EdgeWeightedGraph : IEnumerable<Edge> {
        public int Vertices { get; private set; }
        public int EdgeCount { get { return _edges.Count; } }
        public ICollection<Edge>[] Edges { get; private set; }
        private readonly List<Edge> _edges = new List<Edge>();
		private List<Edge> _enumEdges = new List<Edge> ();

        public EdgeWeightedGraph(int vertices) {
            if (vertices < 1) throw new ArgumentException("There must be at least one vertex in a graph", "vertices");
            Vertices = vertices;
            Edges = new ICollection<Edge>[vertices];
            for (var i = 0; i < vertices; i++) Edges[i] = new LinkedList<Edge>();
        }

        public void AddEdge(Edge edge) {
            Edges[edge.Vertex1].Add(edge);
            Edges[edge.Vertex2].Add(edge);
            _edges.Add(edge);
        }

        public Edge this[int index] {
            get { return _edges[index]; }
        }

		// The enumerator is called heavily, therefore calculate the edges to enumerate
		// beforehand, to avoid redoing the same checks over and over.
		// Uses more space, but only references are kept not copies.
		public void PrepEnumerator() {
			_enumEdges.Clear ();
			for (var i = 0; i < Vertices; i++) {
				foreach (var edge in Edges[i]) {
					if (edge.Vertex2 > i) _enumEdges.Add(edge);
				}
			}
		}

        public IEnumerator<Edge> GetEnumerator() {
			return _enumEdges.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public static EdgeWeightedGraph FromStream(TextReader stream, bool manualInput) {
            var vertices = stream.ReadLine().ToInt();
            var edges = stream.ReadLine().ToInt();
            var graph = new EdgeWeightedGraph(vertices);
            var line = "";

            while ((line = stream.ReadLine()) != null) {
                var parts = line.Split(' ');
                if (parts.Length < 3) {
                    throw new ArgumentException("Each line for the edges must contain three numbers: int int int");
                };
                var vertex1 = parts[0].ToInt();
                var vertex2 = parts[1].ToInt();
                var weight = parts[2].ToInt();

                var edge = new Edge(graph.EdgeCount + 1, vertex1, vertex2, weight);
                graph.AddEdge(edge);
                if (manualInput && edges == graph.EdgeCount) break;
            }

            if (edges != graph.EdgeCount) throw new ArgumentException("Amount of edges read and the amount of edges supposed to be there does not match.");

            return graph;
        }
    }
    
}
