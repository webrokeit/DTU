package mfmst;

public class Edge implements Comparable<Edge> {
	private int _id = 0;
	private int _vertex1 = 0;
	private int _vertex2 = 0;
	private int _weight = 0;

    public Edge(int id, int vertex1, int vertex2, int weight) {
        if (vertex1 == vertex2) throw new IllegalArgumentException("The vertices cannot be the same");
        _id = id;
        // The -1 is to make the vertex id a zero based index
        if (vertex1 < vertex2) {
            _vertex1 = vertex1-1;
            _vertex2 = vertex2-1;
        } else {
            _vertex1 = vertex2-1;
            _vertex2 = vertex1-1;
        }
        _weight = weight;
    }
    
    public int getId(){
    	return _id;
    }
    
    public int getVertex1(){
    	return _vertex1;
    }
    
    public int getVertex2(){
    	return _vertex2;
    }
    
    public int getWeight(){
    	return _weight;
    }
    
    public int OtherVertex(int vertex) {
        return vertex == _vertex1 ? _vertex2 : vertex == _vertex2 ? _vertex1 : -1;
    }

    public int compareTo(Edge other) {
        return _weight < other._weight ? -1 : _weight > other._weight ? 1 : 0;
    }

    public String toString() {
        return String.format("Edge {0:d}: {4}{1:d}, {2:d}{5}; weight: {3:d}", _id, _vertex1+1, _vertex2+1, _weight, "{", "}");
    }
}
