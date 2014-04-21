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
	public class AStarPathFinder<TNode, TEdge> : IPathFinder<TNode, TEdge> where TNode : class,INode where TEdge : class,IDirectedEdge<TNode>, IWeighted {
		public IHeuristic Heuristic { get; private set; }
		private readonly IComparer<int> _comparer;

		public AStarPathFinder(IHeuristic heuristic) {
			Heuristic = heuristic;
			_comparer = Comparer<int>.Create((val1, val2) => val1.CompareTo(val2));
		}

		public IList<TNode> ShortestPath(IDirectedGraph<TNode, TEdge> graph, TNode source, TNode goal) {
			if (source == null || goal == null) return null;

			var visited = new HashSet<string>();
			var queue = new KeyValueHeap<string, int>(_comparer);
			var cameFrom = new Dictionary<string, string>();

			var gScore = new Dictionary<string, int>();
			var fScore = new Dictionary<string, int>();

			queue.Insert(source.Id, 0);
			gScore[source.Id] = 0;
			fScore[source.Id] = gScore[source.Id] + Heuristic.Evaluate(source, goal);

			while (!queue.Empty) {
				var current = graph[queue.RemoveRootKey()];
				if (current.Id == goal.Id) {
					return ReconstructPath(graph, cameFrom, current.Id);
				}

				visited.Add(current.Id);
				foreach (var neighbor in graph.Outgoing(current).Where(neighbor => !visited.Contains(neighbor.Id))) {
					var score = gScore[current.Id] + graph[current, neighbor].Weight;

					if (!queue.ContainsKey(neighbor.Id) || score < gScore[neighbor.Id]) {
						cameFrom[neighbor.Id] = current.Id;
						gScore[neighbor.Id] = score;
						fScore[neighbor.Id] = score + Heuristic.Evaluate(neighbor, goal);
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

		//public IList<TNode> ShortestPath(IDirectedGraph<TNode, TEdge> graph, TNode source, ICollection<TNode> goals) {
		//	return ShortestPath(graph, new[] { source }, goals);
		//}

		//public IList<TNode> ShortestPath(IDirectedGraph<TNode, TEdge> graph, ICollection<TNode> sources, TNode goal) {
		//	return ShortestPath(graph, sources, new[] { goal });
		//}

		//public IList<TNode> ShortestPath(IDirectedGraph<TNode, TEdge> graph, ICollection<TNode> sources, ICollection<TNode> goals) {
			
		//}

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
