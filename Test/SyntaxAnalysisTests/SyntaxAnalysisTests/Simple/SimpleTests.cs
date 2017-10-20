using System;
using System.Linq;
using LexicalAnalysis.Analyzer;
using LexicalAnalysis.Tokens;
using NUnit.Framework;
using SyntaxAnalysis.Analyzer;

namespace SyntaxAnalysisTests.Simple
{
    [TestFixture]
    public sealed class SimpleTests
    {
        [Test]
        public void WontStartIfLexErrorOrBadParam()
        {
            const string code = "program_kezd;\r\n" +
                                "program_vége";

            Assert.Throws<ArgumentNullException>(() => new SyntaxAnalyzer(null));
            Assert.Throws<ArgumentException>(() => new SyntaxAnalyzer(Enumerable.Empty<Token>()));
            Assert.Throws<SyntaxAnalysisException>(() => new SyntaxAnalyzer(new LexicalAnalyzer(code).Analyze().Tokens));
        }

        [Test]
        public void ShortestCode()
        {
            const string code = "program_kezd\r\n" +
                                "kilép\r\n" +
                                "program_vége";

            var parser = new SyntaxAnalyzer(new LexicalAnalyzer(code).Analyze().Tokens);
            SyntaxAnalyzerResult res = parser.Start();
            TestHelper.CheckForUnsuccessfulOrEmpty(res);
        }
    }
}