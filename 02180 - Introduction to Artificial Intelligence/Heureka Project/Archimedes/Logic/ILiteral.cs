using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Logic {
    public interface ILiteral : IExpression {
        string Value { get; }
        string Negated { get; }
        bool Valid { get; }
        Func<bool> Validator { get; set; }
    }
}
