namespace Kadda.CodeAnalysis.Syntax
{
    public sealed class UnaryExpressionSyntax : ExpressionSyntax
    {
        public UnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionSyntax operand)
        {
            Operand = operand;
            OpperatorToken = operatorToken;
        }
        public override SyntaxKind Kind => SyntaxKind.UnaryExpression;
        public SyntaxToken OpperatorToken {get;}
        public ExpressionSyntax Operand {get;}

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpperatorToken;
            yield return Operand;
        }
    }
}