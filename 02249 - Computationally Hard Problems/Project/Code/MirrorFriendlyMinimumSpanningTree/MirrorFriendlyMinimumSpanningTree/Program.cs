using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MFMSTProject.MFMST;

//using MirrorFriendlyMinimumSpanningTree.MST;
using System.Collections.Generic;

namespace MFMSTProject {
    class Program {
        static void Main(string[] args) {
            EdgeWeightedGraph G = null;
            if (Console.IsInputRedirected && Console.In.Peek() > 0) {
                G = EdgeWeightedGraph.FromStream(Console.In, true);
            } else {
                using (var fs = new FileStream("TestFiles/test03.uwg", FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    using (var sr = new StreamReader(fs)) {
                        G = EdgeWeightedGraph.FromStream(sr, false);
                    }
                }
            }

            var mst = new MirrorFriendlyMinimumSpanningTree(G);
			var timings = new List<long> ();

			for (var i = 0; i < 1000; i++) {
				var t = new Stopwatch ();
				t.Start ();
				mst.Run ();
				t.Stop ();
				timings.Add (t.ElapsedMilliseconds);
				Console.WriteLine ("Done with #" + (i + 1));
			}

			Console.WriteLine ("Worst running time: " + timings.Max ());
			Console.WriteLine ("Mean running time: " + (timings.Sum () / timings.Count));
			Console.WriteLine ("Best running time: " + timings.Min ());

            //Console.WriteLine("Done, time taken: " + t.ElapsedMilliseconds + " ms");
            Console.WriteLine("Solution:");
			foreach (var edge in mst.OrderBy(edge => edge.Id)) {
				Console.WriteLine(edge + ", mirrored weight: " + mst.MirrorEdge(edge).Weight);
			}


            Console.WriteLine("");
            Console.WriteLine("Solution value: " + mst.Weight + " / " + mst.MirrorWeight);
            if (!Console.IsInputRedirected) {
                Console.WriteLine("");
                Console.WriteLine("Program done, press any key to exit..");
                Console.ReadKey(true);
            }
        }
    }
}
