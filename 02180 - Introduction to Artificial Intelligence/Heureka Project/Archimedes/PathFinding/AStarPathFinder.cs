using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Graph;
using Archimedes.Heaps;
using Archimedes.Heuristics;

namespace Archimedes.PathFinding {
	public class AStarPathFinder<TNode, TEdge> : IPathFinder<TNode, TEdge> where TNode : INode where TEdge : IDirectedEdge<TNode>, IWeighted {
		public IHeuristic Heuristic { get; private set; }

		public AStarPathFinder(IHeuristic heuristic) {
			Heuristic = heuristic;
		}

		public IList<TNode> ShortestPath(IDirectedGraph<TNode, TEdge> graph, TNode from, TNode to) {
			var visited = new HashSet<string>();
			var queue = new KeyValueHeap<string, int>(Comparer<int>.Create((val1, val2) => val1.CompareTo(val2)));
			var cameFrom = new Dictionary<string, string>();

			var gScore = new Dictionary<string, int>();
			var fScore = new Dictionary<string, int>();

			queue.Insert(from.Id, 0);
			gScore[from.Id] = 0;
			fScore[from.Id] = gScore[from.Id] + Heuristic.Evaluate(from, to);

			while (!queue.Empty) {
				var current = graph[queue.RemoveRootKey()];
				if (current.Id == to.Id) {
					return ReconstructPath(graph, cameFrom, current.Id);
				}

				visited.Add(current.Id);
				foreach (var neighbor in graph.Outgoing(current).Where(neighbor => !visited.Contains(neighbor.Id))) {
					var score = gScore[current.Id] + graph[current, neighbor].Weight;

					if (!queue.ContainsKey(neighbor.Id) || score < gScore[neighbor.Id]) {
						cameFrom[neighbor.Id] = current.Id;
						gScore[neighbor.Id] = score;
						fScore[neighbor.Id] = score + Heuristic.Evaluate(neighbor, to);
						if (queue.ContainsKey(neighbor.Id)) {
							queue.SetValue(neighbor.Id, fScore[neighbor.Id]);
						} else {
							queue.Insert(neighbor.Id, fScore[neighbor.Id]);
						}
					}
				}
			}

			return null;
		}

		private static IList<TNode> ReconstructPath(IDirectedGraph<TNode, TEdge> graph, IReadOnlyDictionary<string, string> cameFrom, string currentNodeId) {
			var path = new List<TNode>();
			ReconstructPath(graph, cameFrom, currentNodeId, path);
			return path;
		}

		private static void ReconstructPath(IDirectedGraph<TNode, TEdge> graph, IReadOnlyDictionary<string, string> cameFrom, string currentNodeId, ICollection<TNode> thelist) {
			if (cameFrom.ContainsKey(currentNodeId)) {
				ReconstructPath(graph, cameFrom, cameFrom[currentNodeId], thelist);
			}
			thelist.Add(graph[currentNodeId]);
		}
	}
}
