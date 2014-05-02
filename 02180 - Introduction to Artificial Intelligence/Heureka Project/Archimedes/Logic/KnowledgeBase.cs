using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Graph;
using Archimedes.Extensions;

namespace Archimedes.Logic {
    public class KnowledgeBase : IKnowledgeBase {
        private readonly IDirectedGraph<ILogicNode, IWeightedDirectedEdge<ILogicNode>> _graph;
		private readonly ISet<string> _satisfiedLiterals;

        public KnowledgeBase() {
            _graph = new DirectedGraph<ILogicNode, IWeightedDirectedEdge<ILogicNode>>();
			_satisfiedLiterals = new HashSet<string> ();
        }

        public void AddClause(IClause clause) {
            if (clause == null) return;
			_satisfiedLiterals.Clear ();

            if (clause.Head != null && clause.Head.Negated && clause.Body.Count > 0) {
                var newClause = new Clause(new Literal(clause.Head.NegatedValue()));
                foreach (var literal in clause.Body) {
                    newClause.Body.Add(new Literal(literal.NegatedValue()));
                }
                AddClause(newClause);
                return;
            }

            var headNode = clause.Head != null ? (_graph.GetNode(clause.Head.Value) ?? _graph.AddNode(new LiteralNode(clause.Head.Value)).GetNode(clause.Head.Value)) : null;
			
            if (clause.Body.Count < 1) {
                if (headNode == null) throw new NullReferenceException("Clause Head is null");
				headNode.Fact = true;
			} else if (clause.Body.Count == 1) {
			    if (clause.Head == null) throw new NullReferenceException("Clause Head is null");
                var literal = clause.Body.First();

				if (clause.Head.Value != literal.Value) {
                    var node = _graph.GetNode(literal.Value) ?? _graph.AddNode(new LiteralNode(literal.Value)).GetNode(literal.Value);

                    if (_graph.GetEdge(headNode, node) == null) {
						_graph.AddEdge (new WeightedDirectedEdge<ILogicNode> (headNode, node, 1));
					}
				}
            } else if (clause.Body.Count > 1) {
				if (clause.Head != null) {
					var clauseKey = ClauseKey (clause.Body);
                    var clauseNode = _graph.GetNode(clauseKey) ?? _graph.AddNode(new ClauseNode(clauseKey)).GetNode(clauseKey);

                    if (_graph.GetEdge(headNode, clauseNode) == null) {
						_graph.AddEdge (new WeightedDirectedEdge<ILogicNode> (headNode, clauseNode, clause.Body.Count));
					}

					foreach (var literalNode in clause.Body.Where(literal => clause.Head.Value != literal.Value).Select(literal => _graph.GetNode(literal.Value) ?? _graph.AddNode(new LiteralNode(literal.Value)).GetNode(literal.Value)).Where(literalNode => _graph.GetEdge(clauseNode, literalNode) == null)) {
					    _graph.AddEdge (new WeightedDirectedEdge<ILogicNode> (clauseNode, literalNode, 1));
					}
				} else {
					var disjointLiteralKey = DisjointLiteralKey (clause.Body);
                    var disjointLiteral = _graph.GetNode(disjointLiteralKey) ?? _graph.AddNode(new LiteralNode(disjointLiteralKey)).GetNode(disjointLiteralKey);

					foreach (var literalNode in clause.Body.Select(literal => _graph.GetNode (literal.Value) ?? _graph.AddNode(new LiteralNode(literal.Value)).GetNode(literal.Value)).Where(literalNode => _graph.GetEdge(disjointLiteral, literalNode) == null)) {
					    _graph.AddEdge(new WeightedDirectedEdge<ILogicNode>(disjointLiteral, literalNode, 1));
					}
				}
            }

			var negated = clause.Head != null ? new Clause (new Literal(clause.Head.NegatedValue())) : new Clause();
			foreach (var dependency in clause.Body) {
				negated.Body.Add (new Literal (dependency.NegatedValue()));
			}
			AddNegatedClause (negated);

            if (headNode != null && negated.Head != null) {
                var negatedHeadNode = _graph.GetNode(negated.Head.Value);
                if (negatedHeadNode != null) {
                    if (headNode.Fact && negatedHeadNode.Fact) {
                        throw new Exception("KB is inconsistent, " + headNode.Id + " and " + negatedHeadNode.Id + " are both facts!");
                    }

                    if (clause.Body.Count > 0) {
                        var nodesA = clause.Body.Select(literal => _graph.GetNode(literal.Value));
                        var nodesB = clause.Body.Select(literal => _graph.GetNode(literal.NegatedValue()));
                        if (SharedAncestor(nodesA, nodesB)) {
                            Console.WriteLine("Adding " + clause.ToProperString() + " will cause conflict skipping clause...");
                            RemoveClause(clause);
                        }
                    }
                }
            }

            if (headNode != null && headNode.Fact) {
                FactFlow(headNode);
            }
            if (clause.Body.Count > 0) {
                foreach (var node in clause.Body.Select(literal => _graph.GetNode(literal.Value)).Where(node => node.Fact)) {
                    FactFlow(node);
                }

                foreach (var node in clause.Body.Select(literal => _graph.GetNode(literal.NegatedValue())).Where(node => node.Fact)) {
                    FactFlow(node);
                }
            }
        }

