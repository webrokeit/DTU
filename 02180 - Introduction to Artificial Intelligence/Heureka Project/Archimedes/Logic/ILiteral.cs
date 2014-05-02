using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Logic {
    public interface ILiteral : IExpression {
        string Value { get; }
        bool Negated { get; }

		string NegatedValue();
    }
}
