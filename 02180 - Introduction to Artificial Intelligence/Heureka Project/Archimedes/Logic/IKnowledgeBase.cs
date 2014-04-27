using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archimedes.Logic {
    public interface IKnowledgeBase {
        void AddClause(IClause clause);
		bool DirectQuery(IQuery query);
		bool RefutationQuery(IQuery query);
    }
}
