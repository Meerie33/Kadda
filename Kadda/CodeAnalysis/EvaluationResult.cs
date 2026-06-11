using Kadda.CodeAnalysis.Binding;
using Kadda.CodeAnalysis.Syntax;

namespace Kadda.CodeAnalysis
{
    public class EvaluationResult
    {
        public EvaluationResult(IEnumerable<Diagnostic> diagnostics, object value)
        {
            Diagnostics = diagnostics.ToArray();
            Value = value;
        }

        public IReadOnlyList<Diagnostic> Diagnostics { get; }
        public object Value { get; }
    }
}