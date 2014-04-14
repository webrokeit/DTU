using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Extensions;
using Archimedes.Graph;

namespace Heureka.Factories {
    public static class GraphFactory {
		public static IDirectedGraph<ICoordinateNode, IWeightedNamedDirectedEdge<ICoordinateNode>> FromInput(){
			var oldIn = Console.In;
			var graph = FromInput(Console.OpenStandardInput());
			Console.SetIn(oldIn);
			return graph;
		}

		public static IDirectedGraph<ICoordinateNode, IWeightedNamedDirectedEdge<ICoordinateNode>> FromInput(Stream input) {
			return FromInput(new StreamReader(input));
		}

		public static IDirectedGraph<ICoordinateNode, IWeightedNamedDirectedEdge<ICoordinateNode>> FromInput(StreamReader input)  {
			var graph = new DirectedGraph<ICoordinateNode, IWeightedNamedDirectedEdge<ICoordinateNode>>();

			while (!input.EndOfStream) {
				var line = input.ReadLine();
				if (string.IsNullOrEmpty(line)) continue;

				var parts = line.Split(' ');
				if (parts.Length != 5) {
					Debug.WriteLine("Line did not have proper amount of parts: " + parts);
					continue;
				}

				var nodeFromX = parts[0].ToInt(-1);
				var nodeFromY = parts[1].ToInt(-1);
				var nodeToX = parts[3].ToInt(-1);
				var nodeToY = parts[4].ToInt(-1);
				var edgeName = parts[2];

				var nodeFrom = new CoordinateNode(new Point(nodeFromX, nodeFromY));
				var nodeTo = new CoordinateNode(new Point(nodeToX, nodeToY));

				var weight = Point.DistanceBetween(nodeFrom.Point, nodeTo.Point);

				var edge = new WeightedNamedDirectedEdge<ICoordinateNode>(edgeName, nodeFrom, nodeTo, weight);

				graph.AddNode(nodeFrom).AddNode(nodeTo).AddEdge(edge);
			}

			return graph;
		}

		public static IDirectedGraph<ICoordinateNode, IWeightedNamedDirectedEdge<ICoordinateNode>> FromInput(string inputContent) {
			using (var ms = new MemoryStream()) {
				var buffer = Encoding.Default.GetBytes(inputContent);
				ms.Write(buffer, 0, buffer.Length);
				ms.Position = 0;
				return FromInput(new StreamReader(ms));
			}
		}
	}
}
