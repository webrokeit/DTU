using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archimedes.Logic {
    public class Clause : IClause {
        public ILiteral Head { get; private set; }
		public ISet<ILiteral> Body { get; private set; }

        private string _properString;

        public Clause() {
			Body = new HashSet<ILiteral>();
        }

        public Clause(ILiteral head) : this() {
            Head = head;
        }

        public string ToProperString() {
            if (_properString == null) {
                var sb = new StringBuilder();
				if (Body.Count == 1) {
					sb.Append(Body.First().ToProperString());
                } else {
                    sb.Append("(");
                    var cnt = 0;
					foreach (var literal in Body) {
                        cnt++;
						sb.Append(literal.ToProperString());
						if (cnt < Body.Count) {
                            sb.Append(" & ");
                        }
                    }
                    sb.Append(")");
                }
                if (Head != null) {
                    sb.Insert(0, Head.Value + " <- ");
                }
                _properString = sb.ToString();
            }
            return _properString;
        }

        public IEnumerator<IExpression> GetEnumerator() {
			return Body.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
