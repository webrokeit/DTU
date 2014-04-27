using System;
using System.Collections.Generic;
using System.Linq;
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

			var headNode = _graph.GetNode(clause.Head.Value);
            if (headNode == null) {
                headNode = new LiteralNode(clause.Head.Value);
                _graph.AddNode(headNode);
            }

			if (clause.Body.Count < 1) {
				headNode.Fact = true;
			} else if (clause.Body.Count == 1) {
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
                var clauseKey = ClauseKey(clause.Body);
				var clauseNode = _graph.GetNode(clauseKey);
                if (clauseNode == null) {
                    clauseNode = new ClauseNode(clauseKey);
                    _graph.AddNode(clauseNode);
                }

				var headEdge = _graph.GetEdge(headNode, clauseNode);
                if (headEdge == null) {
                    headEdge = new DirectedEdge<ILogicNode>(headNode, clauseNode);
                    _graph.AddEdge(headEdge);
                }

                foreach (var literal in clause.Body) {
					if (clause.Head.Value == literal.Value) {
						Console.WriteLine ("Skipping " + clause.Head.Value + " -> " + literal.Value);
						continue;
					}

					var literalNode = _graph.GetNode(literal.Value);
                    if (literalNode == null) {
                        literalNode = new LiteralNode(literal.Value);
                        _graph.AddNode(literalNode);
                    }
					var literalEdge = _graph.GetEdge(clauseNode, literalNode);
                    if (literalEdge == null) {
                        literalEdge = new DirectedEdge<ILogicNode>(clauseNode, literalNode);
                        _graph.AddEdge(literalEdge);
                    }
                }
            }

			var negated = new Clause (new Literal(clause.Head.NegatedValue()));
			foreach (var dependency in clause.Body) {
				negated.Body.Add (new Literal (dependency.NegatedValue()));
			}
			AddNegatedClause (negated);
        }

		private void AddNegatedClause(IClause clause){
			var headNode = _graph.GetNode(clause.Head.Value);
			if (headNode == null) {
				headNode = new LiteralNode(clause.Head.Value);
				_graph.AddNode(headNode);
			}

			if (clause.Body.Count > 0) {
				foreach (var literal in clause.Body) {
					var literalNode = _graph.GetNode(literal.Value);
					if (literalNode == null) {
						literalNode = new LiteralNode(literal.Value);
						_graph.AddNode(literalNode);
					}
					var literalEdge = _graph.GetEdge(headNode, literalNode);
					if (literalEdge == null) {
						literalEdge = new DirectedEdge<ILogicNode>(headNode, literalNode);
						_graph.AddEdge(literalEdge);
					}
				}
			}
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
			foreach (var literal in literals) {
				if (_satisfiedLiterals.Contains (literal.Value)) continue;

				var sourceNode = _graph.GetNode (literal.Value);
				if (sourceNode == null) {
					// If literal is not expressed anywhere,
					// we cannot make any claims about it,
					// defaults to true (by logic of implication)
					_satisfiedLiterals.Add (literal.Value);
					continue;
				}

				if (Satisfied (sourceNode, _satisfiedLiterals)) {
					_satisfiedLiterals.Add (sourceNode.Id);
				}
			}

			return literals.ToDictionary(literal => literal.Value, literal => _satisfiedLiterals.Contains (literal.Value));
		}

		private bool Satisfied(ILogicNode source, ISet<string> satisfiedLiterals){
			var newlySatisfiedLiterals = new HashSet<string> ();
			if (Satisfied (source, satisfiedLiterals, newlySatisfiedLiterals)) {
				foreach (var satLit in newlySatisfiedLiterals) {
					satisfiedLiterals.Add (satLit);
				}
				return true;
			}
			return false;
		}

		private bool Satisfied(ILogicNode source, ISet<string> satisfiedLiterals, ISet<string> newlySatisfiedLiterals){
			var dependencies = _graph.Outgoing (source).ToList();
			var satisfied = false;
			var newSat = new HashSet<string> ();

			if (source is ILiteralNode) {
				var litNode = (ILiteralNode)source;
				if (satisfiedLiterals.Contains (litNode.Id) || newlySatisfiedLiterals.Contains (litNode.Id)) {
					satisfied = true;
				} else if (litNode.Fact) {
					satisfied = true;
					newlySatisfiedLiterals.Add (litNode.Id);
				} else {
					foreach (var dependency in dependencies) {
						newSat.Clear ();
						if (Satisfied (dependency, satisfiedLiterals, newSat)) {
							newSat.Add (dependency.Id);
							satisfied = true;
							break;
						}
					}
				}
			} else if(source is IClauseNode) {
				var clsNode = (IClauseNode)source;
				if (satisfiedLiterals.Contains (clsNode.Id) || newlySatisfiedLiterals.Contains (clsNode.Id)) {
					satisfied = true;
				} else {
					foreach (var dependency in dependencies) {
						if (!Satisfied (dependency, satisfiedLiterals, newSat)) {
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
				Console.WriteLine (source.Id + " is satisfied");
				foreach (var newSatLit in newSat) {
					newlySatisfiedLiterals.Add (newSatLit);
				}
			} else {
				Console.WriteLine (source.Id + " is not satisfied");
			}

			return satisfied;
		}

        private static string ClauseKey(IEnumerable<ILiteral> literals) {
            var sorted = new SortedDictionary<string, ILiteral>();
            foreach (var lit in literals) {
				sorted[lit.Value] = lit;
            }

            return string.Join(" & ", sorted.Keys);
        }
    }
}
