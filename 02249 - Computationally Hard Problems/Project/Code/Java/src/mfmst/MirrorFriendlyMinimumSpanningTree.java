package mfmst;

import java.lang.reflect.Array;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashSet;
import java.util.Iterator;
import java.util.Set;

import util.IndexMinPriorityQueue;

public class MirrorFriendlyMinimumSpanningTree implements Iterable<Edge> {
    private IndexMinPriorityQueue<Integer> _priorityQueue = null;
    private final EdgeWeightedGraph _graph = null;
    private ArrayList<HashSet<Integer>> _breakers = null;
    private final int[] _baseDistTo = null;
    private boolean _terminatedPrematurely = false;
    private Edge[] _edgeTo = null;
    private int[] _distTo = null;
    private boolean[] _marked = null;

    public MirrorFriendlyMinimumSpanningTree(EdgeWeightedGraph graph) {
        _graph = graph;
        _baseDistTo = new int[_graph.getVertexCount()];
        for (int i = 0; i < _baseDistTo.length; i++) _baseDistTo[i] = Integer.MAX_VALUE;
        Reset();
    }
    
    public boolean isTerminatedPrematurely(){
    	return _terminatedPrematurely;
    }
    
    public Edge[] getEdgeTo(){
    	return _edgeTo;
    }
    
    public int[] getDistTo(){
    	return _distTo;
    }
    
    public boolean[] getMarked(){
    	return _marked;
    }
    
    public boolean isMst(){
    	for(Edge edge : this){
    		if(edge == null) return false;
    	}
    	return true;
    }
    
    public int getWeight(){
    	int weight = 0;
    	for(Edge edge : this){
    		weight += edge.getWeight();
    	}
    	return weight;
    }
    
    public int getMirrorWeight(){
    	int weight = 0;
    	for(Edge edge : this){
    		weight += getMirrorEdge(edge).getWeight();
    	}
    	return weight;
    }

    private Edge[] cloneEdgeTo() {
        Edge[] edgeToClone = new Edge[_edgeTo.length];
        System.arraycopy(_edgeTo,  0, edgeToClone, 0, _edgeTo.length);
        return edgeToClone;
    }

    public void run(final int maxExecutionTime) {
		Thread t = new Thread(new Runnable(){
			@Override
			public void run() {
				try{
					Thread.currentThread().sleep(maxExecutionTime);
				} catch(Exception ex) { 
					// Just return, don't set terminate
					return;
				}
				_terminatedPrematurely = true;
			}
		});
		t.setDaemon(true);
		t.start ();

		_terminatedPrematurely = false;
        makeMst(new HashSet<Integer>());
        int threshold = Math.max(getWeight(), getMirrorWeight());

        _breakers = new ArrayList<HashSet<Integer>>(); 
        HashSet<Integer> reqEdges = getRequiredEdgeIds();
        Edge[] edgeToRes = cloneEdgeTo();
        ArrayList<Edge> edgeToClone = new ArrayList<Edge>();
        for(Edge edge : edgeToRes){
        	if(edge != null && !reqEdges.contains(edge.getId())){
        		edgeToClone.add(edge);
        	}
        }
        Collections.sort(edgeToClone);


		if (!_terminatedPrematurely) {
			for(HashSet<Integer> excludes : permutateMstEdges(edgeToClone)){
				
			}
			
			foreach (var excludes in PermutateMstEdges(edgeToClone).Where(excludes => excludes.Count > 0)) {
				Reset ();
				MakeMst (excludes);
				if (TerminatedPrematurely) break;
				if (IsMst) {
					// Save the weights in local variables as it's calculated using
					// enumeration which can be time consuming.
					var weight = Weight;
					var mirrorWeight = MirrorWeight;
					if (Math.Max (weight, mirrorWeight) < threshold) {
						// Update our threshold value.
						threshold = Math.Max (weight, mirrorWeight);
						edgeToRes = CloneEdgeTo ();
					} else if (Math.Min (weight, mirrorWeight) >= threshold) {
						// If the current set of excludes causes the lowest weight
						// to be larger than our current threshold then we obviously
						// made a wrong choice excluding the edges and would like not
						// to make the same mistakes again.
						_breakers.Add (new HashSet<int> (excludes));
					} 
				} else {
					// The current set of excludes makes it impossible for the graph
					// to contain a MST, do not make the same mistake again.
					_breakers.Add (new HashSet<int> (excludes));
				}
			}
		}

        EdgeTo = edgeToRes;
		if (!TerminatedPrematurely && t.IsAlive) t.Abort ();
    }

