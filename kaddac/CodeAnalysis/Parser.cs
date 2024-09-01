namespace Kadda.CodeAnalysis
{
    internal sealed class Parser
    {

        private readonly SyntaxToken[] _tokens;

        private List<string> _diagnostigs = new List<string>();
        private int _position;
        public Parser(string text)
        {
            var tokens = new List<SyntaxToken>();

            var lexer = new Alexa(text);
            SyntaxToken token;
            do
            {
                token = lexer.Alex();
                if(token.Kind != SyntaxKind.WhitespaceToken  && token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            }
            while (token.Kind != SyntaxKind.EndOfFileToken);

            _tokens = tokens.ToArray();
            _diagnostigs.AddRange(lexer.Diagnostics);
        }

        public IEnumerable<string> Diagnostics => _diagnostigs;

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if(index >= _tokens.Length)
            return _tokens[_tokens.Length - 1];

            return _tokens[index];
        }

        private SyntaxToken Current => Peek(0);

        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

         private SyntaxToken MatchToken(SyntaxKind kind)
        {
            if(Current.Kind == kind)
            return NextToken();

            _diagnostigs.Add($"ERROR: Scheiß Token '{Current.Kind}', expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(_diagnostigs, expression, endOfFileToken);
        }

        private ExpressionSyntax ParseExpression()
        {
            return ParseTerm();
        }

        private ExpressionSyntax ParseTerm()
        {
            var left = ParseFactor();

            while(Current.Kind == SyntaxKind.PlusToken || Current.Kind == SyntaxKind.MinusToken)
            {
                var opperatorToken = NextToken();
                var right = ParseFactor();
                left = new BinaryExpressionSyntax(left, opperatorToken, right);

            }

            return left;
        }

        private ExpressionSyntax ParseFactor()
        {
            var left = ParsePrimaryExpression();

            while(Current.Kind == SyntaxKind.StarToken || Current.Kind == SyntaxKind.SlashToken)
            {
                var opperatorToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, opperatorToken, right);

            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            if(Current.Kind == SyntaxKind.OpenParenthesisToken)
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = MatchToken(SyntaxKind.CloseParenthesisToken);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            var numbertoken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numbertoken);
        }
    }
}