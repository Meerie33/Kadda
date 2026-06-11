using Kadda.CodeAnalysis.Binding;
using Kadda.CodeAnalysis.Syntax;

namespace Kadda.CodeAnalysis
{
    public class EvaluationResult
    {
        public EvaluationResult(IEnumerable<string> diagnostics, object value)
        {
            Diagnostics = diagnostics.ToArray();
            Value = value;
        }

        public IReadOnlyList<string> Diagnostics { get; }
        public object Value { get; }
    }
}