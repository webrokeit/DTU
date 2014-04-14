using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Heaps {
	public interface IKeyValueHeap<TKey, TValue> : IEnumerable<TValue> {
		IComparer<TValue> Comparer { get; }
		bool Empty { get; }

		void Insert(TKey key, TValue value);
		void SetValue(TKey key, TValue value);
		TValue Remove(TKey key);
		TValue RemoveRoot();
		TKey RemoveRootKey();
		TValue Peek();
		TKey PeekKey();

		bool ContainsKey(TKey key);
	}
}
