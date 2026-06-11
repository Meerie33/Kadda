using Kadda.CodeAnalysis.Binding;
using Kadda.CodeAnalysis.Syntax;

namespace Kadda.CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundExpression _root;
        public Evaluator(BoundExpression root)
        {
            _root = root;
        }
        public object Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            if(node is BoundLiteralExpression n)
                return n.Value;

            if(node is BoundUnaryExpression u)
            {
                var operand = (int) EvaluateExpression(u.Operand);


                switch (u.OpertorKind)
                {
                    case BoundUnaryOperatorKind.Identity:
                        return operand;
                    case BoundUnaryOperatorKind.Negation:
                        return -operand;
                    default:
                        throw new Exception($"Unexpected unary operator {u.OpertorKind}");
                }
            }

            if(node is BoundBinaryExpression b)
            {
                var left = (int) EvaluateExpression(b.Left);
                var right = (int) EvaluateExpression(b.Right);

                switch (b.OpertorKind)
                {
                    case BoundBinaryOperatorKind.Addition:
                        return left + right;
                    case BoundBinaryOperatorKind.Substraction:
                        return left - right;
                    case BoundBinaryOperatorKind.Mulitplication:
                        return left * right;
                    case BoundBinaryOperatorKind.Division:
                        return left / right;
                    default:
                        throw new Exception($"Unexpected binary operator {b.OpertorKind}");
                }
            }

            throw new Exception($"Unexpected node {node.Kind}");
        }
    }
}