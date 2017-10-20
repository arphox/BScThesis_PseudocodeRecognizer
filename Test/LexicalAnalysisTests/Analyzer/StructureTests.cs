using LexicalAnalysis.Analyzer;
using NUnit.Framework;

namespace LexicalAnalysisTests.Analyzer
{
    [TestFixture]
    public sealed class StructureTests
    {
        [Test]
        public void CanRecognizeIfThenElse()
        {
            const string code = "program_kezd\n" +
                                "ha igaz akkor\n" +
                                "\tkilép\n" +
                                "különben\n" +
                                "\tkilép\n" +
                                "elágazás_vége\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Analyze();

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword("ha");
            tt.ExpectLogikaiLiteral("igaz");
            tt.ExpectKeyword("akkor");
            tt.NewLine();

            tt.ExpectKeyword("kilép");
            tt.NewLine();

            tt.ExpectKeyword("különben");
            tt.NewLine();

            tt.ExpectKeyword("kilép");
            tt.NewLine();

            tt.ExpectKeyword("elágazás_vége");
            tt.NewLine();

            tt.ExpectEnd();
            tt.ExpectNoMore();

            // Symbol table
            Assert.That(result.SymbolTable.IsEmpty);
        }

        [Test]
        public void CanRecognizeWhile()
        {
            const string code = "program_kezd\n" +
                                "ciklus_amíg hamis\n" +
                                "\tkilép\n" +
                                "ciklus_vége\n" +
                                "program_vége";

            LexicalAnalyzerResult result = new LexicalAnalyzer(code).Analyze();

            TokenTester tt = new TokenTester(result);

            tt.ExpectStart();
            tt.NewLine();

            tt.ExpectKeyword("ciklus_amíg");
            tt.ExpectLogikaiLiteral("hamis");
            tt.NewLine();

            tt.ExpectKeyword("kilép");
            tt.NewLine();

            tt.ExpectKeyword("ciklus_vége");
            tt.NewLine();

            tt.ExpectEnd();
            tt.ExpectNoMore();

            // Symbol table
            Assert.That(result.SymbolTable.IsEmpty);
        }
    }
}