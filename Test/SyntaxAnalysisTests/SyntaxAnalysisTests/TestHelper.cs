using NUnit.Framework;
using SyntaxAnalysis.Analyzer;

namespace SyntaxAnalysisTests
{
    internal static class TestHelper
    {
        internal static void CheckForUnsuccessfulOrEmpty(SyntaxAnalyzerResult result)
        {
            Assert.That(result.IsSuccessful, "The syntax analysis has failed.");
            Assert.That(result.ParseTree.Root.Children.Count > 0, "The parse tree seems to be empty.");
        }
    }
}