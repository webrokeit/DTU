using System;
using System.Collections.Generic;
using System.Linq;
using Archimedes;

namespace Archimedes.Logic
{
	public class Query : IQuery {
		public ISet<ILiteral> Literals { get; private set; }

		public bool Invalid {
			get { return Literals.Any(literal => Literals.Contains(new Literal(literal.NegatedValue()))); }
		}

		public Query () {
			Literals = new HashSet<ILiteral> ();
		}

	    public override string ToString() {
	        return Literals.Count < 1 ? "The Empty Clause" : string.Join(" & ", Literals);
	    }
	}
}

