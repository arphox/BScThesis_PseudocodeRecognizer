using Common;

namespace LexicalAnalysis.Analyzer
{
    public sealed class LexicalAnalysisException : ArphoxCompilerException
    {
        public LexicalAnalyzerResult Result { get; }

        public LexicalAnalysisException(LexicalAnalyzerResult result)
        {
            Result = result;
        }
    }
}