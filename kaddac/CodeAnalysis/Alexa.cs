namespace Kadda.CodeAnalysis
{
    // Alexa, schalte Sofa an
    class Alexa
    {
        private readonly string _text;
        private int _position;
        private List<string> _diagnostigs = new List<string>();

        public Alexa(string text)
        {        
            _text = text;    
        }
        public IEnumerable<string> Diagnostics => _diagnostigs;
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

        public SyntaxToken NextToken()
        {
            if(_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);

            if(char.IsDigit(Current))
            {
                var start = _position;

                while(char.IsDigit(Current))
                {
                    Next();
                }

                var lenght = _position - start;
                var text = _text.Substring(start, lenght);
                if(!int.TryParse(text, out var value))
                {
                    _diagnostigs.Add($"The number {_text} isnt a valid int32");
                }
                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            if(char.IsWhiteSpace(Current))
            {
                var start = _position;

                while(char.IsWhiteSpace(Current))
                {
                    Next();
                }

                var lenght = _position - start;
                var text = _text.Substring(start, lenght);
                // int.TryParse(text, out var value);
                
                return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, null);
            }

            if(Current == '+')
                return new SyntaxToken(SyntaxKind.PlusToken, _position++,"+", null);
            else if(Current == '-')
                return new SyntaxToken(SyntaxKind.MinusToken, _position++,"-", null);
            else if(Current == '(')
                return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++,"(", null);
            else if(Current == ')')
                return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++,")", null);
            else if(Current == '*')
                return new SyntaxToken(SyntaxKind.StarToken, _position++,"*", null);
            else if(Current == '/')
                return new SyntaxToken(SyntaxKind.SlashToken, _position++,"/", null);

            _diagnostigs.Add($"ERROR: bad character input: '{Current}'");
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position -1, 1), null);
        }
    }
}