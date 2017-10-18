using System.Linq;
using LexicalAnalysis.Analyzer;
using LexicalAnalysis.Tokens;
using NUnit.Framework;

namespace LexicalAnalysisTests.Analyzer
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
            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Analyze();
            Assert.That(result.Tokens.Any(t => t is ErrorToken), Is.False);
        }
    }
}