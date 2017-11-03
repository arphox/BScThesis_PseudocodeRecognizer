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
        public void NoSemanticAnalysisNeeded()
        {
            const string code = "program_kezd\r\n" +
                                "kilép\r\n" +
                                "program_vége";

            LexicalAnalyzerResult lexerResult = new LexicalAnalyzer(code).Start();
            SyntaxAnalyzerResult parserResult = new SyntaxAnalyzer(lexerResult).Start();

            new SemanticAnalyzer(parserResult, lexerResult.SymbolTable).Start();
        }


    }
}