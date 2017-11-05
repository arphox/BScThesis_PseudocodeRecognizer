namespace SemanticAnalysis.Exceptions
{
    public sealed class AnotherTypeExpectedException : SemanticAnalysisException
    {
        public string Expected { get; }

        public string Actual { get; }

        public AnotherTypeExpectedException(string expected, string actual, string message = null)
            :base(message ?? $"Expected type to be `{expected}`, but was `{actual}`.")
        {
            Expected = expected;
            Actual = actual;
        }
    }
}