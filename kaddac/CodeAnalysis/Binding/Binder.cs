using System.Net;
using System.Runtime.CompilerServices;
using Kadda.CodeAnalysis.Syntax;

namespace Kadda.Binding
{
    internal enum BoundNodeKind
    {
        UnaryExpression,
        LiteralExpression
    }
    
    internal abstract class BoundNode
    {
        public abstract BoundNodeKind Kind { get; }
    }

    internal abstract class BoundExpression : BoundNode
    {
        public abstract Type Type { get; }
    }

    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value)
        {
            Value = value;
        }
        public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
        public override Type Type => Value.GetType();
        public object Value { get; }
    }

    internal enum BoundUnaryOperatorKind
    {
        Identity,
        Negation
    }
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperatorKind opertorKind, BoundExpression operand)
        {
            OpertorKind = opertorKind;
            Operand = operand;
        }
        public override Type Type => Operand.Type;
        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
        public BoundUnaryOperatorKind OpertorKind { get; }
        public BoundExpression Operand { get; }
    }

    internal enum BoundBinaryOperatorKind
    {
        Addition,
        Substraction,
        Mulitplication,
        Division
    }

    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression left, BoundBinaryOperatorKind opertorKind, BoundExpression right)
        {
            Left = left;
            OpertorKind = opertorKind;
            Right = right;
        }
        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
        public override Type Type => Left.Type;

        public BoundExpression Left { get; }
        public BoundBinaryOperatorKind OpertorKind { get; }
        public BoundExpression Right { get; }
    }

    internal sealed class Binder
    {
        private readonly List<string> _diagnostics = new List<string>();
        public IEnumerable<string> Diagnostics => _diagnostics;
        public BoundExpression BindExpression (ExpressionSyntax syntax)
        {
            switch(syntax.Kind)
            {
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);

                default:
                    throw new Exception ($"Unexpected syntax {syntax.Kind}");
            }
        }
        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.LiteralToken.Value as int? ?? 0;
            return new BoundLiteralExpression(value);
        }
        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperant = BindExpression(syntax.Operand);
            var boundOperatorKind = BindUnaryOperatorKind(syntax.OpperatorToken.Kind, boundOperant.Type);

            if(boundOperatorKind == null)
            {
                _diagnostics.Add($"Unary oeperator '{syntax.OpperatorToken.Text}' is not defined for type{boundOperant.Type}.");
                return boundOperant;
            }

            return new BoundUnaryExpression(boundOperatorKind.Value, boundOperant);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperatorKind = BindBinaryOperatorKind(syntax.OpperatorToken.Kind, boundLeft.Type, boundRight.Type);

            if(boundOperatorKind == null)
            {
                _diagnostics.Add($"Binary oeperator '{syntax.OpperatorToken.Text}' is not defined for type {boundLeft.Type} and {boundRight.Type}.");
                return boundLeft;
            }

            return new BoundBinaryExpression(boundLeft, boundOperatorKind.Value, boundRight);
        }

        private BoundUnaryOperatorKind? BindUnaryOperatorKind(SyntaxKind kind, Type operandType)
        {
            if(operandType == typeof(int))
                return null;

            switch(kind)
            {
                case SyntaxKind.PlusToken:
                    return BoundUnaryOperatorKind.Identity;
                case SyntaxKind.MinusToken:
                    return BoundUnaryOperatorKind.Negation;

                default:
                    throw new Exception ($"Unexpected unary operator {kind}");
            }
        }

        private BoundBinaryOperatorKind? BindBinaryOperatorKind(SyntaxKind kind, Type leftType, Type rightType)
        {
            if(leftType != typeof(int) || rightType != typeof(int))
                return null;
            
            switch(kind)
            {
                case SyntaxKind.PlusToken:
                    return BoundBinaryOperatorKind.Addition;
                case SyntaxKind.MinusToken:
                    return BoundBinaryOperatorKind.Substraction;
                case SyntaxKind.StarToken:
                    return BoundBinaryOperatorKind.Mulitplication;
                case SyntaxKind.SlashToken:
                    return BoundBinaryOperatorKind.Division;

                default:
                    throw new Exception ($"Unexpected unary operator {kind}");
            }
        }
    }
}