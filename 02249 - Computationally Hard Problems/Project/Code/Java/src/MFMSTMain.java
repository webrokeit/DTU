import java.io.*;
import java.util.*;
import util.*;
import mfmst.*;

public class MFMSTMain {

	public static void main(String[] args) throws FileNotFoundException {
		// Default timeout of 20 seconds (based on the contest description)
		int maxExecutionTime = 20000;

        EdgeWeightedGraph G = null;
		long t = System.currentTimeMillis();

		String fileName = args.length > 0 ? args [0] : "bin/TestFiles/test03.uwg";
		File fi = new File (fileName);
		if (fi.exists()) {
			Scanner scan = new Scanner(new FileInputStream(fi));
			G = EdgeWeightedGraph.fromStream (scan, false);
			scan.close();
		} else {
			System.out.println("Usage: java MFMSTMain <UWG_filename> <timeout_in_ms>");
			System.out.println("Tried to open file, but it did not exists: " + fi.getAbsolutePath());
			return;
		}
		if(args.length > 1) maxExecutionTime = StringFunc.toInt(args[1], maxExecutionTime);

        MirrorFriendlyMinimumSpanningTree mst = new MirrorFriendlyMinimumSpanningTree(G);
		mst.run (maxExecutionTime);

		System.out.println("Time taken: " + (System.currentTimeMillis() - t) + " ms" + (mst.isTerminatedPrematurely() ? " [TIME BOUND TERMINATION]" : ""));
		System.out.println("Solution:");
		
		if(mst.isMst()){
			Edge[] solution = new Edge[mst.getEdgeTo().length - 1];
	        System.arraycopy(mst.getEdgeTo(),  1, solution, 0, mst.getEdgeTo().length - 1);
	  
			Arrays.sort(solution, new Comparator<Edge>(){
				@Override
				public int compare(Edge o1, Edge o2) {
					return o1.getId() < o2.getId() ? -1 : o1.getId() == o2.getId() ? 0 : 1;
				}
			});
			for(Edge edge : solution) {
				System.out.println(edge + ", mirrored weight: " + mst.getMirrorEdge(edge).getWeight());
			}
	
			System.out.println("");
			System.out.println("Solution value: " + mst.getWeight() + " / " + mst.getMirrorWeight());
		}else{
			System.out.println("No MFMST found!");
		}
	}

}
