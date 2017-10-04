using LexicalAnalysis;
using NUnit.Framework;

namespace LexicalAnalysisTests.LexicalAnalyzer
{
    [TestFixture]
    public sealed class LiteralTests
    {
        [TestCaseSource(nameof(GenerateLiterals), new object[] { "egész literál", new []{ "0", "+0", "-0", "1", "5", "13273211", "-8908001" } })]
        [TestCaseSource(nameof(GenerateLiterals), new object[] { "tört literál", new[] { "0,1", "+1,2", "-3,4", "123,45", "-3123,78895", "132,73211", "-8,908001" } })]
        [TestCaseSource(nameof(GenerateLiterals), new object[] { "szöveg literál", new[] { "\"\"", "\"asd\"" } })]
        public void CanRecognizeLiterals(string type, string value)
        {
            string code = "program_kezd\n" +
                          value + "\n" +
                          "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectLiteral(type, value);
            tt.NewLine();

            tt.ExpectEnd();

            tt.ExpectNoMore();

            // Symbol table
            Assert.That(result.SymbolTable.IsEmpty);
        }

        [Test]
        public void CanRecognizeLogicalLiterals()
        {
            const string code = "program_kezd\n" +
                                "igaz\n" +
                                "hamis\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalysis.LexicalAnalyzer().Analyze(code);

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectLogikaiLiteral("igaz");
            tt.NewLine();

            tt.ExpectLogikaiLiteral("hamis");
            tt.NewLine();

            tt.ExpectEnd();

            tt.ExpectNoMore();

            // Symbol table
            Assert.That(result.SymbolTable.IsEmpty);
        }


        private static string[][] GenerateLiterals(string type, string[] array)
        {
            string[][] result = new string[array.Length][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new[] { type, array[i] };
            }
            return result;
        }
    }
}