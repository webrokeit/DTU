using System;
using System.Diagnostics;
using System.IO;
using MFMSTProject.MFMST;

//using MirrorFriendlyMinimumSpanningTree.MST;

namespace MFMSTProject {
    class Program {
        static void Main(string[] args) {
            //var G = EdgeWeightedGraph.FromStream(Console.In, !Console.IsInputRedirected);

            EdgeWeightedGraph G = null;
            if (Console.IsInputRedirected) {
                G = EdgeWeightedGraph.FromStream(Console.In, false);
            } else {
                using (var fs = new FileStream("TestFiles/test03.uwg", FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    using (var sr = new StreamReader(fs)) {
                        G = EdgeWeightedGraph.FromStream(sr, false);
                    }
                }
            }

            var mst = new MirrorFriendlyMinimumSpanningTree(G);

            var t = new Stopwatch();
            t.Start();
            mst.Run();
            t.Stop();

            Console.WriteLine("Done, time taken: " + t.ElapsedMilliseconds + " ms");
            Console.WriteLine("Solution:");
            mst.PrintSolution();

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