		private void AddNegatedClause(IClause clause){
			var headNode = clause.Head != null ? (_graph.GetNode(clause.Head.Value) ?? _graph.AddNode(new ClauseNode(clause.Head.Value)).GetNode(clause.Head.Value)): null;

			if (clause.Body.Count < 1) return;

            if (headNode != null) {
			    var disjointLiteralKey = DisjointLiteralKey(clause.Body);
                var disjointLiteral = _graph.GetNode(disjointLiteralKey) ?? _graph.AddNode(clause.Body.Count > 1 ? (ILogicNode) new LiteralNode(disjointLiteralKey) : new ClauseNode(disjointLiteralKey)).GetNode(disjointLiteralKey);

                if (_graph.GetEdge(headNode, disjointLiteral) == null) {
                    _graph.AddEdge(new WeightedDirectedEdge<ILogicNode>(headNode, disjointLiteral, clause.Body.Count));
                }

				foreach (var literalNode in clause.Body.Select(literal => _graph.GetNode(literal.Value) ?? _graph.AddNode(new ClauseNode(literal.Value)).GetNode(literal.Value)).Where(literalNode => _graph.GetEdge(disjointLiteral, literalNode) == null)) {
                    _graph.AddEdge(new WeightedDirectedEdge<ILogicNode>(disjointLiteral, literalNode, 1));
				}
            } else {
                var jointLiteralKey = ClauseKey(clause.Body);
                var jointLiteral = _graph.GetNode(jointLiteralKey) ?? _graph.AddNode(new ClauseNode(jointLiteralKey)).GetNode(jointLiteralKey);

				foreach (var literalNode in clause.Body.Select(literal => _graph.GetNode(literal.Value) ?? _graph.AddNode(new ClauseNode(literal.Value)).GetNode(literal.Value)).Where(literalNode => _graph.GetEdge(jointLiteral, literalNode) == null)) {
				    _graph.AddEdge(new WeightedDirectedEdge<ILogicNode>(jointLiteral, literalNode, 1));
				}
			}
		}

        private void RemoveClause(IClause clause) {
            var headNode = clause.Head != null ? _graph.GetNode(clause.Head.Value) : null;
            if (headNode != null) {
                if (clause.Body.Count == 1) {
                    var literalNode = _graph.GetNode(clause.Body.First().Value);
                    var edge = _graph.GetEdge(headNode, literalNode);
                    _graph.RemoveEdge(edge);

                    headNode = _graph.GetNode(clause.Head.NegatedValue());
                    literalNode = _graph.GetNode(clause.Body.First().NegatedValue());
                    edge = _graph.GetEdge(headNode, literalNode);
                    _graph.RemoveEdge(edge);
                } else if (clause.Body.Count > 1) {
                    var clauseNode = _graph.GetNode(ClauseKey(clause.Body));
                    var edge = _graph.GetEdge(headNode, clauseNode);
                    _graph.RemoveEdge(edge);

                    if (_graph.InDegree(clauseNode) < 1) {
                        _graph.RemoveNode(clauseNode);
                    }

                    headNode = _graph.GetNode(clause.Head.NegatedValue());
                    foreach (var literalNode in clause.Body.Select(dependency => _graph.GetNode(dependency.NegatedValue()))) {
                        edge = _graph.GetEdge(headNode, literalNode);
                        _graph.RemoveEdge(edge);
                    }
                }
            }
        }

