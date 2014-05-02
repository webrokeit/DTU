using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Logic {
    public class Literal : ILiteral {
        public string Value { get; private set; }
        public bool Negated { get; private set; }

        public Literal(string value) {
            Value = value;
            Negated = value[0] == '!';
        }

        public IExpression Reduce() {
            return this;
        }

		public string NegatedValue(){
            return Negated ? Value.Substring(1) : "!" + Value;
		}

        public string ToProperString() {
            return Value;
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (obj == this) return true;
            var lObj = (Literal) obj;
            return lObj != null && Value == lObj.Value;
        }

        public override string ToString() {
			return Value;
        }
    }
}
