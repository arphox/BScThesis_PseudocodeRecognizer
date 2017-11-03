using Common;

namespace SemanticAnalysis
{
    public sealed class SemanticAnalysisException : ArphoxCompilerException
    {
        public SemanticAnalysisErrorType ErrorType { get; }

        public SemanticAnalysisException(SemanticAnalysisErrorType errorType, string message)
            : base(message)
        {
            ErrorType = errorType;
        }
    }
}