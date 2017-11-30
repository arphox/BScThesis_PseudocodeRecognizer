using System;
using System.Linq;
using LexicalAnalysis.Analyzer;
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

        internal static AggregateException DoSemanticAnalysisWithExceptionSwallowing(string code)
        {
            try
            {
                DoSemanticAnalysis(code);
            }
            catch (AggregateException e)
            {
                return e;
            }
            return null;
        }

        internal static SemanticAnalysisException ExpectSingleException(AggregateException aggregate)
        {
            Assert.That(aggregate.InnerExceptions.Count, Is.EqualTo(1));
            return (SemanticAnalysisException)aggregate.InnerExceptions.Single();
        }

        internal static void ExpectAnotherTypeExpectedException(SemanticAnalysisException exception, string expectedExpected, string expectedActual, int line)
        {
            AnotherTypeExpectedException e = exception as AnotherTypeExpectedException;

            Assert.That(exception != null, "Expected any exception, but was none.");
            Assert.That(e != null, $"Expected {nameof(AnotherTypeExpectedException)}, but was {exception.GetType().Name}");
            Assert.That(e.Expected == expectedExpected, $"Expected the `Expected` value to be {expectedExpected}, but was {e.Expected}.");
            Assert.That(e.Actual == expectedActual, $"Expected the `Actual` value to be {expectedActual}, but was {e.Actual}.");
            Assert.That(e.Line == line, $"Expected line to be {line}, but was {e.Line}.");
        }
    }
}