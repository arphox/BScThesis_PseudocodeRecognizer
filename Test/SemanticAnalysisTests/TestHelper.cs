using LexicalAnalysis.Analyzer;
using SemanticAnalysis;
using SyntaxAnalysis.Analyzer;

namespace SemanticAnalysisTests
{
    internal static class TestHelper
    {
        internal static void DoSemanticAnalysis(string code)
        {
            LexicalAnalyzerResult lexerResult = new LexicalAnalyzer(code).Start();
            SyntaxAnalyzerResult parserResult = new SyntaxAnalyzer(lexerResult).Start();

            new SemanticAnalyzer(parserResult, lexerResult.SymbolTable).Start();
        }
    }
}