using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Graph {
    public class Point : IPoint {
        public int X { get; set; }
        public int Y { get; set; }

        public Point() { }

        public Point(int x, int y) {
            X = x;
            Y = y;
        }

        public override string ToString() {
            return "Point{" + X + ", " + Y + "}";
        }

	    public static int DistanceBetween(IPoint pointA, IPoint pointB) {
		    if (pointA == null || pointB == null) return 0;

		    var horizontalDistance = Math.Abs(pointA.X - pointB.X);
		    var verticalDistance = Math.Abs(pointA.Y - pointB.Y);

		    return (int) Math.Sqrt(horizontalDistance*horizontalDistance + verticalDistance*verticalDistance);
	    }
    }
}
