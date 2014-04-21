using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Extensions {
    public static class ListExtensions {
        public static void RemoveLastItems<T>(this IList<T> list, int amount) {
            if (list == null || amount < 1) return;
            if (list.Count == amount) {
                list.Clear();
            } else {
                var i = 0;
                while (i++ < amount) {
                    list.RemoveAt(list.Count - 1);
                }
            }
        }
    }
}
