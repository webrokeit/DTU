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

                var parts = line.ToLower().Split(' ');
                var head = new Literal(parts[0], null);
                var clause = new Clause(head);

                if (parts.Length > 1) {
                    if (parts[1] == "if") {
                        if (parts.Length < 2) continue;
                        for (var i = 2; i < parts.Length; i++) {
                            var literal = new Literal(parts[i], null);
                            clause.Body.Add(literal);
                        }
                    } else {
                        // We do not handle this yet!
                        continue;
                    }
                }

                kb.AddClause(clause);
            }

            return kb;
        }
    }
}
