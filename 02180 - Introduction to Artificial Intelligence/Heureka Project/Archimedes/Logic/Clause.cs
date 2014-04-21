using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Extensions;

namespace Archimedes.Logic {
    public class Clause : IClause {
        public ILiteral Head { get; private set; }
        public ICollection<ILiteral> Body { get; private set; }

        public IEnumerable<IExpression> Dependencies { get { return _dependencies; } }
        public int DependencyCount { get { return _dependencies.Count; } }

        private readonly IList<IExpression> _dependencies;
        private string _properString;

        public Clause() {
            _dependencies = new List<IExpression>();
            Body = new List<ILiteral>();
        }

        public Clause(ILiteral head) : this() {
            Head = head;
        }

        public bool Evaluate() {
            return Dependencies.All(expression => expression.Evaluate());
        }

        public void AddDependency(IExpression dependency) {
            _dependencies.Add(dependency);
            _properString = null;
        }

        public IExpression Reduce() {
            var newExpr = (IExpression)this;
            var clauses = new List<IClause>(_dependencies.Count);
            var literals = new List<ILiteral>(_dependencies.Count);
            var seenLiterals = new HashSet<string>();
            var duplicateLiterals = new HashSet<string>();

            var modifier = 0;
            for (var i = 0; i < _dependencies.Count; i++) {
                _dependencies[i - modifier] = _dependencies[i].Reduce();

                var dependency = _dependencies[i];
                if (dependency == null) {
                    modifier++;
                    continue;
                }

                var clause = dependency as IClause;
                if (clause != null) {
                    clauses.Add(clause);
                    continue;
                }

                var literal = dependency as ILiteral;
                if (literal != null) {
                    if (seenLiterals.Contains(literal.Value)) {
                        _dependencies[i - modifier] = null;
                        modifier++;
                        continue;
                    }
                    seenLiterals.Add(literal.Value);
                    literals.Add(literal);
                }
            }

            _dependencies.RemoveLastItems(modifier);

            if (_dependencies.Count == 1) {
                newExpr = _dependencies[0];
            } else if (_dependencies.Count == 1) {
                newExpr = null;
            }

            return newExpr;
        }

        public string ToProperString() {
            if (_properString == null) {
                var sb = new StringBuilder();
                if (_dependencies.Count <= 1) {
                    sb.Append(_dependencies[0].ToProperString());
                } else {
                    sb.Append("(");
                    var cnt = 0;
                    foreach (var dependency in _dependencies) {
                        cnt++;
                        var clause = (IClause) dependency;
                        if (clause != null) {
                            sb.Append(clause.ToProperString());
                            if (cnt < _dependencies.Count) {
                                sb.Append(") & (");
                            }
                        } else {
                            sb.Append(dependency.ToProperString());
                            if (cnt < _dependencies.Count) {
                                sb.Append(" & ");
                            }
                        }
                    }
                    sb.Append(")");
                }
                _properString = sb.ToString();
            }
            return _properString;
        }

        public IEnumerator<IExpression> GetEnumerator() {
            return _dependencies.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
