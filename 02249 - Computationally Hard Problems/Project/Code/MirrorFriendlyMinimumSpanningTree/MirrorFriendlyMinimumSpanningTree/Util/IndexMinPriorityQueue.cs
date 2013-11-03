using System;

namespace MFMSTProject.Util {
    public class IndexMinPriorityQueue<T> where T : IComparable<T> {
        private readonly int[] _priorityQueue;
        private readonly int[] _priorityQueueReverse;
        private readonly T[] _keys;

        public bool IsEmpty {get { return Size == 0; }}
        public int Size { get; private set; }

        public T Minimum {
            get { return _keys[MinimumIndex]; }
        }

        public int MinimumIndex {
            get { return _priorityQueue[1]; }
        }

        public IndexMinPriorityQueue(int items) {
            _priorityQueue = new int[items + 1];
            _priorityQueueReverse = new int[items + 1];
            _keys = new T[items + 1];
            for (var i = 0; i <= items; i++) _priorityQueueReverse[i] = -1;
        }

        public void Insert(int index, T item) {
            Size++;
            _priorityQueueReverse[index] = Size;
            _priorityQueue[Size] = index;
            _keys[index] = item;
            Swim(Size);
        }

        public bool Contains(int index) {
            return _priorityQueueReverse[index] != -1;
        }

        public void Change(int index, T item) {
            _keys[index] = item;
            Swim(_priorityQueueReverse[index]);
            Sink(_priorityQueueReverse[index]);
        }

        public int DeleteMinimum() {
            var minIndex = MinimumIndex;
            Exchange(1, Size--);
            Sink(1);
            _keys[_priorityQueue[Size + 1]] = default(T);
            _priorityQueueReverse[_priorityQueue[Size + 1]] = -1;
            return minIndex;
        }

        private bool Bigger(int x, int y) {
            return _keys[_priorityQueue[x]].CompareTo(_keys[_priorityQueue[y]]) > 0;
        }

        private void Exchange(int x, int y) {
            var temp = _priorityQueue[x];
            _priorityQueue[x] = _priorityQueue[y];
            _priorityQueue[y] = temp;
            _priorityQueueReverse[_priorityQueue[x]] = x;
            _priorityQueueReverse[_priorityQueue[y]] = y;
        }

        private void Swim(int index) {
            while (index > 1 && Bigger(index / 2, index)) {
                Exchange(index, index / 2);
                index = index / 2;
            }
        }

        private void Sink(int index) {
            while (index * 2 <= Size) {
                var childIndex = index * 2;
                if (childIndex < Size && Bigger(childIndex, childIndex + 1)) childIndex++;
                if (!Bigger(index, childIndex)) break;
                Exchange(index, childIndex);
                index = childIndex;
            }
        }
    }
}
