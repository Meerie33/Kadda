namespace Kadda.CodeAnalysis.Syntax
{
    public sealed class LiteralExpressionSyntax : ExpressionSyntax
    {
        public LiteralExpressionSyntax(SyntaxToken literaltoken) : this(literaltoken, literaltoken.Value)
        {
        }
        public LiteralExpressionSyntax(SyntaxToken literaltoken, object value)
        {
            LiteralToken = literaltoken;
            Value = value;
        }

        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;
        public SyntaxToken LiteralToken {get; }
        public object Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LiteralToken;
        }
    }
}