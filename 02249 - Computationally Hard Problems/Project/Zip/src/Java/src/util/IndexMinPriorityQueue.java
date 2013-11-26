package util;

/*
 * Indexed Minimum Priority Queue, found in Sedgewick & Wayne, Algorithms (4th ED, 2011)
 * Written by:
 * 	Andreas Kjeldsen (s092638),
 * 	Morten Eskesen (s133304)
 */
public class IndexMinPriorityQueue<T extends Comparable<T>> {
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
    
    public boolean isEmpty(){
    	return getSize() == 0;
    }
    
    public int getSize(){
    	return _size;
    }
    
    public T getMinimum(){
    	return _keys[getMinimumIndex()];
    }
    
    public int getMinimumIndex(){
    	return _priorityQueue[1];
    }
    
    public void insert(int index, T item) {
        _size++;
        _priorityQueueReverse[index] = _size;
        _priorityQueue[_size] = index;
        _keys[index] = item;
        swim(_size);
    }

    public boolean contains(int index) {
        return _priorityQueueReverse[index] != -1;
    }

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

    private boolean bigger(int x, int y) {
        return _keys[_priorityQueue[x]].compareTo(_keys[_priorityQueue[y]]) > 0;
    }

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
