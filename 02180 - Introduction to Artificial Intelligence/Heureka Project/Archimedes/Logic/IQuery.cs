using System.Collections.Generic;

namespace Archimedes.Logic {
	public interface IQuery {
		ISet<ILiteral> Literals { get; }
		bool Invalid { get; }
	}
}

