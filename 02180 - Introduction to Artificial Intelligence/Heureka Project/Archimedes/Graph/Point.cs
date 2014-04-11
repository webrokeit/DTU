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
    }
}
