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
	}
}
