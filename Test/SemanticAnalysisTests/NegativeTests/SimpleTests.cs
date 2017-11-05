using NUnit.Framework;
using SemanticAnalysis;
using System;
using LexicalAnalysis.Analyzer;
using SemanticAnalysis.Exceptions;
using SyntaxAnalysis.Analyzer;

namespace SemanticAnalysisTests.NegativeTests
{
    [TestFixture]
    public sealed class SimpleTests
    {
        [Test]
        public void ParameterTests()
        {
            string code = "program_kezd\r\n" + "kilép\r\n" + "program_vége";
            LexicalAnalyzerResult lexerResult = new LexicalAnalyzer(code).Start();
            SyntaxAnalyzerResult parserResult = new SyntaxAnalyzer(lexerResult).Start();

            Assert.Throws<ArgumentNullException>(() => new SemanticAnalyzer(parserResult, null));
            Assert.Throws<ArgumentNullException>(() => new SemanticAnalyzer(null, lexerResult.SymbolTable));
            Assert.Throws<ArgumentNullException>(() => new SemanticAnalyzer(null, null));

            parserResult = new SyntaxAnalyzerResult(parserResult.ParseTree, false);
            Assert.Throws<SemanticAnalysisException>(() => new SemanticAnalyzer(parserResult, lexerResult.SymbolTable));
        }
    }
}