using System;
using System.Collections.Generic;
using Archimedes;

namespace Archimedes.Logic
{
	public class Query : IQuery {
		public ISet<ILiteral> Literals { get; private set; }

		public bool Invalid {
			get {
				foreach (var literal in Literals) {
					if(Literals.Contains(new Literal(literal.NegatedValue()))) {
						return true;
					}
				}

				return false;
			}
		}

		public Query () {
			Literals = new HashSet<ILiteral> ();
		}
	}
}

