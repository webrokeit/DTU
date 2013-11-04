package mfmst;

import java.util.ArrayList;
import java.util.LinkedList;
import java.util.Scanner;

import util.StringFunc;

public class EdgeWeightedGraph /*: IEnumerable<Edge>*/ {
	private int _vertices;
	private LinkedList<Edge>[] _edges;
	private ArrayList<Edge> _allEdges = new ArrayList<Edge>();

    @SuppressWarnings("unchecked")
	public EdgeWeightedGraph(int vertices) {
        if (vertices < 1) throw new IllegalArgumentException("There must be at least one vertex in a graph");
        _vertices = vertices;
        _edges = (LinkedList<Edge>[]) new LinkedList[_vertices];
        for (int i = 0; i < vertices; i++) _edges[i] = new LinkedList<Edge>();
    }

    public void addEdge(Edge edge) {
        _edges[edge.getVertex1()].add(edge);
        _edges[edge.getVertex2()].add(edge);
        _allEdges.add(edge);
    }
    
    public Edge getEdge(int index) {
        return _allEdges.get(index);
    }
    
    public int getEdgeCount(){
    	return _allEdges.size();
    }
    
    public int getVertexCount(){
    	return _vertices;
    }
    
    public Iterable<Edge> getAdjacent(int vertex){
    	return _edges[vertex];
    }
    
    public static EdgeWeightedGraph fromStream(Scanner stream, boolean manualInput) {
        int vertices = stream.nextInt();
        int edges = stream.nextInt();
        EdgeWeightedGraph graph = new EdgeWeightedGraph(vertices);
        String line = "";

        while ((line = stream.nextLine()) != null) {
            String[] parts = line.split(" ");
            if (parts.length < 3) {
                throw new IllegalArgumentException("Each line for the edges must contain three numbers: int int int");
            }
            int vertex1 = StringFunc.toInt(parts[0]);
            int vertex2 = StringFunc.toInt(parts[1]);
            int weight = StringFunc.toInt(parts[2]);

            Edge edge = new Edge(graph.getEdgeCount() + 1, vertex1, vertex2, weight);
            graph.addEdge(edge);
            if (manualInput && edges == graph.getEdgeCount()) break;
        }

        if (edges != graph.getEdgeCount()) throw new IllegalArgumentException("Amount of edges read and the amount of edges supposed to be there does not match.");

        return graph;
    }
}
