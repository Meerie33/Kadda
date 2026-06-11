namespace Kadda.CodeAnalysis.Binding
{
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
}