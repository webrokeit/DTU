using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Logic {
    public interface IClause : IExpression, IEnumerable<IExpression> {
        ILiteral Head { get; }
        ICollection<ILiteral> Body { get; }
        
        IEnumerable<IExpression> Dependencies { get; }
        int DependencyCount { get; }
        void AddDependency(IExpression dependency);
    }
}
