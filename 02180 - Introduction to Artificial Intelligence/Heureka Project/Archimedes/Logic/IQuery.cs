using System;
using System.Collections.Generic;
using Archimedes.Logic;

namespace Archimedes.Logic {
	public interface IQuery {
		ISet<ILiteral> Literals { get; }
		bool Invalid { get; }
	}
}

