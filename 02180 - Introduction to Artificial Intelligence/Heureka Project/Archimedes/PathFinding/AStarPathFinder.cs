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
			var queue = new Heap<KeyValuePair<TNode, int>>(Comparer<KeyValuePair<TNode, int>>.Create((node1, node2) => node1.Value.CompareTo(node2.Value)));
			var cameFrom = new Dictionary<string, string>();

			var gScore = new Dictionary<string, int>();
			var fScore = new Dictionary<string, int>();

			queue.Insert(new KeyValuePair<TNode, int>(from, 0));
			gScore[from.Id] = 0;
			fScore[from.Id] = gScore[from.Id] + Heuristic.Evaluate(from, to);

			while (!queue.Empty) {
				var current = queue.RemoveRoot().Key;
				if (current.Id == to.Id) {
					// Return path;
				}

				visited.Add(current.Id);
				foreach (var neighbor in graph.Outgoing(current).Where(neighbor => !visited.Contains(neighbor.Id))) {
					var score = gScore[current.Id] + graph[current, neighbor].Weight;

				}
			}

			return null;
		}


	}
}
