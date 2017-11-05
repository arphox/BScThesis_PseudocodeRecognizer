using LexicalAnalysis.Analyzer;
using LexicalAnalysis.SymbolTableManagement;
using NUnit.Framework;
using SemanticAnalysis;
using SemanticAnalysis.Exceptions;
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

        internal static SemanticAnalysisException DoSemanticAnalysisWithExceptionSwallowing(string code)
        {
            try
            {
                DoSemanticAnalysis(code);
            }
            catch (SemanticAnalysisException e)
            {
                return e;
            }
            return null;
        }

        internal static void ExpectAnotherTypeExpectedException(SemanticAnalysisException exception, string expectedExpected, string expectedActual)
        {
            AnotherTypeExpectedException e = exception as AnotherTypeExpectedException;

            Assert.That(e, Is.Not.Null, $"Expected {nameof(AnotherTypeExpectedException)}, but was {exception.GetType().Name}");
            Assert.That(e.Expected == expectedExpected, $"Expected the `Expected` value to be {expectedExpected}, but was {e.Expected}.");
            Assert.That(e.Actual == expectedActual, $"Expected the `Actual` value to be {expectedActual}, but was {e.Actual}.");
        }
    }
}