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
using Archimedes.Extensions;

namespace Heureka {
	public class Program {
		private static IDictionary<string, string> _arguments;

		public static void Main(string[] args) {
			_arguments = ArgumentsFactory.ArgumentsAsDictionary (args);

			var function = _arguments.GetOrDefault ("func", "path");
			var input = _arguments.GetOrDefault ("file", "input.txt");

			if (input == null) {
				ExitWithMsg ("No input file specified");
			} else if (!File.Exists (input)) {
				ExitWithMsg ("Input file does not exist: " + input);
			}

			if (function == "path") {
				PathPlanning (input);
			} else if (function == "logic") {

			} else {
				ExitWithMsg ("Invalid function specified, valid values are: 'path' and 'logic'");
			}


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

			var query = KnowledgeBaseFactory.QueryFromLine("a");
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

		private static void PathPlanning(string fileName){
			INamedDirectedGraph<ICoordinateNode, IWeightedNamedDirectedEdge<ICoordinateNode>> graph;
			using (var fs = new FileStream (fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				graph = GraphFactory.NamedDirectedFromInput (fs);
			}
			
			Console.WriteLine ("Map loaded, it contains " + graph.NodeCount + " nodes and " + graph.EdgeCount + " edges");
			var heuristic = new StraightLineHeuristic ();
			var pathFinder = new AStarPathFinder<ICoordinateNode, IWeightedNamedDirectedEdge<ICoordinateNode>> (heuristic);
			Console.WriteLine ("Type exit to quit the program.");
			Console.WriteLine ();

			var read = string.Empty;
			while (read != "exit") {
				ICollection<ICoordinateNode> startNodes = null;

				while (startNodes == null || startNodes.Count < 1) {
					Console.Write ("Enter street names for starting point: ");
					read = Console.ReadLine ();
					if (read == "quit") {
						ExitWithMsg (null);
					}

					// SktPedersStraede Larsbjoernsstraede
					var parts = read.Split (new char[]{' '}, 2);
					if (parts.Any (part => graph.GetEdgesByName (part).Count < 1)) {
						Console.WriteLine ("Invalid street name supplied");
						continue;
					}
					startNodes = graph.GetNodesByEdgeNames (parts);

				}

				ICollection<ICoordinateNode> endNodes = null;
				while (endNodes == null || endNodes.Count < 1) {
					Console.Write ("Enter street names for ending point: ");
					read = Console.ReadLine ();
					if (read == "quit") {
						ExitWithMsg (null);
					}

					// Studiestraede Larsbjoernsstraede
					var parts = read.Split (new char[]{' '}, 2);
					if (parts.Any (part => graph.GetEdgesByName (part).Count < 1)) {
						Console.WriteLine ("Invalid street name supplied");
						continue;
					}
					endNodes = graph.GetNodesByEdgeNames (parts);
				}
					
				var path = pathFinder.ShortestPath (graph, startNodes.First(), endNodes.First());
				if (path.Count == 1) {
					Console.WriteLine ("Path found: Just stand still, you're already there");
				}else if (path.Count > 1) {
					var prev = path [0];

					Console.WriteLine ("Path found:");
					for (var i = 1; i < path.Count; i++) {
						var edge = graph.GetEdge (prev, path [i]);
						Console.Write (edge.Name);
						if (i + 1 < path.Count) {
							Console.Write (" - > ");
						}
						prev = path [i];
					}
				} else {
					Console.WriteLine ("No path found");
				}
				Console.WriteLine ();
			}
		}

		private static void ExitWithMsg(string msg){
			if (!string.IsNullOrEmpty (msg)) {
				Console.WriteLine (msg);
				Console.WriteLine ("Press any key to exit...");
				Console.ReadKey ();
			}
			Environment.Exit (0);
		}
    }
}
