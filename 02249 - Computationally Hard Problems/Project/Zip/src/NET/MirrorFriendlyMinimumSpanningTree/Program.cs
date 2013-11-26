using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MFMSTProject.MFMST;
using MFMSTProject.Util;

using System.Collections.Generic;

namespace MFMSTProject {
    class Program {
        static void Main(string[] args) {
			// Default timeout of 20 seconds (based on the contest description)
			var maxExecutionTime = 20000;

            EdgeWeightedGraph G = null;
			var t = new Stopwatch ();
			t.Start ();

			if (Console.IsInputRedirected && Console.In.Peek () > 0) {
				G = EdgeWeightedGraph.FromStream(Console.In, true);
				if(args.Length > 0) maxExecutionTime = args[0].ToInt(maxExecutionTime);
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
					Console.WriteLine ("Usage: MFMSTProject.exe <UWG_filename> <timeout_in_ms>");
					Console.WriteLine ("Tried to open file, but it did not exists: " + fi.FullName);
					return;
				}
				if(args.Length > 1) maxExecutionTime = args[1].ToInt(maxExecutionTime);
            }

            var mst = new MirrorFriendlyMinimumSpanningTree(G);
			mst.Run (maxExecutionTime);
			t.Stop ();

            Console.WriteLine("Time taken: " + t.ElapsedMilliseconds + " ms" + (mst.TerminatedPrematurely ? " [TIME BOUND TERMINATION]" : ""));
			Console.WriteLine("Solution:");
			if (mst.IsMst) {
				foreach (var edge in mst.OrderBy(edge => edge.Id)) {
					Console.WriteLine (edge + ", mirrored weight: " + mst.MirrorEdge (edge).Weight);
				}

				Console.WriteLine ("");
				Console.WriteLine ("Solution value: " + mst.Weight + " / " + mst.MirrorWeight);
			} else {
				Console.WriteLine ("No MFMST found!");
			}

            Console.WriteLine("Program done, press any key to exit..");
            Console.ReadKey(true);
        }
    }
}
