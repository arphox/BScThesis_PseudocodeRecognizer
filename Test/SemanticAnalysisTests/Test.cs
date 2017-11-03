using LexicalAnalysis.Analyzer;
using NUnit.Framework;
using SemanticAnalysis;
using SyntaxAnalysis.Analyzer;

namespace SemanticAnalysisTests
{
    [TestFixture]
    public sealed class Test
    {
        [Test]
        public void SemATest()
        {
            string code = "";
            LexicalAnalyzerResult lexerResult = new LexicalAnalyzer(code).Start();
            SyntaxAnalyzerResult parserResult = new SyntaxAnalyzer(lexerResult).Start();

            new SemanticAnalyzer(parserResult, lexerResult.SymbolTable).Start();
        }
    }
}