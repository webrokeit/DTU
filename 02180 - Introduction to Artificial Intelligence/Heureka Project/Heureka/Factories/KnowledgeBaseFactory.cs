using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Extensions;
using Archimedes.Graph;
using Archimedes.Logic;
using Archimedes;
using System.Text.RegularExpressions;

namespace Heureka.Factories {
    public static class KnowledgeBaseFactory {
        public static IKnowledgeBase FromInput(Stream input) {
            return FromInput(new StreamReader(input));
        }

        public static IKnowledgeBase FromInput(StreamReader input) {
            var kb = new KnowledgeBase();

            while (!input.EndOfStream) {
                var line = input.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;

				var clause = ClauseFromLine (line);

                kb.AddClause(clause);
            }

            return kb;
        }

		public static IClause ClauseFromLine(string line){
			line = CleanLine (line);
			var parts = line.ToLower().Split(' ');
			var head = new Literal(parts[0]);
			var clause = new Clause(head);

			if (parts.Length > 1) {
				var i = 1;
				if (parts [1] == "if") {
					if (parts.Length < 2) return clause;
					i = 2;
				} else {
					clause = new Clause ();
					clause.Body.Add (head);
				}

				while (i < parts.Length) {
					var literal = new Literal (parts [i++]);
					clause.Body.Add (literal);
				}

			    if (clause.Head == null && clause.Body.Count == 1) {
			        clause = new Clause(clause.Body.First());
			    }
			}

			return clause;
		}

		public static IQuery QueryFromLine(string line){
			var query = new Query();
			line = CleanLine (line);

		    if (!string.IsNullOrEmpty(line)) {
                var parts = line.ToLower().Split(' '); 
                for (var i = 0; i < parts.Length; i++) {
                    parts[i] = parts[i].Trim();
                    if (string.IsNullOrEmpty(parts[i])) continue;
                    var literal = new Literal(parts[i]);
                    query.Literals.Add(literal);
                }
		    }

			return query;
		}

		private static readonly Regex cleanLineRegex = new Regex (@"[^a-zA-Z_0-9 !-]+", RegexOptions.Compiled);
		private static string CleanLine(string line){
			return cleanLineRegex.Replace (line, string.Empty).Trim();
		}
    }
}
