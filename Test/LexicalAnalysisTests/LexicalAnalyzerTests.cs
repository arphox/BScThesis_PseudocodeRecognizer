using System;
using NUnit.Framework;
using LexicalAnalysis;

namespace LexicalAnalysisTests
{
    [TestFixture]
    public class LexicalAnalyzerTests
    {
        [Test]
        public void ArgumentTests()
        {
            Assert.Throws<ArgumentException>(() => new LexicalAnalyzer().Analyze(null));
            Assert.Throws<ArgumentException>(() => new LexicalAnalyzer().Analyze(string.Empty));
            Assert.Throws<ArgumentException>(() => new LexicalAnalyzer().Analyze(" "));
            Assert.Throws<ArgumentException>(() => new LexicalAnalyzer().Analyze("\t"));
            Assert.Throws<ArgumentException>(() => new LexicalAnalyzer().Analyze("\n"));
            Assert.Throws<ArgumentException>(() => new LexicalAnalyzer().Analyze("   \t    \n"));
        }
    }
}