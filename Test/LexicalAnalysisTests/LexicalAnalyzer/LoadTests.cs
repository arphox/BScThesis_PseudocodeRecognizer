using LexicalAnalysis;
using LexicalAnalysis.Tokens;
using NUnit.Framework;
using System.Linq;
using LexicalAnalysis.LexicalAnalyzer;

namespace LexicalAnalysisTests.LexicalAnalyzer
{
    [TestFixture]
    public sealed class LoadTests
    {
        [Test, Timeout(1000)]
        public void LoadTest1K() => DoLoadTest(Resources.LoadTests.loadtest1k);

        [Test, Timeout(1000)]
        public void LoadTest10K() => DoLoadTest(Resources.LoadTests.loadtest10k);

        private static void DoLoadTest(string code)
        {
            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer.LexicalAnalyzer().Analyze(code);
            Assert.That(result.Tokens.Any(t => t is ErrorToken), Is.False);
        }
    }
}