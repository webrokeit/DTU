import java.io.*;
import java.util.*;

public class MFMST {
	public static void main(String[] args) throws FileNotFoundException {
		// Default timeout of 20 seconds (based on the contest description)
		int maxExecutionTime = 20000;
        EdgeWeightedGraph G = null;
		String fileName = args.length > 0 ? args [0] : "";
		if(fileName.length() < 1){
			System.out.println("Usage: java MFMST <UWG_filename> <timeout_in_ms>");
			System.out.println("No file specified.");
			return;
		}
		File fi = new File (fileName);
		if (fi.exists()) {
			Scanner scan = new Scanner(new FileInputStream(fi));
			G = new EdgeWeightedGraph(scan);
			scan.close();
		} else {
			System.out.println("Usage: java MFMST <UWG_filename> <timeout_in_ms>");
			System.out.println("Tried to open file, but it did not exists: " + fi.getAbsolutePath());
			return;
		}
		if(args.length > 1) maxExecutionTime = toInt(args[1], maxExecutionTime);

        MirrorFriendlyMinimumSpanningTree mst = new MirrorFriendlyMinimumSpanningTree(G);
		mst.run (maxExecutionTime);

		if(mst.isMst()){
			System.out.print(mst.getWeight());
			for(int i = 1; i < mst.getEdgeTo().length; i++){
				System.out.print(" " + mst.getEdgeTo()[i].getId());
			}
		}else{
			System.out.println("0");
		}
	}
	public static int toInt(String parse, int fallback){
		try{ return Integer.parseInt(parse, 10);
		}catch(NumberFormatException nfe){ return fallback; }
	}
	public static class Edge implements Comparable<Edge> {
		private int _id = 0;
		private int _vertex1 = 0;
		private int _vertex2 = 0;
		private int _weight = 0;
	    public Edge(int id, int vertex1, int vertex2, int weight) {
	        if (vertex1 == vertex2) throw new IllegalArgumentException("The vertices cannot be the same");
	        _id = id;
	        if (vertex1 < vertex2) {
	            _vertex1 = vertex1-1; _vertex2 = vertex2-1;
	        } else {
	            _vertex1 = vertex2-1; _vertex2 = vertex1-1;
	        }
	        _weight = weight;
	    }
	    public int getId(){ return _id;}
	    public int getVertex1(){ return _vertex1; }
	    public int getVertex2(){ return _vertex2; }
	    public int getWeight(){ return _weight; }
	    public int getOtherVertex(int vertex) { return vertex == _vertex1 ? _vertex2 : vertex == _vertex2 ? _vertex1 : -1; }
	    public int compareTo(Edge other) { return _weight < other._weight ? -1 : _weight > other._weight ? 1 : 0; }
	}
	public static class EdgeWeightedGraph {
		private int _vertices;
		private LinkedList<Edge>[] _edges;
		private ArrayList<Edge> _allEdges = new ArrayList<Edge>();
	    @SuppressWarnings("unchecked")
		public EdgeWeightedGraph(Scanner stream) {
	        _vertices = stream.nextInt();
	        if (_vertices < 1) throw new IllegalArgumentException("There must be at least one vertex in a graph");
	        _edges = (LinkedList<Edge>[]) new LinkedList[_vertices];
	        for (int i = 0; i < _vertices; i++) _edges[i] = new LinkedList<Edge>();
	        int requiredEdgeCount = stream.nextInt();
	        String line = stream.hasNextLine() ? stream.nextLine() : null;
	        while (stream.hasNextLine() && (line = stream.nextLine()) != null) {
	            String[] parts = line.split(" ");
	            if (parts.length < 3) throw new IllegalArgumentException("Each line for the edges must contain three numbers: int int int");
	            int vertex1 = toInt(parts[0], 0);
	            int vertex2 = toInt(parts[1], 0);
	            int weight = toInt(parts[2], 0);
	            Edge edge = new Edge(getEdgeCount() + 1, vertex1, vertex2, weight);
	            addEdge(edge);
	        }
	        if (requiredEdgeCount != getEdgeCount()) throw new IllegalArgumentException("Amount of edges read and the amount of edges supposed to be there does not match.");
	    }
	    public void addEdge(Edge edge) {
	        _edges[edge.getVertex1()].add(edge);
	        _edges[edge.getVertex2()].add(edge);
	        _allEdges.add(edge);
	    }
	    public Edge getEdge(int index) { return _allEdges.get(index); }
	    public int getEdgeCount(){ return _allEdges.size(); }
	    public int getVertexCount(){ return _vertices; }
	    public Iterable<Edge> getAdjacent(int vertex){ return _edges[vertex]; }
	}
	public static class MirrorFriendlyMinimumSpanningTree {
	    private IndexMinPriorityQueue<Integer> _priorityQueue = null;
	    private EdgeWeightedGraph _graph = null;
	    private int[] _baseDistTo = null;
	    private boolean _terminatedPrematurely = false;
	    private Edge[] _edgeTo = null;
	    private Edge[] _edgeToResult = null;
	    private int[] _distTo = null;
	    private boolean[] _marked = null;
	    private int _threshold = 0;
	    public MirrorFriendlyMinimumSpanningTree(EdgeWeightedGraph graph) {
	        _graph = graph;
	        _baseDistTo = new int[_graph.getVertexCount()];
	        for (int i = 0; i < _baseDistTo.length; i++) _baseDistTo[i] = Integer.MAX_VALUE;
	        reset();
	    }
	    public boolean isTerminatedPrematurely(){ return _terminatedPrematurely; }
	    public Edge[] getEdgeTo(){ return _edgeTo; }
	    public boolean isMst(){
	    	for(int i = 1; i < _edgeTo.length; i++) if(_edgeTo[i] == null) return false;
	    	return true;
	    }
	    public int getWeight(){
	    	int weight = 0;
	    	for(int i = 1; i < _edgeTo.length; i++) weight += _edgeTo[i].getWeight();
	    	return weight;
	    }
	    public int getMirrorWeight(){
	    	int weight = 0;
	    	for(int i = 1; i < _edgeTo.length; i++) weight += getMirrorEdge(_edgeTo[i]).getWeight();
	    	return weight;
	    }
	    public Edge getMirrorEdge(Edge edge) { return _graph.getEdge(_graph.getEdgeCount() - edge.getId()); }
	    public Edge[] cloneEdgeTo() {
	        Edge[] edgeToClone = new Edge[_edgeTo.length];
	        System.arraycopy(_edgeTo,  0, edgeToClone, 0, _edgeTo.length);
	        return edgeToClone;
	    }
	    public void run(final int maxExecutionTime) {
			Thread t = new Thread(new Runnable(){
				@Override
				public void run() {
					try{
						Thread.sleep(maxExecutionTime);
					} catch(InterruptedException ex) { return; }
					_terminatedPrematurely = true;
				}
			});
			t.setDaemon(true);
			t.start ();
			_terminatedPrematurely = false;
	        makeMst(new HashSet<Integer>());
	        if(isMst()){
		        _threshold = Math.max(getWeight(), getMirrorWeight());
		        Set<Integer> reqEdges = getRequiredEdgeIds();
		        _edgeToResult = cloneEdgeTo();
		        ArrayList<Edge> edgeToClone = new ArrayList<Edge>();
		        for(Edge edge : _edgeToResult) if(edge != null && !reqEdges.contains(edge.getId())) edgeToClone.add(edge);
		        Collections.sort(edgeToClone);
		        if (!_terminatedPrematurely) permutateMstEdges(edgeToClone.toArray(new Edge[edgeToClone.size()]));
	        }
	        _edgeTo = _edgeToResult;
	        _edgeToResult = null;
			if (!_terminatedPrematurely && t.isAlive()) t.interrupt();
	    }
	    private void permutateMstEdges(Edge[] edges){ permutateMstEdges(edges, 0, new HashSet<Integer>(), new LinkedList<Set<Integer>>()); }
	    private void permutateMstEdges(Edge[] edges, int index, Set<Integer> excludes, LinkedList<Set<Integer>> breakers){
		    if (index >= edges.length) {
		    	if(excludes.size() < 1) return;
		    	reset (); makeMst (excludes);
				if (_terminatedPrematurely) return;
				if (isMst()) {
					int weight = getWeight();
					int mirrorWeight = getMirrorWeight();
					if (Math.max (weight, mirrorWeight) < _threshold) {
						_threshold = Math.max (weight, mirrorWeight);
						_edgeToResult = cloneEdgeTo ();
					} else if (Math.min (weight, mirrorWeight) >= _threshold) {
						breakers.add (new HashSet<Integer> (excludes));
					} 
				} else {
					breakers.add (new HashSet<Integer> (excludes));
				}
		    } else {
		    	permutateMstEdges(edges, index + 1, excludes, breakers);
				if (_terminatedPrematurely) return;
		    	excludes.add(edges[index].getId());
		    	if(!containsSubset(breakers, excludes)){
		    		permutateMstEdges(edges, index + 1, excludes, breakers);
					if (_terminatedPrematurely) return;
		    	}
		    	excludes.remove(edges[index].getId());
		    }
		}
	    private boolean containsSubset(LinkedList<Set<Integer>> children, Set<Integer> parent){
	    	for(Set<Integer> child : children) if(isSubset(child, parent)) return true;
	    	return false;
	    }
	    private boolean isSubset(Set<Integer> child, Set<Integer> parent){
	    	for(Integer i : child) if(!parent.contains(i)) return false;
	    	return true;
	    }
	    private void makeMst(Set<Integer> excludes) {
			while (!_terminatedPrematurely && !_priorityQueue.isEmpty()) visit(_graph, _priorityQueue.deleteMinimum(), excludes);
	    }
	    private void visit(EdgeWeightedGraph graph, int vertex, Set<Integer> excludes) {
	        _marked[vertex] = true;
	        for (Edge edge : graph.getAdjacent(vertex)) {
	            if(excludes.contains(edge.getId())) continue;
	        	int other = edge.getOtherVertex(vertex);
	            if (_marked[other] || edge.getWeight() >= _distTo[other]) continue;
	            _edgeTo[other] = edge;
	            _distTo[other] = edge.getWeight();
	            if (_priorityQueue.contains(other)) {
	                _priorityQueue.change(other, _distTo[other]);
	            } else {
	                _priorityQueue.insert(other, _distTo[other]);
	            }
	        }
	    }
	    private Set<Integer> getRequiredEdgeIds() {
	        HashSet<Integer> set = new HashSet<Integer>();
	        int[] required = new int[_graph.getVertexCount()];
	        for(int i = 0; i < _graph.getEdgeCount(); i++) {
	            Edge edge = _graph.getEdge(i);
				required[edge.getVertex1()] = required[edge.getVertex1()] == 0 ? edge.getId() : -1;
	            required[edge.getVertex2()] = required[edge.getVertex2()] == 0 ? edge.getId() : -1;
	        }
	        for (int i = 0; i < required.length; i++) {
	            if (required[i] < 1) continue;
	            set.add(required[i]);
	        }
	        return set;
	    }
	    public void reset() {
	        _edgeTo = new Edge[_graph.getVertexCount()];
	        _distTo = new int[_graph.getVertexCount()];
	        _marked = new boolean[_graph.getVertexCount()];
	        _priorityQueue = new IndexMinPriorityQueue<Integer>(_graph.getVertexCount());
	        System.arraycopy(_baseDistTo, 1, _distTo, 1, _graph.getVertexCount()-1);
	        _priorityQueue.insert(0, 0);
	    }
	}
	public static class IndexMinPriorityQueue<T extends Comparable<T>> {
		private final int[] _priorityQueue;
	    private final int[] _priorityQueueReverse;
	    private final T[] _keys;
	    private int _size = 0;
	    @SuppressWarnings("unchecked")
		public IndexMinPriorityQueue(int items) {
	        _priorityQueue = new int[items + 1];
	        _priorityQueueReverse = new int[items + 1];
	        _keys = (T[]) new Comparable[items + 1];
	        for (int i = 0; i <= items; i++) _priorityQueueReverse[i] = -1;
	    }
	    public boolean isEmpty(){ return getSize() == 0; }
	    public int getSize(){ return _size; }
	    public T getMinimum(){ return _keys[getMinimumIndex()]; }
	    public int getMinimumIndex(){ return _priorityQueue[1]; }
	    public void insert(int index, T item) {
	        _size++;
	        _priorityQueueReverse[index] = _size;
	        _priorityQueue[_size] = index;
	        _keys[index] = item;
	        swim(_size);
	    }
	    public boolean contains(int index) { return _priorityQueueReverse[index] != -1; }
	    public void change(int index, T item) {
	        _keys[index] = item;
	        swim(_priorityQueueReverse[index]);
	        sink(_priorityQueueReverse[index]);
	    }
	    public int deleteMinimum() {
	        int minIndex = getMinimumIndex();
	        exchange(1, _size--);
	        sink(1);
	        _keys[_priorityQueue[_size + 1]] = null;
	        _priorityQueueReverse[_priorityQueue[_size + 1]] = -1;
	        return minIndex;
	    }
	    private boolean bigger(int x, int y) { return _keys[_priorityQueue[x]].compareTo(_keys[_priorityQueue[y]]) > 0; }
	    private void exchange(int x, int y) {
	        int temp = _priorityQueue[x];
	        _priorityQueue[x] = _priorityQueue[y];
	        _priorityQueue[y] = temp;
	        _priorityQueueReverse[_priorityQueue[x]] = x;
	        _priorityQueueReverse[_priorityQueue[y]] = y;
	    }
	    private void swim(int index) {
	        while (index > 1 && bigger(index / 2, index)) {
	            exchange(index, index / 2);
	            index = index / 2;
	        }
	    }
	    private void sink(int index) {
	        while (index * 2 <= _size) {
	            int childIndex = index * 2;
	            if (childIndex < _size && bigger(childIndex, childIndex + 1)) childIndex++;
	            if (!bigger(index, childIndex)) break;
	            exchange(index, childIndex);
	            index = childIndex;
	        }
	    }
	}
}
