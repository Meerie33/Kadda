namespace Kadda.CodeAnalysis
{
    public sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken opperatorToken, ExpressionSyntax right)
        {
            Left = left;
            Right = right;
            OpperatorToken = opperatorToken;
        }
        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
        public ExpressionSyntax Left {get;}
        public SyntaxToken OpperatorToken {get;}
        public ExpressionSyntax Right {get;}

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OpperatorToken;
            yield return Right;
        }
    }
}