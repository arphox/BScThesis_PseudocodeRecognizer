namespace SemanticAnalysis.Exceptions
{
    public sealed class AnotherTypeExpectedException : SemanticAnalysisException
    {
        public string Expected { get; }

        public string Actual { get; }

        public AnotherTypeExpectedException(string expected, string actual, int line, string message = null)
            :base(message ?? $"Expected type to be `{expected}`, but was `{actual}` ", line)
        {
            Expected = expected;
            Actual = actual;
        }
    }
}