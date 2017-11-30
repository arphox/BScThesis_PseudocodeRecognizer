using Common;

namespace SemanticAnalysis.Exceptions
{
    public class SemanticAnalysisException : ArphoxCompilerException
    {
        public int Line { get; }

        public SemanticAnalysisException(string message, int line)
            : base(message + $" at line #{line}.")
        {
            Line = line;
        }
    }
}