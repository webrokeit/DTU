using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Inspiration: http://www.informit.com/guides/content.aspx?g=dotnet&seqNum=789

namespace Archimedes.Heaps {
	public class Heap<T> : IHeap<T> {
		public IComparer<T> Comparer { get; private set; }
		public int Count { get { return _items.Count; } }
		public bool Empty {get { return Count < 1; }}

		private readonly IList<T> _items;

		public Heap(IComparer<T> comparer) {
			Comparer = comparer;
			_items = new List<T>();
		}  

		public void Insert(T item) {
			var i = Count;
			_items.Add(item);
			while (i > 0 && Comparer.Compare(_items[(i - 1) / 2], item) > 0) {
				_items[i] = _items[(i - 1) / 2];
				i = (i - 1) / 2;
			}
			_items[i] = item;
		}

		public T Peek() {
			if (_items.Count < 1) throw new InvalidOperationException("The heap is empty.");
			return _items[0];
		}

		public T RemoveRoot() {
			if (_items.Count < 1) throw new InvalidOperationException("The heap is empty.");

			var res = _items[0];
			var tmp = _items[_items.Count - 1];
			_items.RemoveAt(_items.Count - 1);

			if (_items.Count < 1) return res;
			var i = 0;
			while (i < _items.Count / 2) {
				var j = (2 * i) + 1;
				if ((j < _items.Count - 1) && (Comparer.Compare(_items[j], _items[j + 1]) > 0)) {
					++j;
				}
				if (Comparer.Compare(_items[j], tmp) >= 0) break;
				_items[i] = _items[j];
				i = j;
			}
			_items[i] = tmp;

			return res;
		}

		public void Clear() {
			_items.Clear();
		}

		public IEnumerator<T> GetEnumerator() {
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
