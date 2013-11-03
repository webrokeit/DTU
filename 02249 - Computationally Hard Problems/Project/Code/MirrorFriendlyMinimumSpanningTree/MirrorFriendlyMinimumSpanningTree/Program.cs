using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MFMSTProject.MFMST;

//using MirrorFriendlyMinimumSpanningTree.MST;
using System.Collections.Generic;

namespace MFMSTProject {
    class Program {
        static void Main(string[] args) {;
            EdgeWeightedGraph G = null;
			var t = new Stopwatch ();
			t.Start ();
			if (Console.IsInputRedirected && Console.In.Peek () > 0) {
				G = EdgeWeightedGraph.FromStream(Console.In, true);
            } else {
				var fileName = args.Length > 0 ? args [0] : "TestFiles/test03.uwg";
				var fi = new FileInfo (fileName);
				if (fi.Exists) {
					using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
						using (var sr = new StreamReader(fs)) {
							G = EdgeWeightedGraph.FromStream (sr, false);
						}
					}
				} else {
					Console.WriteLine ("Usage: MFMSTProject.exe <UWG_filename>");
					Console.WriteLine ("Tried to open file, but it did not exists: " + fi.FullName);
					return;
				}
            }

            var mst = new MirrorFriendlyMinimumSpanningTree(G);
			mst.Run ();
			t.Stop ();

            Console.WriteLine("Time taken: " + t.ElapsedMilliseconds + " ms");
            Console.WriteLine("Solution:");
			foreach (var edge in mst.OrderBy(edge => edge.Id)) {
				Console.WriteLine(edge + ", mirrored weight: " + mst.MirrorEdge(edge).Weight);
			}

            Console.WriteLine("");
            Console.WriteLine("Solution value: " + mst.Weight + " / " + mst.MirrorWeight);
        }
    }
}
