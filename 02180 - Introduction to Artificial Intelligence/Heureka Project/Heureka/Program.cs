using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Heaps;

namespace Heureka {
    class Program {
        static void Main(string[] args) {
	        var heap = new KeyValueHeap<string, int>(Comparer<int>.Default);

	        for (var i = 0; i < 25; i++) {
		        heap.Insert("Key#" + i, i);
	        }
	        Console.WriteLine(heap.ToString());
	        Console.ReadKey(true);
        }
    }
}
