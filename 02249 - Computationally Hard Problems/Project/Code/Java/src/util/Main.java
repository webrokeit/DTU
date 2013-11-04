package util;

import java.io.File;
import java.io.FileInputStream;
import java.util.Scanner;

import mfmst.EdgeWeightedGraph;


public class Main {

	public static void main(String[] args) {
		// Default timeout of 20 seconds (based
		int maxExecutionTime = 20000;

        EdgeWeightedGraph G = null;
		long t = System.currentTimeMillis();

		if (System.in != null) {
			Scanner scan = new Scanner(System.in);
			G = EdgeWeightedGraph.fromStream(scan, true);
			scan.close();
			if(args.length > 0) maxExecutionTime = StringFunc.toInt(args[0], maxExecutionTime);
        } else {
			String fileName = args.length > 0 ? args [0] : "TestFiles/test03.uwg";
			File fi = new File (fileName);
			if (fi.exists()) {
				Scanner scan = new Scanner(new FileInputStream(fi));
				G = EdgeWeightedGraph.fromStream (scan, false);
				scan.close();
			} else {
				System.out.println("Usage: MFMSTProject.exe <UWG_filename> <timeout_in_ms>");
				System.out.println("Tried to open file, but it did not exists: " + fi.getAbsolutePath());
				return;
			}
			if(args.length > 1) maxExecutionTime = StringFunc.toInt(args[1], maxExecutionTime);
        }

        MirrorFriendlyMinimumSpanningTree mst = new MirrorFriendlyMinimumSpanningTree(G);
		mst.Run (maxExecutionTime);
		long tEnd = System.currentTimeMillis();

		System.out.println("Time taken: " + (tEnd - t) + " ms" + (mst.TerminatedPrematurely ? " [TIME BOUND TERMINATION]" : ""));
		System.out.println("Solution:");
		foreach (var edge in mst.OrderBy(edge => edge.Id)) {
			System.out.println(edge + ", mirrored weight: " + mst.MirrorEdge(edge).Weight);
		}

		System.out.println("");
		System.out.println("Solution value: " + mst.Weight + " / " + mst.MirrorWeight);
	}

}
