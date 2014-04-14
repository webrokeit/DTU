using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Graph;
using Archimedes.Heaps;
using Archimedes.Heuristics;
using Archimedes.PathFinding;
using Heureka.Factories;

namespace Heureka {
    class Program {
        static void Main(string[] args) {
			//var heap = new KeyValueHeap<string, int>(Comparer<int>.Default);

			//for (var i = 0; i < 25; i++) {
			//	heap.Insert("Key#" + i, i);
			//}

			//while (!heap.Empty) {
			//	Console.WriteLine("Removing " + heap.PeekKey());
			//	heap.RemoveRoot();
			//	Console.WriteLine(heap.ToString());
			//	if(heap.Count > 0) Console.WriteLine();
			//}

	        IDirectedGraph<ICoordinateNode, IWeightedNamedDirectedEdge<ICoordinateNode>> graph;
	        using (var fs = new FileStream("TestInputs/pathtestinput01.txt", FileMode.Open, FileAccess.Read, FileShare.Read)) {
		        graph = GraphFactory.FromInput(fs);
	        }

	        Console.WriteLine("Graph has " + graph.NodeCount + " nodes and " + graph.EdgeCount + " edges");

	        var heuristic = new StraightLineHeuristic();
	        var pathFinder = new AStarPathFinder<ICoordinateNode, IWeightedNamedDirectedEdge<ICoordinateNode>>(heuristic);

			var start = graph[""]
			var path = pathFinder.ShortestPath(graph, )

	        Console.ReadKey(true);
        }
    }
}
