namespace Common.Exceptions
{
    public class SyntaxAnalyzerException : CompilerException
    {
        public SyntaxAnalyzerException() : base() { }
        public SyntaxAnalyzerException(string message) : base(message) { }
    }
}