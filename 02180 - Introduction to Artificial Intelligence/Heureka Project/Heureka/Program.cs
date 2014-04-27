using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Graph;
using Archimedes.Heaps;
using Archimedes.Heuristics;
using Archimedes.Logic;
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

//	        INamedDirectedGraph<ICoordinateNode, IWeightedNamedDirectedEdge<ICoordinateNode>> graph;
//	        using (var fs = new FileStream("TestInputs/pathtestinput01.txt", FileMode.Open, FileAccess.Read, FileShare.Read)) {
//		        graph = GraphFactory.NamedDirectedFromInput(fs);
//	        }
//
//	        Console.WriteLine("Graph has " + graph.NodeCount + " nodes and " + graph.EdgeCount + " edges");
//
//	        var heuristic = new StraightLineHeuristic();
//	        var pathFinder = new AStarPathFinder<ICoordinateNode, IWeightedNamedDirectedEdge<ICoordinateNode>>(heuristic);
//
//			var starts = graph.GetNodesByEdgeNames("SktPedersStraede", "Larsbjoernsstraede").ToList();
//			var ends = graph.GetNodesByEdgeNames("Studiestraede", "Larsbjoernsstraede").ToList();
//
//	        var sw = new Stopwatch();
//			sw.Start();
//	        var path = pathFinder.ShortestPath(graph, starts[0], ends[0]);
//			sw.Stop();
//			Console.WriteLine("Time taken to find path was " + sw.ElapsedMilliseconds + " ms, path is:");
//	        Console.WriteLine(string.Join(" -> ", path));
//            Console.WriteLine();

            IKnowledgeBase kb;
			using (var fs = new FileStream("TestInputs/logic02simple.txt", FileMode.Open, FileAccess.Read, FileShare.Read)) {
                kb = KnowledgeBaseFactory.FromInput(fs);
            }

			var query = KnowledgeBaseFactory.QueryFromLine("h l");
			var qStr = string.Join (" & ", query.Literals);

			for (var i = 0; i < 2; i++) {
				Console.WriteLine ("Round #" + (i + 1));
				Console.WriteLine ("Is " + qStr + " satisfiable? [Direct]");
				var satisfied = kb.DirectQuery (query);
				Console.WriteLine ((satisfied ? "Yes " + qStr + " is" : "No " + qStr + " is not") + " satisfiable [Direct]");
				Console.WriteLine ();
				Console.WriteLine ("Is " + qStr + " satisfiable? [Refutation]");
				var refuSatisfied = kb.RefutationQuery (query);
				Console.WriteLine ((refuSatisfied ? "Yes " + qStr + " is" : "No " + qStr + " is not") + " satisfiable [Refuation]");
				Console.WriteLine ();
			}

	        Console.ReadKey(true);
        }
    }
}
