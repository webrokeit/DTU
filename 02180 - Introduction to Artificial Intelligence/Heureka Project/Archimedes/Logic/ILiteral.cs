namespace Archimedes.Logic {
    public interface ILiteral : IExpression {
        string Value { get; }
        bool Negated { get; }

		string NegatedValue();
    }
}
