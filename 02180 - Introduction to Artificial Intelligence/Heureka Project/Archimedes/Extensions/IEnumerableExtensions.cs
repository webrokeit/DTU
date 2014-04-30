using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Extensions {
    public static class IEnumerableExtensions {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enume) {
            var set = new HashSet<T>();
            foreach (var item in enume) {
                set.Add(item);
            }
            return set;
        }

        public static HashSet<K> ToHashSet<T,K>(this IEnumerable<T> enume, Func<T,K> selector) {
            var set = new HashSet<K>();
            foreach (var item in enume) {
                set.Add(selector(item));
            }
            return set;
        }
    }
}
