namespace Kadda.CodeAnalysis.Syntax
{
    // Lexer
    internal sealed class Alexa
    {
        private readonly string _text;
        private int _position;
        private DiagnosticBag _diagnostics = new DiagnosticBag();

        public Alexa(string text)
        {        
            _text = text;    
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        private char Current 
        {
            get
            {
                if(_position >= _text.Length)
                {
                    return '\0';
                }
                return _text[_position];
            }
        }

        private void Next(){
            _position++;
        }

        private char Peek(int offset)
        {
            var index = _position + offset;
            if(index >= _text.Length)
                return '\0';
            return _text[index];
        }

        //todo rename to Lex
        public SyntaxToken Alex()
        {
            if(_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);

            if(char.IsDigit(Current))
            {
                var start = _position;

                while(char.IsDigit(Current))
                    Next();

                var lenght = _position - start;
                var text = _text.Substring(start, lenght);
                if(!int.TryParse(text, out var value))
                    _diagnostics.ReportInvalidNumber(new TextSpan(start, lenght), _text, typeof(int));

                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            if(char.IsWhiteSpace(Current))
            {
                var start = _position;

                while(char.IsWhiteSpace(Current))
                    Next();

                var lenght = _position - start;
                var text = _text.Substring(start, lenght);
                return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, null);
            }

            if(char.IsLetter(Current))
            {
                var start = _position;

                while(char.IsLetter(Current))
                    Next();

                var lenght = _position - start;
                var text = _text.Substring(start, lenght);
                var kind = SyntaxFacts.GetKeywordKind(text);
                
                return new SyntaxToken(kind, start, text, null);
            }
            // false

            switch (Current)
            {
                case '+':
                    return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
                case '-':
                    return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
                case '*':
                    return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
                case '/':
                    return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
                case '(':
                    return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
                case ')':
                    return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);
                case '&':
                    if (Peek(1) == '&')
                        return new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, _position += 2, "&&", null);
                    break;
                case '|':
                    if (Peek(1) == '|')
                        return new SyntaxToken(SyntaxKind.PipePipeToken, _position += 2, "||", null);
                    break;
                case '=':
                    if (Peek(1) == '=')
                        return new SyntaxToken(SyntaxKind.EqualsEqualsToken, _position += 2, "==", null);
                    break;
                case '!':
                    if (Peek(1) == '=')
                        return new SyntaxToken(SyntaxKind.BangEqualsToken, _position += 2, "!=", null);
                    break;
            }

            _diagnostics.ReportBadCharacter(_position, Current);
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position -1, 1), null);
        }
    }
}