    private void makeMst(Set<Integer> excludes) {
		while (!_terminatedPrematurely && !_priorityQueue.isEmpty()) {
            visit(_graph, _priorityQueue.deleteMinimum(), excludes);
        }
    }

    private void visit(EdgeWeightedGraph graph, int vertex, Set<Integer> excludes) {
        _marked[vertex] = true;
        for (Edge edge : graph.getEdges(vertex).Where(edge => !excludes.Contains(edge.Id))) {
            if(excludes.contains(edge.getId())) continue;
        	Edge other = edge.getOtherVertex(vertex);
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

	// Required edges are edges that are the only link between a vertex
	// and the rest of the graph (single path to the vertex).
    private Set<Integer> getRequiredEdgeIds() {
        HashSet<Integer> set = new HashSet<Integer>();
        int[] required = new int[_graph.getVertexCount()];
        for(int i = 0; i < _graph.getEdgeCount(); i++) {
            Edge edge = _graph.getEdge(i);
			required[edge.getVertex1()] = required[edge.getVertex1()] == 0 ? edge.Id : -1;
            required[edge.getVertex2()] = required[edge.getVertex2()] == 0 ? edge.Id : -1;
        }
        for (int i = 0; i < required.length; i++) {
            if (required[i] < 1) continue;
            set.add(required[i]);
        }
        return set;
    }

    public void Reset() {
        _edgeTo = new Edge[_graph.getVertexCount()];
        _distTo = new int[_graph.getVertexCount()];
        _marked = new boolean[_graph.getVertexCount()];
        _priorityQueue = new IndexMinPriorityQueue<Integer>(_graph.getVertexCount());

        // Copy the int.maxValue to DistTo by copying the bytes - *should* be faster
        System.arraycopy(_baseDistTo, 1, _distTo, 1, _graph.getVertexCount()-1);
        _priorityQueue.insert(0, 0); 
    }

    public Edge getMirrorEdge(Edge edge) {
        return _graph.getEdge(_graph.getEdgeCount() - edge.getId());
    }
    
    @Override
    public Iterator<Edge> iterator(){
    	return new Iterator<Edge>(){
    		private int index = 1;
			@Override
			public boolean hasNext() {
				return index < _edgeTo.length;
			}

			@Override
			public Edge next() {
				return _edgeTo[index++];
			}

			@Override
			public void remove() {
				// Not used
			}
    		
    	};
    }
    

    private Iterator<Set<Integer>> permutateMstEdges(Edge[] edges) {
        return permutateMstEdges(edges, 0, new HashSet<Integer>());
    }

    private Iterator<Set<Integer>> permutateMstEdges(final Edge[] edges, final int index, final Set<Integer> set) {
    	return new Iterator<Set<Integer>>(){
    		private boolean returnBreak = false;
    		private boolean tryNoAdd = false;
    		private boolean tryAdd = false;
    		private Iterator<Set<Integer>> inner = null;
    		
			@Override
			public boolean hasNext() {
				return !returnBreak && (tryNoAdd || tryAdd) && inner != null && inner.hasNext();
			}

			@Override
			public Set<Integer> next() {
				if(index >= edges.length){
					returnBreak = true;
					return set;
				}else{
					if(inner != null && !inner.hasNext()){
						inner = null;
					}
					if(inner == null && !tryNoAdd){
						tryNoAdd = true;
						inner = permutateMstEdges(edges, index + 1, set);
					}else if(inner == null && !tryAdd){
						tryAdd = true;
						set.add(edges[index].getId());
						inner = permutateMstEdges(edges, index + 1, set);
						set.remove(edges[index].getId());
					}
					return inner.next();
				}
			}

			@Override
			public void remove() {
				// Not used
			}
    		
    	};
    }
        if (index >= edges.length) {
            yield return set;
        } else {
			foreach (var perm in PermutateMstEdges(edges, index + 1, set).Where(perm => perm.Count > 0)) {
				yield return perm;
			}
			set.Add (edges [index].Id);
			if (!_breakers.Any (breakset => breakset.IsSubsetOf (set))) {
				foreach (var perm in PermutateMstEdges(edges, index + 1, set).Where(perm => perm.Count > 0)) {
					yield return perm;
				}
			}

				
			set.Remove (edges [index].Id);
        }
    }