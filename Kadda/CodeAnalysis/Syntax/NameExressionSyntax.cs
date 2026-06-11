namespace Kadda.CodeAnalysis.Syntax
{
    public sealed class NameExressionSyntax : ExpressionSyntax
    {
        public NameExressionSyntax(SyntaxToken identifierToken)
        {
            IdentifierToken = identifierToken;
        }
        public override SyntaxKind Kind => SyntaxKind.NameExpression;
        public SyntaxToken IdentifierToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return IdentifierToken;
        }
    }
}