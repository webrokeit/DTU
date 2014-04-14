using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Heaps {
	public interface IHeap<T> : IEnumerable<T> {
		IComparer<T> Comparer { get; }
		int Count { get; }
		bool Empty { get; }

		void Insert(T item);
		T RemoveRoot();
		T Peek();
		void Clear();
	}
}
