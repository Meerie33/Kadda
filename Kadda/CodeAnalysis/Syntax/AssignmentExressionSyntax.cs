namespace Kadda.CodeAnalysis.Syntax
{
    public sealed class AssignmentExressionSyntax : ExpressionSyntax
    {
        public AssignmentExressionSyntax(SyntaxToken identifierToken, SyntaxToken equalsToken, ExpressionSyntax expression)
        {
            IdentifierToken = identifierToken;
            EqualsToken = equalsToken;
            Expression = expression;
        }
        public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;
        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax Expression { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return IdentifierToken;
            yield return EqualsToken;
            yield return Expression;
        }
    }
}