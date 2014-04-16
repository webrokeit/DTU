using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Logic {
    public class Literal : ILiteral {
        public string Value { get; private set; }
        public string Negated { get; private set; }
        public bool Valid { get { return Validator(); } }
        public Func<bool> Validator { get; set; } 

        public Literal(string value, Func<bool> validator) {
            Value = value;
            Negated = value[0] == '!' ? value.Substring(1) : "!" + Value;
            Validator = validator;
        }

        public bool Evaluate() {
            return Valid;
        }

        public IExpression Reduce() {
            return this;
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
            return "[" + Value + ":" + (Valid ? "true" : "false") + "]";
        }
    }
}
