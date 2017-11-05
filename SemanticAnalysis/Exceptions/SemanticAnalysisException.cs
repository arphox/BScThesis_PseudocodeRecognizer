using Common;

namespace SemanticAnalysis.Exceptions
{
    public class SemanticAnalysisException : ArphoxCompilerException
    {
        public SemanticAnalysisException()
        { }

        public SemanticAnalysisException(string message)
            : base(message)
        { }
    }
}