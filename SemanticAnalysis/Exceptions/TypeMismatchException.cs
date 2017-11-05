namespace SemanticAnalysis.Exceptions
{
    public sealed class TypeMismatchException : SemanticAnalysisException
    {
        public string Left { get; }
        public string Right { get; }

        public TypeMismatchException(string left, string right, string message = null)
            :base(message ?? $"The types on the two sides of the expression should match. Left: `{left}`. Right: `{right}`.")
        {
            Left = left;
            Right = right;
        }
    }
}