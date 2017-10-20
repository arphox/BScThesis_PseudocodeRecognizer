using LexicalAnalysis.Analyzer;
using NUnit.Framework;

namespace LexicalAnalysisTests.Analyzer
{
    [TestFixture]
    public sealed class LiteralTests
    {
        #region [ Tested string literals ]

        private const string EmptyString = "\"\"";
        private const string Newline = @"""\n""";
        private const string OneDot = @""".""";
        private const string OneSpace = @""" """;
        private const string EscapedQuotationMark = "\"\\\"\"";
        private const string EscapedBackslash = "\"\\\\\"";
        private const string EscapedNewline = "\"\\n\"";
        private const string EscapedTab = "\"\\t\"";
        private const string SimpleWord = "\"word\"";
        private const string Sentence = "\"This is a complete sentence in the English language.\"";
        private const string HungarianCharacters = @"""árvíztűrő tükörfúrógép alapon nyugvó elnyűhetetlen hóbelebanc.""";
        private const string SpecialCharacters = "\",?;.:>-_*<§'+!%/=()~ˇ^˘°˛`˙´˝¨¸|Ä€÷×äđĐ[]łŁ$ß¤#&@{}\"";
        private const string EverySimpleKeyOnHungarianKeyboard = "\"0123456789öüóqwertzuiopőúasdfghjkléáűíyxcvbnmÖÜÓQWERTZUIOPŐÚASDFGHJKLÉÁŰÍYXCVBNM\"";
        private const string Complex = @"""Dear Emily,\nThank you \\n \\t for your interest of my wonderful \""compiler\""!\n\n\t\t\\Karoly\\""";

        private static readonly string[] StringLiterals =
        {
            EmptyString, Newline, EscapedQuotationMark, EscapedBackslash,
            EscapedNewline, EscapedTab, OneDot, OneSpace, SimpleWord, Sentence, HungarianCharacters, SpecialCharacters, EverySimpleKeyOnHungarianKeyboard, Complex
        };

        #endregion

        [TestCaseSource(nameof(GenerateLiterals), new object[] { "egész literál", new[] { "0", "-0", "1", "5", "13273211", "-8908001" } })]
        [TestCaseSource(nameof(GenerateLiterals), new object[] { "tört literál", new[] { "0,1", "-3,4", "123,45", "-3123,78895", "132,73211", "-8,908001" } })]
        public void CanRecognizeLiterals(string type, string value)
        {
            string code = "program_kezd\n" +
                          value + "\n" +
                          "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Analyze();

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

        [TestCaseSource(nameof(StringLiterals))]
        public void CanRecognizeSzovegLiterals(string value)
        {
            string code = "program_kezd\n" +
                           value + "\n" +
                          "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Analyze();

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectSzovegLiteral(value.Substring(1, value.Length - 2));
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

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Analyze();

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
        
        private static string[][] GenerateLiterals(string type, string[] arrayOfValues)
        {
            string[][] result = new string[arrayOfValues.Length][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new[] { type, arrayOfValues[i] };
            }
            return result;
        }
    }
}