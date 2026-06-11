namespace Kadda.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        // Tokens
        BadToken,
        EndOfFileToken,
        WhitespaceToken,
        NumberToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        BangToken,
        AmpersandAmpersandToken,
        PipePipeToken,
        EqualsEqualsToken,
        BangEqualsToken,

        OpenParenthesisToken,
        CloseParenthesisToken,
        IndentifierToken,

        //Keyword
        TrueKeyword,
        FalseKeyword,

        // Expressions 
        LiteralExpression,
        NameExpression,
        AssignmentExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression
    }
}