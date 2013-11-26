package mfmst;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashSet;
import java.util.LinkedList;
import java.util.Set;
import util.IndexMinPriorityQueue;

/*
 * Highly customized version Prims eager MST algorithm, found in Sedgewick & Wayne, Algorithms (4th ED, 2011)
 * Customized to find Mirror Friendly Minimum Spanning Trees
 * Customized to find the MFMST with the lowest weight (regular and mirrored)
 * Written by:
 * 	Andreas Kjeldsen (s092638),
 * 	Morten Eskesen (s133304)
*/
public class MirrorFriendlyMinimumSpanningTree {
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
    
    public boolean isTerminatedPrematurely(){
    	return _terminatedPrematurely;
    }
    
    public Edge[] getEdgeTo(){
    	return _edgeTo;
    }
    
    public boolean isMst(){
    	for(int i = 1; i < _edgeTo.length; i++){
    		if(_edgeTo[i] == null) return false;
    	}
    	return true;
    }
    
    public int getWeight(){
    	int weight = 0;
    	for(int i = 1; i < _edgeTo.length; i++){
    		weight += _edgeTo[i].getWeight();
    	}
    	return weight;
    }
    
    public int getMirrorWeight(){
    	int weight = 0;
    	for(int i = 1; i < _edgeTo.length; i++){
    		weight += getMirrorEdge(_edgeTo[i]).getWeight();
    	}
    	return weight;
    }

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
				} catch(InterruptedException ex) { 
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
        
        if(isMst()){
	        _threshold = Math.max(getWeight(), getMirrorWeight());
	
	        Set<Integer> reqEdges = getRequiredEdgeIds();
	        _edgeToResult = cloneEdgeTo();
	        ArrayList<Edge> edgeToClone = new ArrayList<Edge>();
	        for(Edge edge : _edgeToResult){
	        	if(edge != null && !reqEdges.contains(edge.getId())){
	        		edgeToClone.add(edge);
	        	}
	        }
	        Collections.sort(edgeToClone);
	
			if (!_terminatedPrematurely) {
				permutateMstEdges(edgeToClone.toArray(new Edge[edgeToClone.size()]));
			}
        }
        _edgeTo = _edgeToResult;
        _edgeToResult = null;
		if (!_terminatedPrematurely && t.isAlive()) t.interrupt();
    }
    
    private void permutateMstEdges(Edge[] edges){
    	permutateMstEdges(edges, 0, new HashSet<Integer>(), new LinkedList<Set<Integer>>());
    }
    
    private void permutateMstEdges(Edge[] edges, int index, Set<Integer> excludes, LinkedList<Set<Integer>> breakers){
	    if (index >= edges.length) {
	    	if(excludes.size() < 1) return;
	    	reset ();
			makeMst (excludes);
			if (_terminatedPrematurely) return;
			if (isMst()) {
				// Save the weights in local variables as it's calculated using
				// enumeration which can be time consuming.
				int weight = getWeight();
				int mirrorWeight = getMirrorWeight();
				if (Math.max (weight, mirrorWeight) < _threshold) {
					// Update our threshold value.
					_threshold = Math.max (weight, mirrorWeight);
					_edgeToResult = cloneEdgeTo ();
				} else if (Math.min (weight, mirrorWeight) >= _threshold) {
					// If the current set of excludes causes the lowest weight
					// to be larger than our current threshold then we obviously
					// made a wrong choice excluding the edges and would like not
					// to make the same mistakes again.
					breakers.add (new HashSet<Integer> (excludes));
				} 
			} else {
				// The current set of excludes makes it impossible for the graph
				// to contain a MST, do not make the same mistake again.
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
    	for(Set<Integer> child : children){
    		if(isSubset(child, parent)){
    			return true;
    		}
    	}
    	return false;
    }
    
    private boolean isSubset(Set<Integer> child, Set<Integer> parent){
    	for(Integer i : child){
    		if(!parent.contains(i)){
    			return false;
    		}
    	}
    	return true;
    }
    
    private void makeMst(Set<Integer> excludes) {
		while (!_terminatedPrematurely && !_priorityQueue.isEmpty()) {
            visit(_graph, _priorityQueue.deleteMinimum(), excludes);
        }
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

	// Required edges are edges that are the only link between a vertex
	// and the rest of the graph (single path to the vertex).
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

        // Copy the Integer.MAX_VALUE to DistTo by copying the bytes - *should* be faster
        System.arraycopy(_baseDistTo, 1, _distTo, 1, _graph.getVertexCount()-1);
        _priorityQueue.insert(0, 0); 
    }

    public Edge getMirrorEdge(Edge edge) {
        return _graph.getEdge(_graph.getEdgeCount() - edge.getId());
    }
}