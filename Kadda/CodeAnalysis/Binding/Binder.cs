using System.Net;
using System.Runtime.CompilerServices;
using Kadda.CodeAnalysis.Syntax;

namespace Kadda.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private readonly Dictionary<string, object> _variables;
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        public Binder(Dictionary<string, object> variables)
        {
            _variables = variables;
        }

        public DiagnosticBag Diagnostics => _diagnostics;
        public BoundExpression BindExpression (ExpressionSyntax syntax)
        {
            switch(syntax.Kind)
            {
                case SyntaxKind.ParenthesizedExpression:
                    return BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax);
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);
                case SyntaxKind.NameExpression:
                    return BindNameExpression((NameExpressionSyntax)syntax);
                case SyntaxKind.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);

                default:
                    throw new Exception ($"Unexpected syntax {syntax.Kind}");
            }
        }
        private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
        {
            return BindExpression(syntax.Expression);
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            if(!_variables.TryGetValue(name, out var value))
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
                return new BoundLiteralExpression(0);
            }

           var type = typeof(int);
           return new BoundVariableExpression(name, type);
        }
        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);
            return new BoundAssignmentExpression(name, boundExpression);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperant = BindExpression(syntax.Operand);
            var boundOperator = BoundUnaryOperator.Bind(syntax.OpperatorToken.Kind, boundOperant.Type);

            if(boundOperator == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OpperatorToken.Span, syntax.OpperatorToken.Text, boundOperant.Type);
                return boundOperant;
            }

            return new BoundUnaryExpression(boundOperator, boundOperant);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperator = BoundBinaryOperator.Bind(syntax.OpperatorToken.Kind, boundLeft.Type, boundRight.Type);

            if(boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OpperatorToken.Span, syntax.OpperatorToken.Text, boundLeft.Type, boundRight.Type);
                return boundLeft;
            }

            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }
        /*
        private BoundUnaryOperatorKind? BindUnaryOperatorKind(SyntaxKind kind, Type operandType)
        {
            if(operandType == typeof(int))
            {
                switch(kind)
                {
                    case SyntaxKind.PlusToken:
                        return BoundUnaryOperatorKind.Identity;
                    case SyntaxKind.MinusToken:
                        return BoundUnaryOperatorKind.Negation;
                }
            }

            if(operandType == typeof(bool))
            {
                switch(kind)
                {
                    case SyntaxKind.BangToken:
                        return BoundUnaryOperatorKind.LogicalNegation;
                }
            }

            return null;
        }

        private BoundBinaryOperatorKind? BindBinaryOperatorKind(SyntaxKind kind, Type leftType, Type rightType)
        {
            if(leftType == typeof(int) && rightType == typeof(int))
            {
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
                }
            }

            if(leftType == typeof(bool) && rightType == typeof(bool))
            {
                switch(kind)
                {
                    case SyntaxKind.AmpersandAmpersandToken:
                        return BoundBinaryOperatorKind.LogicalAnd;
                    case SyntaxKind.PipePipeToken:
                        return BoundBinaryOperatorKind.LogicalOr;
                }
            }
            
            return null;
        }
        */
    }
}