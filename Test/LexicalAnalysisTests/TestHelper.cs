using LexicalAnalysis.Analyzer;

namespace LexicalAnalysisTests
{
    internal static class TestHelper
    {
        public static LexicalAnalyzerResult GetLexicalAnalyzerResultWithExceptionSwallowed(string code)
        {
            LexicalAnalyzerResult result;
            try
            {
                result = new LexicalAnalyzer(code).Start();
            }
            catch (LexicalAnalysisException e)
            {
                result = e.Result;
            }
            return result;
        }
    }
}