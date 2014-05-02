using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Extensions {
	public static class StringExtensions {
		public static int ToInt(this string self) {
			return ToInt(self, 0);
		}

		public static int ToInt(this string self, int defaultValue) {
			int res;
			return int.TryParse(self, out res) ? res : defaultValue;
		}

	    public static bool Contains(this string self, char c1) {
	        return self.Any(t => t == c1);
	    }

        public static bool Contains(this string self, char c1, char c2) {
            return self.Any(t => t == c1 || t == c2);
        }
	}
}
