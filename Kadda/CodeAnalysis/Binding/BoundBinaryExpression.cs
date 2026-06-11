namespace Kadda.CodeAnalysis.Binding
{
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
}