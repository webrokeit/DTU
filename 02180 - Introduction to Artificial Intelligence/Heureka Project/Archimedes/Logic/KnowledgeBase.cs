using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Graph;

namespace Archimedes.Logic {
    public class KnowledgeBase : IKnowledgeBase {
        private readonly IDirectedGraph<ILogicNode, IDirectedEdge<ILogicNode>> _graph;

        public KnowledgeBase() {
            _graph = new DirectedGraph<ILogicNode, IDirectedEdge<ILogicNode>>();
        }

        public void AddClause(IClause clause) {
            if (clause == null) return;

            var headNode = _graph[clause.Head.Value];
            if (headNode == null || (!headNode.Fact && clause.Body.Count < 1)) {
                headNode = new LiteralNode(clause.Head.Value, clause.Body.Count < 1);
                _graph.AddNode(headNode);
            }

            if (clause.Body.Count == 1) {
                var literal = clause.Body.First();

                var node = _graph[literal.Value];
                if (node == null) {
                    node = new LiteralNode(literal.Value);
                    _graph.AddNode(node);
                }

                var edge = _graph[headNode, node];
                if (edge == null) {
                    edge = new DirectedEdge<ILogicNode>(headNode, node);
                    _graph.AddEdge(edge);
                }
            } else if (clause.Body.Count > 1) {
                var clauseKey = ClauseKey(clause.Body);
                var clauseNode = _graph[clauseKey];
                if (clauseNode == null) {
                    clauseNode = new ClauseNode(clauseKey);
                    _graph.AddNode(clauseNode);
                }

                var headEdge = _graph[headNode, clauseNode];
                if (headEdge == null) {
                    headEdge = new DirectedEdge<ILogicNode>(headNode, clauseNode);
                    _graph.AddEdge(headEdge);
                }

                foreach (var literal in clause.Body) {
                    var literalNode = _graph[literal.Value];
                    if (literalNode == null) {
                        literalNode = new LiteralNode(literal.Value);
                        _graph.AddNode(literalNode);
                    }
                    var literalEdge = _graph[clauseNode, literalNode];
                    if (literalEdge == null) {
                        literalEdge = new DirectedEdge<ILogicNode>(clauseNode, literalNode);
                        _graph.AddEdge(literalEdge);
                    }
                }
            }
        }

        private static string ClauseKey(IEnumerable<ILiteral> literals) {
            var sorted = new SortedDictionary<string, ILiteral>();
            foreach (var lit in literals) {
                sorted.Add(lit.Value, lit);
            }

            return string.Join(" & ", sorted.Keys);
        }
    }
}
