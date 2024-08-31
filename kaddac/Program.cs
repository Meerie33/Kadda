using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace kaddac
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                Console.Write(">> ");
                var line = Console.ReadLine();

                if(string.IsNullOrWhiteSpace(line))
                    return;

                var parser = new Parser(line);
                var syntaxTree = parser.Parse();

                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                PrettyPrint(syntaxTree.Root);
                Console.ForegroundColor = color;

                // Error found
                if(syntaxTree.Diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    foreach(var diagnostics in parser.Diagnostics)
                        Console.WriteLine(diagnostics);
                    
                    Console.ForegroundColor = color;                   
                }
            }
        }   

        static void PrettyPrint(SyntaxNode node, string indend = "", bool isLast = true)
        {
            // Unix Tree
            // 
            // └──
            // │ 
            // ├── 

            var marker = isLast ? "└──" : "├──";
            Console.Write(indend);
            Console.Write(marker);
            Console.Write(node.Kind);

            if(node is SyntaxToken t && t.Value != null)
            {
                Console.Write("");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indend += isLast ? "    " : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach(var child in node.GetChildren())
                PrettyPrint(child, indend, child == lastChild);
        }
    }

    enum SyntaxKind
    {
        NumberToken,
        WhitespaceToken,
        PlusToken,
        MinusToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        StarToken,
        SlashToken,
        BadToken,
        EndOfFileToken,
        NumberExpression,
        BinaryExpression
    }

    class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text; 
            Value = value;
        }

        public override SyntaxKind Kind { get; }
        public int Position {get; }
        public string Text { get; }
        public object Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }

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
                int.TryParse(text, out var value);
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
                if(!int.TryParse(text, out var value))
                {
                    _diagnostigs.Add($"ERROR: The number {_text} isnt a valid int32");
                }
                
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

    abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind {get;}

        public abstract IEnumerable<SyntaxNode> GetChildren();

    }

    abstract class ExpressionSyntax : SyntaxNode
    {
    }

    sealed class NumberExpressionSyntax : ExpressionSyntax
    {
        public NumberExpressionSyntax(SyntaxToken numbertoken)
        {
            NumberToken = numbertoken;
        }

        public override SyntaxKind Kind => SyntaxKind.NumberExpression;
        public SyntaxToken NumberToken {get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NumberToken;
        }
    }

    sealed class BinaryExpressionSyntax : ExpressionSyntax
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

    sealed class SyntaxTree
    {
        public SyntaxTree(IReadOnlyList<string> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            EndOfFileToken = endOfFileToken;
        }
        public IReadOnlyList<string> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }
    }
    class Parser
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
                token = lexer.NextToken();
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

         private SyntaxToken Match(SyntaxKind kind)
        {
            if(Current.Kind == kind)
            return NextToken();

            _diagnostigs.Add($"ERROR: Scheiß Token '{Current.Kind}', expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var endOfFileToken = Match(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(_diagnostigs, expression, endOfFileToken);
        }

        private ExpressionSyntax ParseExpression()
        {
            var left = ParsePrimaryExpression();

            while(Current.Kind == SyntaxKind.PlusToken || Current.Kind == SyntaxKind.MinusToken)
            {
                var opperatorToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, opperatorToken, right);

            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            var numbertoken = Match(SyntaxKind.NumberToken);
            return new NumberExpressionSyntax(numbertoken);
        }
    }

    class Evaluator
    {
        private readonly ExpressionSyntax _root;
        public Evaluator(ExpressionSyntax root)
        {
            this._root = root;
        }
        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        // root = node
        private int EvaluateExpression(ExpressionSyntax root)
        {
            if(root is NumberExpressionSyntax n)
                return(int) n.NumberToken.Value;

            if(root is BinaryExpressionSyntax b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                if(b.OpperatorToken.Kind == SyntaxKind.PlusToken)
                    return left + right;
                else if(b.OpperatorToken.Kind == SyntaxKind.MinusToken)
                    return left - right;
                else if(b.OpperatorToken.Kind == SyntaxKind.StarToken)
                    return left * right;
                else if(b.OpperatorToken.Kind == SyntaxKind.SlashToken)
                    return left / right;
                else
                throw new Exception($"Unexpected binary operator {b.OpperatorToken.Kind}");
            }
            throw new Exception($"Unexpected node {root.Kind}");
        }
    }
}