        private void FactFlow(ILogicNode startNode) {
            if (startNode is ILiteralNode && _graph.OutDegree(startNode) == 1) {
                foreach (var node in _graph.Outgoing(startNode).Where(node => !node.Fact)) {
                    if (node is ILiteralNode) {
                        var negNode = _graph.GetNode(Negate(node.Id));
                        if (negNode.Fact) {
                            throw new Exception("KB is inconsistent, " + node.Id + " and " + negNode.Id + " are both facts!");
                        }
                    }
                    node.Fact = true;
                    FactFlow(node);
                }
            }

            foreach (var node in _graph.Incoming(startNode).Where(node => !node.Fact)) {
                if (node is IClauseNode) {
                    if (_graph.Outgoing(node).All(dependency => dependency.Fact)) {
                        node.Fact = true;
                        FactFlow(node);
                    }
                } else if(node is ILiteralNode && (_graph.OutDegree(node) == 1 || node.Id.Contains("|"))) {
                    var negNode = _graph.GetNode(Negate(node.Id));
                    if (negNode.Fact) {
                        throw new Exception("KB is inconsistent, " + node.Id + " and " + negNode.Id + " are both facts!");
                    }
                    node.Fact = true;
                    FactFlow(node);
                }
            }
        }

        private static string Negate(string id) {
            if (id.Contains('|', '&')) {
                var parts = id.Split('|');
                if (parts.Length > 1) {
                    return string.Join(" & ", parts.Select(lit => Negate(lit.Trim()))); 
                }
                parts = id.Split('&');
                return string.Join(" | ", parts.Select(lit => Negate(lit.Trim())));
            }
            return id[0] == '!' ? id.Substring(1) : "!" + id;
        }

        private bool SharedAncestor(IEnumerable<ILogicNode> nodeA, IEnumerable<ILogicNode> nodeB) {
            var visitedA = new HashSet<string>();
            var visitedB = new HashSet<string>();
            var queue = new Queue<ILogicNode>();

            foreach (var node in nodeA) {
                visitedA.Add(node.Id);
                queue.Enqueue(node);
            }

            foreach (var node in nodeB) {
                if (visitedA.Contains(node.Id)) return true;
                visitedB.Add(node.Id);
                queue.Enqueue(node);
            }

            while (queue.Count > 0) {
                var node = queue.Dequeue();
                if (visitedA.Contains(node.Id)) {
                    foreach (var dependency in _graph.Incoming(node)) {
                        if (visitedB.Contains(dependency.Id)) return true;
                        if (visitedA.Contains(dependency.Id)) continue;
                        visitedA.Add(dependency.Id);
                        queue.Enqueue(dependency);
                    }
                } else {
                    foreach (var dependency in _graph.Incoming(node)) {
                        if (visitedA.Contains(dependency.Id)) return true;
                        if (visitedB.Contains(dependency.Id)) continue;
                        visitedB.Add(dependency.Id);
                        queue.Enqueue(dependency);
                    }
                }
            }
            return false;
        }

		public bool DirectQuery(IQuery query){
			if(query == null || query.Literals.Count < 1) {
				// Empty clause is true
				return true;
			}

			if (query.Invalid) return false;
		    if (query.Literals.Select(literal => _graph.GetNode(literal.NegatedValue())).Any(node => node != null && node.Fact)) {
		        return false;
		    }

			var satisfied = Satisfied (query.Literals);
			return satisfied.All (q => q.Value);
		}

		public bool RefutationQuery(IQuery query){
			if(query == null || query.Literals.Count < 1) {
				// Empty clause is true
				return true;
			}

			if (query.Invalid) return false;
            if (query.Literals.Select(literal => _graph.GetNode(literal.NegatedValue())).Any(node => node != null && node.Fact)) {
                return false;
            }

			var satisfied = Satisfied (query.Literals.Select(literal => new Literal(literal.NegatedValue())).Cast<ILiteral>().ToList());
			return !satisfied.Any (q => q.Value);
		}

