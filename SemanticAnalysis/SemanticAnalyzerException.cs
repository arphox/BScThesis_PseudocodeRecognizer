using System;

namespace SemanticAnalysis
{
    public sealed class SemanticAnalyzerException : ApplicationException
    {
        public SemanticAnalyzerException(string message)
            : base(message)
        { }
        
        public override string Message => base.Message + ", " + ToString();
    }
}