using Kadda.CodeAnalysis.Binding;
using Kadda.CodeAnalysis.Syntax;

namespace Kadda.CodeAnalysis
{
    public struct TextSpan
    {
        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }

        public int Start { get; }
        public int Length { get; }
        public int End => Start + Length;
    } 
}