		private IDictionary<string, bool> Satisfied(ICollection<ILiteral> literals){
			var checkedLiterals = new HashSet<string> ();
			foreach (var literal in literals.Where(literal => !_satisfiedLiterals.Contains(literal.Value))) {
				var sourceNode = _graph.GetNode (literal.Value);
				if (sourceNode == null) {
					// If literal is not expressed anywhere,
					// we cannot make any claims about it,
					// defaults to false (by Closed World Assumption)
					continue;
				}

			    Satisfied(sourceNode, checkedLiterals);
			}

			return literals.ToDictionary(literal => literal.Value, literal => _satisfiedLiterals.Contains (literal.Value));
		}

		private bool Satisfied(ILogicNode source, ISet<string> checkedLiterals){
		    if (checkedLiterals.Contains(source.Id))  return _satisfiedLiterals.Contains(source.Id);

			var satisfied = false;
            checkedLiterals.Add(source.Id);

		    if (source.Fact) {
		        satisfied = true;
		    } else if (source is ILiteralNode) {
				satisfied = _graph.Outgoing(source).Any(dependency => Satisfied(dependency, checkedLiterals));
			} else if(source is IClauseNode) {
			    satisfied = _graph.Outgoing(source).All(dependency => Satisfied(dependency, checkedLiterals));
			}

			if (satisfied) {
			    if (!_satisfiedLiterals.Contains(source.Id)) {
                    var negatedNode = _graph.GetNode(new Literal(source.Id).NegatedValue());
			        if (negatedNode != null && _satisfiedLiterals.Contains(negatedNode.Id)) {
			            throw new Exception("KB is inconsistent, both " + source.Id + " and " + negatedNode.Id + " are true");
			        }

			        Console.WriteLine(source.Id + " is satisfied");
                    _satisfiedLiterals.Add(source.Id);
			    }
			} else {
				Console.WriteLine (source.Id + " is not satisfied");
			}

			return satisfied;
		}


        // Output in Graphviz format (mainly for debugging)
        public override string ToString() {
            var seen = new Dictionary<string, int>();
            var labeled = new HashSet<string>();
            var cnt = 0;
            const string indent = "  ";
            var sb = new StringBuilder();

            sb.AppendLine("digraph g{");
            foreach (var from in _graph.Nodes) {
                if (!seen.ContainsKey(from.Id)) seen[from.Id] = ++cnt;

                foreach (var to in _graph.Outgoing(from)) {
                    if (!seen.ContainsKey(to.Id)) seen[to.Id] = ++cnt;

                    var edge = _graph.GetEdge(from, to);
                    sb.AppendLine(indent + seen[from.Id] + " -> " + seen[to.Id] + " [label=\"" + edge.Weight + "\"]");

                    if (labeled.Contains(to.Id)) continue;
                    labeled.Add(to.Id);
                    if (to is IClauseNode) {
                        sb.AppendLine(indent + seen[to.Id] + " [label=\"" + to.Id + "\",shape=box,fillcolor=\"#" + (to.Fact ? "B2D78D" : "EEF2D3") + "\",style=\"filled\"];");
                    } else {
                        sb.AppendLine(indent + seen[to.Id] + " [label=\"" + to.Id + "\",shape=ellipse,fillcolor=\"#" + (to.Fact ? "D6EBFF" : "EEEEEE") + "\",style=\"filled\"];");
                    }
                }

                if (labeled.Contains(from.Id)) continue;
                labeled.Add(from.Id);
                if (from is IClauseNode) {
                    sb.AppendLine(indent + seen[from.Id] + " [label=\"" + from.Id + "\",shape=box,fillcolor=\"#" + (from.Fact ? "B2D78D" : "EEF2D3") + "\",style=\"filled\"];");
                } else {
                    sb.AppendLine(indent + seen[from.Id] + " [label=\"" + from.Id + "\",shape=ellipse,fillcolor=\"#" + (from.Fact ? "D6EBFF" : "EEEEEE") + "\",style=\"filled\"];");
                }
            }
            sb.Append("}");

            return sb.ToString();
        }

        private static string ClauseKey(IEnumerable<ILiteral> literals) {
            var sorted = new SortedSet<string>();
            foreach (var lit in literals) {
                sorted.Add(lit.Value);
            }

            return string.Join(" & ", sorted);
        }

		private static string DisjointLiteralKey(IEnumerable<ILiteral> literals){
			var sorted = new SortedSet<string>();
			foreach (var lit in literals) {
			    sorted.Add(lit.Value);
			}

			return string.Join(" | ", sorted);
		}
    }
}
