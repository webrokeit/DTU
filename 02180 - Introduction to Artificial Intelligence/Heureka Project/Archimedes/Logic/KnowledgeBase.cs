using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Graph;
using Archimedes.Extensions;

namespace Archimedes.Logic {
    public class KnowledgeBase : IKnowledgeBase {
        private readonly IDirectedGraph<ILogicNode, IDirectedEdge<ILogicNode>> _graph;
		private readonly ISet<string> _satisfiedLiterals;

        public KnowledgeBase() {
            _graph = new DirectedGraph<ILogicNode, IDirectedEdge<ILogicNode>>();
			_satisfiedLiterals = new HashSet<string> ();
        }

        public void AddClause(IClause clause) {
            if (clause == null) return;
			_satisfiedLiterals.Clear ();

			var headNode = clause.Head != null ? _graph.GetNode(clause.Head.Value) : null;
			if (clause.Head != null && headNode == null) {
                headNode = new LiteralNode(clause.Head.Value);
                _graph.AddNode(headNode);
            }

			if (clause.Body.Count < 1) {
                if (headNode == null) throw new NullReferenceException("Clause Head is null");
				headNode.Fact = true;
			} else if (clause.Body.Count == 1) {
                if (clause.Head == null) throw new NullReferenceException("Clause Head is null");
                var literal = clause.Body.First();

				if (clause.Head.Value != literal.Value) {
					var node = _graph.GetNode (literal.Value);
					if (node == null) {
						node = new LiteralNode (literal.Value);
						_graph.AddNode (node);
					}

					var edge = _graph.GetEdge (headNode, node);
					if (edge == null) {
						edge = new DirectedEdge<ILogicNode> (headNode, node);
						_graph.AddEdge (edge);
					}
				}
            } else if (clause.Body.Count > 1) {
				if (clause.Head != null) {
					var clauseKey = ClauseKey (clause.Body);
					var clauseNode = _graph.GetNode (clauseKey);
					if (clauseNode == null) {
						clauseNode = new ClauseNode (clauseKey);
						_graph.AddNode (clauseNode);
					}

					var headEdge = _graph.GetEdge (headNode, clauseNode);
					if (headEdge == null) {
						headEdge = new DirectedEdge<ILogicNode> (headNode, clauseNode);
						_graph.AddEdge (headEdge);
					}

					foreach (var literal in clause.Body) {
						if (clause.Head.Value == literal.Value) continue;

						var literalNode = _graph.GetNode (literal.Value);
						if (literalNode == null) {
							literalNode = new LiteralNode (literal.Value);
							_graph.AddNode (literalNode);
						}
						var literalEdge = _graph.GetEdge (clauseNode, literalNode);
						if (literalEdge == null) {
							literalEdge = new DirectedEdge<ILogicNode> (clauseNode, literalNode);
							_graph.AddEdge (literalEdge);
						}
					}
				} else {
					var disjointLiteralKey = DisjointLiteralKey (clause.Body);
					var disjointLiteral = _graph.GetNode (disjointLiteralKey);
					if (disjointLiteral == null) {
						disjointLiteral = new LiteralNode (disjointLiteralKey);
						_graph.AddNode (disjointLiteral);
					}

					foreach (var literal in clause.Body) {
						var literalNode = _graph.GetNode (literal.Value);
						if (literalNode == null) {
							literalNode = new LiteralNode (literal.Value);
							_graph.AddNode (literalNode);
						}
						var disjointLiteralEdge = _graph.GetEdge (disjointLiteral, literalNode);
						if (disjointLiteralEdge == null) {
							disjointLiteralEdge = new DirectedEdge<ILogicNode> (disjointLiteral, literalNode);
							_graph.AddEdge (disjointLiteralEdge);
						}
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

                    if (NodesConnected(headNode, negatedHeadNode)) { 
                        throw new Exception("KB is inconsistent, one or more literal can be both satisfied and unsatisfied at the same time!");
                    }
                }
            }
        }

		private void AddNegatedClause(IClause clause){
			var headNode = clause.Head != null ? _graph.GetNode(clause.Head.Value) : null;
			if (clause.Head != null && headNode == null) {
				headNode = new LiteralNode(clause.Head.Value);
				_graph.AddNode(headNode);
			}

			if (clause.Body.Count < 1) return;

			if (clause.Head != null) {
				foreach (var literal in clause.Body) {
					var literalNode = _graph.GetNode (literal.Value);
					if (literalNode == null) {
						literalNode = new LiteralNode (literal.Value);
						_graph.AddNode (literalNode);
					}
					var literalEdge = _graph.GetEdge (headNode, literalNode);
					if (literalEdge == null) {
						literalEdge = new DirectedEdge<ILogicNode> (headNode, literalNode);
						_graph.AddEdge (literalEdge);
					}
				}
			} else {
				var jointLiteralKey = ClauseKey (clause.Body);
				var jointLiteral = _graph.GetNode (jointLiteralKey);

				if (jointLiteral == null) {
					jointLiteral = new ClauseNode (jointLiteralKey);
					_graph.AddNode (jointLiteral);
				}

				foreach (var literal in clause.Body) {
					var literalNode = _graph.GetNode (literal.Value);
					if (literalNode == null) {
						literalNode = new LiteralNode (literal.Value);
						_graph.AddNode (literalNode);
					}

					var jointEdge = _graph.GetEdge (jointLiteral, literalNode);
					if (jointEdge == null) {
						jointEdge = new DirectedEdge<ILogicNode> (jointLiteral, literalNode);
						_graph.AddEdge (jointEdge);
					}
				}
			}
		}

        private bool NodesConnected(ILogicNode startNode, ILogicNode endNode) {
            var visited = new HashSet<string>();
            var queue = new Queue<ILogicNode>();
            queue.Enqueue(startNode);

            while (queue.Count > 0) {
                var node = queue.Dequeue();
                if (node.Id == endNode.Id) return true;

                foreach (var dependency in _graph.Outgoing(node).Where(dependency => !visited.Contains(dependency.Id))) {
                    visited.Add(dependency.Id);
                    queue.Enqueue(dependency);
                }

                if (!(node is ILiteralNode)) continue;
                foreach (var dependency in _graph.Incoming(node).Where(dependency => dependency is IClauseNode && !visited.Contains(dependency.Id))) {
                    visited.Add(dependency.Id);
                    queue.Enqueue(dependency);
                }
            }

            return false;
        }

		public bool DirectQuery(IQuery query){
			if(query == null || query.Literals.Count < 1) {
				// Empty clause is true
				return true;
			}

			if (query.Invalid) {
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

			if (query.Invalid) {
				return false;
			}

			var satisfied = Satisfied (query.Literals.Select(literal => new Literal(literal.NegatedValue())).Cast<ILiteral>().ToList());
			return !satisfied.Any (q => q.Value);
		}

		private IDictionary<string, bool> Satisfied(ICollection<ILiteral> literals){
			var checkedLiterals = new HashSet<string> ();
			foreach (var literal in literals) {
				if (_satisfiedLiterals.Contains (literal.Value)) continue;

				var sourceNode = _graph.GetNode (literal.Value);
				if (sourceNode == null) {
					// If literal is not expressed anywhere,
					// we cannot make any claims about it,
					// defaults to false (by Closed World Assumption)
					// _satisfiedLiterals.Add (literal.Value);
					continue;
				}

				if (Satisfied (sourceNode, checkedLiterals)) {
				    Console.WriteLine("Learned that " + sourceNode.Id + " is satisfied");
					_satisfiedLiterals.Add (sourceNode.Id);
				}
			}

			return literals.ToDictionary(literal => literal.Value, literal => _satisfiedLiterals.Contains (literal.Value));
		}

		private bool Satisfied(ILogicNode source, ISet<string> checkedLiterals){
			var newlySatisfiedLiterals = new HashSet<string> ();
			if (Satisfied (source, newlySatisfiedLiterals, checkedLiterals)) {
				foreach (var satLit in newlySatisfiedLiterals) {
                    Console.WriteLine("Learned that " + satLit + " is satisfied");
					_satisfiedLiterals.Add (satLit);
				}
				return true;
			}
			return false;
		}

		private bool Satisfied(ILogicNode source, ISet<string> newlySatisfiedLiterals, ISet<string> checkedLiterals){
			var dependencies = _graph.Outgoing (source).ToList();
			var satisfied = false;
			var newSat = new HashSet<string> ();

			if (source is ILiteralNode) {
				checkedLiterals.Add (source.Id);
				var litNode = (ILiteralNode)source;
				if (_satisfiedLiterals.Contains (litNode.Id) || newlySatisfiedLiterals.Contains (litNode.Id)) {
					satisfied = true;
				} else if (litNode.Fact) {
					satisfied = true;
					newlySatisfiedLiterals.Add (litNode.Id);
				} else {
					newSat.Clear ();
					foreach (var dependency in dependencies.Where(dependency => !checkedLiterals.Contains(dependency.Id))) {
						if (Satisfied (dependency, newSat, checkedLiterals)) {
							newSat.Add (dependency.Id);
							satisfied = true;
							break;
						}
					}
				}
			} else if(source is IClauseNode) {
				checkedLiterals.Add (source.Id);
				var clsNode = (IClauseNode)source;
				if (_satisfiedLiterals.Contains (clsNode.Id) || newlySatisfiedLiterals.Contains (clsNode.Id)) {
					satisfied = true;
				} else {
					foreach (var dependency in dependencies.Where(dependency => !checkedLiterals.Contains(dependency.Id))) {
						if (!Satisfied (dependency, newSat, checkedLiterals)) {
							satisfied = false;
							newSat.Clear ();
							break;
						} else {
							newSat.Add (dependency.Id);
							satisfied = true;
						}
					}
				}
			}

			if (satisfied) {
				var negatedLiteral = new Literal (source.Id);
				var negatedNode = _graph.GetNode (negatedLiteral.NegatedValue ());
				if (negatedNode != null) {
					if (_satisfiedLiterals.Contains (negatedNode.Id) || newlySatisfiedLiterals.Contains (negatedNode.Id)) {
						throw new Exception ("KB is inconsistent, both " + source.Id + " and " + negatedNode.Id + " are true");
					}
				}

				Console.WriteLine (source.Id + " is satisfied");
				foreach (var newSatLit in newSat) {
					newlySatisfiedLiterals.Add (newSatLit);
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

                    sb.AppendLine(indent + seen[from.Id] + " -> " + seen[to.Id]);

                    if (labeled.Contains(to.Id)) continue;
                    labeled.Add(to.Id);
                    if (to is IClauseNode) {
                        sb.AppendLine(indent + seen[to.Id] + " [label=\"" + to.Id + "\",shape=box,fillcolor=\"#EEF2D3\",style=\"filled\"];");
                    } else {
                        sb.AppendLine(indent + seen[to.Id] + " [label=\"" + to.Id + "\",shape=ellipse,fillcolor=\"#" + (to.Fact ? "D6EBFF" : "EEEEEE") + "\",style=\"filled\"];");
                    }
                }

                if (labeled.Contains(from.Id)) continue;
                labeled.Add(from.Id);
                if (from is IClauseNode) {
                    sb.AppendLine(indent + seen[from.Id] + " [label=\"" + from.Id + "\",shape=box,fillcolor=\"#EEF2D3\",style=\"filled\"];");
                } else {
                    sb.AppendLine(indent + seen[from.Id] + " [label=\"" + from.Id + "\",shape=ellipse,fillcolor=\"#" + (from.Fact ? "D6EBFF" : "EEEEEE") + "\",style=\"filled\"];");
                }
            }
            sb.Append("}");

            return sb.ToString();
        }

        private static string ClauseKey(IEnumerable<ILiteral> literals) {
            var sorted = new SortedDictionary<string, ILiteral>();
            foreach (var lit in literals) {
				sorted[lit.Value] = lit;
            }

            return string.Join(" & ", sorted.Keys);
        }

		private static string DisjointLiteralKey(IEnumerable<ILiteral> literals){
			var x = literals.ToList ();
			var sorted = new SortedDictionary<string, ILiteral>();
			foreach (var lit in x) {
				sorted[lit.Value] = lit;
			}

			return string.Join(" | ", sorted.Keys);
		}
    }
}
