using System;

namespace LexicalAnalysis.Analyzer
{
    public sealed class LexicalAnalysisException : ApplicationException
    {
        public LexicalAnalyzerResult Result { get; }

        public LexicalAnalysisException(LexicalAnalyzerResult result)
        {
            Result = result;
        }
    }
}