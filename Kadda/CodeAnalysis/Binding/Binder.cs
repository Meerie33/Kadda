using System.Net;
using System.Runtime.CompilerServices;
using Kadda.CodeAnalysis.Syntax;

namespace Kadda.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        public DiagnosticBag Diagnostics => _diagnostics;
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
                case SyntaxKind.ParenthesizedExpression:
                    return BindExpression(((ParenthesizedExpressionSyntax)syntax).Expression); 

                default:
                    throw new Exception ($"Unexpected syntax {syntax.Kind}");
            }
        }
        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {

            var value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }
        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperant = BindExpression(syntax.Operand);
            var boundOperatorKind = BindUnaryOperatorKind(syntax.OpperatorToken.Kind, boundOperant.Type);

            if(boundOperatorKind == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OpperatorToken.Span, syntax.OpperatorToken.Text, boundOperant.Type);
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
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OpperatorToken.Span, syntax.OpperatorToken.Text, boundLeft.Type, boundRight.Type);
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