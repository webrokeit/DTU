using System;
using System.Collections.Generic;

namespace Heureka
{
	public static class ArgumentsFactory
	{
		public static IDictionary<string, string> ArgumentsAsDictionary(string[] args){
			var dict = new Dictionary<string, string> ();

			if (args != null && args.Length > 0) {
				var i = 0; 
				while (i < args.Length - 1) {
					var key = args [i++];
					if (key [0] == '-') {
						key = key.Substring (1);
					}
					dict [key] = args [i++];
				}
			}

			return dict;
		}
	}
}

