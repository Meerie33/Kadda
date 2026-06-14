using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadda.CodeAnalysis.Binding
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public BoundAssignmentExpression(string name, BoundExpression expression)
        {
            Name = name;
            Expression = expression;
        }

        public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;
        public override Type Type => Expression.Type;
        public string Name { get; }
        public BoundExpression Expression { get; }
    }
}