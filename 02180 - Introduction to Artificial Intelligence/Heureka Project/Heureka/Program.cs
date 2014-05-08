using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

			const string defaultFunction = "logic";
			var function = _arguments.GetOrDefault ("func", defaultFunction);

		    var defaultInput = function + "-input.txt";
			var input = _arguments.GetOrDefault ("file", defaultInput);
			input = "TestInputs/logic02simple.txt";

			if (input == null) {
				ExitWithMsg ("No input file specified");
			} else if (!File.Exists (input)) {
				ExitWithMsg ("Input file does not exist: " + input);
			}

			if (function == "path") {
				PathPlanning (input);
			} else if (function == "logic") {
			    Inference(input);
			} else {
				ExitWithMsg ("Invalid function specified, valid values are: 'path' and 'logic'");
			}
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

		    while (true) {
				ICollection<ICoordinateNode> startNodes = null;

			    string read;
			    while (startNodes == null || startNodes.Count < 1) {
					Console.Write ("Enter street names for starting point: ");
					read = Console.ReadLine ();
                    if (string.IsNullOrEmpty(read)) continue;
                    if (read == "quit") ExitWithMsg(null);

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
				    if (string.IsNullOrEmpty(read)) continue;
                    if (read == "quit") ExitWithMsg(null);

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
					var prevName = string.Empty;

					Console.WriteLine ("Path found:");
					for (var i = 1; i < path.Count; i++) {
						var edge = graph.GetEdge (prev, path [i]);
						if (edge.Name != prevName) {
							Console.Write (edge.Name);
							if (i + 1 < path.Count) {
								Console.Write (" -> ");
							}
						}
						prev = path [i];
						prevName = edge.Name;
					}
				} else {
					Console.WriteLine ("No path found");
				}
				Console.WriteLine ();
			}
		}

	    private static void Inference(string fileName) {
            try {
                IKnowledgeBase kb;
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    kb = KnowledgeBaseFactory.FromInput(fs);
                }

                OutputKnowledgeBaseGraph(kb, "graphviz.txt");

                Console.WriteLine("Type exit to quit the program.");
                Console.WriteLine("A query is a space separated list of literals to test for satisfiability.");
                Console.WriteLine();

	            while (true) {
	                Console.Write("Enter query: ");
	                var read = Console.ReadLine() ?? "";
	                if (read == "quit") ExitWithMsg(null);

	                var query = KnowledgeBaseFactory.QueryFromLine(read);
	                var qStr = query.ToString();

	                Console.WriteLine("[Direct] Is " + qStr + " satisfiable?");
	                var satisfied = kb.DirectQuery(query);
	                Console.WriteLine("[Direct] " + (satisfied ? "Yes " + qStr + " is" : "No " + qStr + " is not") +
	                                  " satisfiable");
                    Console.WriteLine();

	                Console.WriteLine("[Refutation] Is " + qStr + " satisfiable?");
	                var refuSatisfied = kb.RefutationQuery(query);
                    Console.WriteLine("[Refutation] " + (refuSatisfied ? "Yes " + qStr + " is" : "No " + qStr + " is not") +
	                                  " satisfiable");
                    Console.WriteLine();
	            }
            } catch (Exception ex) {
                ExitWithMsg("Error: " + ex.Message);
            }
	    }

	    private static void OutputKnowledgeBaseGraph(IKnowledgeBase kb, string outputFileName) {
	        using (var fs = new StreamWriter(new FileStream(outputFileName, FileMode.Create, FileAccess.Write, FileShare.Write))) {
                fs.Write(kb.ToString());
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
