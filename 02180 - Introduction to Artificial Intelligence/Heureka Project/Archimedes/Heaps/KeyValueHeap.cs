using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Inspiration: http://www.informit.com/guides/content.aspx?g=dotnet&seqNum=789

namespace Archimedes.Heaps {
	public class KeyValueHeap<TKey, TValue> : IKeyValueHeap<TKey, TValue> {

		public IComparer<TValue> Comparer { get; private set; }
		public int Count { get { return _items.Count; } }
		public bool Empty {get { return Count < 1; }}

		public TValue this[TKey key] {
			get { return _dictionary[key]; }
		}

		private readonly IList<TKey> _items;
		private readonly IDictionary<TKey, TValue> _dictionary;

		public KeyValueHeap(IComparer<TValue> comparer) {
			Comparer = comparer;
			_items = new List<TKey>();
			_dictionary = new Dictionary<TKey, TValue>();
		}

		public void Insert(TKey key, TValue value) {
			if (ContainsKey(key)) {
				throw new ArgumentException("Key is already used", "key");
			}
			var i = Count;

			_dictionary[key] = value;
			_items.Add(key);
			while (i > 0 && Comparer.Compare(_dictionary[_items[(i - 1) / 2]], value) > 0) {
				_items[i] = _items[(i - 1) / 2];
				i = (i - 1) / 2;
			}

			_items[i] = key;
		}

		public void SetValue(TKey key, TValue value) {
			_dictionary[key] = value;
			Rebuild();
		}

		private void Rebuild() {
			for (var i = Count/2; i >= 0; i--) {
				PercolateDown(i);
			}
		}

		private void PercolateDown(int index) {
			var tmpKey = _items[index];
			var tmp = _dictionary[tmpKey];

			var i = index;
			while (i < Count / 2) {
				var j = (2 * i) + 1;
				if (j < Count - 1 && Comparer.Compare(_dictionary[_items[j]], _dictionary[_items[j + 1]]) > 0) {
					++j;
				}
				if (Comparer.Compare(_dictionary[_items[j]], tmp) >= 0) break;
				_items[i] = _items[j];
				i = j;
			}
			_items[i] = tmpKey;
		}

		public TValue Remove(TKey key) {
			var value = _dictionary[key];
			_dictionary.Remove(key);
			for (var i = 0; i < Count; i++) {
				if (!_items[i].Equals(key)) continue;
				_items[i] = _items[Count - 1];
				_items.RemoveAt(Count - 1);
				if (i < Count) {
					Console.WriteLine("Percolating!");
					PercolateDown(i);
				}
				Console.WriteLine("Deleted it!");
				break;
			}
			return value;
		}

		public TValue Peek() {
			if (Count < 1) throw new InvalidOperationException("The heap is empty.");
			return _dictionary[_items[0]];
		}

		public TKey PeekKey() {
			if (Count < 1) throw new InvalidOperationException("The heap is empty.");
			return _items[0];
		}

		public TValue RemoveRoot() {
			if (Count < 1) throw new InvalidOperationException("The heap is empty.");

			var root = _dictionary[_items[0]];
			RemoveRootKey();
			return root;
		}

		public TKey RemoveRootKey() {
			if (Count < 1) throw new InvalidOperationException("The heap is empty.");

			var rootKey = _items[0];
			_dictionary.Remove(rootKey);
			_items[0] = _items[Count - 1];
			_items.RemoveAt(Count - 1);

			if (Count > 0) {
				PercolateDown(0);
			}

			return rootKey;
		}

		public bool ContainsKey(TKey key) {
			return _dictionary.ContainsKey(key);
		}

		public void Clear() {
			_items.Clear();
			_dictionary.Clear();
		}

		public IEnumerator<TValue> GetEnumerator() {
			return _items.Select(key => _dictionary[key]).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public override string ToString() {
			var cnt = 0;
			var i = 0;
			var breakLineAt = 1;
			var sb = new StringBuilder();
			while (i < Count) {
				sb.Append("{" + _items[i] + ": " + _dictionary[_items[i]] + "}");

				i++;
				cnt++;
				if (i < Count) {
					if (cnt == breakLineAt) {
						sb.AppendLine();
						breakLineAt *= 2;
						cnt = 0;
					} else {
						sb.Append(", ");
					}
				}
			}
			return sb.ToString();
		}
	}
}
